//using System.Collections;
//using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightGameButtonController : MonoBehaviour
{
    [SerializeField] private RoundController roundController;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private LoadingSceneController loadingSceneController;

    // Start is called before the first frame update
    void Start()
    {
        pausePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickPauseGame()
    {
        JToken data = HeroData.Instance.GetHeroData("HR00");
        Debug.Log(data["Name"]);

		roundController.PauseGame();

        pausePanel.SetActive(true);
    }

    public void OnClickContinueGame()
    {
        roundController.ContinueGame();

        pausePanel.SetActive(false);
    }

    public void OnClickNextLevel()
    {
        roundController.NextLevel();
    }

    public void OnClickExit()
    {
        roundController.SetTimeScale(1);

        FightPlayer1Config.Group[1] = "SK00";
        FightPlayer1Config.Group[2] = "SK00";

        FightPlayer1Config.PassiveEffectGroup[0] = "PS00";
        FightPlayer1Config.PassiveEffectGroup[1] = "PS00";

        //Destroy(AudioManager.Instance.gameObject);
        //SceneManager.LoadSceneAsync("MainMenuScene");
        loadingSceneController.LoadStage("MainMenuScene");
    }

    public void OnClickRestart()
    {
        //SceneManager.LoadSceneAsync("FightScene");
        FightPlayer1Config.NowHP = FightPlayer1Config.StartHP;
        FightPlayer1Config.isFirstTimeEnter = true;

        FightPlayer1Config.Group[1] = "SK00";
        FightPlayer1Config.Group[2] = "SK00";

        FightPlayer1Config.PassiveEffectGroup[0] = "PS00";
        FightPlayer1Config.PassiveEffectGroup[1] = "PS00";

        SceneManager.LoadScene("FightScene");
        //Time.timeScale = 1;
    }

    public void PlayClickSound()
    {
		AudioManager.Instance.PlaySFX(SFXAudioClips.Instance.ClickButton);
	}
    //public void OnClickNextLevel()
    //{
    //    //SceneManager.LoadSceneAsync("FightScene");
    //    FightPlayer1Config.CurrentLevel++;
    //    SceneManager.LoadScene("FightScene");
    //    //Time.timeScale = 1;
    //}
}
