using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballController : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 10f; // �y���̤j�t��
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // �ϥ� FixedUpdate �B�z���z�B��
    void FixedUpdate()
    {
        // �Y�y���t�׶W�L maxSpeed�A�h�N��t�׭���b�̤j��
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
}
