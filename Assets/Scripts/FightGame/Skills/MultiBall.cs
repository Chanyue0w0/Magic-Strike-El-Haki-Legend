using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiBall : MonoBehaviour
{
    [SerializeField] private int playerNumber = 1;
    //[SerializeField] private int skillDamage = 100;
    [SerializeField] private GameObject cloneBallObj;
    [SerializeField] private GameObject explosion;
    [SerializeField] private int cloneBallAmount = 3;

    //[SerializeField] private GameObject player1;
    //[SerializeField] private GameObject player2;
    [SerializeField] private GameObject ball;


    public void SetPlayerNumber(int pNumber) // initial
    {
        playerNumber = pNumber;
        InitializedSkillInfo();
    }

    public void InitializedSkillInfo()
    {
        //float skillDamageTMP = 0;
        cloneBallObj = Resources.Load<GameObject>("Prefabs/Balls/CloneBall");

        explosion = Resources.Load<GameObject>("Prefabs/Skills/Explosions/SmokeExplosion"); 
        //player1 = GameObject.FindGameObjectWithTag("Player1");
        //player2 = GameObject.FindGameObjectWithTag("Player2");
        ball = GameObject.FindGameObjectWithTag("Ball");
    }

    public void Active()
    {

        StartCoroutine(DelayedInstCloneBall(0f));
    }

    private IEnumerator DelayedInstCloneBall(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (explosion != null)
            Instantiate(explosion, ball.transform.position, Quaternion.identity);

        for(int i=0;i<cloneBallAmount;i++)
        {
            Instantiate(cloneBallObj, ball.transform.position, Quaternion.identity);
            //GameObject obj = Instantiate(cloneBallObj, ball.transform.position, Quaternion.identity);
        }


    }
}
