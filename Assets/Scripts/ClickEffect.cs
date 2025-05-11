using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickEffect : MonoBehaviour
{
	public GameObject clickEffectPrefab; // 拖入你的點擊特效 prefab


	public static ClickEffect Instance { get; private set; }

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(this); // 若已存在一個，新的就銷毀，避免重複
		}
	}

	void Update()
	{
		Vector3 spawnPosition;

		// 滑鼠點擊
		if (Input.GetMouseButtonDown(0))
		{
			spawnPosition = GetWorldPosition(Input.mousePosition);
			SpawnEffect(spawnPosition);
		}

		// 手機觸控
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			spawnPosition = GetWorldPosition(Input.GetTouch(0).position);
			SpawnEffect(spawnPosition);
		}
	}

	Vector3 GetWorldPosition(Vector3 screenPosition)
	{
		Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
		worldPos.z = 0f; // 確保在正確的 Z 軸位置（2D 使用）
		return worldPos;
	}

	void SpawnEffect(Vector3 position)
	{
		Instantiate(clickEffectPrefab, position, Quaternion.identity);
	}
}
