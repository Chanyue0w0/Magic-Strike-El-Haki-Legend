using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttackManager : MonoBehaviour
{
    public static NormalAttackManager Instance { get; private set; }//單例宣告
    [Header("----------------- PlayerGameObject ------------------")]
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    [Header("----------------- NormalAttackGameObject ------------------")]
    [SerializeField] private GameObject player1_NormalAttack;
    [SerializeField] private GameObject player2_NormalAttack;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // 確保在場景切換時不會被銷毀
    }

    private void Start()
    {
        player1_NormalAttack = Resources.Load<GameObject>("Prefabs/NormalAttack/" + FightPlayer1Config.NormalAttack);
        if (player1_NormalAttack == null)
            Debug.Log("No P1 NormalAttack");
        player2_NormalAttack = Resources.Load<GameObject>("Prefabs/NormalAttack/" + FightPlayer2Config.NormalAttack);
        if (player2_NormalAttack == null)
            Debug.Log("No P2 NormalAttack");
    }

    public void InstNormalAttack(int instPlayerNumber, int targetPlayerNumber)
    {
        if(instPlayerNumber == 1)
        {
            Instantiate(player1_NormalAttack, player1.transform.position, Quaternion.identity);
            //Set Normal Attack targetPlayerNumber
        }
        else
        {
            Instantiate(player2_NormalAttack, player2.transform.position, Quaternion.identity);
            //Set Normal Attack targetPlayerNumber
        }
        Debug.Log($"Player {instPlayerNumber} attacks Player {targetPlayerNumber}");
        // 在這裡實作攻擊邏輯
    }
}
