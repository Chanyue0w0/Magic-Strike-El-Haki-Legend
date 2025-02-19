using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerRelative : MonoBehaviour
{
    [Header("移動設定")]
    [Tooltip("微小移動閥值，用來濾除雜訊（目前設為0）")]
    public float movementThreshold = 0.0f;

    [Tooltip("桌面所在的Layer，用於射線偵測")]
    public LayerMask tableLayerMask;

    private Camera mainCamera;
    private bool isDragging = false;
    private Rigidbody2D rb;

    // 拖曳起始時手指的位置與玩家的位置
    private Vector2 initialTouchWorldPos;
    private Vector2 initialPlayerPosition;
    private float dragStartTime;

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
            // 用射線檢測確認是否點擊到指定區域（例如桌面）
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0.1f, tableLayerMask);
            if (hit.collider != null)
            {
                isDragging = true;
                // 記錄起始的手指與玩家位置，以及開始時間
                initialTouchWorldPos = worldPoint;
                initialPlayerPosition = rb.position;
                dragStartTime = Time.time;
            }
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            // 計算手指相對起始點的偏移
            Vector2 offset = worldPoint - initialTouchWorldPos;
            // 若設定了 movementThreshold 可額外過濾小幅移動
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

            // 手指的總位移與平均速度
            Vector2 gestureDistanceVec = worldPoint - initialTouchWorldPos;
            float gestureDistance = gestureDistanceVec.magnitude;
            float gestureSpeed = (dragDuration > 0) ? gestureDistance / dragDuration : 0f;

            // 玩家實際移動的距離與速度
            Vector2 playerMovementVec = rb.position - initialPlayerPosition;
            float playerMovementDistance = playerMovementVec.magnitude;
            float playerSpeed = (dragDuration > 0) ? playerMovementDistance / dragDuration : 0f;

            //Debug.Log($"[滑鼠拖曳] 手指移動：距離 {gestureDistance:F2}，速度 {gestureSpeed:F2} 單位/秒；玩家移動：距離 {playerMovementDistance:F2}，速度 {playerSpeed:F2} 單位/秒。");

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

                    //Debug.Log($"[手指拖曳] 手指移動：距離 {gestureDistance:F2}，速度 {gestureSpeed:F2} 單位/秒；玩家移動：距離 {playerMovementDistance:F2}，速度 {playerSpeed:F2} 單位/秒。");

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
