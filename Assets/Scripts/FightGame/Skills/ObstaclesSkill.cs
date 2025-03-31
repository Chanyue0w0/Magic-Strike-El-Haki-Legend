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
    [SerializeField] private float nowTime = 0;
    [SerializeField] private float destroyTime = 999;

    [SerializeField] private bool canInstExplosion = true;
    [SerializeField] private bool animatorDestroy = false;//透過動畫控制刪除

    [SerializeField] private GameObject explosion;

    [SerializeField] private Animator animator;

    void Start()
    {
        canInstExplosion = true;
        //Destroy(gameObject, destroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        nowTime += Time.deltaTime;

        if(obstacleHP <= 0 || nowTime >= destroyTime)
        {
            if(explosion != null && canInstExplosion)
            {
                Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
                canInstExplosion = false;
                animator.SetTrigger("Die");
                VibrationPattern.Instance.StartVibrationPattern();

                if (gameObject.GetComponent<AimPlayerShootSkill>() != null)
                {
                    gameObject.GetComponent<AimPlayerShootSkill>().StopMoving();
                }

                if (gameObject.GetComponent<InstObjectSkill>() != null)
                {
                    gameObject.GetComponent<InstObjectSkill>().DeleteObjects();
                }
            }
            //animator.SetBool("Die", true);
            //Destroy(gameObject,1f);
            
        }

        if(animatorDestroy)
        {
            Destroy(gameObject);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if((collision.gameObject.CompareTag("Ball") || collision.gameObject.CompareTag("DamageSkill") || collision.gameObject.CompareTag("CloneBall")) 
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
            //Debug.Log("Damage Skill");
            obstacleHP--;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DamageSkill")
            && obstacleType == ObstacleType.canBreakable)
        {
            //Debug.Log("Damage Skill");
            obstacleHP--;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DamageSkill")
            && obstacleType == ObstacleType.canBreakable)
        {
            //Debug.Log("Damage Skill");
            obstacleHP--;
        }
    }
}
