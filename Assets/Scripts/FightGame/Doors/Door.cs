using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private enum UserPosition { player1, player2 };
    [SerializeField] private UserPosition playerNumber;
    [Header("----------------- BallResetPosition Setting ------------------")]
    [SerializeField] private GameObject ballResetPositionUp;
    [SerializeField] private GameObject ballResetPositionBottom;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ball"))
        {
            if(playerNumber == UserPosition.player1)//玩家1球門
            {
                collision.transform.position = ballResetPositionUp.transform.position;
                collision.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            }
            else if (playerNumber == UserPosition.player2)//玩家2球門
            {
                collision.transform.position = ballResetPositionBottom.transform.position;
                collision.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            }
        }
    }
}
