using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoneSkill : MonoBehaviour
{
    [SerializeField] private int playerNumber = 1;
    [SerializeField] private int baseSkillDamage = 100;
    [SerializeField] private int skillDamage = 100;

    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;


    public void SetPlayerNumber(int pNumber) // initial
    {
        playerNumber = pNumber;
        InitializedSkillInfo();
    }

    public void InitializedSkillInfo()
    {
        float skillDamageTMP = 0;
        if (playerNumber == 1)
        {
            skillDamageTMP = baseSkillDamage * (1 + FightPlayer1Config.SkillDamageIncrease);
        }
        else
        {
            skillDamageTMP = baseSkillDamage * (1 + FightPlayer2Config.SkillDamageIncrease);
        }
        player1 = GameObject.FindGameObjectWithTag("Player1");
        player2 = GameObject.FindGameObjectWithTag("Player2");
    }

    public void Active()
    {


    }

}
