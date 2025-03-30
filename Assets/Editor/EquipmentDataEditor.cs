using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

public class EquipmentDataEditor : EditorWindow
{
	private string jsonFilePath = "Assets/Resources/jsonData/EquipmentData.json";
	private JObject equipmentData;
	private Vector2 scrollPos;
	private List<string> equipmentIDs = new List<string>();
	private Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();
	private string[] rarityOptions = new[] { "Normal", "Common", "Rare", "Special", "Legendary" };

	[MenuItem("JsonEditor/EquipmentData")]
	public static void ShowWindow()
	{
		GetWindow<EquipmentDataEditor>("Equipment Data Editor");
	}

	private void OnEnable() => LoadJson();

	private void LoadJson()
	{
		if (File.Exists(jsonFilePath))
		{
			equipmentData = JObject.Parse(File.ReadAllText(jsonFilePath));
			equipmentIDs = new List<string>(equipmentData.Properties().Select(p => p.Name));
		}
		else
		{
			equipmentData = new JObject();
			equipmentIDs = new List<string>();
		}
	}

	private void SaveJson()
	{
		File.WriteAllText(jsonFilePath, equipmentData.ToString());
		AssetDatabase.Refresh();
	}

	private void OnGUI()
	{
		EditorGUILayout.BeginHorizontal();
		jsonFilePath = EditorGUILayout.TextField("JSON Path", jsonFilePath);
		if (GUILayout.Button("Refresh", GUILayout.Width(80))) LoadJson();
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(10);
		if (equipmentData == null) return;

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

		int removeIndex = -1;
		for (int i = 0; i < equipmentIDs.Count; i++)
		{
			string id = equipmentIDs[i];
			JObject item = (JObject)equipmentData[id];

			EditorGUILayout.BeginVertical("box");
			if (!foldoutStates.ContainsKey(id)) foldoutStates[id] = true;
			foldoutStates[id] = EditorGUILayout.Foldout(foldoutStates[id], id + " - " + (item["Name"] ?? "Unnamed"), true);
			if (!foldoutStates[id]) { EditorGUILayout.EndVertical(); continue; }

			EditorGUILayout.BeginHorizontal();
			string newID = EditorGUILayout.TextField("ID", id);
			if (newID != id && !string.IsNullOrEmpty(newID) && !equipmentData.ContainsKey(newID))
			{
				equipmentData.Remove(id);
				equipmentData[newID] = item;
				equipmentIDs[i] = newID;
				foldoutStates[newID] = foldoutStates[id];
				foldoutStates.Remove(id);
				GUI.FocusControl(null);
				Repaint();
				break;
			}
			if (GUILayout.Button("Remove", GUILayout.Width(80))) removeIndex = i;
			EditorGUILayout.EndHorizontal();

			item["Name"] = EditorGUILayout.TextField("Name", item["Name"]?.ToString());
			item["Type"] = EditorGUILayout.TextField("Type", item["Type"]?.ToString());
			item["SetType"] = EditorGUILayout.TextField("SetType", item["SetType"]?.ToString());
			item["AttackPower"] = EditorGUILayout.IntField("Attack Power", (int)item["AttackPower"]);
			item["HealthPoints"] = EditorGUILayout.IntField("Health Points", (int)item["HealthPoints"]);
			item["Description"] = EditorGUILayout.TextField("Description", item["Description"]?.ToString());

			int rarityIndex = System.Array.IndexOf(rarityOptions, item["Rarity"]?.ToString());
			if (rarityIndex < 0) rarityIndex = 0;
			rarityIndex = EditorGUILayout.Popup("Rarity", rarityIndex, rarityOptions);
			item["Rarity"] = rarityOptions[rarityIndex];

			GUILayout.Space(5);
			string imagePath = $"Assets/Resources/Arts/EquipmentImgaes/{item["Name"]}.jpg";
			Texture2D img = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath);
			if (img) GUILayout.Label(img, GUILayout.Width(64), GUILayout.Height(64));
			else GUILayout.Label("No Preview Found");

			EditorGUILayout.EndVertical();
		}

		if (removeIndex >= 0)
		{
			equipmentData.Remove(equipmentIDs[removeIndex]);
			equipmentIDs.RemoveAt(removeIndex);
		}

		if (GUILayout.Button("+ Add Equipment"))
		{
			string newID = "EQ" + equipmentData.Count.ToString("D2");
			JObject newItem = new JObject
			{
				["ID"] = newID,
				["Name"] = "New Equipment",
				["Type"] = "Armor",
				["SetType"] = "Default",
				["Rarity"] = "Normal",
				["AttackPower"] = 0,
				["HealthPoints"] = 0,
				["Description"] = ""
			};
			equipmentData[newID] = newItem;
			equipmentIDs.Add(newID);
		}

		EditorGUILayout.EndScrollView();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("💾 Save")) SaveJson();
		if (GUILayout.Button("📁 Export JSON")) EditorUtility.RevealInFinder(jsonFilePath);
		EditorGUILayout.EndHorizontal();
	}
}
