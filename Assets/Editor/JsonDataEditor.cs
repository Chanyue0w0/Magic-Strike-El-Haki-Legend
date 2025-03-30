// JsonDataEditor.cs
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class JsonDataEditor : EditorWindow
{
	private string jsonFilePath = "Assets/Resources/jsonData/StageData.json";
	private JObject stageData;
	private Vector2 scrollPos;

	private string chapterNumberInput = "1";
	private string levelNumberInput = "1";

	private string filteredChapterKey = "";
	private string filteredLevelKey = "";

	private string[] prefabOptions = new string[] { "GrassSlimeObj", "RockSlimeObj", "WaterSlimeObj", "FireSlimeObj" };

	private List<bool> foldouts = new List<bool>();

	[MenuItem("JsonEditor/StageData")]
	public static void ShowWindow()
	{
		GetWindow<JsonDataEditor>("Stage Data Editor");
	}

	private void OnEnable() => LoadJson();

	private void LoadJson()
	{
		if (File.Exists(jsonFilePath))
			stageData = JObject.Parse(File.ReadAllText(jsonFilePath));
		else
		{
			stageData = new JObject();
			Debug.LogWarning("StageData.json not found, creating new data.");
		}
	}

	private void SaveJson()
	{
		File.WriteAllText(jsonFilePath, stageData.ToString());
		AssetDatabase.Refresh();
		Debug.Log("Saved to " + jsonFilePath);
	}

	private void OnGUI()
	{
		if (stageData == null) return;
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		DrawSearchSection();
		DrawStageEditor();
		DrawAddStageSection();
		GUILayout.Space(10);
		if (GUILayout.Button("💾 Save JSON")) SaveJson();
		EditorGUILayout.EndScrollView();
	}

	private void DrawSearchSection()
	{
		GUILayout.Label("Search Chapter & Level", EditorStyles.boldLabel);
		chapterNumberInput = EditorGUILayout.TextField("Chapter #", chapterNumberInput);
		levelNumberInput = EditorGUILayout.TextField("Level #", levelNumberInput);

		if (GUILayout.Button("🔍 Search"))
		{
			string chapterKey = $"Chapter_{chapterNumberInput}";
			string levelKey = $"Level_{levelNumberInput}";

			if (stageData.ContainsKey(chapterKey) && ((JObject)stageData[chapterKey]).ContainsKey(levelKey))
			{
				filteredChapterKey = chapterKey;
				filteredLevelKey = levelKey;
			}
			else if (EditorUtility.DisplayDialog("Data Not Found", $"{chapterKey} / {levelKey} not found. Create it?", "Yes", "No"))
			{
				if (!stageData.ContainsKey(chapterKey))
					stageData[chapterKey] = new JObject();

				JObject chapter = (JObject)stageData[chapterKey];
				if (!chapter.ContainsKey(levelKey))
					chapter[levelKey] = new JArray();

				var sorted = new SortedDictionary<string, JToken>();
				foreach (var pair in stageData)
					sorted[pair.Key] = pair.Value;
				stageData = JObject.FromObject(sorted);

				filteredChapterKey = chapterKey;
				filteredLevelKey = levelKey;
				Debug.Log($"Created and selected {chapterKey} / {levelKey}");
			}
		}
	}

	private void DrawStageEditor()
	{
		if (filteredChapterKey == "" || filteredLevelKey == "") return;

		JObject chapter = (JObject)stageData[filteredChapterKey];
		JArray stages = (JArray)chapter[filteredLevelKey];

		EditorGUILayout.LabelField(filteredChapterKey, EditorStyles.boldLabel);
		EditorGUILayout.LabelField("  " + filteredLevelKey, EditorStyles.boldLabel);

		while (foldouts.Count < stages.Count) foldouts.Add(true);
		int removeIndex = -1;

		for (int i = 0; i < stages.Count; i++)
		{
			var stage = stages[i] as JObject;
			stage["StageNumber"] = (i + 1).ToString();

			EditorGUILayout.BeginVertical("box");
			EditorGUILayout.BeginHorizontal();
			foldouts[i] = EditorGUILayout.Foldout(foldouts[i], "Stage " + stage["StageNumber"], true);
			if (GUILayout.Button("Copy", GUILayout.Width(60)))
			{
				stages.Insert(i + 1, (JObject)stage.DeepClone());
				foldouts.Insert(i + 1, true);
				break;
			}
			if (GUILayout.Button("Remove", GUILayout.Width(80)))
			{
				removeIndex = i;
			}
			EditorGUILayout.EndHorizontal();

			if (foldouts[i]) DrawStageFields(stage);
			EditorGUILayout.EndVertical();
		}

		if (removeIndex >= 0)
		{
			stages.RemoveAt(removeIndex);
			foldouts.RemoveAt(removeIndex);
		}
	}

	private void DrawStageFields(JObject stage)
	{
		stage["DeadFrameBackGroundImage"] = EditorGUILayout.TextField("      Dead BG", stage["DeadFrameBackGroundImage"]?.ToString());
		stage["BackGroundImage"] = EditorGUILayout.TextField("      Background", stage["BackGroundImage"]?.ToString());
		stage["FieldImage"] = EditorGUILayout.TextField("      Field Image", stage["FieldImage"]?.ToString());
		stage["FieldHSBackGroundImage"] = EditorGUILayout.TextField("      Field HS BG", stage["FieldHSBackGroundImage"]?.ToString());
		stage["BGM"] = EditorGUILayout.TextField("      BGM", stage["BGM"]?.ToString());
		stage["AI_level"] = EditorGUILayout.FloatField("      AI Level", (float)stage["AI_level"]);

		JObject attackFreq = stage["AI_AttackFrequency"] as JObject;
		if (attackFreq != null)
		{
			attackFreq["x"] = EditorGUILayout.FloatField("      AI_Attack X", (float)attackFreq["x"]);
			attackFreq["y"] = EditorGUILayout.FloatField("      AI_Attack Y", (float)attackFreq["y"]);
		}

		stage["StartHP"] = EditorGUILayout.IntField("      Start HP", (int)stage["StartHP"]);
		stage["StartATK"] = EditorGUILayout.IntField("      Start ATK", (int)stage["StartATK"]);
		stage["Player2Skin"] = EditorGUILayout.TextField("      Player2 Skin", stage["Player2Skin"]?.ToString());
		stage["Player2PuckSkin"] = EditorGUILayout.TextField("      Player2 Puck", stage["Player2PuckSkin"]?.ToString());

		DrawGroupArray(stage);
		DrawSpawnObjects(stage);
	}

	private void DrawGroupArray(JObject stage)
	{
		EditorGUILayout.LabelField("      Player2 Group:");
		var groupArray = stage["player2_Group"] as JArray;
		if (groupArray == null) return;

		int removeIndex = -1;
		for (int i = 0; i < groupArray.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();
			groupArray[i] = EditorGUILayout.TextField($"        Group {i + 1}", groupArray[i]?.ToString());
			if (GUILayout.Button("-", GUILayout.Width(20))) removeIndex = i;
			EditorGUILayout.EndHorizontal();
		}
		if (removeIndex >= 0) groupArray.RemoveAt(removeIndex);

		if (GUILayout.Button("      + Add Group Member")) groupArray.Add("MS00");
	}

	private void DrawSpawnObjects(JObject stage)
	{
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("      Spawn Objects:", EditorStyles.boldLabel);
		var spawnObjects = stage["spawnObjects"] as JArray ?? new JArray();
		stage["spawnObjects"] = spawnObjects;

		int removeIndex = -1;
		for (int j = 0; j < spawnObjects.Count; j++)
		{
			var obj = spawnObjects[j] as JObject;
			EditorGUILayout.BeginVertical("helpbox");

			int selected = System.Array.IndexOf(prefabOptions, obj["prefabName"]?.ToString());
			if (selected < 0) selected = 0;
			selected = EditorGUILayout.Popup("        Prefab", selected, prefabOptions);
			obj["prefabName"] = prefabOptions[selected];

			JObject pos = obj["position"] as JObject ?? new JObject();
			pos["x"] = EditorGUILayout.FloatField("        Pos X", (float)(pos["x"] ?? 0));
			pos["y"] = EditorGUILayout.FloatField("        Pos Y", (float)(pos["y"] ?? 0));
			pos["z"] = EditorGUILayout.FloatField("        Pos Z", (float)(pos["z"] ?? 0));
			obj["position"] = pos;

			JObject rot = obj["rotation"] as JObject ?? new JObject();
			rot["x"] = EditorGUILayout.FloatField("        Rot X", (float)(rot["x"] ?? 0));
			rot["y"] = EditorGUILayout.FloatField("        Rot Y", (float)(rot["y"] ?? 0));
			rot["z"] = EditorGUILayout.FloatField("        Rot Z", (float)(rot["z"] ?? 0));
			obj["rotation"] = rot;

			if (GUILayout.Button("        Remove Object")) removeIndex = j;
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
		}

		if (removeIndex >= 0) spawnObjects.RemoveAt(removeIndex);

		if (GUILayout.Button("      + Add Spawn Object"))
		{
			var newObj = new JObject
			{
				["prefabName"] = "GrassSlimeObj",
				["position"] = new JObject { ["x"] = 0f, ["y"] = 0f, ["z"] = 0f },
				["rotation"] = new JObject { ["x"] = 0f, ["y"] = 0f, ["z"] = 0f }
			};
			spawnObjects.Add(newObj);
		}
	}

	private void DrawAddStageSection()
	{
		GUILayout.Space(20);
		GUILayout.Label("Add Stage to Current Chapter/Level", EditorStyles.boldLabel);

		if (GUILayout.Button("+ Add Stage"))
		{
			if (filteredChapterKey == "" || filteredLevelKey == "")
			{
				Debug.LogError("Please search and select a valid Chapter and Level first.");
				return;
			}

			JObject chapter = stageData[filteredChapterKey] as JObject;
			if (!chapter.ContainsKey(filteredLevelKey)) chapter[filteredLevelKey] = new JArray();

			JArray stageArray = chapter[filteredLevelKey] as JArray;
			int nextIndex = stageArray.Count + 1;

			var newStage = new JObject
			{
				["StageNumber"] = nextIndex.ToString(),
				["DeadFrameBackGroundImage"] = "",
				["BackGroundImage"] = "",
				["FieldImage"] = "",
				["FieldHSBackGroundImage"] = "",
				["BGM"] = "",
				["AI_level"] = 1.0f,
				["AI_AttackFrequency"] = new JObject { ["x"] = 5, ["y"] = 7 },
				["StartHP"] = 100,
				["StartATK"] = 50,
				["Player2Skin"] = "",
				["Player2PuckSkin"] = "",
				["player2_Group"] = new JArray(),
				["spawnObjects"] = new JArray()
			};

			stageArray.Add(newStage);
			foldouts.Add(true);
		}
	}
}
