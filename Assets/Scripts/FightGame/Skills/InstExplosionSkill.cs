using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstExplosionSkill : MonoBehaviour
{
    [SerializeField] private Vector2 instObjectOffset;
    [SerializeField] private float instObjectAmount = 1;
    [SerializeField] private float instObjectStartTime = 3;
    [SerializeField] private float instObjectDelayTime = 0;
    [SerializeField] private GameObject instObject;

    void Start()
    {
        StartCoroutine(SpawnObjects());
    }

    private IEnumerator SpawnObjects()
    {
        yield return new WaitForSeconds(instObjectStartTime); // 每次生成後等待指定的時間
        for (int i = 0; i < instObjectAmount; i++)
        {
            GameObject newObj = Instantiate(instObject, gameObject.transform.position + (Vector3)instObjectOffset, Quaternion.identity);
            yield return new WaitForSeconds(instObjectDelayTime); // 每次生成後等待指定的時間
        }
    }
}
