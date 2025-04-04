using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineSlime : MonoBehaviour
{
    [SerializeField] private int playerNumber = 2;
    //[SerializeField] private int slimeAmount = 3;
    [SerializeField] private GameObject graspingVineObject;

    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;

    [SerializeField] private AIController player2_controller;

    [SerializeField] private Animator player2_animator;

    // 儲存目前的 Coroutine
    private Coroutine graspingVineCoroutine;
    private Coroutine startMovingCoroutine;

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
        //instSmoke = Resources.Load<GameObject>("Prefabs/MonsterSkills/MagicPowerGain_Yellow");
        graspingVineObject = Resources.Load<GameObject>("Prefabs/MonsterSkills/GraspingVine");
    }

    public void Active()
    {
        player2_animator.SetTrigger("Attack");

        player2_controller.SetStopMoving(true);

        // 停止上一輪的 coroutine（如果還在執行）
        if (graspingVineCoroutine != null)
        {
            StopCoroutine(graspingVineCoroutine);
        }
        if (startMovingCoroutine != null)
        {
            StopCoroutine(startMovingCoroutine);
        }

        // 啟動新的 coroutine，並儲存引用
        graspingVineCoroutine = StartCoroutine(DelayedInstGraspingVine(1f));
        startMovingCoroutine = StartCoroutine(DelayedStartMoving(3.5f));

    }
    private IEnumerator DelayedInstGraspingVine(float delay)
    {
        yield return new WaitForSeconds(delay);// (-0.23f,-0.7f)
        GameObject obj = Instantiate(graspingVineObject
            , new Vector2(player2.transform.position.x - 0.23f, player2.transform.position.y - 0.8f)
            , Quaternion.identity);
        // 設定回傳來源
        GraspingVine gv = obj.GetComponent<GraspingVine>();
        if (gv != null)
        {
            gv.SetVineSlime(this);
        }
    }
    private IEnumerator DelayedStartMoving(float delay)
    {
        yield return new WaitForSeconds(delay);
        player2_controller.SetStopMoving(false);
    }

    public void OnGraspSuccess()
    {
        player2_animator.SetTrigger("GraspGot");
    }

    //public void StopAllCoroutine()
    //{
    //    player2_animator.SetTrigger("GraspGot");
    //    // 停止上一輪的 coroutine（如果還在執行）
    //    if (graspingVineCoroutine != null)
    //    {
    //        StopCoroutine(graspingVineCoroutine);
    //    }
    //    if (startMovingCoroutine != null)
    //    {
    //        StopCoroutine(startMovingCoroutine);
    //    }
    //}

}
