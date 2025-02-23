using UnityEngine;
using System;

public class PlayerCollisionNotifier : MonoBehaviour
{
    // �w�q��P Ball �o�͸I���ɪ��ƥ�
    [SerializeField] public event Action<GameObject> OnBallCollision;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            //Debug.Log("Player2 �P Ball �o�͸I���I");
            OnBallCollision?.Invoke(collision.gameObject);
        }
    }

}
