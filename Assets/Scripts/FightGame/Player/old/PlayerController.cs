//using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private enum MoveVersion { byVelocity, byAddForce , byPosition };
	[Header("------------------- Config --------------------")]
	[SerializeField] private float moveSpeed = 1f;
	[SerializeField] private MoveVersion moveVersion;

	[SerializeField] private bool showTouchStatusInDebugLog = false;
	[Header("------------------- Variable Observe--------------------")]
	[SerializeField] private int firstLawTouch = -1;
	[SerializeField] private Vector2 touchMovement; // Movement vector
	[Header("------------------- Value Reference --------------------")]
	[SerializeField] private new Camera camera;
    [SerializeField] private GameObject playerObject;
	[SerializeField] private GameObject touchCircle;
	[SerializeField] private Transform touchEdgeTopLeft;
	[SerializeField] private Transform touchEdgeButtomRight;
	[SerializeField] private Transform moveEdgeTopLeft;
	[SerializeField] private Transform moveEdgeButtomRight;


	private List<TouchLocation> touches = new();

	private Vector3 startPoition;
	private float moveStartTime;
	private Vector3 endPoition;
	private Rigidbody2D playerRigidbody2D;
	private bool isDragging;

	private void Start()
	{
		playerRigidbody2D = playerObject.GetComponent<Rigidbody2D>();
		firstLawTouch = -1;
	}

	// Update is called once per frame
	void Update()
	{
		// keep player in move edge
		playerObject.transform.position = PositionToInEdge(playerObject.transform.position);

		// touch move
		int i = 0;
		while (i < Input.touchCount)
		{
			Touch t = Input.GetTouch(i);

			if (t.phase == TouchPhase.Began)
			{
				if (showTouchStatusInDebugLog) Debug.Log("touch began");
				touches.Add(new TouchLocation(t.fingerId, CreateCircle(t)));
				startPoition = t.position;
				moveStartTime = Time.deltaTime;
			}
			else if (t.phase == TouchPhase.Ended)
			{
				EndTouch(t);
			}
			else if (t.phase == TouchPhase.Moved)
			{
				// touch not in edge
				if(!IsTouchInEdge(t))
				{
					EndTouch(t);
				}

				if(showTouchStatusInDebugLog) Debug.Log("touch is moving");
				TouchLocation thisTouch = touches.Find(TouchLocation => TouchLocation.touchId == t.fingerId);
				if (thisTouch != null) // touch is legal
				{
					if (firstLawTouch == -1) firstLawTouch = thisTouch.touchId; // init first controll touch
					if (firstLawTouch == thisTouch.touchId) // is controll touch
					{
						// init data
						endPoition = t.position;
						float deltaTime = moveStartTime - Time.deltaTime;
						touchMovement = GetWorldPoistionByCamera(endPoition)- GetWorldPoistionByCamera(startPoition); // world Movement vector

						// move version
						//if (moveVersion == MoveVersion.byPosition)
						//{
						//    //playerRigidbody2D.velocity = Vector3.zero;

						//    Vector3 targetPoiston = PositionToInEdge(playerObject.transform.position + (Vector3)touchMovement * moveSpeed);
						//    playerObject.transform.position = targetPoiston;
						//}
						if (moveVersion == MoveVersion.byPosition)
						{
							Vector3 targetPosition = PositionToInEdge(playerObject.transform.position + (Vector3)touchMovement * moveSpeed);
							playerRigidbody2D.MovePosition(targetPosition);
						}

						//else if (moveVersion == MoveVersion.byVelocity)
						//{
						//	playerRigidbody2D.velocity = Vector3.zero;

						//	//Vector3 movePostion = (Vector3)touchMovement * moveSpeed;
						//	//Vector3 targetPoiston = PositionToInEdge(playerObject.transform.position + movePostion);
						//	playerRigidbody2D.velocity = endPoition-startPoition;
						//	if(t.deltaPosition == Vector2.zero) playerRigidbody2D.velocity = Vector3.zero;
						//}
						if (moveVersion == MoveVersion.byVelocity)
						{
							float minMoveThreshold = 0.5f; // 設定手指移動最小閾值（可以根據需求調整）

							// 先清除舊速度
							playerRigidbody2D.velocity = Vector2.zero;

							// 計算世界座標中的 deltaPosition
							Vector2 worldDeltaPosition = (GetWorldPoistionByCamera(t.position) - GetWorldPoistionByCamera(t.position - t.deltaPosition)) / Time.deltaTime;

							// 如果手指移動量小於閾值，則停止玩家移動
							if (t.deltaPosition.magnitude < minMoveThreshold)
							{
								playerRigidbody2D.velocity = Vector2.zero;
							}
							else
							{
								playerRigidbody2D.velocity = worldDeltaPosition * moveSpeed; // 設定移動速度
							}
						}
						else if (moveVersion == MoveVersion.byAddForce)
						{
							playerRigidbody2D.velocity = Vector3.zero;
							playerRigidbody2D.AddForce(t.deltaPosition * (moveSpeed*5) * playerRigidbody2D.mass);
						}
						else
						{
							if (showTouchStatusInDebugLog)
								Debug.Log("no move version");
						}

						// update data
						startPoition = endPoition;
						moveStartTime = Time.deltaTime;

					}

					// on touch poistion show white circle
					thisTouch.circle.transform.position = GetWorldPoistionByCamera(t.position);
				}

			}

			i += 1;
		}
	}


	private void EndTouch(Touch t)
	{
		if (showTouchStatusInDebugLog) Debug.Log("touch ended");
		TouchLocation thisTouch = touches.Find(TouchLocation => TouchLocation.touchId == t.fingerId);
		if (thisTouch == null) return;

		if (thisTouch.touchId == firstLawTouch) firstLawTouch = -1;
		Destroy(thisTouch.circle);
		touches.RemoveAt(touches.IndexOf(thisTouch));
	}
	private bool IsTouchInEdge(Touch t)
	{
		Vector2 pos = GetWorldPoistionByCamera(t.position);
		return pos.x > touchEdgeTopLeft.position.x && pos.x < touchEdgeButtomRight.position.x && pos.y > touchEdgeButtomRight.position.y && pos.y < touchEdgeTopLeft.position.y;
	}

	private Vector3 PositionToInEdge(Vector3 pos)
	{
		if (pos.x < moveEdgeTopLeft.position.x) pos.x = moveEdgeTopLeft.position.x;
		if (pos.x > moveEdgeButtomRight.position.x) pos.x = moveEdgeButtomRight.position.x;
		if (pos.y < moveEdgeButtomRight.position.y) pos.y = moveEdgeButtomRight.position.y;
		if (pos.y > moveEdgeTopLeft.position.y) pos.y = moveEdgeTopLeft.position.y;
		return pos;
	}
	private Vector2 GetWorldPoistionByCamera(Vector2 touchPosition)
	{
		return camera.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, camera.nearClipPlane));
	}

	private GameObject CreateCircle(Touch t)
	{
		GameObject c = Instantiate(touchCircle) as GameObject;

		c.name = "Touch" + t.fingerId;
		c.transform.position = GetWorldPoistionByCamera(t.position);
		return c;
	}

	public void SetMoveSpeed(float speed)
	{
		moveSpeed = speed;
	}

	public bool SetMoveVersion(string ver)
	{
		if (ver == "byAddForce") moveVersion = MoveVersion.byAddForce;
		else if (ver == "byVelocity") moveVersion = MoveVersion.byVelocity;
		else if (ver == "byPosition") moveVersion = MoveVersion.byPosition;
		else return false;
		return true;
	}
}
