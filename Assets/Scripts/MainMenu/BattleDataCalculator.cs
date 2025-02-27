using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDataCalculator : MonoBehaviour
{
	[SerializeField] private string playerName;
	[SerializeField] private int totalHP;
	[SerializeField] private int totalATK;
	[SerializeField] private float critRateIncrease;
	[SerializeField] private float skillDamageIncrease;
	[SerializeField] private float poisonDamageIncrease;
	[SerializeField] private float controlSkillDurationIncrease;
	[SerializeField] private float totalSkillBubbleCooldownReduction;
	[SerializeField] private string setEffect;

	void Start()
	{
		CalculateBattleData("HR01");
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
			var equipment = PlayerEquipmentManager.Instance.GetAllEquipmentData().Find(e => e.id == equipmentId);
			if (equipment != null)
			{
				equipmentTotalHP += equipment.healthPoints;
				equipmentTotalATK += equipment.attackPower;

				foreach (var buff in equipment.buffs)
				{
					switch (buff.Key)
					{
						case "Total Health Increase%":
							totalHPBuff += ParseBuffValue(buff.Value);
							break;
						case "Total Attack Increase%":
							totalATKBuff += ParseBuffValue(buff.Value);
							break;
						case "Critical Rate Increase%":
							critRateIncrease += ParseBuffValue(buff.Value);
							break;
						case "Skill Damage Increase%":
							skillDamageIncrease += ParseBuffValue(buff.Value);
							break;
						case "Poison Damage Increase%":
							poisonDamageIncrease += ParseBuffValue(buff.Value);
							break;
						case "Control Duration Increase%":
							controlSkillDurationIncrease += ParseBuffValue(buff.Value);
							break;
						case "Reduce Skill Bubble Generation Time":
							totalSkillBubbleCooldownReduction += ParseBuffValue(buff.Value);
							break;
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
	}

	private float ParseBuffValue(string buff)
	{
		string[] parts = buff.Split(' ');
		if (parts.Length > 1 && float.TryParse(parts[1].Replace("%", "").Replace("s", ""), out float value))
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
}
