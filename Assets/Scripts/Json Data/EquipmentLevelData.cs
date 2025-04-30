using System.Collections.Generic;
using UnityEngine;

public class EquipmentLevelData : MonoBehaviour
{
	[SerializeField] private string filePath = "jsonData/EquipmentLevel";
	private class EquipmentStats
	{
		public int AttackPower;
		public int HealthPoints;
		public int CostMoney;
	}

	private Dictionary<string, Dictionary<int, EquipmentStats>> equipmentData = new();

	void Awake()
	{
		LoadCSV(filePath); // Resources/EquipmentLevel.csv
	}

	private void LoadCSV(string fileName)
	{
		TextAsset csvFile = Resources.Load<TextAsset>(fileName);
		if (csvFile == null)
		{
			Debug.LogError("CSV file not found: " + fileName);
			return;
		}

		string[] lines = csvFile.text.Split('\n');
		string currentID = "";
		for (int i = 1; i < lines.Length; i++) // skip header
		{
			if (string.IsNullOrWhiteSpace(lines[i])) continue;

			string[] tokens = lines[i].Split(',');

			if (!string.IsNullOrWhiteSpace(tokens[0]))
				currentID = tokens[0].Trim();

			if (string.IsNullOrEmpty(currentID)) continue;

			int level, attack, hp, cost;
			if (!int.TryParse(tokens[2], out level) || !int.TryParse(tokens[3], out attack) || !int.TryParse(tokens[4], out hp) || !int.TryParse(tokens[5], out cost))
			{
				Debug.Log($"資料格式錯誤，跳過第 {i + 1} 行: {lines[i]}");
				continue;
			}

			if (!equipmentData.ContainsKey(currentID))
				equipmentData[currentID] = new Dictionary<int, EquipmentStats>();

			equipmentData[currentID][level] = new EquipmentStats
			{
				AttackPower = attack,
				HealthPoints = hp,
				CostMoney = cost
			};
		}
	}

	public int GetAttackPower(string id, int level)
	{
		return equipmentData[id][level].AttackPower;
	}

	public int GetHealthPoints(string id, int level)
	{
		return equipmentData[id][level].HealthPoints;
	}

	public int GetCostMoney(string id, int level)
	{
		return equipmentData[id][level].CostMoney;
	}
}
