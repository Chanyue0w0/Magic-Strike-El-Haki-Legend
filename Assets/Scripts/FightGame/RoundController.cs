//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class RoundController : MonoBehaviour
{

	[Header("----------------- Value ------------------")]
	//[SerializeField] private float timeScale = 1f;
	[SerializeField] private string gameStatus = "Continue";


	[Header("----------------- Variable Reference ------------------")]
	[SerializeField] private PlayerStatusManager player1Status;
	[SerializeField] private PlayerStatusManager player2Status;

	[SerializeField] private GameObject gameOverPanel;
	//// Start is called before the first frame update
	void Start()
	{
		Application.targetFrameRate = 60;
	}


	// Update is called once per frame
	void Update()
	{
		// p1 or p2 hp == 0 end game
		if (player1Status.GetHP() == 0)
		{
			GameOver();
			// defeat
		}
		else if (player2Status.GetHP() == 0)
		{
			GameOver();
			// win

		}
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
		PauseGame();
		gameOverPanel.SetActive(true);

		gameStatus = "gameover";
	}
}
