using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : MonoBehaviour
{
    [Header("----------------- PlayerGameObject ------------------")]
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    [Header("----------------- Data ------------------")]
    [SerializeField] private int playerNumber;
    [SerializeField] private int targetNumber;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationDistanceThreshold = 1.0f; // 旋轉距離閾值
    [SerializeField] private int NormalAttackDamage = 0; // 傷害值
    [SerializeField] private StatusEffect EffectToApply = StatusEffect.Stun; // 要套用的狀態
    [SerializeField] private Rigidbody2D rb;

    [Header("----------------- GameObjects ------------------")]
    [SerializeField] private GameObject explosion;

    void Start()
    {
        player1 = GameObject.FindGameObjectWithTag("Player1");
        player2 = GameObject.FindGameObjectWithTag("Player2");
        rb = gameObject.GetComponent<Rigidbody2D>();
        //explosion = (GameObject)Resources.Load("Prefabs/Skills/ElectircExplosion", typeof(GameObject)); //抓取粒子特效 
    }

    // Update is called once per frame
    void Update()
    {
        // 獲取當前目標玩家的位置
        Vector2 targetPosition;

        if (targetNumber == 1)
        {
            if (player1 != null)
                targetPosition = player1.transform.position;
            else
                return; // 如果目標不存在，退出更新
        }
        else
        {
            if (player2 != null)
                targetPosition = player2.transform.position;
            else
                return; // 如果目標不存在，退出更新
        }

        // 計算方向並移動
        Vector2 currentPosition = rb.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;
        rb.velocity = direction * speed;

        // 判斷距離是否大於閾值再旋轉
        float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);
        if (distanceToTarget > rotationDistanceThreshold)
        {
            // 計算旋轉角度並設置（朝向目標的世界座標）
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // 修正角度偏移，保證物體前方朝向目標
            rb.rotation = angle;
        }
    }

    public void SetPlayerNumber(int pNumber)
    {
        playerNumber = pNumber;
    }

    public void SetDamage(int damage)
    {
        NormalAttackDamage = damage;
    }    

    public void SetTargetNumber(int tNumber)
    {
        targetNumber = tNumber;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetNumber == 1 && collision.CompareTag("Player1") 
            || (targetNumber == 2 && collision.CompareTag("Player2")))
        {
            Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
            //doorScore.DoorTracingAttackOnHit();
            // 嘗試獲取 IDamageable 介面（目標可受傷）
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(NormalAttackDamage);
                damageable.GetMagicPointNotify(playerNumber);
                //Debug.Log($"{collision.gameObject.name} 受到 {NormalAttackDamage} 傷害！");
            }

            // 嘗試獲取 IStatusEffectReceiver 介面（目標可受 Buff/Debuff）
            IStatusEffectReceiver statusReceiver = collision.GetComponent<IStatusEffectReceiver>();
            if (statusReceiver != null)
            {
                statusReceiver.ApplyStatusEffect(EffectToApply);
                //Debug.Log($"{collision.gameObject.name} 受到狀態影響：{EffectToApply}");
            }

            Destroy(gameObject);


        }
        
    }
}
