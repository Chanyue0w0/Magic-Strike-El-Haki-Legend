using System.Collections.Generic;
using UnityEngine;

public class EquipmentLevelData : MonoBehaviour
{
	[SerializeField] private string filePath = "jsonData/EquipmentLevel"; // Resources/EquipmentLevel.csv

	private class EquipmentStats
	{
		public int AttackPower;
		public int HealthPoints;
		public int CostMoney;
	}

	// 第一層 key: id
	// 第二層 key: rarity (string)
	// 第三層 key: level
	private Dictionary<string, Dictionary<string, Dictionary<int, EquipmentStats>>> equipmentLevelData = new();

	void Awake()
	{
		LoadCSV(filePath);
	}

	private void LoadCSV(string fileName)
	{
		TextAsset csvFile = Resources.Load<TextAsset>(fileName);
		if (csvFile == null)
		{
			Debug.LogError($"CSV file not found: {fileName}");
			return;
		}

		string[] lines = csvFile.text.Split('\n');
		string currentID = "";
		string currentRare = "";

		for (int i = 1; i < lines.Length; i++) // skip header
		{
			string line = lines[i].Trim();
			if (string.IsNullOrWhiteSpace(line)) continue;

			var tokens = line.Split(',');
			if (tokens.Length < 6)
			{
				Debug.LogWarning($"Invalid CSV format at line {i + 1}: {line}");
				continue;
			}

			// 更新 currentID、currentRare（若該欄位為空則沿用上一筆）
			if (!string.IsNullOrWhiteSpace(tokens[0]))
				currentID = tokens[0].Trim();
			if (!string.IsNullOrWhiteSpace(tokens[1]))
				currentRare = tokens[1].Trim();

			if (string.IsNullOrEmpty(currentID) || string.IsNullOrEmpty(currentRare))
				continue;

			if (!int.TryParse(tokens[2], out int level) ||
				!int.TryParse(tokens[3], out int attack) ||
				!int.TryParse(tokens[4], out int hp) ||
				!int.TryParse(tokens[5], out int cost))
			{
				Debug.Log($"Load equipment level data, Parse error at line {i + 1}: {line}");
				continue;
			}

			// 建立多層字典
			if (!equipmentLevelData.ContainsKey(currentID))
				equipmentLevelData[currentID] = new Dictionary<string, Dictionary<int, EquipmentStats>>();

			if (!equipmentLevelData[currentID].ContainsKey(currentRare))
				equipmentLevelData[currentID][currentRare] = new Dictionary<int, EquipmentStats>();

			equipmentLevelData[currentID][currentRare][level] = new EquipmentStats
			{
				AttackPower = attack,
				HealthPoints = hp,
				CostMoney = cost
			};
		}
	}

	/// <summary>
	/// 取得 AttackPower；若指定 rarity 不存在則發出警告並改為 "Common"
	/// </summary>
	public int GetAttackPower(string id, string rarity, int level)
	{
		if (!equipmentLevelData.ContainsKey(id))
		{
			Debug.LogError($"Equipment ID '{id}' not found.");
			return 0;
		}

		var rarityKey = rarity;
		if (!equipmentLevelData[id].ContainsKey(rarityKey))
		{
			Debug.LogWarning($"Rarity '{rarityKey}' not found for ID '{id}'. Falling back to 'Common'.");
			rarityKey = "Common";
		}

		if (!equipmentLevelData[id].ContainsKey(rarityKey) ||
			!equipmentLevelData[id][rarityKey].ContainsKey(level))
		{
			Debug.LogError($"Data missing for ID='{id}', Rarity='{rarityKey}', Level={level}.");
			return 0;
		}

		return equipmentLevelData[id][rarityKey][level].AttackPower;
	}

	/// <summary>
	/// 取得 HealthPoints；若指定 rarity 不存在則發出警告並改為 "Common"
	/// </summary>
	public int GetHealthPoints(string id, string rarity, int level)
	{
		if (!equipmentLevelData.ContainsKey(id))
		{
			Debug.LogError($"Equipment ID '{id}' not found.");
			return 0;
		}

		var rarityKey = rarity;
		if (!equipmentLevelData[id].ContainsKey(rarityKey))
		{
			Debug.LogWarning($"Rarity '{rarityKey}' not found for ID '{id}'. Falling back to 'Common'.");
			rarityKey = "Common";
		}

		if (!equipmentLevelData[id].ContainsKey(rarityKey) ||
			!equipmentLevelData[id][rarityKey].ContainsKey(level))
		{
			Debug.LogError($"Data missing for ID='{id}', Rarity='{rarityKey}', Level={level}.");
			return 0;
		}

		return equipmentLevelData[id][rarityKey][level].HealthPoints;
	}

	/// <summary>
	/// 取得 CostMoney；若指定 rarity 不存在則發出警告並改為 "Common"
	/// </summary>
	public int GetCostMoney(string id, string rarity, int level)
	{
		if (!equipmentLevelData.ContainsKey(id))
		{
			Debug.LogError($"Equipment ID '{id}' not found.");
			return 0;
		}

		var rarityKey = rarity;
		if (!equipmentLevelData[id].ContainsKey(rarityKey))
		{
			Debug.LogWarning($"Rarity '{rarityKey}' not found for ID '{id}'. Falling back to 'Common'.");
			rarityKey = "Common";
		}

		if (!equipmentLevelData[id].ContainsKey(rarityKey) ||
			!equipmentLevelData[id][rarityKey].ContainsKey(level))
		{
			Debug.LogError($"Data missing for ID='{id}', Rarity='{rarityKey}', Level={level}.");
			return 0;
		}

		return equipmentLevelData[id][rarityKey][level].CostMoney;
	}
}
