using UnityEngine;
using System;

public interface IDamageable
{
    void TakeDamage(int damage);
    //event Action<int, GameObject> OnDamageReceived; // �ƥ�G�q�� PlayerManager�]�ˮ`�ȡB�Q���������a�^
}

public interface IStatusEffectReceiver
{
    void ApplyStatusEffect(StatusEffect effect);
}

public enum StatusEffect
{
    None,    // �L�ĪG
    Stun,    // �w�t
    Charmed, // �y�b
    Poison,  // ���r

    Burn,    // �U�N
    Freeze,  // �ᵲ
    Slow     // ��t
}
