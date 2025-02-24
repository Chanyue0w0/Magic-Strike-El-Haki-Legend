using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerNotification : MonoBehaviour, IDamageable, IStatusEffectReceiver
{
    [SerializeField] private int nowDamage;
    [SerializeField] private StatusEffect nowStatusEffect;

    // 事件通知 PlayerManager
    public event Action<int, GameObject> OnDamageReceived;

    public void Start()
    {
        
        nowStatusEffect = 0;
    }

    public void ResetNowDamage()
    {
        nowDamage = 0;
    }

    public void ResetNowStatusEffect()
    {
        nowStatusEffect = StatusEffect.None;
    }

    public void TakeDamage(int damage)
    {
        nowDamage += damage;
        Debug.Log($"{gameObject.name} 受到 {damage} 傷害");

        // 觸發事件，通知 PlayerManager
        OnDamageReceived?.Invoke(damage, gameObject);

    }

    public void ApplyStatusEffect(StatusEffect effect)
    {
        nowStatusEffect = effect;
        Debug.Log($"{gameObject.name} 受到狀態影響：{effect}");

        switch (effect)
        {
            case StatusEffect.None:
                Debug.Log($"{gameObject.name} 無特殊效果！");
                break;
            case StatusEffect.Burn:
                StartCoroutine(BurnEffect());
                break;
            case StatusEffect.Freeze:
                Debug.Log($"{gameObject.name} 被凍結！");
                break;
            case StatusEffect.Stun:
                Debug.Log($"{gameObject.name} 被暈眩！");
                break;
        }
    }

    private System.Collections.IEnumerator BurnEffect()
    {
        for (int i = 0; i < 5; i++) // 燃燒 5 秒，每秒扣 5 點血
        {
            TakeDamage(5);
            yield return new WaitForSeconds(1);
        }
    }
}
