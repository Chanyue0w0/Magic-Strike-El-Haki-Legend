using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] private float MaxMovementSpeed; // = 10 * GlobalIndex.AILevel
    [SerializeField] private float originMaxMovementSpeed; // = 10 * GlobalIndex.AILevel
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Vector2 startingPosition;

    [SerializeField] private GameObject player2;
    [SerializeField] private Rigidbody2D Ball;
    [SerializeField] private GameObject ball;
    [SerializeField] private BallController ballController;
    [SerializeField] private float AISightAlpha = 0.8f;

    [Header("���ʽd�򭭨�")]
    [SerializeField] private Transform topLeftBoundary;
    [SerializeField] private Transform bottomRightBoundary;

    private Vector2 targetPosition;

    private bool isFirstTimeInOpponentsHalf = true;
    private float offsetXFromTarget;
    public float upBoardY = 3f;

    private void Start()
    {
        originMaxMovementSpeed = MaxMovementSpeed;
        ball = GameObject.Find("ball");
        ballController = ball.GetComponent<BallController>();//���o�y������A

        player2 = GameObject.Find("Player2");
        rb = player2.GetComponent<Rigidbody2D>();
        startingPosition = rb.position;
        
    }

    private void FixedUpdate()
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
