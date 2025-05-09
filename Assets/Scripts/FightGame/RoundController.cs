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
	[SerializeField] private bool isOpeningRogueLike = true;

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

	[Header("----------------- Camera Focus for Death ------------------")]
	[SerializeField] private Camera mainCamera;
	[SerializeField] private float zoomedCameraSize = 2.5f;
	[SerializeField] private float cameraFadeInSpeed = 5f;
	[SerializeField] private float cameraFadeOutSpeed = 7f;
	[SerializeField] private float cameraZoomDuration = 1.5f;
	[Header("----------------- Slime Death Panel ------------------")]
	[SerializeField] private GameObject slimeDeathPanel;
	[SerializeField] private Image slimeDeathImage;

	private float originalCameraSize;
	private Vector3 originalCameraPosition;



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
		originalCameraSize = mainCamera.orthographicSize;
		originalCameraPosition = mainCamera.transform.position;


		nowTime = maxTime;
		Application.targetFrameRate = 60;

		// 從 StageData 取得總章節、關卡、戰鬥數量
		(var totalChapters, var levelsPerChapter, var stagesPerLevel) = StageData.Instance.GetStageCounts();

		this.totalChapters = totalChapters;
		this.totalLevelsPerChapter = levelsPerChapter;
		this.totalStagesPerLevel = stagesPerLevel;

		GameStart();
		
		//OpenStagePanel();
		//StartCoroutine(CloseStagePanelDelayed(1.5f));

		//PauseGame();
		//StartCoroutine(ContinueGameDelayed(1.5f));

		if (currentChapterIndex == 1 && currentLevelIndex == 1)
		{
			//TutorialManager.Instance.OpenTutorial();
			StartCoroutine(OpenTutorialDelayed(1.6f));

            FightPlayer1Config.Group[1] = "SK04";
            FightPlayer1Config.Group[2] = "SK04";
            SkillManager.Instance.InitialSkillManager();
			MagicPointsManager.Instance.InitialMagicPointsManager();
			PassiveSkillManager.Instance.InitialPassiveSkillManager();
			player1Status.InitStatus(); // 多一次重製玩家
		}
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
				//gameStatus = "Gameover";
				//Instantiate(dieEffect, player1.transform.position, Quaternion.Euler(-90,0,0));
				LosePanel.SetActive(true);
				GameOver();
				// defeat
			}
			else if (player2Status.GetHP() <= 0 && canInstFountain)
			{
				//gameStatus = "Win";
				PlayDeathAnimation();
				AudioManager.Instance.PlaySFXAtPosition(SFXAudioClips.Instance.SlimeDie, new Vector3(0, 0.65f, -20));
				Instantiate(dieEffect, player2.transform.position, Quaternion.Euler(-90, 0, 0));
				StartCoroutine(DelayInstCoinFountain(1f));

				canInstFountain = false;
				PauseMainObjects();
				//PauseGame();

				// 判斷是否是該 Level 的最後一關
				int totalStagesInCurrentLevel = 0;
				if (totalStagesPerLevel.ContainsKey((currentChapterIndex, currentLevelIndex)))
				{
					totalStagesInCurrentLevel = totalStagesPerLevel[(currentChapterIndex, currentLevelIndex)];
				}

				if (currentStageIndex < totalStagesInCurrentLevel)  // 尚未最後一關，顯示 RogueLike 面板
				{
					StartCoroutine(HandleStageClearRogueLikeFlow());
				}
				else
				{
					// 若已是最後一關，執行 LevelFinished 流程
					StartCoroutine(DelayLevelFinished(3f)); // 可以稍微延遲一下讓動畫播完
				}
			}

		}

	}

	private IEnumerator DelayLevelFinished(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		LevelFinished();
		PauseGame();
	}

	//開啟RogueLikePanel Delay
	private IEnumerator OpenInitialRogueLikePanelDelayed()
	{
        yield return new WaitForSecondsRealtime(0.1f);

        //yield return new WaitForSeconds(0.1f);

        PauseGame();
		RogueLikePanelManager.Instance.SetPanelActive(true);
		RogueLikePanelManager.Instance.DrawSkills(FightPlayer1Config.CurrentStage);

		isOpeningRogueLike = true; // 開場觸發

    }

	public void OnRogueLikePanelFinished()
	{
		RogueLikePanelManager.Instance.SetPanelActive(false);

		// 技能與魔力值重製
		SkillManager.Instance.InitialSkillManager();
		MagicPointsManager.Instance.InitialMagicPointsManager();
		PassiveSkillManager.Instance.InitialPassiveSkillManager();
		player1Status.InitStatus(); // 多一次重製玩家

		//ContinueGame();

		//player1.transform.position = new Vector2(0, -2f);

		if (isOpeningRogueLike)
		{
			// 是開場第一次 RogueLike 選擇，不切換關卡，只是繼續遊戲
			isOpeningRogueLike = false;
		}
		else
		{
			// 是戰鬥勝利後的 RogueLike 選擇，進入下一關
			continueMainObjects();
			NextStage();
		}


		OpenStagePanel();
		StartCoroutine(CloseStagePanelDelayed(1.5f));
		PauseGame();
	}



	private IEnumerator HandleStageClearRogueLikeFlow()
	{
		// Step 1：等待 3 秒
		yield return new WaitForSecondsRealtime(3f);

		// Step 2：生成金幣
		//Instantiate(coinFountain, player2.transform.position, Quaternion.Euler(-90, 0, 0));

		// Step 3：再等 3 秒撿金幣
		//yield return new WaitForSecondsRealtime(1f);

		// Step 4：此時再 Pause 遊戲並顯示 RogueLike 面板
		//PauseGame();

		//RogueLikePanelManager.Instance.SetPanelActive(true);
		//RogueLikePanelManager.Instance.DrawSkills(FightPlayer1Config.CurrentStage + 1);

		// 進入下一關
		continueMainObjects();
		NextStage();
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
		ContinueGame();
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
		}
		else
        {
			FightPlayer1Config.CurrentStage = currentStageIndex;
			FightPlayer1Config.CurrentLevel = currentLevelIndex;
			FightPlayer1Config.CurrentChapter = currentChapterIndex;

			SceneManager.LoadScene("FightScene");
		}
	}

	public void LevelFinished()
	{
		PassiveSkillManager.Instance.ResetAllEffect();
		AudioManager.Instance.StopBGM();
		AudioManager.Instance.PlaySFXAtPosition(SFXAudioClips.Instance.WinSoundEffect, new Vector3(0, 0.65f, -20));
		WinPanel.SetActive(true);

		FightPlayer1Config.CurrentStage = 1; // 強制重製

		FightPlayer1Config.Group[1] = "SK00"; // 重製持有技能
		FightPlayer1Config.Group[2] = "SK00"; // 重製持有技能

		RewardManager.Instance.GenerateReward();

		// 當前關卡資訊
		int currentChapter = FightPlayer1Config.CurrentChapter;
		int currentLevel = FightPlayer1Config.CurrentLevel;

		// 嘗試找下一個 Level 的第一個 Stage
		int nextChapter = currentChapter;
		int nextLevel = currentLevel + 1;
		int nextStage = 1;

		StageDataEntry nextStageEntry = StageData.Instance.FindStage(nextChapter, nextLevel, nextStage);

		if (nextChapter == 3 && nextLevel == 3)
        {
			SceneManager.LoadSceneAsync("MainMenuScene");
		}

		if (nextStageEntry == null)
		{
			// 若找不到，嘗試下一個章節的 Level 1
			nextChapter = currentChapter + 1;
			nextLevel = 1;
			nextStageEntry = StageData.Instance.FindStage(nextChapter, nextLevel, nextStage);
		}

		// 更新最高紀錄
		if (nextStageEntry != null)
		{
			if((PlayerDataManager.Instance.GetCurrentChapter() == nextChapter
				&& PlayerDataManager.Instance.GetCurrentLevel() < nextLevel)
				|| (PlayerDataManager.Instance.GetCurrentChapter() < nextChapter)) //最高紀錄同Chapter且當前Level更大 or 最高紀錄Chapter比當前小
			{
				PlayerDataManager.Instance.SetCurrentChapter(nextChapter); // 紀錄最高章節
				PlayerDataManager.Instance.SetCurrentLevel(nextLevel);     // 紀錄最高關卡
			}
		}
		else
		{
			Debug.Log("已達最後關卡，無更多關卡可以解鎖。");
		}
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


			//PlayerDataManager.Instance.SetCurrentChapter(currentChapterIndex); //紀錄最高章節
			//PlayerDataManager.Instance.SetCurrentLevel(currentLevelIndex); //紀錄最高關卡

			SceneManager.LoadScene("FightScene");
		}
		else
		{
			MagicPointsManager.Instance.InitialMagicPointsManager();
			FightPlayer1Config.CurrentStage = currentStageIndex;
			FightPlayer1Config.CurrentLevel = currentLevelIndex;
			FightPlayer1Config.CurrentChapter = currentChapterIndex;

			//PlayerDataManager.Instance.SetCurrentChapter(currentChapterIndex); //紀錄最高章節
			//PlayerDataManager.Instance.SetCurrentLevel(currentLevelIndex); //紀錄最高關卡

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
		else if (FightPlayer2Config.BGM == "CH3 Combat music")
		{
			AudioManager.Instance.PlayBGM(MusicAudioClips.Instance.Ch3BGM);

		}

		if(FightPlayer1Config.CurrentChapter == 1 
			&& FightPlayer1Config.CurrentLevel == 1 
			&& FightPlayer1Config.CurrentStage == 1)
        {
			Debug.Log("Tutorial No RogueLike");
			//FightPlayer1Config.Group[1] = "SK01";
			//FightPlayer1Config.Group[2] = "SK01";
		}
		else
        {
			//開場RogueLike
			StartCoroutine(OpenInitialRogueLikePanelDelayed());
		}
		

		//PauseGame();
		//RogueLikePanelManager.Instance.SetPanelActive(true);
		//RogueLikePanelManager.Instance.DrawSkills(FightPlayer1Config.CurrentStage);

		//技能與魔力值重製
		//SkillManager.Instance.InitialSkillManager();
		//MagicPointsManager.Instance.InitialMagicPointsManager();
	}




	public void PlayDeathAnimation()
	{
		StartCoroutine(SlimeDeathAnimationCoroutine());
	}

	private IEnumerator SlimeDeathAnimationCoroutine()
	{
		// 載入圖片資源
		string resourcePath = "Arts/FightScene/SlimeDeath/" + FightPlayer2Config.PlayerSkin + "Death";
		Debug.Log("嘗試讀取圖片路徑：" + resourcePath);
		Sprite deathSprite = Resources.Load<Sprite>(resourcePath);

		// 顯示面板
		slimeDeathPanel.SetActive(true);

		if (deathSprite != null)
		{
			slimeDeathImage.sprite = deathSprite;
		}
		else
		{
			Debug.LogWarning("找不到死亡圖片資源：" + resourcePath);
		}


		// 等待 1 秒（非受 Time.timeScale 影響）
		yield return new WaitForSecondsRealtime(1f);

		// 關閉面板
		slimeDeathPanel.SetActive(false);
	}


	private IEnumerator DeathCameraZoomCoroutine()
	{
		//Time.timeScale = 0;
		float elapsedTime = 0f;
		Vector3 targetPosition = new Vector3(player2.transform.position.x, player2.transform.position.y, originalCameraPosition.z);

		// Zoom in and move camera
		while (mainCamera.orthographicSize > zoomedCameraSize)
		{
			mainCamera.orthographicSize -= cameraFadeInSpeed * Time.unscaledDeltaTime;
			mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, cameraFadeInSpeed * Time.unscaledDeltaTime);
			yield return null;
		}

		mainCamera.orthographicSize = zoomedCameraSize;
		mainCamera.transform.position = targetPosition;

		// Hold for a moment
		while (elapsedTime < cameraZoomDuration)
		{
			elapsedTime += Time.unscaledDeltaTime;
			yield return null;
		}

		// Zoom out and reset position
		while (mainCamera.orthographicSize < originalCameraSize)
		{
			mainCamera.orthographicSize += cameraFadeOutSpeed * Time.unscaledDeltaTime;
			mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, originalCameraPosition, cameraFadeOutSpeed * Time.unscaledDeltaTime);
			yield return null;
		}

		mainCamera.orthographicSize = originalCameraSize;
		mainCamera.transform.position = originalCameraPosition;
		//Time.timeScale = 1;
	}

	//回傳當前遊戲狀態
	public string GetGameStatus()
    {
		return gameStatus;
    }

	public void SetTimeScale(float tScale)
    {
		Time.timeScale = tScale;
    }

	public void PauseMainObjects()
    {
		//ballController.SetPauseBallMoving(true);
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

	private IEnumerator OpenTutorialDelayed(float delay)
    {
		yield return new WaitForSecondsRealtime(delay);
        TutorialManager.Instance.OpenTutorial();
    }

	public void ContinueGame()
    {
		SetTimeScale(1);

        gameStatus = "Continue";
    }

	private void GameOver()
	{
		FightPlayer1Config.NowHP = player1Status.GetHP();
		FightPlayer1Config.CurrentStage = 1; // 強制重製
		//PauseGame();
		//gameOverPanel.SetActive(true);
		AudioManager.Instance.StopBGM();
		AudioManager.Instance.PlaySFXAtPosition(SFXAudioClips.Instance.LoseSoundEffect, new Vector3(0, 0.65f, -20));
		LosePanel.SetActive(true);

		PauseGame();
		gameStatus = "Gameover";
	}
}
