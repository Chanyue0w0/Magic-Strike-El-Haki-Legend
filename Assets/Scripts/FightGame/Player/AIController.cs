using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] private float MaxMovementSpeed; // = 10 * GlobalIndex.AILevel
    [SerializeField] private float originMaxMovementSpeed; // = 10 * GlobalIndex.AILevel
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

    [Header("移動範圍限制")]
    [SerializeField] private Transform topLeftBoundary;
    [SerializeField] private Transform bottomRightBoundary;

    private Vector2 targetPosition;

    private bool isFirstTimeInOpponentsHalf = true;
    private float offsetXFromTarget;
    public float upBoardY = 3f;

    [Header("----------------- Stun Effect ------------------")]
    [SerializeField] private bool isStuned = false;

    private void Start()
    {
        originMaxMovementSpeed = MaxMovementSpeed;
        ball = GameObject.Find("ball");
        ballController = ball.GetComponent<BallController>();//取得球的控制狀態

        player2 = GameObject.Find("Player2");
        rb = player2.GetComponent<Rigidbody2D>();
        startingPosition = rb.position;
        animator = GameObject.Find("Player2Sprite").GetComponent<Animator>();
        player2Collider = player2.GetComponent<CircleCollider2D>();

        // 嘗試取得 CollisionNotifier
        collisionNotifier = player2.GetComponent<PlayerCollisionNotifier>();
        if (collisionNotifier != null)
        {
            collisionNotifier.OnBallCollision += HandleBallCollision;
        }

        // 在 Start 時嘗試找到 PlayerNotification 並綁定事件
        PlayerNotification notification = player2.GetComponent<PlayerNotification>();
        RegisterPlayerNotification(notification);//訂閱通知
    }

    private void FixedUpdate()
    {
        if (!isStuned)
        {
            AIMoving();
        }
    }

    private void AIMoving()
    {
        float movementSpeed;
        //AI移動，非暈眩狀態
        if (Ball.position.y < 0 || Ball.position.y > upBoardY)//球在P1場
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
        else//球在AI場
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

    //訂閱通知
    public void RegisterPlayerNotification(PlayerNotification playerNotification)
    {
        Debug.Log($"{gameObject.name} 收到 傷害");
        playerNotification.OnStatusEffectApplied += HandleStatusEffectApplied;
        //playerNotification.OnGetMagicPointApplied += HandleGetMagicPointNotification;
    }


    // 接收 `PlayerNotification` 的受到效果通知
    private void HandleStatusEffectApplied(StatusEffect effect, GameObject player)
    {

        if (effect == StatusEffect.Stun)
        {
            isStuned = true;
            StartCoroutine(StunEffect()); // 在這裡觸發暈眩效果
        }
    }

    private IEnumerator StunEffect()//暈眩效果
    {
        //玩家1 暈眩時間來自於 玩家2是否有加成
        float stunTime = 3 * (1 + FightPlayer1Config.CC_SkillTimeIncrease);
        yield return new WaitForSeconds(stunTime);
        isStuned = false;
    }

    private void HandleBallCollision(GameObject ballObject)
    {
        //Debug.Log("AIController 收到 Player2 與 Ball 碰撞的通知");

        // 在這裡處理碰撞時的邏輯
        animator.SetTrigger("OnHit");
    }

    public void SkillAttack()
    {
        
    }

    public void TheSkillAttack(int chooseCard)
    {

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
