using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonFlowerSlime : MonoBehaviour
{
    [SerializeField] private int playerNumber = 2;
    [SerializeField] private GameObject poisonCloudObject;

    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;

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
        //instSmoke = Resources.Load<GameObject>("Prefabs/MonsterSkills/MagicPowerGain_Yellow");
        poisonCloudObject = Resources.Load<GameObject>("Prefabs/MonsterSkills/PoisonCloud");
    }

    public void Active()
    {
        player2_animator.SetTrigger("Attack");

        player2_controller.SetStopMoving(true);

        StartCoroutine(DelayedInstPoison(1f));

        StartCoroutine(DelayedStartMoving(3f));

    }
    private IEnumerator DelayedInstPoison(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject obj = Instantiate(poisonCloudObject, player2.transform.position, Quaternion.Euler(90, 0, 0));
        obj.GetComponent<AimPlayerShootSkill>().SetTargetNumber(1);
    }
    private IEnumerator DelayedStartMoving(float delay)
    {
        yield return new WaitForSeconds(delay);
        player2_controller.SetStopMoving(false);
    }

    //private IEnumerator DelayedInstSlime(Vector2 position, float delay)
    //{
    //    yield return new WaitForSeconds(delay);

    //    GameObject obj = Instantiate(windmillSlimeObject, position, Quaternion.identity);
    //    obj.GetComponent<Animator>().SetTrigger("Blowing");
    //}
}
