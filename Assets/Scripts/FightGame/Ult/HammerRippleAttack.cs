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
    [SerializeField] private int attackDamage = 0; // �ˮ`��
    [SerializeField] private StatusEffect EffectToApply = StatusEffect.Stun; // �n�M�Ϊ����A

    [Header("----------------- GameObjects ------------------")]
    [SerializeField] private GameObject explosion;

    void Start()
    {
        //explosion = (GameObject)Resources.Load("Prefabs/Skills/ElectircExplosion", typeof(GameObject)); //����ɤl�S�� 
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
            // ������� IDamageable �����]�ؼХi���ˡ^
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }

            // ������� IStatusEffectReceiver �����]�ؼХi�� Buff/Debuff�^
            IStatusEffectReceiver statusReceiver = collision.GetComponent<IStatusEffectReceiver>();
            if (statusReceiver != null)
            {
                statusReceiver.ApplyStatusEffect(EffectToApply);
                //Debug.Log($"{collision.gameObject.name} ���쪬�A�v�T�G{EffectToApply}");
            }
            //Destroy(gameObject);
        }
    }
}
