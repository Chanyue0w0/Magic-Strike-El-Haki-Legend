using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class PlayerEquipmentViewer : EditorWindow
{
	private string jsonFilePath = "Assets/Resources/jsonData/playerEquipment.json";
	private JArray equipmentList;
	private Vector2 scrollPos;
	private List<bool> foldouts = new List<bool>();

	[MenuItem("JsonEditor/Player Equipment Viewer")]
	public static void ShowWindow()
	{
		GetWindow<PlayerEquipmentViewer>("Player Equipment Viewer");
	}

	private void OnEnable() => LoadJson();

	private void LoadJson()
	{
		if (File.Exists(jsonFilePath))
		{
			string json = File.ReadAllText(jsonFilePath);
			equipmentList = JArray.Parse(json);
		}
		else
		{
			equipmentList = new JArray();
			Debug.LogWarning("playerEquipment.json not found.");
		}

		foldouts = new List<bool>();
		for (int i = 0; i < equipmentList.Count; i++)
			foldouts.Add(true);
	}

	private void OnGUI()
	{
		DrawFilePathSection();

		if (equipmentList == null) return;

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		DrawEquipmentList();
		EditorGUILayout.EndScrollView();
	}

	private void DrawFilePathSection()
	{
		EditorGUILayout.BeginHorizontal();
		jsonFilePath = EditorGUILayout.TextField("JSON Path", jsonFilePath);
		if (GUILayout.Button("Refresh", GUILayout.Width(80)))
		{
			LoadJson();
		}
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(5);
	}

	private void DrawEquipmentList()
	{
		for (int i = 0; i < equipmentList.Count; i++)
		{
			JObject item = (JObject)equipmentList[i];
			DrawEquipmentItem(item, i);
		}
	}

	private void DrawEquipmentItem(JObject item, int index)
	{
		string displayName = item["name"]?.ToString() ?? "Unnamed";
		EditorGUILayout.BeginVertical("box");
		foldouts[index] = EditorGUILayout.Foldout(foldouts[index], displayName, true);
		if (foldouts[index])
		{
			EditorGUILayout.LabelField("ID", item["id"]?.ToString());
			EditorGUILayout.LabelField("Name", item["name"]?.ToString());
			EditorGUILayout.LabelField("Type", item["equipmentType"]?.ToString());
			EditorGUILayout.LabelField("Rarity", item["rarity"]?.ToString());
			EditorGUILayout.LabelField("Set", item["setType"]?.ToString());
			EditorGUILayout.LabelField("Level", item["currentLevel"]?.ToString());
			EditorGUILayout.LabelField("Attack Power", item["attackPower"]?.ToString());
			EditorGUILayout.LabelField("Health Points", item["healthPoints"]?.ToString());
			EditorGUILayout.LabelField("Equipped By", item["equippedByHero"]?.ToString());
			EditorGUILayout.LabelField("Description", item["description"]?.ToString(), EditorStyles.wordWrappedLabel);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Buffs", EditorStyles.boldLabel);
			var buffs = item["buffs"] as JObject;
			if (buffs != null)
				DrawBuffList(buffs);
		}
		EditorGUILayout.EndVertical();
		GUILayout.Space(5);
	}

	private void DrawBuffList(JObject buffs)
	{
		foreach (var buff in buffs)
		{
			EditorGUILayout.LabelField($"  - {buff.Key}: {buff.Value}");
		}
	}
}
