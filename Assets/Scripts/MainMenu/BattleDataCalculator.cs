using System.Collections.Generic;
using UnityEngine;

public class BattleDataCalculator : MonoBehaviour
{
	[SerializeField] public string playerName;
	[SerializeField] public int totalHP;
	[SerializeField] public int totalATK;
	[SerializeField] public float critRateIncrease;
	[SerializeField] public float skillDamageIncrease;
	[SerializeField] public float poisonDamageIncrease;
	[SerializeField] public float controlSkillDurationIncrease;
	[SerializeField] public float totalSkillBubbleCooldownReduction;
	[SerializeField] public string setEffect;
	[SerializeField] public string skill_1;
	[SerializeField] public string skill_2;

	void Start()
	{
	}

	public void CalculateBattleData(string heroID)
	{
		var hero = PlayerHeroManager.Instance.GetHeroByID(heroID);
		if (hero == null)
		{
			Debug.LogWarning("Hero not found: " + heroID);
			return;
		}
		playerName = hero.name;
		int baseHP = hero.baseHP;
		int baseATK = hero.baseATK;
		skill_1 = hero.equippedItems[3];
		skill_2 = hero.equippedItems[4];

		int equipmentTotalHP = 0;
		int equipmentTotalATK = 0;
		float totalHPBuff = 1.0f;
		float totalATKBuff = 1.0f;

		critRateIncrease = 0f;
		skillDamageIncrease = 0f;
		poisonDamageIncrease = 0f;
		controlSkillDurationIncrease = 0f;
		totalSkillBubbleCooldownReduction = 0f;

		Dictionary<string, int> setTypeCount = new Dictionary<string, int>();

		foreach (var equipmentId in hero.equippedItems)
		{
			var equipment = PlayerEquipmentManager.Instance.GetEquipmentByID(equipmentId);
			if (equipment != null)
			{
				equipmentTotalHP += equipment.healthPoints;
				equipmentTotalATK += equipment.attackPower;

				// 根據 rarity 決定允許計算的 buff 數量
				int allowedBuffCount = GetEquipmentRarity(equipment.rarity); // Normal=0, Common=1, Rare=2, Special=3, Legendary=4
				if (allowedBuffCount > 0)
				{
					List<string> buffKeys = new List<string>(equipment.buffs.Keys);
					int countToApply = Mathf.Min(allowedBuffCount, buffKeys.Count);
					for (int i = 0; i < countToApply; i++)
					{
						string buffKey = buffKeys[i];
						string buffValue = equipment.buffs[buffKey];
						switch (buffKey)
						{
							case "Total Health Increase%":
								totalHPBuff += ParseBuffValue(buffValue);
								break;
							case "Total Attack Increase%":
								totalATKBuff += ParseBuffValue(buffValue);
								break;
							case "Critical Rate Increase%":
								critRateIncrease += ParseBuffValue(buffValue);
								break;
							case "Skill Damage Increase%":
								skillDamageIncrease += ParseBuffValue(buffValue);
								break;
							case "Poison Damage Increase%":
								poisonDamageIncrease += ParseBuffValue(buffValue);
								break;
							case "Control Duration Increase%":
								controlSkillDurationIncrease += ParseBuffValue(buffValue);
								break;
							case "Reduce Skill Bubble Generation Time":
								totalSkillBubbleCooldownReduction += ParseBuffValue(buffValue);
								break;
						}
					}
				}

				if (!string.IsNullOrEmpty(equipment.setType) && equipment.setType != "None")
				{
					if (!setTypeCount.ContainsKey(equipment.setType))
						setTypeCount[equipment.setType] = 0;
					setTypeCount[equipment.setType]++;
				}
			}
		}

		totalHP = (int)((baseHP + equipmentTotalHP) * totalHPBuff);
		totalATK = (int)((baseATK + equipmentTotalATK) * totalATKBuff);
		setEffect = DetermineSetEffect(setTypeCount);


		skill_1 = (skill_1 == null || skill_1 == "") ? skill_2 : skill_1;
		skill_2 = (skill_2 == null || skill_2 == "") ? skill_1 : skill_2;

		if (skill_1 == null || skill_1 == "")
		{
			skill_1 = "SK00";
			skill_2 = "SK00";
		}
	}


	public void ApplyToFightPlayerConfig()
	{
		FightPlayer1Config.StartHP = totalHP;
		FightPlayer1Config.NowHP = totalHP;
		FightPlayer1Config.StartATK = totalATK;
		FightPlayer1Config.NowATK = totalATK;
		FightPlayer1Config.CriticalPercentage = critRateIncrease;
		FightPlayer1Config.SkillDamageIncrease = skillDamageIncrease;
		FightPlayer1Config.PoisonDamageIncrease = poisonDamageIncrease;
		FightPlayer1Config.CC_SkillTimeIncrease = controlSkillDurationIncrease;
		FightPlayer1Config.SkillBubbleTimeDecrease = totalSkillBubbleCooldownReduction;
		FightPlayer1Config.EquipSet = setEffect;
		FightPlayer1Config.Group[1] = skill_1;
		FightPlayer1Config.Group[2] = skill_2;
	}

	private float ParseBuffValue(string buff)
	{
		buff = buff.Replace("%", "").Replace("s", "");
		if (float.TryParse(buff, out float value))
		{
			return value / 100f;
		}
		return 0f;
	}

	private string DetermineSetEffect(Dictionary<string, int> setTypeCount)
	{
		foreach (var set in setTypeCount)
		{
			if (set.Value >= 3)
				return set.Key;
		}
		return "None";
	}

	private int GetEquipmentRarity(string rarity)
	{
		switch (rarity)
		{
			case "Normal":
				return 0;
			case "Common":
				return 1;
			case "Rare":
				return 2;
			case "Special":
				return 3;
			case "Legendary":
				return 4;
			default:
				return -1;
		}
	}
}
