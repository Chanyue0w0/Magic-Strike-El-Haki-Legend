using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    [Header("移動設定")]
    [Tooltip("控制移動敏感度，數值越大移動越快")]
    public float moveMultiplier = 10.0f; // 增加移動靈敏度

    [Tooltip("桌面所在的Layer，用於射線偵測")]
    public LayerMask tableLayerMask;

    private Camera mainCamera;
    private Vector2 lastTouchWorldPos;
    private bool isDragging = false;
    private Rigidbody2D rb; // 取得 Rigidbody2D
    private Vector2 velocity; // 儲存移動速度

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
            rb.isKinematic = true; // 設定為 Kinematic，避免被物理影響但能偵測碰撞
        }

        if (topLeftBoundary == null || bottomRightBoundary == null)
        {
            Debug.LogError("請在 Inspector 中指派左上與右下的邊界物件！");
        }
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // 滑鼠輸入模擬觸控
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0.1f, tableLayerMask);
            Debug.DrawRay(worldPoint, Vector2.zero, Color.red, 0.1f);

            if (hit.collider != null)
            {
                Debug.Log("滑鼠按下，射線擊中：" + hit.collider.name);
                lastTouchWorldPos = hit.point;
                isDragging = true;
            }
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            velocity = (worldPoint - lastTouchWorldPos) / Time.deltaTime; // 計算移動速度
            lastTouchWorldPos = worldPoint;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("滑鼠放開");
            isDragging = false;
            velocity = Vector2.zero; // 停止時歸零
        }

#else
        // ✅ 手機端觸控輸入
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0.1f, tableLayerMask);

            if (hit.collider != null)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    Debug.Log("觸控開始");
                    lastTouchWorldPos = worldPoint;
                    isDragging = true;
                }
                else if (touch.phase == TouchPhase.Moved && isDragging)
                {
                    velocity = (worldPoint - lastTouchWorldPos) / Time.deltaTime; // ✅ 計算移動速度
                    lastTouchWorldPos = worldPoint;
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    Debug.Log("觸控結束");
                    isDragging = false;
                    velocity = Vector2.zero; // 停止時歸零
                }
            }
        }
#endif
    }

    void FixedUpdate()
    {
        if (isDragging)
        {
            Vector2 newPosition = rb.position + velocity * moveMultiplier * Time.fixedDeltaTime; // 平滑移動
            newPosition = ClampPosition(newPosition); // ✅ 限制玩家位置
            rb.MovePosition(newPosition);
        }
    }

    // 限制玩家移動範圍
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
