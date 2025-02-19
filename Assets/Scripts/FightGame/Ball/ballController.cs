using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballController : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 10f; // 球的最大速度
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // 使用 FixedUpdate 處理物理運算
    void FixedUpdate()
    {
        // 若球的速度超過 maxSpeed，則將其速度限制在最大值
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
}
