using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineSlimeObj : MonoBehaviour
{
    [SerializeField] private int playerNumber = 2;
    [SerializeField] private GameObject graspingVineObject;

    [SerializeField] private Animator animator;

    [Header("攻擊時間設定")]
    [SerializeField] private Vector2 attackTimeRange = new Vector2(10f, 15f); // 攻擊間隔範圍
    private float attackTime;

    public void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        graspingVineObject = Resources.Load<GameObject>("Prefabs/MonsterSkills/GraspingVine");

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
        GameObject obj = Instantiate(graspingVineObject
            , new Vector2(gameObject.transform.position.x - 0.23f, gameObject.transform.position.y - 0.8f)
            , Quaternion.identity);
        // 設定回傳來源
        GraspingVine gv = obj.GetComponent<GraspingVine>();
        if (gv != null)
        {
            gv.SetVineSlimeObj(this);
        }
        obj.GetComponent<AimPlayerShootSkill>().SetTargetNumber(1);
    }

    private float GetRandomAttackTime()
    {
        return Random.Range(attackTimeRange.x, attackTimeRange.y);
    }

    public void OnGraspSuccess()
    {
        animator.SetTrigger("GraspGot");
    }
}
