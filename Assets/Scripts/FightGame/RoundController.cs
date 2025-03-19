//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class RoundController : MonoBehaviour
{

	public static RoundController Instance { get; private set; }

	[Header("----------------- Value ------------------")]
	//[SerializeField] private float timeScale = 1f;
	[SerializeField] private string gameStatus = "Continue";


	[Header("----------------- Variable Reference ------------------")]
	[SerializeField] private PlayerStatusManager player1Status;
	[SerializeField] private PlayerStatusManager player2Status;

	[SerializeField] private GameObject gameOverPanel;

	[SerializeField] private SpriteRenderer BackGroundSprite;
	[SerializeField] private SpriteRenderer FieldSprite;

	[Header("----------------- Now Stage Info ------------------")]
	[SerializeField] private int currentChapterIndex = 0; // 當前章節索引
	[SerializeField] private int currentLevelIndex = 0; // 當前關卡索引
	[SerializeField] private int currentStageIndex = 0; // 當前戰鬥索引

	[Header("----------------- Stage Data Count ------------------")]
	[SerializeField] private int totalChapters; // 總章節數
	//[SerializeField] private int nowTotalLevels; // 總關卡數
	//[SerializeField] private int nowTotalStages; // 總戰鬥數
	[SerializeField] private Dictionary<int, int> totalLevelsPerChapter = new Dictionary<int, int>(); // 每章節的關卡數
	[SerializeField] private Dictionary<(int, int), int> totalStagesPerLevel = new Dictionary<(int, int), int>(); // 每關卡的關卡數


	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
		currentStageIndex = FightPlayer1Config.CurrentStage;
		currentLevelIndex = FightPlayer1Config.CurrentLevel;
		currentChapterIndex = FightPlayer1Config.CurrentChapter;
	}

	//// Start is called before the first frame update
	void Start()
	{
		Application.targetFrameRate = 60;

		// 從 StageData 取得總章節、關卡、戰鬥數量
		(var totalChapters, var levelsPerChapter, var stagesPerLevel) = StageData.Instance.GetStageCounts();

		this.totalChapters = totalChapters;
		this.totalLevelsPerChapter = levelsPerChapter;
		this.totalStagesPerLevel = stagesPerLevel;

		// Debug Log 記錄數據
		Debug.Log($"總章節數: {totalChapters}");
		foreach (var levelCount in totalLevelsPerChapter)
		{
			Debug.Log($"Chapter {levelCount.Key} 共有 {levelCount.Value} 個 Level");
		}
		foreach (var stageCount in totalStagesPerLevel)
		{
			Debug.Log($"Chapter {stageCount.Key.Item1}, Level {stageCount.Key.Item2} 共有 {stageCount.Value} 個 Stage");
		}

		//// 組合 Key 為 "Chapter_X_Level_Y"
		//string levelKey = $"Chapter_{currentChapterIndex}_Level_{currentLevelIndex}";

		//// 確保 Key 存在，避免 `KeyNotFoundException`
		//int currentStageCount = totalStagesPerLevel.ContainsKey(levelKey) ? totalStagesPerLevel[levelKey] : 0;

		//Debug.Log($"{currentStageCount} Now Stages");


		GameStart();
	}

	// Update is called once per frame
	void Update()
	{
		// p1 or p2 hp == 0 end game
		if (player1Status.GetHP() <= 0)
		{
			GameOver();
			// defeat
		}
		else if (player2Status.GetHP() <= 0)
		{
			//GameOver();
			// win
			NextStage();
			GameStart();
			//SceneManager.LoadScene("FightScene");
		}
	}

	public void NextStage()
	{
		FightPlayer1Config.NowHP = player1Status.GetHP();
		currentStageIndex++;
		// 需要檢查是否到了新關卡或新章節
		StageDataEntry nextStage = StageData.Instance.FindStage(currentChapterIndex, currentLevelIndex, currentStageIndex);

		if (nextStage == null)
		{
			// 如果找不到下一關，可能需要提升 Level 或 Chapter
			currentStageIndex = 1; // 重置 Stage
			currentLevelIndex++;

			nextStage = StageData.Instance.FindStage(currentChapterIndex, currentLevelIndex, currentStageIndex);
			if (nextStage == null)
			{
				// 如果 Level 也找不到，則提升 Chapter
				currentLevelIndex = 1;
				currentChapterIndex++;
			}
		}

		FightPlayer1Config.CurrentStage = currentStageIndex;
		FightPlayer1Config.CurrentLevel = currentLevelIndex;
		FightPlayer1Config.CurrentChapter= currentChapterIndex;
	}

	public void GameStart()
    {
		Time.timeScale = 1;

		// 透過 StageData Singleton 取得 "StageNumber" 為 currentStageIndex 的關卡
		StageDataEntry currentStage = StageData.Instance.FindStage(currentChapterIndex, currentLevelIndex, currentStageIndex);
		if (currentStage != null)
		{
			// 確保 player2_Group 轉換成 string[]
			string[] groupArray = currentStage.player2_Group != null ? currentStage.player2_Group.ToArray() : new string[0];

			// 設定 FightPlayer2Config
			FightPlayer2Config.AI_level = currentStage.AI_level;
			FightPlayer2Config.StartHP = currentStage.StartHP;
			FightPlayer2Config.NowHP = currentStage.StartHP;
			FightPlayer2Config.StartATK = currentStage.StartATK;
			FightPlayer2Config.NowATK = currentStage.StartATK;
			FightPlayer2Config.Group = currentStage.player2_Group.ToArray();

			FightPlayer2Config.PlayerSkin = currentStage.Player2Skin;
			FightPlayer2Config.PuckSkin = currentStage.Player2PuckSkin;
			FightPlayer2Config.BackGroundImage = currentStage.BackGroundImage;
			FightPlayer2Config.FieldImage = currentStage.FieldImage;
			FightPlayer2Config.BGM = currentStage.BGM;

			Debug.Log("Stage Loaded: " + currentStage.StageNumber);
			Debug.Log("Player2 Group: " + string.Join(", ", FightPlayer2Config.Group));
		}

		FieldSprite.sprite = Resources.Load<Sprite>("Arts/FightScene/Field/" + FightPlayer2Config.FieldImage);
		BackGroundSprite.sprite = Resources.Load<Sprite>("Arts/FightScene/Field/" + FightPlayer2Config.BackGroundImage);

		player1Status.InitStatus();
		player2Status.InitStatus();
		if(FightStageConfig.BGM == "BasicBattleBGM")
		{
			AudioManager.Instance.PlayBGM(MusicAudioClips.Instance.BasicBattleBGM);
		}

		SkillManager.Instance.InitialSkillManager();
		MagicPointsManager.Instance.InitialMagicPointsManager();

	}


	public void PauseGame()
    {
        Time.timeScale = 0;

        gameStatus = "Pause Game";
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;

        gameStatus = "Continue";
    }

	private void GameOver()
	{
		FightPlayer1Config.NowHP = player1Status.GetHP();
		//PauseGame();
		gameOverPanel.SetActive(true);

		gameStatus = "gameover";
	}
}
