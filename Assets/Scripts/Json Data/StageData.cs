using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

[System.Serializable]
public class StageDataEntry
{
    public string ChapterNumber;
    public string LevelsNumber;
    public string StageNumber; // 這是我們要比對的值
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

    private List<StageDataEntry> stages;

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
                stages = JsonConvert.DeserializeObject<List<StageDataEntry>>(jsonFile.text);
                if (stages == null)
                {
                    Debug.LogError("Failed to parse StageData.json!");
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

    public StageDataEntry FindStageByNumber(int stageNumber)
    {
        if (stages != null)
        {
            var stage = stages.FirstOrDefault(s => s.StageNumber == stageNumber.ToString());
            if (stage != null)
            {
                return stage;
            }
        }
        Debug.LogError("StageNumber " + stageNumber + " not found in JSON data!");
        return null;
    }
}
