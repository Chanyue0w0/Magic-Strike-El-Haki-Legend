using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstCoins : MonoBehaviour
{
    [Header("----------------- GameObject ------------------")]
    [SerializeField] private GameObject coinObject;

    [Header("----------------- Data ------------------")]
    [SerializeField] private int instAmount;
    [SerializeField] private int instPerAxis; // 每個硬幣誤差最短距離
    [SerializeField] private Vector2 instRangeX;  // 每個硬幣X軸範圍
    [SerializeField] private Vector2 instRangeY;  // 每個硬幣Y軸範圍
    [SerializeField] private Vector2 instRotationY; // 每個硬幣Y軸旋轉範圍 (+-42)
    [SerializeField] private float delaySeconds;  // 延遲時間

    void Start()
    {
        coinObject = Resources.Load<GameObject>("Prefabs/Effect/Coin");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            InstCoinsFountain();
        }
    }

    public void InstCoinsFountain()
    {
        StartCoroutine(SpawnCoinsAfterDelay());
    }

    private IEnumerator SpawnCoinsAfterDelay()
    {
        yield return new WaitForSeconds(delaySeconds);

        for (int i = 0; i < instAmount; i++)
        {
            // 隨機位置
            float randX = Random.Range(instRangeX.x, instRangeX.y);
            float randY = Random.Range(instRangeY.x, instRangeY.y);
            Vector3 spawnPos = gameObject.transform.position + new Vector3(randX, randY, 0f);

            // 隨機旋轉
            float randRotY = Random.Range(instRotationY.x, instRotationY.y);
            Quaternion spawnRot = Quaternion.Euler(0f, randRotY, 0f);

            // 生成金幣
            Instantiate(coinObject, spawnPos, spawnRot);
        }
    }
}
