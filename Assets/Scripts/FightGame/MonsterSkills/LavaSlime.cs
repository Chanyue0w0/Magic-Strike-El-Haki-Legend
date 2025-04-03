using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaSlime : MonoBehaviour
{
    [SerializeField] private int playerNumber = 2;
    [SerializeField] private float instSkillArriveTime = 2;
    [SerializeField] private GameObject instSmoke;//魔法煙霧(移動至生成定點再產生史萊姆)
    [SerializeField] private GameObject lavaPondObject;

    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;

    [SerializeField] private Vector2 targetPosition;

    [SerializeField] private AIController player2_controller;

    [SerializeField] private Animator player2_animator;


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
        instSmoke = Resources.Load<GameObject>("Prefabs/MonsterSkills/MagicPowerGain_Red");
        lavaPondObject = Resources.Load<GameObject>("Prefabs/MonsterSkills/LavaPond");//FlashAttack
    }

    public void Active()
    {
        player2_animator.SetTrigger("Attack");

        player2_controller.SetStopMoving(true);

        targetPosition = player1.transform.position;

        GameObject CFS = Instantiate(instSmoke, player2.transform.position, Quaternion.identity);
        moveToPositionSkill moveScript = CFS.GetComponent<moveToPositionSkill>();
        moveScript.SetStartPosition(player2.transform.position);
        moveScript.SetTargetPosition(targetPosition);
        moveScript.SetArriveTime(instSkillArriveTime);

        StartCoroutine(DelayedInstLava(2f));

        StartCoroutine(DelayedStartMoving(2f));

    }

    private IEnumerator DelayedInstLava(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject obj = Instantiate(lavaPondObject, targetPosition, Quaternion.Euler(0, 0, 0));
        //obj.GetComponent<AimPlayerShootSkill>().SetTargetNumber(1);
    }

    private IEnumerator DelayedStartMoving(float delay)
    {
        yield return new WaitForSeconds(delay);
        player2_controller.SetStopMoving(false);
    }
}
