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
		public int TotalHP;  // (英雄HP + 裝備總HP) * 總HP Buff
		public int TotalATK; // (英雄ATK + 裝備總ATK) * 總ATK Buff
		public float CriticalRateIncrease; // 爆擊率增加 %
		public float SkillDamageIncrease;  // 技能傷害增加 %
		public float PoisonDamageIncrease; // 中毒傷害增加 %
		public float CCSkillTimeIncrease;  // 控場技能增加時間 %
		public float SkillBubbleCooldownReduction; // 技能泡泡生成時間減少（秒）
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
	//	return setTypes.Count == 1 ? setTypes[0] : "None"; // 只有全套裝備一致才觸發效果
	//}

}
