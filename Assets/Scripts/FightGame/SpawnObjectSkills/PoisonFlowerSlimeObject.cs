using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonFlowerSlimeObject : MonoBehaviour
{
    [SerializeField] private int playerNumber = 2;
    [SerializeField] private GameObject poisonCloudObject;

    //[SerializeField] private GameObject player1;
    //[SerializeField] private GameObject player2;

    [SerializeField] private Animator animator;

    [Header("攻擊時間設定")]
    [SerializeField] private Vector2 attackTimeRange = new Vector2(10f, 15f); // 攻擊間隔範圍
    private float attackTime;

    public void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        poisonCloudObject = Resources.Load<GameObject>("Prefabs/MonsterSkills/PoisonCloud");

        attackTime = GetRandomAttackTime();
    }

    public void Update()
    {
        attackTime -= Time.deltaTime;

        if (attackTime <= 0f)
        {
            Active();
            attackTime = GetRandomAttackTime();
        }
    }

    public void Active()
    {
        animator.SetTrigger("Attack");
        StartCoroutine(DelayedInstPoison(1f));
    }

    private IEnumerator DelayedInstPoison(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject obj = Instantiate(poisonCloudObject, transform.position, Quaternion.Euler(90, 0, 0));
        obj.GetComponent<AimPlayerShootSkill>().SetTargetNumber(1);
    }

    private float GetRandomAttackTime()
    {
        return Random.Range(attackTimeRange.x, attackTimeRange.y);
    }
}
