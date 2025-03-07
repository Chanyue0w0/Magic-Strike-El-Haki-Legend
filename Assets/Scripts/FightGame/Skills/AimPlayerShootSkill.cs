using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimPlayerShootSkill : MonoBehaviour
{
    private enum AimingType { targetDirection, targetPosition, trackingPlayer };
    [Header("----------------- Config Setting ------------------")]
    [SerializeField] private AimingType aimingType;

    [Header("----------------- Skill info ------------------")]
    [SerializeField] private int playerNumber = 1;
    [SerializeField] private int targetNumber = 2;
    [SerializeField] private int skillDamage = 100;
    [SerializeField] private int skillMoveSpeed = 10;
    [SerializeField] private GameObject explosion;

    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;

    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Vector3 moveDirection; // ���ʤ�V
    [SerializeField] private StatusEffect EffectToApply = StatusEffect.None; // �n�M�Ϊ����A

    private void Start()
    {
        player1 = GameObject.FindGameObjectWithTag("Player1");
        player2 = GameObject.FindGameObjectWithTag("Player2");

    }

    private void Update()
    {
        if(aimingType == AimingType.targetDirection)
        {
            RotateTowardsDirection();
            MoveTowardsDirection();
        }
        //else if(aimingType == AimingType.trackingPlayer)
        //{

        //}

    }
    private void RotateTowardsDirection()
    {
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void MoveTowardsDirection()
    {
        transform.position += moveDirection * skillMoveSpeed * Time.deltaTime;
    }

    public void SetPlayerNumber(int pNumber) // initial
    {
        playerNumber = pNumber;
    }

    public void SetTargetNumber(int tNumber) // initial
    {
        targetNumber = tNumber;
    }
    public void SetSkillDamage(int damage)
    {
        skillDamage = damage;
    }

    public void SetTargetPosition(Vector3 tPosition)
    {
        targetPosition = tPosition;
    }

    public void SetTargetDirection(Vector3 tPosition)
    {
        // �p���V�V�q�]�зǤơ^
        moveDirection = (tPosition - transform.position).normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetNumber == 1 && collision.CompareTag("Player1")
            || (targetNumber == 2 && collision.CompareTag("Player2")))
        {
            if (explosion != null)
                Instantiate(explosion, collision.transform.position, Quaternion.identity);

            //doorScore.DoorTracingAttackOnHit();
            // ������� IDamageable �����]�ؼХi���ˡ^
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(skillDamage);
                //Debug.Log($"{collision.gameObject.name} ���� {NormalAttackDamage} �ˮ`�I");
            }

            // ������� IStatusEffectReceiver �����]�ؼХi�� Buff/Debuff�^
            IStatusEffectReceiver statusReceiver = collision.GetComponent<IStatusEffectReceiver>();
            if (statusReceiver != null)
            {
                statusReceiver.ApplyStatusEffect(EffectToApply);
                //Debug.Log($"{collision.gameObject.name} ���쪬�A�v�T�G{EffectToApply}");
            }
            Destroy(gameObject);

        }
        
    }
}
