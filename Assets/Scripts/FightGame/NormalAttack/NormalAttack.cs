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
    [SerializeField] private float rotationDistanceThreshold = 1.0f; // ����Z���H��
    [SerializeField] private int NormalAttackDamage = 0; // �ˮ`��
    [SerializeField] private StatusEffect EffectToApply = StatusEffect.Stun; // �n�M�Ϊ����A
    [SerializeField] private Rigidbody2D rb;

    [Header("----------------- GameObjects ------------------")]
    [SerializeField] private GameObject explosion;

    void Start()
    {
        player1 = GameObject.FindGameObjectWithTag("Player1");
        player2 = GameObject.FindGameObjectWithTag("Player2");
        rb = gameObject.GetComponent<Rigidbody2D>();
        //explosion = (GameObject)Resources.Load("Prefabs/Skills/ElectircExplosion", typeof(GameObject)); //����ɤl�S�� 
    }

    // Update is called once per frame
    void Update()
    {
        // �����e�ؼЪ��a����m
        Vector2 targetPosition;

        if (targetNumber == 1)
        {
            if (player1 != null)
                targetPosition = player1.transform.position;
            else
                return; // �p�G�ؼФ��s�b�A�h�X��s
        }
        else
        {
            if (player2 != null)
                targetPosition = player2.transform.position;
            else
                return; // �p�G�ؼФ��s�b�A�h�X��s
        }

        // �p���V�ò���
        Vector2 currentPosition = rb.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;
        rb.velocity = direction * speed;

        // �P�_�Z���O�_�j���H�ȦA����
        float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);
        if (distanceToTarget > rotationDistanceThreshold)
        {
            // �p����ਤ�רó]�m�]�¦V�ؼЪ��@�ɮy�С^
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // �ץ����װ����A�O�Ҫ���e��¦V�ؼ�
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
            // ������� IDamageable �����]�ؼХi���ˡ^
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(NormalAttackDamage);
                damageable.GetMagicPointNotify(playerNumber);
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
