using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

[System.Serializable]
public class StageDataEntry
{
    public string StageNumber;
    public string DeadFrameBackGroundImage;
    public string BackGroundImage;
    public string FieldImage;
    public string FieldHSBackGroundImage;
    public string BGM;
    public float AI_level;
    //public float AI_AttackFrequencyReduce;
    public Vector2 AI_AttackFrequency;
    public int StartHP;
    public int StartATK;
    public string Player2Skin;
    public string Player2PuckSkin;
    public List<string> player2_Group;
}

public class StageData : MonoBehaviour
{
    public static StageData Instance { get; private set; }

    private Dictionary<string, Dictionary<string, List<StageDataEntry>>> chapters;

    // 儲存各 Chapter、Level、Stage 的總數 (Key 改回 string)
    private int totalChapters = 0; 
    private Dictionary<int, int> totalLevelsPerChapter = new Dictionary<int, int>();
    private Dictionary<(int, int), int> totalStagesPerLevel = new Dictionary<(int, int), int>();


    [SerializeField] private string filePath = "jsonData/StageData"; // JSON 檔案在 Resources 下的路徑

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple StageData instances detected!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
        LoadStageData();
    }

    private void LoadStageData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(filePath);
        if (jsonFile != null)
        {
            try
            {
                chapters = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<StageDataEntry>>>>(jsonFile.text);
                if (chapters == null || chapters.Count == 0)
                {
                    Debug.LogError("Failed to parse StageData.json! Data is null or empty.");
                }
                else
                {
                    //Debug.Log("StageData.json loaded successfully!");
                    CalculateStageCounts();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error parsing StageData.json: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("StageData.json not found in Resources folder!");
        }
    }

    private void CalculateStageCounts()
    {
        totalChapters = chapters.Count;
        totalLevelsPerChapter.Clear();
        totalStagesPerLevel.Clear();

        foreach (var chapter in chapters)
        {
            string chapterKey = chapter.Key;
            if (!chapterKey.StartsWith("Chapter_") || !int.TryParse(chapterKey.Replace("Chapter_", ""), out int chapterNumber))
            {
                Debug.LogError($"Invalid chapter key format: {chapterKey}");
                continue;
            }

            totalLevelsPerChapter[chapterNumber] = chapter.Value.Count;

            foreach (var level in chapter.Value)
            {
                string levelKey = level.Key;
                if (!levelKey.StartsWith("Level_") || !int.TryParse(levelKey.Replace("Level_", ""), out int levelNumber))
                {
                    Debug.LogError($"Invalid level key format: {levelKey}");
                    continue;
                }

                totalStagesPerLevel[(chapterNumber, levelNumber)] = level.Value.Count;
            }
        }

        //Debug.Log("CalculateStageCounts() 計算完成");
    }


    public StageDataEntry FindStage(int chapterNumber, int levelNumber, int stageNumber)
    {
        string chapterKey = $"Chapter_{chapterNumber}";
        string levelKey = $"Level_{levelNumber}";

        if (chapters != null)
        {
            if (chapters.ContainsKey(chapterKey))
            {
                if (chapters[chapterKey].ContainsKey(levelKey))
                {
                    var stageList = chapters[chapterKey][levelKey];
                    var stage = stageList.FirstOrDefault(s => s.StageNumber == stageNumber.ToString());
                    if (stage != null)
                    {
                        return stage;
                    }
                    else
                    {
                        Debug.LogError($"Stage {stageNumber} not found in Chapter {chapterNumber}, Level {levelNumber}.");
                    }
                }
                else
                {
                    Debug.LogError($"Level {levelNumber} not found in Chapter {chapterNumber}.");
                }
            }
            else
            {
                Debug.LogError($"Chapter {chapterNumber} not found in StageData.");
            }
        }
        return null;
    }

    /// <summary>
    /// 取得各章節、關卡、階段的數量資訊
    /// </summary>
    public (int totalChapters, Dictionary<int, int> levelsPerChapter, Dictionary<(int, int), int> stagesPerLevel) GetStageCounts()
    {
        return (totalChapters, totalLevelsPerChapter, totalStagesPerLevel);
    }

}
