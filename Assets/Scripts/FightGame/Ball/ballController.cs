using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private Vector2 velocityNow;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float maxSpeed = 10f;// 球的最大速度
    [SerializeField] private float towardMiddleSpeed = 0.5f; // 置中速度
    [SerializeField] private float decelerationRate = 1f;    // 每秒減速的速度
    [SerializeField] private Vector2 clampPositionX = new Vector2(-1.6f, 1.6f);    // X軸邊界
    [SerializeField] private Vector2 clampPositionY = new Vector2(-3.5f, 3.5f);    // Y軸邊界
    private Rigidbody2D rb;


    [SerializeField] private bool pauseBallMoving = false; //暫停球移動
    [SerializeField] private Vector2 storedVelocity; // 用來儲存暫停前的速度
    //[SerializeField] private Vector2 storedPosition;


    [Header("----------------- OnFieldTime ------------------")]
    private float timeOnCurrentField = 0f;
    private bool isOnPlayer2Field = true; // 初始設為 y > 0 假設球在 player2 場
    private GameObject warningEffectInstance;

    [Header("----------------- BallReset ------------------")]
    // 設定特效 prefab 與球權重設點與方法
    [SerializeField] private AIController aIController;
    [SerializeField] private Animator ballSpriteAnimator;
    [SerializeField] private GameObject ballOnFieldWarningEffect;
    [SerializeField] private Vector2 player1ResetPosition = new Vector2(0f, -1f);
    [SerializeField] private Vector2 player2ResetPosition = new Vector2(0f, 1f);
    [SerializeField] private CircleCollider2D circleCollider;

    //[SerializeField] private BallPossessionManager ballPossessionManager; // 需要掛你控制球權的腳本

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // 使用 FixedUpdate 處理物理運算
    void FixedUpdate()
    {
        if (pauseBallMoving)
        {
            // 第一次進入 pause 時儲存速度與位置，然後鎖定
            if (rb.velocity != Vector2.zero)
            {
                storedVelocity = rb.velocity;
                //storedPosition = rb.position; // rb.position 是 Rigidbody2D 的位置
                rb.velocity = Vector2.zero;
            }

            // 強制維持在儲存的位置（防止因浮點誤差微幅移動）
            //rb.MovePosition(storedPosition);
        }
        else
        {
            // 從暫停恢復
            if (rb.velocity == Vector2.zero && storedVelocity != Vector2.zero)
            {
                rb.velocity = storedVelocity;
                storedVelocity = Vector2.zero;
            }

            MoveOnMaster();
            ClampPosition();
            CheckFieldStayTime();
        }

        velocityNow = rb.velocity;
    }

    public void SetPauseBallMoving(bool pause)
    {
        pauseBallMoving = pause;
        circleCollider.enabled = !pause;
    }

    private void MoveOnMaster()
    {
        // 限制最大速度
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        

        // 當 x 或 y 速度過小，且球體位置接近邊界時，增加置中速度
        if ((Mathf.Abs(rb.velocity.x) < 0.1f || Mathf.Abs(rb.velocity.y) < 0.1f)
            && (Mathf.Abs(transform.position.y) >= 3.5f || Mathf.Abs(transform.position.x) >= 1.6f))
        {
            MoveTowardMiddle(); // 置中
        }
        else
        {
            // 緩慢降低速度至 towardMiddleSpeed
            if (rb.velocity.magnitude > towardMiddleSpeed)
            {
                float newSpeed = rb.velocity.magnitude - decelerationRate * Time.fixedDeltaTime;
                newSpeed = Mathf.Max(newSpeed, 0f); // 確保速度不低於 0
                rb.velocity = rb.velocity.normalized * newSpeed;
            }
        }
        velocityNow = rb.velocity;
    }

    private void MoveTowardMiddle()
    {
        Vector2 ballPosition = transform.position;
        // 取得從球心到原點的方向
        Vector2 direction = ballPosition.normalized;
        rb.velocity += (-direction) * towardMiddleSpeed;
    }

    // 新增：限制球體位置不超出 X座標 ±1.6 及 Y座標 ±3.5
    private void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, clampPositionX.x, clampPositionX.y);
        pos.y = Mathf.Clamp(pos.y, clampPositionY.x, clampPositionY.y);
        transform.position = pos;
    }

    private void CheckFieldStayTime()
    {
        // 判斷現在在哪一場地
        bool currentlyOnPlayer2Field = transform.position.y > 0;

        // 若場地改變，重設時間與刪除特效
        if (currentlyOnPlayer2Field != isOnPlayer2Field)
        {
            isOnPlayer2Field = currentlyOnPlayer2Field;
            timeOnCurrentField = 0f;

            if (warningEffectInstance != null)
            {
                Destroy(warningEffectInstance);
                warningEffectInstance = null;
            }
        }
        else
        {
            // 累加在場時間
            timeOnCurrentField += Time.fixedDeltaTime;

            // 超過4秒產生特效
            if (timeOnCurrentField > 4f && warningEffectInstance == null)
            {
                warningEffectInstance = Instantiate(ballOnFieldWarningEffect, transform);
            }

            // 超過8秒重置球權
            if (timeOnCurrentField > 8f)
            {
                if (gameObject.transform.position.y < 0)
                    ResetBallPosition(2);
                else
                    ResetBallPosition(1);
            }
        }
    }

    public void ResetBallPosition(int pNumber)
    {
        // 根據 pNumber 設定位置與狀態
        if (pNumber == 1)
        {
            isOnPlayer2Field = false;
            transform.position = player1ResetPosition;
        }
        else if (pNumber == 2)
        {
            isOnPlayer2Field = true;
            transform.position = player2ResetPosition;
        }

        ballSpriteAnimator.SetTrigger("ResetBall");

        // 停止球的移動
        rb.velocity = Vector2.zero;

        // 清除警告特效
        if (warningEffectInstance != null)
        {
            Destroy(warningEffectInstance);
            warningEffectInstance = null;
        }

        // 關閉碰撞並延遲重新啟用
        if (circleCollider != null)
        {
            aIController.SetStopMoving(true);
            circleCollider.enabled = false;
            StartCoroutine(ReEnableColliderAfterDelay(1.5f));
        }

        // 重設時間
        timeOnCurrentField = 0f;
    }

    private IEnumerator ReEnableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (circleCollider != null)
        {
            aIController.SetStopMoving(false);
            circleCollider.enabled = true;
        }
    }


    public void ResetTimeOnField()
    {
        timeOnCurrentField = 0f;
    }


}
