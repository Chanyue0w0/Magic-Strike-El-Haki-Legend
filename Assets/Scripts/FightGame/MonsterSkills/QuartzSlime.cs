using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuartzSlime : MonoBehaviour
{
    [SerializeField] private int playerNumber = 2;
    [SerializeField] private float instSkillArriveTime = 1.5f;
    [SerializeField] private GameObject instSmoke;//魔法煙霧(移動至生成定點再產生史萊姆)
    [SerializeField] private GameObject FlashAttackObject;

    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;

    [SerializeField] private Vector2 targetPosition;

    [SerializeField] private AIController player2_controller;

    [SerializeField] private Animator player2_animator;

    [SerializeField] private float instPositionGap = 0.7f; // 每隔這個距離產生一個攻擊物件

    public void SetPlayerNumber(int pNumber) // initial
    {
        playerNumber = pNumber;
        InitializedSkillInfo();
    }

    public void InitializedSkillInfo()
    {
        player1 = GameObject.FindGameObjectWithTag("Player1");
        player2 = GameObject.FindGameObjectWithTag("Player2");
        player2_controller = GameObject.Find("Player 2 Manager").GetComponent<AIController>();
        player2_animator = GameObject.Find("Player2Sprite").GetComponent<Animator>();
        instSmoke = Resources.Load<GameObject>("Prefabs/MonsterSkills/MagicPowerGain_White");
        FlashAttackObject = Resources.Load<GameObject>("Prefabs/MonsterSkills/FlashAttack");//FlashAttack
    }

    public void Active()
    {
        player2_animator.SetTrigger("Attack");

        player2_controller.SetStopMoving(true);

        //targetPosition = player1.transform.position;
        targetPosition = new Vector2(player2.transform.position.x, -3.57f);

        GameObject CFS = Instantiate(instSmoke, player2.transform.position, Quaternion.identity);
        moveToPositionSkill moveScript = CFS.GetComponent<moveToPositionSkill>();
        moveScript.SetStartPosition(player2.transform.position);
        moveScript.SetTargetPosition(targetPosition);
        moveScript.SetArriveTime(instSkillArriveTime);

        //StartCoroutine(DelayedInstFlash(1f));

        // 延遲後啟動攻擊特效生成
        StartCoroutine(DelayedSpawnFlashes(1f));

        // 啟動多點攻擊生成 Coroutine
        //StartCoroutine(SpawnFlashesAlongPath(player2.transform.position, targetPosition, instSkillArriveTime, instPositionGap));


        StartCoroutine(DelayedStartMoving(2f));

    }

    private IEnumerator DelayedSpawnFlashes(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(SpawnFlashesAlongPath(player2.transform.position, targetPosition, instSkillArriveTime, instPositionGap));
    }


    private IEnumerator SpawnFlashesAlongPath(Vector2 startPos, Vector2 endPos, float totalTime, float gap)
    {
        Vector2 direction = (endPos - startPos).normalized;
        float distance = Vector2.Distance(startPos, endPos);
        int spawnCount = Mathf.FloorToInt(distance / gap);
        float intervalTime = totalTime / spawnCount;

        for (int i = 1; i <= spawnCount; i++)
        {
            Vector2 spawnPos = startPos + direction * gap * i;
            Instantiate(FlashAttackObject, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(intervalTime);
        }
    }


    private IEnumerator DelayedInstFlash(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject obj = Instantiate(FlashAttackObject, targetPosition, Quaternion.Euler(0, 0, 0));
        //obj.GetComponent<AimPlayerShootSkill>().SetTargetNumber(1);
    }

    private IEnumerator DelayedStartMoving(float delay)
    {
        yield return new WaitForSeconds(delay);
        player2_controller.SetStopMoving(false);
    }
}
