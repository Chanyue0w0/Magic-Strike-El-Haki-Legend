using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldUp : MonoBehaviour
{
    [SerializeField] private int playerNumber = 1;
    //[SerializeField] private float OriginalShieldPercentage = 0.0f;
    [SerializeField] private float shieldPercentage = 0.5f;
    [SerializeField] private float shieldConsistTime = 8f;
    [SerializeField] private GameObject shieldObj;

    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;

    private Coroutine shieldCoroutine; // 用來記錄當前的協程

    public void SetPlayerNumber(int pNumber) // initial
    {
        playerNumber = pNumber;
        shieldObj = Resources.Load<GameObject>("Prefabs/Skills/ShieldUp");
        player1 = GameObject.FindGameObjectWithTag("Player1");
        player2 = GameObject.FindGameObjectWithTag("Player2");
    }

    public void Active()
    {
        if (shieldCoroutine != null)
        {
            StopCoroutine(shieldCoroutine);
        }

        if (playerNumber == 1)
        {
            FightPlayer1Config.ShieldPercentage = shieldPercentage;
            GameObject obj = Instantiate(shieldObj, player1.transform.position, Quaternion.identity);
            obj.transform.SetParent(player1.transform);
            obj.GetComponent<DestroyObject>().SetDTime(shieldConsistTime);
        }
        else
        {
            FightPlayer2Config.ShieldPercentage = shieldPercentage;
            GameObject obj = Instantiate(shieldObj, player2.transform.position, Quaternion.identity);
            obj.transform.SetParent(player2.transform);
            obj.GetComponent<DestroyObject>().SetDTime(shieldConsistTime);
        }

        // 啟動新的護盾倒數協程
        shieldCoroutine = StartCoroutine(ResetShield());
    }

    private IEnumerator ResetShield()
    {
        yield return new WaitForSeconds(shieldConsistTime);
        if (playerNumber == 1)
        {
            FightPlayer1Config.ShieldPercentage = 0;
        }
        else
        {
            FightPlayer2Config.ShieldPercentage = 0;
        }
    }
}
