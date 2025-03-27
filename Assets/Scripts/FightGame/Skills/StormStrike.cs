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
    [SerializeField] private float skillGap = 0.5f; // 每個雷擊之間最小距離

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
        List<Vector2> usedPositions = new List<Vector2>();

        for (int i = 0; i < skillAmount; i++)
        {
            Vector2 randomPos;
            int maxAttempts = 30; // 最多嘗試 30 次避免死循環
            int attempt = 0;

            do
            {
                if (playerNumber == 1)
                {
                    randomPos = new Vector2(
                        Random.Range(P2Field_instPositionXRange.y, P2Field_instPositionXRange.x),
                        Random.Range(P2Field_instPositionYRange.x, P2Field_instPositionYRange.y)
                    );
                }
                else
                {
                    randomPos = new Vector2(
                        Random.Range(P1Field_instPositionXRange.y, P1Field_instPositionXRange.x),
                        Random.Range(P1Field_instPositionYRange.x, P1Field_instPositionYRange.y)
                    );
                }

                attempt++;

                // 如果嘗試太多次就強制跳出（避免卡住）
                if (attempt > maxAttempts) break;

            } while (!IsFarEnough(randomPos, usedPositions, skillGap));

            usedPositions.Add(randomPos);

            // 生成警告
            Instantiate(WarningEffectObj, randomPos, Quaternion.identity);

            // 延遲生成閃電
            StartCoroutine(SpawnLightningAfterWarning(randomPos, skillWarningTime));

            yield return new WaitForSeconds(skillInstTimeGap);
        }
    }

    private bool IsFarEnough(Vector2 candidate, List<Vector2> existingPoints, float minDistance)
    {
        foreach (Vector2 point in existingPoints)
        {
            if (Vector2.Distance(candidate, point) < minDistance)
            {
                return false;
            }
        }
        return true;
    }


    private IEnumerator SpawnLightningAfterWarning(Vector2 position, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject obj = Instantiate(lightningStrikeObj, position, Quaternion.identity);
        obj.GetComponent<HammerRippleAttack>().SetPlayerNumber(playerNumber);
        obj.GetComponent<HammerRippleAttack>().SetDamage(skillDamage);
    }

}
