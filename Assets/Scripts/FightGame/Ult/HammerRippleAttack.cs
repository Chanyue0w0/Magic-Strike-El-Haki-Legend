using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerRippleAttack : MonoBehaviour
{
    [Header("----------------- PlayerGameObject ------------------")]
    //[SerializeField] private GameObject player1;
    //[SerializeField] private GameObject player2;
    [Header("----------------- Data ------------------")]
    [SerializeField] private int playerNumber;
    [SerializeField] private int targetNumber;
    [SerializeField] private int attackDamage = 0; // 傷害值
    [SerializeField] private StatusEffect EffectToApply = StatusEffect.Stun; // 要套用的狀態

    [Header("----------------- GameObjects ------------------")]
    [SerializeField] private GameObject explosion;

    void Start()
    {
        //explosion = (GameObject)Resources.Load("Prefabs/Skills/ElectircExplosion", typeof(GameObject)); //抓取粒子特效 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayerNumber(int pNumber)
    {
        playerNumber = pNumber;
    }

    public void SetDamage(int damage)
    {
        attackDamage = damage;
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
            if(explosion != null)
                Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
            //doorScore.DoorTracingAttackOnHit();
            // 嘗試獲取 IDamageable 介面（目標可受傷）
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }

            // 嘗試獲取 IStatusEffectReceiver 介面（目標可受 Buff/Debuff）
            IStatusEffectReceiver statusReceiver = collision.GetComponent<IStatusEffectReceiver>();
            if (statusReceiver != null)
            {
                statusReceiver.ApplyStatusEffect(EffectToApply);
                //Debug.Log($"{collision.gameObject.name} 受到狀態影響：{EffectToApply}");
            }
            //Destroy(gameObject);
        }
    }
}
