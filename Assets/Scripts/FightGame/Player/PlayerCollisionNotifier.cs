using UnityEngine;
using System;

public class PlayerCollisionNotifier : MonoBehaviour
{
    // 定義當與 Ball 發生碰撞時的事件
    [SerializeField] public event Action<GameObject> OnBallCollision;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            //Debug.Log("Player2 與 Ball 發生碰撞！");
            OnBallCollision?.Invoke(collision.gameObject);
        }
    }

}
