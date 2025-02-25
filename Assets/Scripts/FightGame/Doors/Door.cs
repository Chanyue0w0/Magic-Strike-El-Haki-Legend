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
    [Header("----------------- PlayerGameObject ------------------")]
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    [Header("----------------- PlayerManager ------------------")]
    [SerializeField] private PlayerStatusManager player1_statusManager;
    [SerializeField] private PlayerStatusManager player2_statusManager;

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
            if(playerNumber == UserPosition.player1)//進玩家1球門，玩家2攻擊
            {
                collision.transform.position = ballResetPositionBottom.transform.position;
                collision.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                NormalAttackManager.Instance.InstNormalAttack(2, 1);
            }
            else if (playerNumber == UserPosition.player2)//進玩家2球門，玩家1攻擊
            {
                collision.transform.position = ballResetPositionUp.transform.position;
                collision.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                NormalAttackManager.Instance.InstNormalAttack(1, 2);
            }
        }
    }
}
