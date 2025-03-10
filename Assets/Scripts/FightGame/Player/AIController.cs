using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Vector2 startingPosition;
    [SerializeField] private Animator animator;

    [SerializeField] private GameObject player2;
    [SerializeField] private CircleCollider2D player2Collider;
    [SerializeField] private Rigidbody2D Ball;
    [SerializeField] private GameObject ball;
    [SerializeField] private BallController ballController;
    [SerializeField] private float AISightAlpha = 0.8f;

    [SerializeField] private PlayerCollisionNotifier collisionNotifier;
    [Header("----------------- Fight Config ------------------")]
    [SerializeField] private float nowAI_Level; // AILevel
    [SerializeField] private float MaxMovementSpeed; // = 10 * GlobalIndex.AILevel
    [SerializeField] private float originMaxMovementSpeed; // = 10 * GlobalIndex.AILevel
    [SerializeField] private float maxAttackFrequency = 15f; // �̤j�����W�v
    [SerializeField] private float attackFrequency; // �����W�v
    [SerializeField] private float nowAttackTime; // �������ɶ�
    [SerializeField] private Coroutine attackCoroutine; // �ΨӦs�x��{�A�H�K�b isStuned �ɨ���
    [SerializeField] private bool attackTimeChosen = false; // �O�_�w��ܧ����ɶ�

    [Header("���ʽd�򭭨�")]
    [SerializeField] private Transform topLeftBoundary;
    [SerializeField] private Transform bottomRightBoundary;

    private Vector2 targetPosition;

    private bool isFirstTimeInOpponentsHalf = true;
    private float offsetXFromTarget;
    public float upBoardY = 3f;

    [Header("----------------- Stun Effect ------------------")]
    [SerializeField] private bool isStuned = false;
    [SerializeField] private GameObject stunEffect;

    private void Start()
    {
        nowAI_Level = FightPlayer2Config.AI_level;
        MaxMovementSpeed = 3 * nowAI_Level;
        originMaxMovementSpeed = MaxMovementSpeed;
        attackFrequency = (maxAttackFrequency - 3 * nowAI_Level);

        ball = GameObject.Find("ball");
        ballController = ball.GetComponent<BallController>();//���o�y������A


        player2 = GameObject.Find("Player2");
        rb = player2.GetComponent<Rigidbody2D>();
        startingPosition = rb.position;
        animator = GameObject.Find("Player2Sprite").GetComponent<Animator>();
        player2Collider = player2.GetComponent<CircleCollider2D>();

        // ���ը��o CollisionNotifier
        collisionNotifier = player2.GetComponent<PlayerCollisionNotifier>();
        if (collisionNotifier != null)
        {
            collisionNotifier.OnBallCollision += HandleBallCollision;
        }

        // �b Start �ɹ��է�� PlayerNotification �øj�w�ƥ�
        PlayerNotification notification = player2.GetComponent<PlayerNotification>();
        RegisterPlayerNotification(notification);//�q�\�q��
    }

    private void FixedUpdate()
    {
        if (!isStuned)
        {
            AIMoving();

            if (!attackTimeChosen) // �u�b�|����ܧ����ɶ��ɰ���
            {
                nowAttackTime = Random.Range(maxAttackFrequency, attackFrequency);
                attackTimeChosen = true;
                attackCoroutine = StartCoroutine(DelayedAttack(nowAttackTime));
            }
        }
        else
        {
            // �Y AI �Q�w�t�A������e�� Attack �p�ɨí��s��ܧ����ɶ�
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
            attackTimeChosen = false; // ���m��ܪ��A�A�w�t������i�H���s���
        }
    }

    private IEnumerator DelayedAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        SkillAttack();
        attackTimeChosen = false; // �����᭫�m��ܪ��A�A���U�@���i�H�A��ܷs�ɶ�
    }
    public void SkillAttack()
    {
        int skillIndex = Random.Range(1, 3);
        SkillManager.Instance.ActiveSkill(2,skillIndex);
    }


    private void AIMoving()
    {
        float movementSpeed;
        //AI���ʡA�D�w�t���A
        if (Ball.position.y < 0 || Ball.position.y > upBoardY)//�y�bP1��
        {
            if (isFirstTimeInOpponentsHalf)
            {
                isFirstTimeInOpponentsHalf = false;
                offsetXFromTarget = Random.Range(-1f, 1f);
            }

            movementSpeed = MaxMovementSpeed * Random.Range(0.1f, 0.3f);
            targetPosition = new Vector2(Mathf.Clamp(Ball.position.x + offsetXFromTarget,
                                        topLeftBoundary.position.x,
                                        bottomRightBoundary.position.x),
                                        startingPosition.y);
        }
        else//�y�bAI��
        {
            isFirstTimeInOpponentsHalf = true;

            movementSpeed = Random.Range(MaxMovementSpeed * 0.4f, MaxMovementSpeed);
            targetPosition = new Vector2(Mathf.Clamp(Ball.position.x, topLeftBoundary.position.x,
                                        bottomRightBoundary.position.x),
                                        Mathf.Clamp(Ball.position.y, bottomRightBoundary.position.y,
                                        topLeftBoundary.position.y));
        }
        rb.MovePosition(Vector2.MoveTowards(rb.position, targetPosition,
                movementSpeed * Time.fixedDeltaTime));

    }

    //�q�\�q��
    public void RegisterPlayerNotification(PlayerNotification playerNotification)
    {
        //Debug.Log($"{gameObject.name} ���� �ˮ`");
        playerNotification.OnStatusEffectApplied += HandleStatusEffectApplied;
        //playerNotification.OnGetMagicPointApplied += HandleGetMagicPointNotification;
    }


    // ���� `PlayerNotification` ������ĪG�q��
    private void HandleStatusEffectApplied(StatusEffect effect, GameObject player)
    {

        if (effect == StatusEffect.Stun && !isStuned) //���i�|�[
        {
            isStuned = true;
            StartCoroutine(StunEffect()); // �b�o��Ĳ�o�w�t�ĪG
        }
    }

    private IEnumerator StunEffect()//�w�t�ĪG
    {
        //���a1 �w�t�ɶ��Ӧ۩� ���a2�O�_���[��
        float stunTime = 3 * (1 + FightPlayer1Config.CC_SkillTimeIncrease);

        GameObject obj = Instantiate(stunEffect, player2.transform.position, Quaternion.identity);
        obj.GetComponent<DestroyObject>().SetDTime(stunTime);
        yield return new WaitForSeconds(stunTime);
        isStuned = false;
    }

    private void HandleBallCollision(GameObject ballObject)
    {
        //Debug.Log("AIController ���� Player2 �P Ball �I�����q��");

        // �b�o�̳B�z�I���ɪ��޿�
        animator.SetTrigger("OnHit");
    }


    public void SetMaxMoveSpeed(float maxMoveSpeed)
    {
        MaxMovementSpeed = maxMoveSpeed;
    }

    public void ResetMaxMoveSpeed()
    {
        MaxMovementSpeed = originMaxMovementSpeed;
    }

    public float GetMaxMoveSpeed()
    {
        return MaxMovementSpeed;
    }
}
