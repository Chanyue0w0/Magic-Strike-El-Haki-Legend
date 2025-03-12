using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstObjectSkill : MonoBehaviour
{
    [SerializeField] private Vector2 instObjectOffset;
    [SerializeField] private float instObjectAmount = 1;
    [SerializeField] private float instObjectDelayTime = 1;
    [SerializeField] private GameObject instObject; // 魔法煙霧(移動至生成定點再產生史萊姆)
    private List<GameObject> nowObjects = new List<GameObject>(); // 記錄當前生成的物件

    void Start()
    {
        StartCoroutine(SpawnObjects());
    }

    private IEnumerator SpawnObjects()
    {
        for (int i = 0; i < instObjectAmount; i++)
        {
            GameObject newObj = Instantiate(instObject, transform.position + (Vector3)instObjectOffset, Quaternion.identity);
            nowObjects.Add(newObj); // 記錄生成的物件
            yield return new WaitForSeconds(instObjectDelayTime); // 每次生成後等待指定的時間
        }
    }

    public void DeleteObjects()
    {
        foreach (GameObject obj in nowObjects)
        {
            if (obj != null)
            {
                Destroy(obj); // 刪除物件
            }
        }
        nowObjects.Clear(); // 清空列表
    }
}
