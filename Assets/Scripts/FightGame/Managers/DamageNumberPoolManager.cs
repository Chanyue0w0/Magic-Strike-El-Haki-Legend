using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumberPoolManager : MonoBehaviour
{
    public static DamageNumberPoolManager Instance { get; private set; }

    [SerializeField] private GameObject[] damageEffects = new GameObject[10];
    private Queue<GameObject>[] damageEffectPools = new Queue<GameObject>[10];
    private GameObject poolContainer;
    private int defaultPoolSize = 10;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        poolContainer = new GameObject("DamageNumberPoolContainer");
        poolContainer.transform.position = Vector3.zero;
        InitializePools();
    }

    private void InitializePools()
    {
        for (int i = 0; i < damageEffects.Length; i++)
        {
            damageEffects[i] = (GameObject)Resources.Load($"Prefabs/DamageNumbers/{i}");
            if (damageEffects[i] == null)
            {
                Debug.LogWarning($"Damage effect prefab for digit {i} not found in Resources.");
            }
            else
            {
                damageEffectPools[i] = new Queue<GameObject>();
                for (int j = 0; j < defaultPoolSize; j++)
                {
                    GameObject obj = Instantiate(damageEffects[i], poolContainer.transform);
                    obj.SetActive(false);
                    damageEffectPools[i].Enqueue(obj);
                }
            }
        }
    }

    public GameObject GetDamageEffect(int digit)
    {
        if (digit < 0 || digit >= damageEffectPools.Length)
        {
            Debug.LogWarning($"Damage effect index {digit} is out of range.");
            return null;
        }

        if (damageEffectPools[digit].Count > 0)
        {
            GameObject obj = damageEffectPools[digit].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            Debug.LogWarning($"Damage effect pool for digit {digit} is empty. Instantiating new object.");
            GameObject obj = Instantiate(damageEffects[digit], poolContainer.transform);
            obj.SetActive(true);
            return obj;
        }
    }

    public void ReturnToPool(GameObject obj, int digit)
    {
        obj.SetActive(false);
        obj.transform.SetParent(poolContainer.transform);
        if (digit >= 0 && digit < damageEffectPools.Length)
        {
            damageEffectPools[digit].Enqueue(obj);
        }
        else
        {
            Debug.LogWarning($"Damage effect index {digit} is out of range.");
        }
    }
}
