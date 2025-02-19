using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballController : MonoBehaviour
{
    [SerializeField] private Vector2 velocityNow;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float maxSpeed = 10f;          // 球的最大速度
    [SerializeField] private float towardMiddleSpeed = 0.5f; // 置中速度
    [SerializeField] private float decelerationRate = 1f;    // 每秒減速的速度
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // 使用 FixedUpdate 處理物理運算
    void FixedUpdate()
    {
        MoveOnMaster();
        ClampPosition(); // 新增：確保球體不會超出設定邊界
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
            && (Mathf.Abs(transform.position.y) >= 3f || Mathf.Abs(transform.position.x) >= 1.6f))
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

    // 新增：限制球體位置不超出 X座標 ±1.87 及 Y座標 ±3.23
    private void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -1.87f, 1.87f);
        pos.y = Mathf.Clamp(pos.y, -3.23f, 3.23f);
        transform.position = pos;
    }
}
