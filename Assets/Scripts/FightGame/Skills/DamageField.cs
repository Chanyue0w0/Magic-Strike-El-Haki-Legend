using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageField : MonoBehaviour
{
    [Header("----------------- Skill info ------------------")]
    [SerializeField] private int playerNumber = 1;
    [SerializeField] private int targetNumber = 2;
    [SerializeField] private int skillDamage = 100;
    [SerializeField] private StatusEffect EffectToApply = StatusEffect.None; // 要套用的狀態
    private HashSet<Collider2D> activeColliders = new HashSet<Collider2D>();//偵測到的物體

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((targetNumber == 1 && collision.CompareTag("Player1"))
            || (targetNumber == 2 && collision.CompareTag("Player2")))
        {
            if (!activeColliders.Contains(collision))
            {
                activeColliders.Add(collision);
                StartCoroutine(DamageOverTimeCoroutine(collision));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (activeColliders.Contains(collision))
        {
            activeColliders.Remove(collision);
        }
    }

    private IEnumerator DamageOverTimeCoroutine(Collider2D target)
    {
        while (activeColliders.Contains(target))
        {
            // 傷害處理
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(skillDamage);
            }

            // 狀態效果處理
            IStatusEffectReceiver statusReceiver = target.GetComponent<IStatusEffectReceiver>();
            if (statusReceiver != null)
            {
                statusReceiver.ApplyStatusEffect(EffectToApply);
            }

            // 等待 1 秒再下一次傷害
            yield return new WaitForSeconds(1f);
        }
    }

}
