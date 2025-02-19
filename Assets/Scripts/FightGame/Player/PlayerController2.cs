using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    [Header("移動設定")]
    [Tooltip("微小移動閥值，用來濾除雜訊")]
    public float movementThreshold = 0.01f;

    [Tooltip("桌面所在的Layer，用於射線偵測")]
    public LayerMask tableLayerMask;

    private Camera mainCamera;
    private bool isDragging = false;
    private Rigidbody2D rb;
    private Vector2 previousTouchWorldPos;

    [SerializeField] private GameObject playerObject;

    [Header("移動範圍限制")]
    [SerializeField] private Transform topLeftBoundary;
    [SerializeField] private Transform bottomRightBoundary;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("MainCamera 未設定！");
        }

        if (playerObject == null)
        {
            Debug.LogError("playerObject 未指派！");
        }

        rb = playerObject.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Player 物件缺少 Rigidbody2D！");
        }
        else
        {
            rb.isKinematic = true;
        }

        if (topLeftBoundary == null || bottomRightBoundary == null)
        {
            Debug.LogError("請在 Inspector 中指派左上與右下的邊界物件！");
        }
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // 滑鼠版
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            // 用射線檢測確認是否點擊到指定區域（桌面）
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0.1f, tableLayerMask);
            if (hit.collider != null)
            {
                isDragging = true;
                previousTouchWorldPos = worldPoint;
            }
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 delta = worldPoint - previousTouchWorldPos;

            // 當手指移動超過閥值時，直接用相同位移更新玩家位置
            if (delta.magnitude > movementThreshold)
            {
                Vector2 newPosition = rb.position + delta;
                newPosition = ClampPosition(newPosition);
                rb.MovePosition(newPosition);
            }
            previousTouchWorldPos = worldPoint;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
#else
        // 手機版
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
                    previousTouchWorldPos = worldPoint;
                }
                else if (touch.phase == TouchPhase.Moved && isDragging)
                {
                    Vector2 delta = worldPoint - previousTouchWorldPos;
                    if (delta.magnitude > movementThreshold)
                    {
                        Vector2 newPosition = rb.position + delta;
                        newPosition = ClampPosition(newPosition);
                        rb.MovePosition(newPosition);
                    }
                    previousTouchWorldPos = worldPoint;
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    isDragging = false;
                }
            }
        }
#endif
    }

    /// <summary>
    /// 限制玩家移動在指定邊界內
    /// </summary>
    /// <param name="position">原始位置</param>
    /// <returns>被限制過的座標</returns>
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
