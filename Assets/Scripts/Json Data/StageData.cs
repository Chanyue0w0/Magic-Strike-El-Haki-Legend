using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

[System.Serializable]
public class StageDataEntry
{
    public string StageNumber;
    public string BackGroundImage;
    public string FieldImage;
    public string BGM;
    public float AI_level;
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
        DontDestroyOnLoad(gameObject);
        LoadStageData();
    }

    private void LoadStageData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(filePath);
        if (jsonFile != null)
        {
            try
            {
                // 解析為 Dictionary，符合 JSON 結構
                chapters = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<StageDataEntry>>>>(jsonFile.text);
                if (chapters == null || chapters.Count == 0)
                {
                    Debug.LogError("Failed to parse StageData.json! Data is null or empty.");
                }
                else
                {
                    Debug.Log("StageData.json loaded successfully!");
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
}
