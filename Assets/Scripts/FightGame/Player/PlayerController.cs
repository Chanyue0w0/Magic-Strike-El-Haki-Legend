//using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

	[Header("------------------- Config --------------------")]
	[SerializeField] private bool showTouchStatusInDebugLog = false;
	[SerializeField] private Transform moveEdgeTopLeft;
	[SerializeField] private Transform moveEdgeButtomRight;
	[Header("------------------- Value Reference --------------------")]
	[SerializeField] private new Camera camera;
    [SerializeField] private GameObject playerObject;
	[SerializeField] private GameObject touchCircle;
	private List<TouchLocation> touches = new();

	[SerializeField] private int firstLawTouch = -1;

	private void Start()
	{
		firstLawTouch = -1;
	}
	// Update is called once per frame
	void Update()
	{
		//Vector2 touchMovement = touch.deltaPosition; // Movement vector
		//Vector2 touchVelocity = touchMovement / touch.deltaTime;

		int i = 0;
		while (i < Input.touchCount)
		{
			Touch t = Input.GetTouch(i);

			if (t.phase == TouchPhase.Began)
			{
				if (showTouchStatusInDebugLog) Debug.Log("touch began");
				touches.Add(new TouchLocation(t.fingerId, CreateCircle(t)));
			}
			else if (t.phase == TouchPhase.Ended)
			{
				EndTouch(t);
			}
			else if (t.phase == TouchPhase.Moved)
			{
				if(!IsTouchInEdge(t))
				{
					EndTouch(t);
				}
				if(showTouchStatusInDebugLog) Debug.Log("touch is moving");
				TouchLocation thisTouch = touches.Find(TouchLocation => TouchLocation.touchId == t.fingerId);
				if (thisTouch != null)
				{
					if (firstLawTouch == -1) firstLawTouch = thisTouch.touchId;

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
		return pos.x > moveEdgeTopLeft.position.x && pos.x < moveEdgeButtomRight.position.x && pos.y > moveEdgeButtomRight.position.y && pos.y < moveEdgeTopLeft.position.y;
	}

	private Vector2 GetWorldPoistionByCamera(Vector2 touchPosition)
	{
		return camera.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, transform.position.z));
	}

	private GameObject CreateCircle(Touch t)
	{
		GameObject c = Instantiate(touchCircle) as GameObject;

		c.name = "Touch" + t.fingerId;
		c.transform.position = GetWorldPoistionByCamera(t.position);
		return c;
	}
}
