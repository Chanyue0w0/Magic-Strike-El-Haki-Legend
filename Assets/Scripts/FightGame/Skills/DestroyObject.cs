using System.Collections;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    [SerializeField] private float dTime = 3f; // 設定刪除時間
    [SerializeField] private bool unscaleDestroy = false; // 是否忽略 TimeScale

    void Start()
    {
        if (!unscaleDestroy)
        {
            Destroy(gameObject, dTime); // 依據一般時間刪除
        }
        else
        {
            StartCoroutine(DestroyAfterUnscaledTime(dTime)); // 依據非受影響時間刪除
        }
    }

    private IEnumerator DestroyAfterUnscaledTime(float time)
    {
        yield return new WaitForSecondsRealtime(time); // 使用 Unscaled 時間來計算
        Destroy(gameObject);
    }
}
