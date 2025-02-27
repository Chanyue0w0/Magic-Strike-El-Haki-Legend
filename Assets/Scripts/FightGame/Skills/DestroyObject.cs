using System.Collections;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    [SerializeField] private float dTime = 3f; // �]�w�R���ɶ�
    [SerializeField] private bool unscaleDestroy = false; // �O�_���� TimeScale

    void Start()
    {
        if (!unscaleDestroy)
        {
            Destroy(gameObject, dTime); // �̾ڤ@��ɶ��R��
        }
        else
        {
            StartCoroutine(DestroyAfterUnscaledTime(dTime)); // �̾ګD���v�T�ɶ��R��
        }
    }

    private IEnumerator DestroyAfterUnscaledTime(float time)
    {
        yield return new WaitForSecondsRealtime(time); // �ϥ� Unscaled �ɶ��ӭp��
        Destroy(gameObject);
    }
}
