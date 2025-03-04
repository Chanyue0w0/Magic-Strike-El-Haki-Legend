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
    [SerializeField] private GameObject p1MagicSparkling;// ���a1 �]�k�i�ϥΰʵe
    //[SerializeField] private GameObject p2MagicBlueSparkling;// ���a2 �]�k�i�ϥΰʵe

    [Header("----------------- Player Ult Prefab ------------------")]
    [SerializeField] private GameObject p1UltEffect;//p1�j�ۮi�ܰʵe
    [SerializeField] private GameObject p2UltEffect;//p2�j�ۮi�ܰʵe

    [Header("----------------- Camera Setting------------------")]
    [SerializeField] private Camera mainCamera; // �D�ṳ��
    [SerializeField] private float cameraZoomDuration = 2.5f; // �۾��Y�����ɶ�
    [SerializeField] private float zoomedCameraSize = 5f; // �۾��Y��᪺�j�p
    [SerializeField] private float cameraFadeInSpeed = 20f; // �۾��H�J�H�X�t��
    [SerializeField] private float cameraFadeOutSpeed = 100f; // �۾��H�J�H�X�t��
    private float originalCameraSize;
    private Vector3 originalCameraPosition;

    [Header("----------------- Double Click------------------")]
    [SerializeField] private float lastClickTime = 0f; // �O���W���I���ɶ�
    [SerializeField] private float doubleClickThreshold = 0.3f; // �����ɶ��H��

    void Start()
    {
        // �b Start �ɹ��է�� PlayerNotification �øj�w�ƥ�
        PlayerNotification notification1 = player1.GetComponent<PlayerNotification>();
        RegisterPlayerNotification(notification1);//�q�\�q��
                                                 // �b Start �ɹ��է�� PlayerNotification �øj�w�ƥ�
        PlayerNotification notification2 = player2.GetComponent<PlayerNotification>();
        RegisterPlayerNotification(notification2);//�q�\�q��

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
        if (Input.GetMouseButtonDown(0)) // �����ƹ������I��
        {
            if (Time.time - lastClickTime < doubleClickThreshold)
            {
                UseUlt(1); // Ĳ�o P1 �j��
            }
            lastClickTime = Time.time; // ��s�W���I���ɶ�
        }
    }

    public void RegisterPlayerNotification(PlayerNotification playerNotification)
    {
        playerNotification.OnGetMagicPointApplied += HandleGetMagicPointNotification;
    }

    private void HandleGetMagicPointNotification(int pNumber)
    {
        //Debug.Log("�u������" + player);
        if (pNumber == 1)
        {
            //Debug.Log("P1����");
            GetOnePointMP(pNumber);
        }
        else if (pNumber == 2)
        {
            //Debug.Log("P2����");
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

    //�j�۰ʵe
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
