using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    [Header("移動設定")]
    public float moveMultiplier = 10.0f;
    public float movementThreshold = 0.01f;

    [Tooltip("桌面所在的Layer，用於射線偵測")]
    public LayerMask tableLayerMask;

    private Camera mainCamera;
    private Vector2 lastTouchWorldPos;
    private Vector2 previousTouchWorldPos;
    private bool isDragging = false;
    private Rigidbody2D rb;
    private Vector2 velocity;

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
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0.1f, tableLayerMask);

            if (hit.collider != null)
            {
                lastTouchWorldPos = hit.point;
                previousTouchWorldPos = hit.point;
                isDragging = true;
            }
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            if ((worldPoint - previousTouchWorldPos).magnitude > movementThreshold)
            {
                velocity = (worldPoint - lastTouchWorldPos) / Time.deltaTime;
                lastTouchWorldPos = worldPoint;
            }
            else
            {
                velocity = Vector2.zero;
            }
            previousTouchWorldPos = worldPoint;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            velocity = Vector2.zero;
        }

#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0.1f, tableLayerMask);

            if (hit.collider != null)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    lastTouchWorldPos = worldPoint;
                    previousTouchWorldPos = worldPoint;
                    isDragging = true;
                }
                else if (touch.phase == TouchPhase.Moved && isDragging)
                {
                    if ((worldPoint - previousTouchWorldPos).magnitude > movementThreshold)
                    {
                        velocity = (worldPoint - lastTouchWorldPos) / Time.deltaTime;
                        lastTouchWorldPos = worldPoint;
                    }
                    else
                    {
                        velocity = Vector2.zero;
                    }
                    previousTouchWorldPos = worldPoint;
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    isDragging = false;
                    velocity = Vector2.zero;
                }
            }
        }
#endif
    }

    void FixedUpdate()
    {
        if (isDragging)
        {
            Vector2 newPosition = rb.position + velocity * moveMultiplier * Time.fixedDeltaTime;
            newPosition = ClampPosition(newPosition);
            rb.MovePosition(newPosition);
        }
    }

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
