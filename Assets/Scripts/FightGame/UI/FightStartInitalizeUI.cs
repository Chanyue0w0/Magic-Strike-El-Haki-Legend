//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class FightStartInitalizeUI: MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject FightPanel;

	// Start is called before the first frame update
	void Start()
    {
        gameOverPanel.SetActive(false);
        FightPanel.SetActive(true);
        
    }

}
