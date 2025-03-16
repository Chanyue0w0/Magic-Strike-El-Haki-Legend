//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

	[Header("----------------- Now Stage Info ------------------")]
	[SerializeField] private int currentStageIndex = 0; // 當前關卡索引

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
	}

	//// Start is called before the first frame update
	void Start()
	{
		Application.targetFrameRate = 60;

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
			SceneManager.LoadScene("FightScene");
		}
	}

	public void NextStage()
    {
		currentStageIndex++;
		FightPlayer1Config.CurrentStage = currentStageIndex;
	}

	public void GameStart()
    {
		Time.timeScale = 1;

		// 透過 StageData Singleton 取得 "StageNumber" 為 currentStageIndex 的關卡
		StageDataEntry currentStage = StageData.Instance.FindStageByNumber(currentStageIndex);
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
			FightPlayer2Config.PlayerSkin = currentStage.Player2Skin;
			FightPlayer2Config.PuckSkin = currentStage.Player2PuckSkin;
			FightPlayer2Config.Group = currentStage.player2_Group.ToArray();

			Debug.Log("Stage Loaded: " + currentStage.StageNumber);
			Debug.Log("Player2 Group: " + string.Join(", ", FightPlayer2Config.Group));
		}

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
