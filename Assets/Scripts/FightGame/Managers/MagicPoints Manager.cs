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
    [SerializeField] private int p1MaxMagicPoint = 5;
    [SerializeField] private int p2MaxMagicPoint = 5;
    [Header("----------------- MagicStonesUI ------------------")]
    [SerializeField] private GameObject MagicStonesUI;
    [SerializeField] private Animator MagicStonesUI_animator;

    [Header("----------------- Magic Blue Sparkling ------------------")]
    [SerializeField] private GameObject p1MagicSparkling;// 玩家1 魔法可使用動畫
    //[SerializeField] private GameObject p2MagicBlueSparkling;// 玩家2 魔法可使用動畫

    [Header("----------------- Player Ult Prefab ------------------")]
    [SerializeField] private GameObject p1UltEffect;//p1大招展示動畫
    [SerializeField] private GameObject p2UltEffect;//p2大招展示動畫

    [Header("----------------- Camera Setting------------------")]
    [SerializeField] private Camera mainCamera; // 主攝像機
    [SerializeField] private float cameraZoomDuration = 2.5f; // 相機縮放持續時間
    [SerializeField] private float zoomedCameraSize = 5f; // 相機縮放後的大小
    [SerializeField] private float cameraFadeInSpeed = 20f; // 相機淡入淡出速度
    [SerializeField] private float cameraFadeOutSpeed = 100f; // 相機淡入淡出速度
    private float originalCameraSize;
    private Vector3 originalCameraPosition;

    [Header("----------------- Double Click------------------")]
    [SerializeField] private float lastClickTime = 0f; // 記錄上次點擊時間
    [SerializeField] private float doubleClickThreshold = 0.3f; // 雙擊時間閾值

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

        originalCameraSize = mainCamera.orthographicSize;
        originalCameraPosition = mainCamera.transform.position;

        if (FightPlayer1Config.Group[0] == "HR00")
        {
            p1UltEffect = Resources.Load<GameObject>("Prefabs/Ult/BigHammerLionP1Ult");
        }

        if (FightPlayer2Config.Group[0] == "HR00")
        {
            p2UltEffect = Resources.Load<GameObject>("Prefabs/Ult/BigHammerLionP2Ult");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 偵測滑鼠左鍵點擊
        {
            if (Time.time - lastClickTime < doubleClickThreshold)
            {
                UseUlt(1); // 觸發 P1 大招
            }
            lastClickTime = Time.time; // 更新上次點擊時間
        }
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
            //Debug.Log("P1收到");
            GetOnePointMP(pNumber);
        }
        else if (pNumber == 2)
        {
            //Debug.Log("P2收到");
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

            if (p1CurrentMagicPoint == p1MaxMagicPoint)
                p1MagicSparkling.SetActive(true);
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
            if (p1CurrentMagicPoint < p1MaxMagicPoint)
                p1CurrentMagicPoint += 1;
            else
                p1CurrentMagicPoint = p1MaxMagicPoint;

            MagicStonesUI_animator.SetInteger("MagicPoint", p1CurrentMagicPoint);

            if (p1CurrentMagicPoint == p1MaxMagicPoint)
                p1MagicSparkling.SetActive(true);
        }
        else
        {
            p2CurrentMagicPoint += 1;
        }
    }

    public void UseUlt(int pNumber)
    {
        if (pNumber == 1 && p1CurrentMagicPoint == p1MaxMagicPoint)
        {
            Instantiate(p1UltEffect, player1.transform.position, Quaternion.identity);
            StartCoroutine(ActivateUltCoroutine(player1.transform.position, pNumber));
            SetMagicPoint(1, 0);
        }
        else if (pNumber == 2 && p2CurrentMagicPoint == p2MaxMagicPoint)
        {
            Instantiate(p2UltEffect, player2.transform.position, Quaternion.identity);
            StartCoroutine(ActivateUltCoroutine(player2.transform.position, pNumber));
            SetMagicPoint(2, 0);
        }
    }

    //大招動畫
    private IEnumerator ActivateUltCoroutine(Vector3 targetPosition, int pNumber)
    {
        // Pause the game
        Time.timeScale = 0;
        float elapsedTime = 0f;

        // Fade in and zoom camera to the target position
        while (mainCamera.orthographicSize > zoomedCameraSize)
        {
            mainCamera.orthographicSize -= cameraFadeInSpeed * Time.unscaledDeltaTime;
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, new Vector3(targetPosition.x, targetPosition.y, mainCamera.transform.position.z), cameraFadeInSpeed * Time.unscaledDeltaTime);
            yield return null;
        }

        mainCamera.orthographicSize = zoomedCameraSize;
        mainCamera.transform.position = new Vector3(targetPosition.x, targetPosition.y, mainCamera.transform.position.z);

        // Wait for the specified duration using unscaled time
        while (elapsedTime < cameraZoomDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // Fade out and reset camera to original size and position
        while (mainCamera.orthographicSize < originalCameraSize)
        {
            mainCamera.orthographicSize += cameraFadeOutSpeed * Time.unscaledDeltaTime;
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, originalCameraPosition, cameraFadeOutSpeed * Time.unscaledDeltaTime);
            yield return null;
        }

        mainCamera.orthographicSize = originalCameraSize;
        mainCamera.transform.position = originalCameraPosition;

        // Resume the game
        Time.timeScale = 1;

        if (pNumber == 1)
        {
            SetMagicPoint(1, 0);
            p1MagicSparkling.SetActive(false);
        }
        else if (pNumber == 2)
        {
            SetMagicPoint(2, 0);
            //p2MagicBlueSparkling.SetActive(false);
        }
    }
}
