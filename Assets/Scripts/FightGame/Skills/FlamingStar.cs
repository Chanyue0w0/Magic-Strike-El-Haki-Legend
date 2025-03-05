using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamingStar : MonoBehaviour
{
    [SerializeField] private int playerNumber = 1;
    //[SerializeField] private float OriginalShieldPercentage = 0.0f;
    [SerializeField] private int skillDamage = 100;
    [SerializeField] private int skillMoveSpeed = 10;
    [SerializeField] private GameObject chargeFireShield;
    [SerializeField] private GameObject flamingStarObj;

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
            skillDamageTMP = skillDamage * (1 + FightPlayer1Config.SkillDamageIncrease);
        }
        else
        {
            skillDamageTMP = skillDamage * (1 + FightPlayer2Config.SkillDamageIncrease);
        }
        skillDamage = Mathf.RoundToInt(skillDamageTMP);
        flamingStarObj = Resources.Load<GameObject>("Prefabs/Skills/FlamingStar");
        chargeFireShield = Resources.Load<GameObject>("Prefabs/Effect/FireShield");
        player1 = GameObject.FindGameObjectWithTag("Player1");
        player2 = GameObject.FindGameObjectWithTag("Player2");
    }

    public void Active()
    {
       
        if (playerNumber == 1)
        {
            GameObject CFS = Instantiate(chargeFireShield, player1.transform.position, Quaternion.identity);
            CFS.transform.SetParent(player1.transform);
            StartCoroutine(DelayedInstFlamingStar(1, 0.8f));
        }
        else
        {
            GameObject CFS = Instantiate(chargeFireShield, player2.transform.position, Quaternion.identity);
            CFS.transform.SetParent(player2.transform);
            StartCoroutine(DelayedInstFlamingStar(2, 0.8f));
        }

        
    }


    private IEnumerator DelayedInstFlamingStar(int pNumber ,float delay)
    {
        yield return new WaitForSeconds(delay);
        if(pNumber == 1)
        {
            GameObject obj = Instantiate(flamingStarObj, player1.transform.position, Quaternion.identity);
            obj.GetComponent<AimPlayerShootSkill>().SetPlayerNumber(playerNumber);
            obj.GetComponent<AimPlayerShootSkill>().SetTargetDirection(player2.transform.position);
            obj.GetComponent<AimPlayerShootSkill>().SetSkillDamage(skillDamage);
        }
        else
        {
            GameObject obj = Instantiate(flamingStarObj, player2.transform.position, Quaternion.identity);
            obj.GetComponent<AimPlayerShootSkill>().SetPlayerNumber(playerNumber);
            obj.GetComponent<AimPlayerShootSkill>().SetTargetDirection(player1.transform.position);
            obj.GetComponent<AimPlayerShootSkill>().SetSkillDamage(skillDamage);
        }
    }


}
