using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerRelative : MonoBehaviour
{
    [Header("���ʳ]�w")]
    [Tooltip("�L�p���ʻ֭ȡA�Ψ��o�����T�]�ثe�]��0�^")]
    public float movementThreshold = 0.0f;

    [Tooltip("�ୱ�Ҧb��Layer�A�Ω�g�u����")]
    public LayerMask tableLayerMask;

    private Camera mainCamera;
    private bool isDragging = false;
    private Rigidbody2D rb;

    // �즲�_�l�ɤ������m�P���a����m
    private Vector2 initialTouchWorldPos;
    private Vector2 initialPlayerPosition;
    private float dragStartTime;

    [SerializeField] private GameObject playerObject;

    [Header("���ʽd�򭭨�")]
    [SerializeField] private Transform topLeftBoundary;
    [SerializeField] private Transform bottomRightBoundary;

    [Header("----------------- Player Gamebject ------------------")]
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;


    [Header("----------------- Stun Effect ------------------")]
    [SerializeField] private bool isStuned = false;
    [SerializeField] private GameObject stunEffect;


    void Start()
    {
        mainCamera = Camera.main;

        rb = playerObject.GetComponent<Rigidbody2D>();
        rb.isKinematic = true;


        // �b Start �ɹ��է�� PlayerNotification �øj�w�ƥ�
        PlayerNotification notification = player1.GetComponent<PlayerNotification>();
        RegisterPlayerNotification(notification);//�q�\�q��
    }

    void Update()
    {
        if(!isStuned)
        {
            PlayerMoving();
        }

    }

    private void PlayerMoving()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // �ƹ���
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            // �ήg�u�˴��T�{�O�_�I������w�ϰ�]�Ҧp�ୱ�^
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0.1f, tableLayerMask);
            if (hit.collider != null)
            {
                isDragging = true;
                // �O���_�l������P���a��m�A�H�ζ}�l�ɶ�
                initialTouchWorldPos = worldPoint;
                initialPlayerPosition = rb.position;
                dragStartTime = Time.time;
            }
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            // �p�����۹�_�l�I������
            Vector2 offset = worldPoint - initialTouchWorldPos;
            // �Y�]�w�F movementThreshold �i�B�~�L�o�p�T����
            if (offset.magnitude >= movementThreshold)
            {
                Vector2 newPosition = initialPlayerPosition + offset;
                newPosition = ClampPosition(newPosition);
                rb.MovePosition(newPosition);
            }
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            float dragDuration = Time.time - dragStartTime;

            // ������`�첾�P�����t��
            Vector2 gestureDistanceVec = worldPoint - initialTouchWorldPos;
            float gestureDistance = gestureDistanceVec.magnitude;
            float gestureSpeed = (dragDuration > 0) ? gestureDistance / dragDuration : 0f;

            // ���a��ڲ��ʪ��Z���P�t��
            Vector2 playerMovementVec = rb.position - initialPlayerPosition;
            float playerMovementDistance = playerMovementVec.magnitude;
            float playerSpeed = (dragDuration > 0) ? playerMovementDistance / dragDuration : 0f;

            //Debug.Log($"[�ƹ��즲] ������ʡG�Z�� {gestureDistance:F2}�A�t�� {gestureSpeed:F2} ���/��F���a���ʡG�Z�� {playerMovementDistance:F2}�A�t�� {playerSpeed:F2} ���/��C");

            isDragging = false;
        }
#else
        // �����
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0.1f, tableLayerMask);
            if (hit.collider != null)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    isDragging = true;
                    initialTouchWorldPos = worldPoint;
                    initialPlayerPosition = rb.position;
                    dragStartTime = Time.time;
                }
                else if (touch.phase == TouchPhase.Moved && isDragging)
                {
                    Vector2 offset = worldPoint - initialTouchWorldPos;
                    if (offset.magnitude >= movementThreshold)
                    {
                        Vector2 newPosition = initialPlayerPosition + offset;
                        newPosition = ClampPosition(newPosition);
                        rb.MovePosition(newPosition);
                    }
                }
                else if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && isDragging)
                {
                    float dragDuration = Time.time - dragStartTime;

                    Vector2 gestureDistanceVec = worldPoint - initialTouchWorldPos;
                    float gestureDistance = gestureDistanceVec.magnitude;
                    float gestureSpeed = (dragDuration > 0) ? gestureDistance / dragDuration : 0f;

                    Vector2 playerMovementVec = rb.position - initialPlayerPosition;
                    float playerMovementDistance = playerMovementVec.magnitude;
                    float playerSpeed = (dragDuration > 0) ? playerMovementDistance / dragDuration : 0f;

                    //Debug.Log($"[����즲] ������ʡG�Z�� {gestureDistance:F2}�A�t�� {gestureSpeed:F2} ���/��F���a���ʡG�Z�� {playerMovementDistance:F2}�A�t�� {playerSpeed:F2} ���/��C");

                    isDragging = false;
                }
            }
        }
#endif
    }

    //�q�\�q��
    public void RegisterPlayerNotification(PlayerNotification playerNotification)
    {
        //Debug.Log($"{gameObject.name} ���� �ˮ`");
        playerNotification.OnStatusEffectApplied += HandleStatusEffectApplied;
        //playerNotification.OnGetMagicPointApplied += HandleGetMagicPointNotification;
    }


    // ���� `PlayerNotification` ������ĪG�q��
    private void HandleStatusEffectApplied(StatusEffect effect, GameObject player)
    {

        if (effect == StatusEffect.Stun && !isStuned)//���i�|�[
        {
            isStuned = true;
            StartCoroutine(StunEffect()); // �b�o��Ĳ�o�w�t�ĪG
        }
    }

    private IEnumerator StunEffect()//�w�t�ĪG
    {
        //���a1 �w�t�ɶ��Ӧ۩� ���a2�O�_���[��
        float stunTime = 3 * (1 + FightPlayer2Config.CC_SkillTimeIncrease);

        GameObject obj = Instantiate(stunEffect, player1.transform.position, Quaternion.identity);
        obj.GetComponent<DestroyObject>().SetDTime(stunTime);
        yield return new WaitForSeconds(stunTime);
        isStuned = false;
    }

    /// <summary>
    /// ����a���ʦb���w��ɤ�
    /// </summary>
    /// <param name="position">��l��m</param>
    /// <returns>�Q����L���y��</returns>
    private Vector2 ClampPosition(Vector2 position)
    {
        float minX = topLeftBoundary.position.x;
        float maxX = bottomRightBoundary.position.x;
        float minY = bottomRightBoundary.position.y;
        float maxY = topLeftBoundary.position.y;

        float clampedX = Mathf.Clamp(position.x, minX, maxX);
        float clampedY = Mathf.Clamp(position.y, minY, maxY);

        return new Vector2(clampedX, clampedY);
    }
}
