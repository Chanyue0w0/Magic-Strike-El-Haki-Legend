using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class EquipmentLevelDataEditor : EditorWindow
{
	private const string csvFilePath = "Assets/Resources/jsonData/EquipmentLevel.csv";
	private Dictionary<string, Dictionary<string, Dictionary<int, EquipmentStats>>> equipmentData
		= new Dictionary<string, Dictionary<string, Dictionary<int, EquipmentStats>>>();

	private Vector2 scrollPos;
	private Dictionary<string, bool> foldoutID = new Dictionary<string, bool>();
	private Dictionary<string, bool> foldoutRarity = new Dictionary<string, bool>();

	// 最上方稀有度選項，加入 "All" 顯示所有稀有度
	private string[] rarityOptions = new[] { "All", "Normal", "Common", "Rare", "Special", "Legendary" };
	private string searchID = string.Empty;
	private int searchRarityIndex = 0;

	[MenuItem("JsonEditor/EquipmentLevel Data")]
	public static void ShowWindow()
	{
		GetWindow<EquipmentLevelDataEditor>("EquipmentLevel Editor");
	}

	private void OnEnable()
	{
		LoadCSV();
	}

	private void LoadCSV()
	{
		equipmentData.Clear();

		if (!File.Exists(csvFilePath))
		{
			Debug.LogError($"CSV not found at {csvFilePath}");
			return;
		}

		var text = File.ReadAllText(csvFilePath);
		var lines = text.Split('\n');
		string currentID = string.Empty;
		string currentRare = string.Empty;

		for (int i = 1; i < lines.Length; i++)
		{
			var line = lines[i].Trim();
			if (string.IsNullOrEmpty(line)) continue;

			var tokens = line.Split(',');
			if (tokens.Length < 6) continue;

			if (!string.IsNullOrWhiteSpace(tokens[0]))
				currentID = tokens[0].Trim();
			if (!string.IsNullOrWhiteSpace(tokens[1]))
				currentRare = tokens[1].Trim();

			if (string.IsNullOrEmpty(currentID) || string.IsNullOrEmpty(currentRare))
				continue;

			if (!int.TryParse(tokens[2], out var level) ||
				!int.TryParse(tokens[3], out var atk) ||
				!int.TryParse(tokens[4], out var hp) ||
				!int.TryParse(tokens[5], out var cost))
			{
				Debug.LogWarning($"Parse error on line {i + 1}: {line}");
				continue;
			}

			if (!equipmentData.ContainsKey(currentID))
				equipmentData[currentID] = new Dictionary<string, Dictionary<int, EquipmentStats>>();
			if (!equipmentData[currentID].ContainsKey(currentRare))
				equipmentData[currentID][currentRare] = new Dictionary<int, EquipmentStats>();

			equipmentData[currentID][currentRare][level] = new EquipmentStats
			{
				AttackPower = atk,
				HealthPoints = hp,
				CostMoney = cost
			};
		}
	}

	private void OnGUI()
	{
		// 上方搜尋欄：ID 欄位及稀有度下拉選單（含 All）
		EditorGUILayout.BeginHorizontal();
		searchID = EditorGUILayout.TextField("Equipment ID", searchID);
		searchRarityIndex = EditorGUILayout.Popup("Rarity", searchRarityIndex, rarityOptions);
		if (GUILayout.Button("Reload CSV", GUILayout.Width(100)))
		{
			LoadCSV();
		}
		EditorGUILayout.EndHorizontal();

		GUILayout.Space(8);
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

		// 查無 ID
		if (!string.IsNullOrEmpty(searchID) && !equipmentData.ContainsKey(searchID))
		{
			EditorGUILayout.HelpBox($"Equipment ID '{searchID}' not found.", MessageType.Error);
		}
		else
		{
			var ids = new List<string>(equipmentData.Keys);
			if (!string.IsNullOrEmpty(searchID))
				ids = new List<string> { searchID };

			foreach (var id in ids)
			{
				if (!foldoutID.ContainsKey(id)) foldoutID[id] = true;
				foldoutID[id] = EditorGUILayout.Foldout(foldoutID[id], id, true);
				if (!foldoutID[id]) continue;

				EditorGUI.indentLevel++;

				// 根據下拉選單選擇顯示稀有度列表
				List<string> rarities;
				if (rarityOptions[searchRarityIndex] == "All")
				{
					rarities = new List<string> { "Normal", "Common", "Rare", "Special", "Legendary" };
				}
				else
				{
					rarities = new List<string> { rarityOptions[searchRarityIndex] };
				}

				foreach (var rarity in rarities)
				{
					string key = id + "|" + rarity;
					if (!foldoutRarity.ContainsKey(key)) foldoutRarity[key] = false;
					foldoutRarity[key] = EditorGUILayout.Foldout(foldoutRarity[key], rarity, true);
					if (!foldoutRarity[key]) continue;

					EditorGUI.indentLevel++;

					// 若缺少此稀有度，顯示警告於此折疊內
					if (!equipmentData[id].ContainsKey(rarity))
					{
						EditorGUILayout.HelpBox("此稀有度數據尚未填寫!!!!", MessageType.Warning);
					}
					else
					{
						var levels = new List<int>(equipmentData[id][rarity].Keys);
						levels.Sort();
						foreach (var lvl in levels)
						{
							var stats = equipmentData[id][rarity][lvl];
							EditorGUILayout.LabelField($"Level {lvl}", EditorStyles.boldLabel);
							EditorGUI.indentLevel++;
							EditorGUILayout.LabelField("AttackPower", stats.AttackPower.ToString());
							EditorGUILayout.LabelField("HealthPoints", stats.HealthPoints.ToString());
							EditorGUILayout.LabelField("CostMoney", stats.CostMoney.ToString());
							EditorGUI.indentLevel--;
							GUILayout.Space(4);
						}
					}

					EditorGUI.indentLevel--;
					GUILayout.Space(6);
				}

				EditorGUI.indentLevel--;
				GUILayout.Space(10);
			}
		}

		EditorGUILayout.EndScrollView();
	}

	private class EquipmentStats
	{
		public int AttackPower;
		public int HealthPoints;
		public int CostMoney;
	}
}