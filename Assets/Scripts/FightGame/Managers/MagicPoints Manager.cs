using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicPointsManager : MonoBehaviour
{
    [Header("----------------- PlayerGameObject ------------------")]
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    [Header("----------------- MagicPointData ------------------")]
    [SerializeField] private int p1CurrentMagicPoint;
    [SerializeField] private int p2CurrentMagicPoint;
    [Header("----------------- MagicStonesUI ------------------")]
    [SerializeField] private GameObject MagicStonesUI;
    [SerializeField] private Animator MagicStonesUI_animator;

    void Start()
    {
        // 在 Start 時嘗試找到 PlayerNotification 並綁定事件
        PlayerNotification notification1 = player1.GetComponent<PlayerNotification>();
        RegisterPlayerNotification(notification1);//訂閱通知
                                                 // 在 Start 時嘗試找到 PlayerNotification 並綁定事件
        PlayerNotification notification2 = player2.GetComponent<PlayerNotification>();
        RegisterPlayerNotification(notification2);//訂閱通知

        SetMagicPoint(1, 0);
        SetMagicPoint(2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RegisterPlayerNotification(PlayerNotification playerNotification)
    {
        playerNotification.OnGetMagicPointApplied += HandleGetMagicPointNotification;
    }

    private void HandleGetMagicPointNotification(int pNumber)
    {
        //Debug.Log("只有收到" + player);
        if (pNumber == 1)
        {
            Debug.Log("P1收到");
            GetOnePointMP(pNumber);
        }
        else if (pNumber == 2)
        {
            Debug.Log("P2收到");
            GetOnePointMP(pNumber);
        }
    }

    public int GetMagicPoint(int pNumber)
    {
        if (pNumber == 1)
            return p1CurrentMagicPoint;
        else
            return p2CurrentMagicPoint;
    }

    public void SetMagicPoint(int pNumber,int point)
    {
        if(pNumber == 1)
        {
            p1CurrentMagicPoint = point;

            MagicStonesUI_animator.SetInteger("MagicPoint", p1CurrentMagicPoint);
        }
        else
        {
            p2CurrentMagicPoint = point;
        }

    }

    public void GetOnePointMP(int pNumber)
    {
        if (pNumber == 1)
        {
            p1CurrentMagicPoint += 1;

            MagicStonesUI_animator.SetInteger("MagicPoint", p1CurrentMagicPoint);
        }
        else
        {
            p2CurrentMagicPoint += 1;
        }
    }
}
