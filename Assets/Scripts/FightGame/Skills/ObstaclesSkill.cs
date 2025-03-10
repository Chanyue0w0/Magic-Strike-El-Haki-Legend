using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesSkill : MonoBehaviour
{
    private enum ObstacleType { unBreakable, onlySkillBreakable, canBreakable };
    [Header("----------------- Config Setting ------------------")]
    [SerializeField] private ObstacleType obstacleType;

    [SerializeField] private int playerNumber = 1;
    [SerializeField] private int obstacleHP = 1;

    [SerializeField] private bool canInstExplosion = true;

    [SerializeField] private GameObject explosion;

    [SerializeField] private Animator animator;

    void Start()
    {
        canInstExplosion = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(obstacleHP <= 0)
        {
            if(explosion != null && canInstExplosion)
            {
                Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
                canInstExplosion = false;
            }
            animator.SetBool("Die", true);
            Destroy(gameObject,0.3f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if((collision.gameObject.CompareTag("Ball") || collision.gameObject.CompareTag("DamageSkill")) 
            && obstacleType == ObstacleType.canBreakable)
        {
            obstacleHP--;

        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DamageSkill")
            && obstacleType == ObstacleType.canBreakable)
        {
            Debug.Log("Damage Skill");
            obstacleHP--;
        }
    }
}
