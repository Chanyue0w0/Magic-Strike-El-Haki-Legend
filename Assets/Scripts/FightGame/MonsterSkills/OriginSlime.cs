using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OriginSlime : MonoBehaviour
{
    [SerializeField] private int playerNumber = 1;
    [SerializeField] private int slimeAmount = 3;
    [SerializeField] private float instSlimeTime = 1;
    [SerializeField] private GameObject instSmoke;//魔法煙霧(移動至生成定點再產生史萊姆)
    [SerializeField] private GameObject grassSlimeObject;
    [SerializeField] private Vector2 instPositionXRange = new Vector2(1.42f, -1.42f);
    [SerializeField] private Vector2 instPositionYRange = new Vector2(0.35f, 2.94f);

    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;


    public void SetPlayerNumber(int pNumber) // initial
    {
        playerNumber = pNumber;
        InitializedSkillInfo();
    }

    public void InitializedSkillInfo()
    {
        player1 = GameObject.FindGameObjectWithTag("Player1");
        player2 = GameObject.FindGameObjectWithTag("Player2");
        //instSmoke = Resources.Load<GameObject>("Prefabs/MonsterSkills/MagicPowerGain_Yellow");
        //grassSlimeObject = Resources.Load<GameObject>("Prefabs/MonsterSkills/GrassSlime");
    }

    public void Active()
    {
        return;
    }
}
