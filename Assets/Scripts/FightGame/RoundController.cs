//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class RoundController : MonoBehaviour
{

	public static RoundController Instance { get; private set; }

	[Header("----------------- Value ------------------")]
    //[SerializeField] private float timeScale = 1f;
    [SerializeField] private string gameStatus = "Continue";


    [Header("----------------- Time Counting Down------------------")]
	[SerializeField] public float nowTime = 180;
	[SerializeField] private float maxTime = 180;
	[SerializeField] private Text timeText;


	[Header("----------------- Variable Reference ------------------")]
	[SerializeField] private PlayerStatusManager player1Status;
	[SerializeField] private PlayerStatusManager player2Status;

	[SerializeField] private GameObject gameOverPanel;
	[SerializeField] private GameObject player1;
	[SerializeField] private GameObject player2;
	[SerializeField] private GameObject dieEffect;

	[SerializeField] private AIController ai_controller;

	[SerializeField] private SpriteRenderer DeadFrameBackGroundSprite;
	[SerializeField] private SpriteRenderer BackGroundSprite;
	[SerializeField] private SpriteRenderer FieldSprite;

	[Header("----------------- Now Stage Info ------------------")]
	[SerializeField] private int currentChapterIndex = 0; // 當前章節索引
	[SerializeField] private int currentLevelIndex = 0; // 當前關卡索引
	[SerializeField] private int currentStageIndex = 0; // 當前戰鬥索引
	[SerializeField] private bool levelIsChanged = false;//是否是不同關卡 
	//[SerializeField] private bool isFirstTimeEnter = true;// 第一次進入關卡預設為true

	[Header("----------------- Stage Data Count ------------------")]
	[SerializeField] private int totalChapters; // 總章節數
	//[SerializeField] private int nowTotalLevels; // 總關卡數
	//[SerializeField] private int nowTotalStages; // 總戰鬥數
	[SerializeField] private Dictionary<int, int> totalLevelsPerChapter = new Dictionary<int, int>(); // 每章節的關卡數
	[SerializeField] private Dictionary<(int, int), int> totalStagesPerLevel = new Dictionary<(int, int), int>(); // 每關卡的關卡數


	[Header("----------------- ShowStage Panel ------------------")]
	[SerializeField] private GameObject showStagePanel;
	[SerializeField] private GameObject coinFountain;
	[SerializeField] private Text roundText;
	[SerializeField] private bool canInstFountain;

	[Header("----------------- HeadStickers ------------------")]
	[SerializeField] private Image PlayerHeadSticker;
	[SerializeField] private Image SlimeHeadSticker;
	[SerializeField] private Image P1HSBackGround;
	[SerializeField] private Image P2HSBackGround;

	[Header("----------------- Win & Lose Panel ------------------")]
	[SerializeField] private GameObject WinPanel;
	[SerializeField] private GameObject LosePanel;
	[SerializeField] private Text totalTimeText;


	[Header("----------------- Ball ------------------")]
	[SerializeField] private BallController ballController;
	[SerializeField] private SpriteRenderer ballSprite;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			//DontDestroyOnLoad(gameObject);
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
		nowTime = maxTime;
		Application.targetFrameRate = 60;

		// 從 StageData 取得總章節、關卡、戰鬥數量
		(var totalChapters, var levelsPerChapter, var stagesPerLevel) = StageData.Instance.GetStageCounts();

		this.totalChapters = totalChapters;
		this.totalLevelsPerChapter = levelsPerChapter;
		this.totalStagesPerLevel = stagesPerLevel;

		// Debug Log 記錄數據
		//Debug.Log($"總章節數: {totalChapters}");
		//foreach (var levelCount in totalLevelsPerChapter)
		//{
		//	Debug.Log($"Chapter {levelCount.Key} 共有 {levelCount.Value} 個 Level");
		//}
		//foreach (var stageCount in totalStagesPerLevel)
		//{
		//	Debug.Log($"Chapter {stageCount.Key.Item1}, Level {stageCount.Key.Item2} 共有 {stageCount.Value} 個 Stage");
		//}

		//// 組合 Key 為 "Chapter_X_Level_Y"
		//string levelKey = $"Chapter_{currentChapterIndex}_Level_{currentLevelIndex}";

		//// 確保 Key 存在，避免 `KeyNotFoundException`
		//int currentStageCount = totalStagesPerLevel.ContainsKey(levelKey) ? totalStagesPerLevel[levelKey] : 0;

		//Debug.Log($"{currentStageCount} Now Stages");


		GameStart();

		OpenStagePanel();
		StartCoroutine(CloseStagePanelDelayed(1.5f));

		PauseGame();
		StartCoroutine(ContinueGameDelayed(1.5f));

		
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey(KeyCode.T))
        {
			//Debug.Log("T");
			PauseGame();
		}
		else if(Input.GetKeyUp(KeyCode.T))
        {
			SetTimeScale(1);

        }

		nowTime -= Time.deltaTime;
		timeText.text = "" + ((int)nowTime);
		// p1 or p2 hp == 0 end game
		if(gameStatus == "Continue")
        {
			if (player1Status.GetHP() <= 0 || nowTime <= 0)
			{
				//Instantiate(dieEffect, player1.transform.position, Quaternion.Euler(-90,0,0));
				LosePanel.SetActive(true);
				GameOver();
				// defeat
			}
			else if (player2Status.GetHP() <= 0 && canInstFountain)
			{
				//GameOver();
				//GameStart();

				Instantiate(dieEffect, player2.transform.position, Quaternion.Euler(-90, 0, 0));
				// win
				//NextStage();
				//Instantiate(coinFountain, player2.transform.position, Quaternion.Euler(-90,0,0));
				StartCoroutine(DelayInstCoinFountain(1f));

				canInstFountain = false;
				StartCoroutine(ReloadSceneDelayed(5f));
				//SetTimeScale(0.5f);
				//PauseGame();
				PauseMainObjects();

				StartCoroutine(PauseGameDelayed(5f));

				//StartCoroutine(ContinueGameDelayed(3.5f));
			}
		}
		
	}

	private IEnumerator DelayInstCoinFountain(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		Instantiate(coinFountain, player2.transform.position, Quaternion.Euler(-90, 0, 0));
	}

	private IEnumerator ReloadSceneDelayed(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);

		canInstFountain = true;
		NextStage();
		//SceneManager.LoadScene("FightScene"); //在NextStage裡面loadScene
	}

	public void OpenStagePanel()
	{
		showStagePanel.SetActive(true);

		// 取得當前這個章節與關卡下，總共的 stage 數
		int totalStagesInCurrentLevel = 0;
		if (totalStagesPerLevel.ContainsKey((currentChapterIndex, currentLevelIndex)))
		{
			totalStagesInCurrentLevel = totalStagesPerLevel[(currentChapterIndex, currentLevelIndex)];
		}

		// 顯示為 Round X / Y
		roundText.text = $"Round {currentStageIndex} / {totalStagesInCurrentLevel}";
	}
	private IEnumerator CloseStagePanelDelayed(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		CloseStagePanel();
	}
	public void CloseStagePanel()
	{
		showStagePanel.SetActive(false);
	}


	public void NextStage()
	{
		FightPlayer1Config.NowHP = player1Status.GetHP();
		FightPlayer1Config.NowMagicPoint = MagicPointsManager.Instance.GetMagicPoint(1);
		currentStageIndex++;
		// 需要檢查是否到了新關卡或新章節
		StageDataEntry nextStage = StageData.Instance.FindStage(currentChapterIndex, currentLevelIndex, currentStageIndex);

		if (nextStage == null)
		{
			//NextLevel();
			LevelFinished();
			// 如果找不到下一關，可能需要提升 Level 或 Chapter
			// 顯示結算畫面

			//currentStageIndex = 1; // 重置 Stage
			//currentLevelIndex++;

			//nextStage = StageData.Instance.FindStage(currentChapterIndex, currentLevelIndex, currentStageIndex);
			//if (nextStage == null)
			//{
			//	// 如果 Level 也找不到，則提升 Chapter
			//	currentLevelIndex = 1;
			//	currentChapterIndex++;
			//}
		}
		else
        {

			FightPlayer1Config.CurrentStage = currentStageIndex;
			FightPlayer1Config.CurrentLevel = currentLevelIndex;
			FightPlayer1Config.CurrentChapter = currentChapterIndex;
			SceneManager.LoadScene("FightScene");
		}
		//if (currentLevelIndex != FightPlayer1Config.CurrentLevel 
		//	|| currentChapterIndex != FightPlayer1Config.CurrentChapter)//若換關卡了
		//{
		//	levelIsChanged = true;

		//}

	}

	public void LevelFinished()
	{
		AudioManager.Instance.StopBGM();
		AudioManager.Instance.PlaySFXAtPosition(SFXAudioClips.Instance.WinSoundEffect, new Vector3(0, 0.65f, -20));
		WinPanel.SetActive(true);
		RewardManager.Instance.GenerateReward();
	}
	public void NextLevel()
	{
		currentStageIndex = 1;
		currentLevelIndex++;
		FightPlayer1Config.NowHP = FightPlayer1Config.StartHP;
		FightPlayer1Config.NowMagicPoint = 0;
		// 需要檢查是否到了新關卡或新章節
		StageDataEntry nextStage = StageData.Instance.FindStage(currentChapterIndex, currentLevelIndex, currentStageIndex);

		if (nextStage == null)
		{
			currentChapterIndex++;
			currentStageIndex = 1;
			currentLevelIndex = 1;

			MagicPointsManager.Instance.InitialMagicPointsManager();
			FightPlayer1Config.CurrentStage = currentStageIndex;
			FightPlayer1Config.CurrentLevel = currentLevelIndex;
			FightPlayer1Config.CurrentChapter = currentChapterIndex;
			SceneManager.LoadScene("FightScene");
		}
		else
		{
			MagicPointsManager.Instance.InitialMagicPointsManager();
			FightPlayer1Config.CurrentStage = currentStageIndex;
			FightPlayer1Config.CurrentLevel = currentLevelIndex;
			FightPlayer1Config.CurrentChapter = currentChapterIndex;
			SceneManager.LoadScene("FightScene");
		}
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
            //FightPlayer2Config.AI_AttackFrequencyReduce = currentStage.AI_AttackFrequencyReduce;
            FightPlayer2Config.AI_AttackFrequency = currentStage.AI_AttackFrequency;
            FightPlayer2Config.StartHP = currentStage.StartHP;
			FightPlayer2Config.NowHP = currentStage.StartHP;
			FightPlayer2Config.StartATK = currentStage.StartATK;
			FightPlayer2Config.NowATK = currentStage.StartATK;
			FightPlayer2Config.Group = currentStage.player2_Group.ToArray();

			FightPlayer2Config.PlayerSkin = currentStage.Player2Skin;
			FightPlayer2Config.PuckSkin = currentStage.Player2PuckSkin;
			FightPlayer2Config.DeadFrameBackGroundImage = currentStage.DeadFrameBackGroundImage;
			FightPlayer2Config.BackGroundImage = currentStage.BackGroundImage;
			FightPlayer2Config.FieldImage = currentStage.FieldImage;
			FightPlayer2Config.HSBackGroundImage = currentStage.FieldHSBackGroundImage;
			FightPlayer2Config.BGM = currentStage.BGM;

			//Debug.Log("Stage Loaded: " + currentStage.StageNumber);
			//Debug.Log("Player2 Group: " + string.Join(", ", FightPlayer2Config.Group));
		}

		foreach (var spawnInfo in currentStage.spawnObjects)
		{
			GameObject prefab = Resources.Load<GameObject>($"Prefabs/SpawnObjects/{spawnInfo.prefabName}");
			if (prefab != null)
			{
				Instantiate(prefab, spawnInfo.position, Quaternion.Euler(spawnInfo.rotation));
			}
			else
			{
				Debug.LogWarning($"Prefab not found: {spawnInfo.prefabName}");
			}
		}

		FieldSprite.sprite = Resources.Load<Sprite>("Arts/FightScene/Field/" + FightPlayer2Config.FieldImage);
		DeadFrameBackGroundSprite.sprite = Resources.Load<Sprite>("Arts/MainScenes/BackgroundImage/" + FightPlayer2Config.DeadFrameBackGroundImage);
		BackGroundSprite.sprite = Resources.Load<Sprite>("Arts/FightScene/Field/" + FightPlayer2Config.BackGroundImage);
		PlayerHeadSticker.sprite = Resources.Load<Sprite>("Arts/FightScene/HeadStickers/" + FightPlayer1Config.PlayerSkin);
		SlimeHeadSticker.sprite = Resources.Load<Sprite>("Arts/FightScene/HeadStickers/" + FightPlayer2Config.PlayerSkin);

		ballSprite.sprite = Resources.Load<Sprite>("Arts/FightScene/Field/FieldObjects/Chapter" + FightPlayer1Config.CurrentChapter + "BallSprite");

		P1HSBackGround.sprite = Resources.Load<Sprite>("Arts/FightScene/UI/Magic Panel ver2/" + FightPlayer2Config.HSBackGroundImage);
		P2HSBackGround.sprite = Resources.Load<Sprite>("Arts/FightScene/UI/Magic Panel ver2/" + FightPlayer2Config.HSBackGroundImage);

		if (levelIsChanged || FightPlayer1Config.isFirstTimeEnter)//有換關卡才要重設置音樂 & 重製魔力值
		{
			//MagicPointsManager.Instance.InitialMagicPointsManager();
			levelIsChanged = false; //暫時仍無法持續播放
			FightPlayer1Config.isFirstTimeEnter = false;
			FightPlayer1Config.NowMagicPoint = 0;
			FightPlayer1Config.NowHP = FightPlayer1Config.StartHP;
		}

		player1Status.InitStatus();
		player2Status.InitStatus();

		ai_controller.InitAI();
		
		if (FightPlayer2Config.BGM == "battle_theme_1")
		{
			AudioManager.Instance.PlayBGM(MusicAudioClips.Instance.BasicBattleBGM);
		}
		else if (FightPlayer2Config.BGM == "CH1 Combat music")
		{
			AudioManager.Instance.PlayBGM(MusicAudioClips.Instance.Ch1BGM);

		}
		else if (FightPlayer2Config.BGM == "CH2 Combat music")
		{
			AudioManager.Instance.PlayBGM(MusicAudioClips.Instance.Ch2BGM);

		}

		SkillManager.Instance.InitialSkillManager();
		MagicPointsManager.Instance.InitialMagicPointsManager();
	}

	public void SetTimeScale(float tScale)
    {
		Time.timeScale = tScale;
    }

	public void PauseMainObjects()
    {
		ballController.SetPauseBallMoving(true);
		ai_controller.SetPause(true);
	}

	public void continueMainObjects()
	{
		ballController.SetPauseBallMoving(false);
		ai_controller.SetPause(false);
	}

	private IEnumerator PauseGameDelayed(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		PauseGame();
	}

	public void PauseGame()
    {
		SetTimeScale(0);

		gameStatus = "Pause";
	}
	private IEnumerator ContinueGameDelayed(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		ContinueGame();
	}

	public void ContinueGame()
    {
		SetTimeScale(1);

        gameStatus = "Continue";
    }

	private void GameOver()
	{
		FightPlayer1Config.NowHP = player1Status.GetHP();
		//PauseGame();
		//gameOverPanel.SetActive(true);
		AudioManager.Instance.StopBGM();
		AudioManager.Instance.PlaySFXAtPosition(SFXAudioClips.Instance.LoseSoundEffect, new Vector3(0, 0.65f, -20));
		LosePanel.SetActive(true);

		PauseGame();
		gameStatus = "Gameover";
	}
}
