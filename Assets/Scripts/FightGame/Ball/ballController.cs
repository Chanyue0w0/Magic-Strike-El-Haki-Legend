using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballController : MonoBehaviour
{
    [SerializeField] private Vector2 velocityNow;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float maxSpeed = 10f;          // �y���̤j�t��
    [SerializeField] private float towardMiddleSpeed = 0.5f; // �m���t��
    [SerializeField] private float decelerationRate = 1f;    // �C���t���t��
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // �ϥ� FixedUpdate �B�z���z�B��
    void FixedUpdate()
    {
        MoveOnMaster();
        ClampPosition(); // �s�W�G�T�O�y�餣�|�W�X�]�w���
    }

    private void MoveOnMaster()
    {
        // ����̤j�t��
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        

        // �� x �� y �t�׹L�p�A�B�y���m������ɮɡA�W�[�m���t��
        if ((Mathf.Abs(rb.velocity.x) < 0.1f || Mathf.Abs(rb.velocity.y) < 0.1f)
            && (Mathf.Abs(transform.position.y) >= 3f || Mathf.Abs(transform.position.x) >= 1.6f))
        {
            MoveTowardMiddle(); // �m��
        }
        else
        {
            // �w�C���C�t�צ� towardMiddleSpeed
            if (rb.velocity.magnitude > towardMiddleSpeed)
            {
                float newSpeed = rb.velocity.magnitude - decelerationRate * Time.fixedDeltaTime;
                newSpeed = Mathf.Max(newSpeed, 0f); // �T�O�t�פ��C�� 0
                rb.velocity = rb.velocity.normalized * newSpeed;
            }
        }
        velocityNow = rb.velocity;
    }

    private void MoveTowardMiddle()
    {
        Vector2 ballPosition = transform.position;
        // ���o�q�y�ߨ���I����V
        Vector2 direction = ballPosition.normalized;
        rb.velocity += (-direction) * towardMiddleSpeed;
    }

    // �s�W�G����y���m���W�X X�y�� ��1.87 �� Y�y�� ��3.23
    private void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -1.87f, 1.87f);
        pos.y = Mathf.Clamp(pos.y, -3.23f, 3.23f);
        transform.position = pos;
    }
}
