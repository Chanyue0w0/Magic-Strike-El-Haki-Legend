using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerNotification : MonoBehaviour, IDamageable, IStatusEffectReceiver
{
    [SerializeField] private int playerNumber;
    [SerializeField] private int nowDamage;
    [SerializeField] private StatusEffect nowStatusEffect;

    // 事件通知 PlayerManager
    public event Action<int, GameObject> OnDamageReceived;
    public event Action<StatusEffect, GameObject> OnStatusEffectApplied; // 新增事件，通知 PlayerStatusManager
    public event Action<int> OnGetMagicPointApplied; // 新增事件，通知 PlayerStatusManager 得到一點魔力


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
        //Debug.Log($"{gameObject.name} 受到 {damage} 傷害");

        // 觸發事件，通知 PlayerManager
        OnDamageReceived?.Invoke(damage, gameObject);

        ResetNowDamage();
    }

    public void ApplyStatusEffect(StatusEffect effect)
    {
        nowStatusEffect = effect;
        // 觸發事件，通知 PlayerStatusManager
        OnStatusEffectApplied?.Invoke(effect, gameObject);

        switch (effect)
        {
            case StatusEffect.None:
                //Debug.Log($"{gameObject.name} 傳輸無特殊效果！");
                break;
            case StatusEffect.Burn:
                //Debug.Log($"{gameObject.name} 傳輸被燃燒！");
                break;
            case StatusEffect.Freeze:
                //Debug.Log($"{gameObject.name} 傳輸被凍結！");
                break;
            case StatusEffect.Stun:
                //Debug.Log($"{gameObject.name} 傳輸被暈眩！");
                break;
        }
    }

    public void GetMagicPointNotify(int pNumber)//提醒得到魔力
    {
        OnGetMagicPointApplied?.Invoke(pNumber);
        Debug.Log("傳輸" + pNumber);
    }

    //private System.Collections.IEnumerator BurnEffect()
    //{
    //    for (int i = 0; i < 5; i++) // 燃燒 5 秒，每秒扣 5 點血
    //    {
    //        int burnDamage = Mathf.RoundToInt(20 * (1 + FightPlayer1Config.BurnDamageIncrease));
    //        TakeDamage(burnDamage);
    //        yield return new WaitForSeconds(1);
    //    }
    //}
}
