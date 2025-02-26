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
        SceneManager.LoadSceneAsync("Main menu");
    }

    public void OnClickRestart()
    {
        //SceneManager.LoadSceneAsync("FightScene");
        SceneManager.LoadScene("FightScene");
        Time.timeScale = 1;
    }
}
