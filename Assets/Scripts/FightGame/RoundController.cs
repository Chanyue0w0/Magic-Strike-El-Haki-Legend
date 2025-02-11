//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class RoundController : MonoBehaviour
{
    [SerializeField] private float timeScale = 1f;
    [SerializeField] private string GameStatus = "Continues";


    //// Start is called before the first frame update
    //void Start()
    //{
            
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    // p1 or p2 hp == 0 end game
    //    //GameStatus = "End" + p1orp2 = Win
    //}

    public void PauseGame()
    {
        Time.timeScale = 0;

        GameStatus = "Pause Game";
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;

        GameStatus = "Continues";
    }
}
