using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerNotification : MonoBehaviour, IDamageable, IStatusEffectReceiver
{
    [SerializeField] private int nowDamage;
    [SerializeField] private StatusEffect nowStatusEffect;

    // �ƥ�q�� PlayerManager
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
        Debug.Log($"{gameObject.name} ���� {damage} �ˮ`");

        // Ĳ�o�ƥ�A�q�� PlayerManager
        OnDamageReceived?.Invoke(damage, gameObject);

    }

    public void ApplyStatusEffect(StatusEffect effect)
    {
        nowStatusEffect = effect;
        Debug.Log($"{gameObject.name} ���쪬�A�v�T�G{effect}");

        switch (effect)
        {
            case StatusEffect.None:
                Debug.Log($"{gameObject.name} �L�S��ĪG�I");
                break;
            case StatusEffect.Burn:
                StartCoroutine(BurnEffect());
                break;
            case StatusEffect.Freeze:
                Debug.Log($"{gameObject.name} �Q�ᵲ�I");
                break;
            case StatusEffect.Stun:
                Debug.Log($"{gameObject.name} �Q�w�t�I");
                break;
        }
    }

    private System.Collections.IEnumerator BurnEffect()
    {
        for (int i = 0; i < 5; i++) // �U�N 5 ��A�C�� 5 �I��
        {
            TakeDamage(5);
            yield return new WaitForSeconds(1);
        }
    }
}
