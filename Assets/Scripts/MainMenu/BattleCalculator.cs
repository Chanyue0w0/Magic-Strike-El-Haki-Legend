using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleCalculator : MonoBehaviour
{
	[System.Serializable]
	public class PreBattleStats
	{
		public string PlayerSkin;
		public int TotalHP;  // (�^��HP + �˳��`HP) * �`HP Buff
		public int TotalATK; // (�^��ATK + �˳��`ATK) * �`ATK Buff
		public float CriticalRateIncrease; // �z���v�W�[ %
		public float SkillDamageIncrease;  // �ޯ�ˮ`�W�[ %
		public float PoisonDamageIncrease; // ���r�ˮ`�W�[ %
		public float CCSkillTimeIncrease;  // �����ޯ�W�[�ɶ� %
		public float SkillBubbleCooldownReduction; // �ޯ�w�w�ͦ��ɶ���֡]��^
		public string SetBonus; // None, Warrior, Archer, Magician, Healer
	}

	//public static PreBattleStats CalculateFinalStats(HeroData.Hero player, EquipmentData[] equippedItems, float hpBuff, float atkBuff)
	//{
	//	int totalHP = (player.BaseHP + SumEquipmentHP(equippedItems)) * Mathf.RoundToInt(1 + hpBuff);
	//	int totalATK = (player.BaseATK + SumEquipmentATK(equippedItems)) * Mathf.RoundToInt(1 + atkBuff);

	//	return new PreBattleStats
	//	{
	//		PlayerSkin = player.Skin,
	//		TotalHP = totalHP,
	//		TotalATK = totalATK,
	//		CriticalRateIncrease = player.CriticalRate + SumEquipmentCriticalRate(equippedItems),
	//		SkillDamageIncrease = player.SkillDamageBuff + SumEquipmentSkillDamage(equippedItems),
	//		PoisonDamageIncrease = player.PoisonDamageBuff + SumEquipmentPoisonDamage(equippedItems),
	//		CCSkillTimeIncrease = player.CCIncreaseBuff + SumEquipmentCCBuff(equippedItems),
	//		SkillBubbleCooldownReduction = player.SkillBubbleReduction + SumEquipmentBubbleReduction(equippedItems),
	//		SetBonus = DetermineSetBonus(equippedItems)
	//	};
	//}

	//private static int SumEquipmentHP(EquipmentData[] items) => items.Sum(e => e.HealthPoints);
	//private static int SumEquipmentATK(EquipmentData[] items) => items.Sum(e => e.AttackPower);

	//private static string DetermineSetBonus(EquipmentData[] items)
	//{
	//	var setTypes = items.Select(e => e.SetType).Distinct().ToList();
	//	return setTypes.Count == 1 ? setTypes[0] : "None"; // �u�����M�˳Ƥ@�P�~Ĳ�o�ĪG
	//}

}
