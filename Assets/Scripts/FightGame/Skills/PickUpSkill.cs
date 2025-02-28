using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSkill : MonoBehaviour
{
    [SerializeField] private int playerNumber = 1;

    [SerializeField] private int cardSkillSetNumber = 0;

    [SerializeField] private float destroyOriginTime = 3f;
    [SerializeField] private float destroyTime = 3f;

    [SerializeField] private Animator animator;

    [SerializeField] private GameObject pickUpExplosionEffect;

    //[SerializeField] private GameObject cardSkillSetObj;
    //[SerializeField] private CardSkillSet cardSkillSetScript;


    void Start()
    {
        destroyTime = destroyOriginTime;
        animator = gameObject.GetComponent<Animator>();
        //cardSkillSetObj = GameObject.Find("card (" + cardSkillSetNumber + ")");
        //cardSkillSetScript = cardSkillSetObj.GetComponent<CardSkillSet>();
        pickUpExplosionEffect = Resources.Load<GameObject>("Prefabs/Skills/Explosions/FireworkExplosion");

        
    }

    void Update()
    {
        destroyTime -= Time.deltaTime;
        if (destroyTime < 0)
        {
            destroyTime = 999;
            animator.SetTrigger("Destroy");
            Destroy(gameObject, 0.5f);
        }


    }

    public void SetPlayerNumber(int pNumber)
    {
        playerNumber = pNumber;
    }

    public void SetCardSkillSetNumber(int cssNumber)
    {
        cardSkillSetNumber = cssNumber;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            //cardSkillSetScript.Active();
            Instantiate(pickUpExplosionEffect, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
