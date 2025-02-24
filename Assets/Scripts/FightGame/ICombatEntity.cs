using UnityEngine;
using System;

public interface IDamageable
{
    void TakeDamage(int damage);
    //event Action<int, GameObject> OnDamageReceived; // 事件：通知 PlayerManager（傷害值、被攻擊的玩家）
}

public interface IStatusEffectReceiver
{
    void ApplyStatusEffect(StatusEffect effect);
}

public enum StatusEffect
{
    None,    // 無效果
    Stun,    // 暈眩
    Charmed, // 魅惑
    Poison,  // 中毒

    Burn,    // 燃燒
    Freeze,  // 凍結
    Slow     // 減速
}
