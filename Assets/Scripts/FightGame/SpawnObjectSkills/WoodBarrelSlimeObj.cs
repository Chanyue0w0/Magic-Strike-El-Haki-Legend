using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodBarrelSlimeObj : MonoBehaviour
{
    [SerializeField] private int playerNumber = 2;
    [SerializeField] private GameObject rollingWoodBarrelSlime;


    [SerializeField] private Animator animator;

    [Header("攻擊時間設定")]
    [SerializeField] private Vector2 attackTimeRange = new Vector2(10f, 15f); // 攻擊間隔範圍
    private float attackTime;

    public void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        rollingWoodBarrelSlime = Resources.Load<GameObject>("Prefabs/MonsterSkills/WoodBarrelSlime");

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
        //animator.SetTrigger("Attack");
        StartCoroutine(DelayedInstBarrel(0f));
    }

    private IEnumerator DelayedInstBarrel(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject obj = Instantiate(rollingWoodBarrelSlime, transform.position, Quaternion.identity);
        obj.GetComponent<Animator>().SetTrigger("Rolling");
        obj.GetComponent<AimPlayerShootSkill>().SetTargetNumber(1);
        Destroy(gameObject);
    }

    private float GetRandomAttackTime()
    {
        return Random.Range(attackTimeRange.x, attackTimeRange.y);
    }
}
