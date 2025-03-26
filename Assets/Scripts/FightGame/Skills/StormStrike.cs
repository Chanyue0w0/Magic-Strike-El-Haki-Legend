using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormStrike : MonoBehaviour
{
    [SerializeField] private int playerNumber = 1;
    //[SerializeField] private float OriginalShieldPercentage = 0.0f;
    [SerializeField] private int skillDamage = 30;
    [SerializeField] private int skillAmount = 10;
    [SerializeField] private float skillInstTimeGap = 0.1f;//每次雷擊生成時間差
    [SerializeField] private float skillWarningTime = 1f;//警告持續
    //[SerializeField] private int skillMoveSpeed = 10;
    [SerializeField] private GameObject lightningStrikeObj;
    [SerializeField] private GameObject WarningEffectObj;

    //[SerializeField] private GameObject player1;
    //[SerializeField] private GameObject player2;
    [SerializeField] private Vector2 P1Field_instPositionXRange = new Vector2(1.42f, -1.42f);
    [SerializeField] private Vector2 P1Field_instPositionYRange = new Vector2(-0.35f, -2.94f);

    [SerializeField] private Vector2 P2Field_instPositionXRange = new Vector2(1.42f, -1.42f);
    [SerializeField] private Vector2 P2Field_instPositionYRange = new Vector2(0.35f, 2.94f);


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
        lightningStrikeObj = Resources.Load<GameObject>("Prefabs/Skills/LightningStrike");
        
        WarningEffectObj = Resources.Load<GameObject>("Prefabs/Effect/OvalWarning");
        //chargeFireShield = Resources.Load<GameObject>("Prefabs/Effect/FireShield");
        //player1 = GameObject.FindGameObjectWithTag("Player1");
        //player2 = GameObject.FindGameObjectWithTag("Player2");
    }

    public void Active()
    {
        StartCoroutine(DelayedInstLightningStrike());
    }

    private IEnumerator DelayedInstLightningStrike()
    {
        for (int i = 0; i < skillAmount; i++)
        {
            Vector2 randomPos;

            if (playerNumber == 1)
            {
                // player 1 使用技能，打 player 2 的場地
                randomPos = new Vector2(
                    Random.Range(P2Field_instPositionXRange.y, P2Field_instPositionXRange.x),
                    Random.Range(P2Field_instPositionYRange.x, P2Field_instPositionYRange.y)
                );
            }
            else
            {
                // player 2 使用技能，打 player 1 的場地
                randomPos = new Vector2(
                    Random.Range(P1Field_instPositionXRange.y, P1Field_instPositionXRange.x),
                    Random.Range(P1Field_instPositionYRange.x, P1Field_instPositionYRange.y)
                );
            }

            // 生成警告特效
            GameObject warning = Instantiate(WarningEffectObj, randomPos, Quaternion.identity);

            // 等待 skillWarningTime 秒後再生成閃電攻擊
            StartCoroutine(SpawnLightningAfterWarning(randomPos, skillWarningTime));

            // 每次生成的時間間隔
            yield return new WaitForSeconds(skillInstTimeGap);
        }
    }


    private IEnumerator SpawnLightningAfterWarning(Vector2 position, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject obj = Instantiate(lightningStrikeObj, position, Quaternion.identity);
        obj.GetComponent<HammerRippleAttack>().SetPlayerNumber(playerNumber);
        obj.GetComponent<HammerRippleAttack>().SetDamage(skillDamage);
    }

}
