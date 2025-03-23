//using System.Collections;
//using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightGameButtonController : MonoBehaviour
{
    [SerializeField] private RoundController roundController;
    [SerializeField] private GameObject pausePanel;

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

    public void OnClickExit()
    {
        Destroy(AudioManager.Instance.gameObject);
        SceneManager.LoadSceneAsync("MainMenuScene");
    }

    public void OnClickRestart()
    {
        //SceneManager.LoadSceneAsync("FightScene");
        FightPlayer1Config.NowHP = FightPlayer1Config.StartHP;
        SceneManager.LoadScene("FightScene");
        //Time.timeScale = 1;
    }

    public void OnClickNextLevel()
    {
        //SceneManager.LoadSceneAsync("FightScene");
        FightPlayer1Config.CurrentLevel++;
        SceneManager.LoadScene("FightScene");
        //Time.timeScale = 1;
    }
}
