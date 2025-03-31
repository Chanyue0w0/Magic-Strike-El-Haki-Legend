using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

public class StageDataEditor_Expanded : EditorWindow
{
	private string jsonFilePath = "Assets/Resources/jsonData/StageData.json";
	private JObject stageData;
	private Vector2 scrollPos;

	private string jumpChapterInput = "1";
	private string jumpLevelInput = "1";

	private Dictionary<string, bool> chapterFoldouts = new Dictionary<string, bool>();
	private Dictionary<string, Dictionary<string, bool>> levelFoldouts = new Dictionary<string, Dictionary<string, bool>>();
	private Dictionary<string, List<bool>> stageFoldouts = new Dictionary<string, List<bool>>();

	[MenuItem("JsonEditor/Stage Data (Expanded)")]
	public static void ShowWindow()
	{
		GetWindow<StageDataEditor_Expanded>("Stage Data Editor");
	}

	private void OnEnable() => LoadJson();

	private void LoadJson()
	{
		if (File.Exists(jsonFilePath))
		{
			stageData = JObject.Parse(File.ReadAllText(jsonFilePath));
		}
		else
		{
			stageData = new JObject();
			Debug.LogWarning("StageData.json not found. Creating new data.");
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
		DrawPathAndRefresh();
		DrawJumpSection();

		if (stageData == null)
			return;

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		DrawChapters();
		DrawAddChapterButton();
		EditorGUILayout.EndScrollView();

		DrawBottomButtons();
	}

	// 畫上方 JSON 路徑與 Refresh 區塊
	private void DrawPathAndRefresh()
	{
		EditorGUILayout.BeginHorizontal();
		jsonFilePath = EditorGUILayout.TextField("JSON Path", jsonFilePath);
		if (GUILayout.Button("Refresh", GUILayout.Width(80)))
		{
			LoadJson();
		}
		EditorGUILayout.EndHorizontal();
	}

	// 畫跳轉區塊，包含關閉其他展開、跳轉或建立章節與關卡
	private void DrawJumpSection()
	{
		GUILayout.Space(10);
		GUILayout.Label("Jump to Chapter / Level", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
		jumpChapterInput = EditorGUILayout.TextField("Chapter #", jumpChapterInput);
		jumpLevelInput = EditorGUILayout.TextField("Level #", jumpLevelInput);
		if (GUILayout.Button("➡ Jump or Create", GUILayout.Width(140)))
		{
			// 關閉所有章節與關卡展開狀態
			foreach (var key in new List<string>(chapterFoldouts.Keys))
			{
				chapterFoldouts[key] = false;
			}
			foreach (var chapter in levelFoldouts.Values)
			{
				var keys = new List<string>(chapter.Keys);
				foreach (var key in keys)
				{
					chapter[key] = false;
				}
			}

			string chapterKey = $"Chapter_{jumpChapterInput}";
			string levelKey = $"Level_{jumpLevelInput}";

			// 若章節不存在則建立
			if (!stageData.ContainsKey(chapterKey))
			{
				stageData[chapterKey] = new JObject();
				Debug.Log($"Created {chapterKey}");
			}
			JObject chapterObj = (JObject)stageData[chapterKey];
			// 若關卡不存在則建立
			if (!chapterObj.ContainsKey(levelKey))
			{
				chapterObj[levelKey] = new JArray();
				Debug.Log($"Created {levelKey} under {chapterKey}");
			}

			// 展開目標章節與關卡
			chapterFoldouts[chapterKey] = true;
			if (!levelFoldouts.ContainsKey(chapterKey))
				levelFoldouts[chapterKey] = new Dictionary<string, bool>();
			levelFoldouts[chapterKey][levelKey] = true;
			if (!stageFoldouts.ContainsKey(chapterKey))
				stageFoldouts[chapterKey] = new List<bool>();
		}
		EditorGUILayout.EndHorizontal();
	}

	// 畫所有章節，注意先將 Properties 轉為 List 以避免在迭代時修改字典產生問題
	private void DrawChapters()
	{
		foreach (var chapterProperty in stageData.Properties().ToList())
		{
			string chapterKey = chapterProperty.Name;
			JObject chapter = chapterProperty.Value as JObject;
			DrawChapter(chapterKey, chapter);
		}
	}

	// 畫單一章節區塊，包含所有關卡、章節的 Remove 按鈕與新增關卡按鈕
	private void DrawChapter(string chapterKey, JObject chapter)
	{
		EditorGUILayout.BeginHorizontal();
		// 初始化展開狀態
		if (!chapterFoldouts.ContainsKey(chapterKey))
			chapterFoldouts[chapterKey] = true;

		chapterFoldouts[chapterKey] = EditorGUILayout.Foldout(chapterFoldouts[chapterKey], chapterKey, true);
		// Remove 按鈕
		if (GUILayout.Button("Remove", GUILayout.Width(80)))
		{
			if (EditorUtility.DisplayDialog("Confirm Remove", "Remove chapter " + chapterKey + "?", "Yes", "No"))
			{
				stageData.Remove(chapterKey);
				chapterFoldouts.Remove(chapterKey);
				levelFoldouts.Remove(chapterKey);
				stageFoldouts.Remove(chapterKey);
				EditorGUILayout.EndHorizontal();
				return;
			}
		}
		EditorGUILayout.EndHorizontal();

		if (!chapterFoldouts[chapterKey])
			return;

		if (!levelFoldouts.ContainsKey(chapterKey))
			levelFoldouts[chapterKey] = new Dictionary<string, bool>();

		if (!stageFoldouts.ContainsKey(chapterKey))
			stageFoldouts[chapterKey] = new List<bool>();

		// 畫每個關卡
		foreach (var levelPair in chapter)
		{
			string levelKey = levelPair.Key;
			JArray stages = levelPair.Value as JArray;
			DrawLevel(chapterKey, levelKey, stages);
		}

		GUILayout.Space(10);
		// 新增「+ Add Level」按鈕
		if (GUILayout.Button("  + Add Level"))
		{
			int nextLevelNumber = chapter.Count + 1;
			string newLevelKey = "Level_" + nextLevelNumber;
			chapter[newLevelKey] = new JArray();
			levelFoldouts[chapterKey][newLevelKey] = true;
		}

		GUILayout.Space(10);
	}

	// 畫單一關卡區塊與其內部的 stage 列表，並新增關卡 Remove 按鈕
	private void DrawLevel(string chapterKey, string levelKey, JArray stages)
	{
		EditorGUILayout.BeginHorizontal();
		if (!levelFoldouts[chapterKey].ContainsKey(levelKey))
			levelFoldouts[chapterKey][levelKey] = true;

		levelFoldouts[chapterKey][levelKey] = EditorGUILayout.Foldout(levelFoldouts[chapterKey][levelKey], $"  {levelKey}", true);
		// Remove 關卡按鈕
		if (GUILayout.Button("Remove", GUILayout.Width(80)))
		{
			if (EditorUtility.DisplayDialog("Confirm Remove", "Remove level " + levelKey + " from " + chapterKey + "?", "Yes", "No"))
			{
				JObject chapterObj = (JObject)stageData[chapterKey];
				chapterObj.Remove(levelKey);
				levelFoldouts[chapterKey].Remove(levelKey);
				return;
			}
		}
		EditorGUILayout.EndHorizontal();

		if (!levelFoldouts[chapterKey][levelKey])
			return;

		// 確保 stageFoldouts 對應數量
		while (stageFoldouts[chapterKey].Count < stages.Count)
			stageFoldouts[chapterKey].Add(true);

		int stageRemoveIndex = -1;
		for (int i = 0; i < stages.Count; i++)
		{
			JObject stage = stages[i] as JObject;
			stage["StageNumber"] = (i + 1).ToString();

			EditorGUILayout.BeginVertical("box");
			EditorGUILayout.BeginHorizontal();
			stageFoldouts[chapterKey][i] = EditorGUILayout.Foldout(stageFoldouts[chapterKey][i], $"Stage {stage["StageNumber"]}", true);
			if (GUILayout.Button("Copy", GUILayout.Width(60)))
			{
				stages.Insert(i + 1, (JObject)stage.DeepClone());
				stageFoldouts[chapterKey].Insert(i + 1, true);
				break;
			}
			if (GUILayout.Button("Remove", GUILayout.Width(80)))
			{
				stageRemoveIndex = i;
			}
			EditorGUILayout.EndHorizontal();

			if (stageFoldouts[chapterKey][i])
				DrawStageFields(stage);

			EditorGUILayout.EndVertical();
		}

		if (stageRemoveIndex >= 0)
		{
			stages.RemoveAt(stageRemoveIndex);
			stageFoldouts[chapterKey].RemoveAt(stageRemoveIndex);
		}

		GUILayout.Space(5);
		if (GUILayout.Button($"+ Add Stage to {chapterKey} / {levelKey}"))
		{
			var newStage = new JObject
			{
				["StageNumber"] = (stages.Count + 1).ToString(),
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
			stages.Add(newStage);
			stageFoldouts[chapterKey].Add(true);
		}
		GUILayout.Space(10);
	}

	// 畫最下方的 Save 與 Reveal JSON 按鈕
	private void DrawBottomButtons()
	{
		GUILayout.Space(5);
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("💾 Save"))
			SaveJson();
		if (GUILayout.Button("📁 Reveal JSON"))
			EditorUtility.RevealInFinder(jsonFilePath);
		EditorGUILayout.EndHorizontal();
	}

	// 畫 "+ Add Chapter" 按鈕
	private void DrawAddChapterButton()
	{
		if (GUILayout.Button("+ Add Chapter"))
		{
			int maxChapterNumber = 0;
			foreach (var prop in stageData.Properties())
			{
				if (prop.Name.StartsWith("Chapter_"))
				{
					int num;
					if (int.TryParse(prop.Name.Substring("Chapter_".Length), out num))
					{
						if (num > maxChapterNumber)
							maxChapterNumber = num;
					}
				}
			}
			string newChapterKey = "Chapter_" + (maxChapterNumber + 1);
			stageData[newChapterKey] = new JObject();
			chapterFoldouts[newChapterKey] = true;
			levelFoldouts[newChapterKey] = new Dictionary<string, bool>();
			stageFoldouts[newChapterKey] = new List<bool>();
		}
	}

	// 畫單一 Stage 的欄位
	private void DrawStageFields(JObject stage)
	{
		stage["DeadFrameBackGroundImage"] = EditorGUILayout.TextField("    Dead BG", stage["DeadFrameBackGroundImage"]?.ToString());
		stage["BackGroundImage"] = EditorGUILayout.TextField("    Background", stage["BackGroundImage"]?.ToString());
		stage["FieldImage"] = EditorGUILayout.TextField("    Field Image", stage["FieldImage"]?.ToString());
		stage["FieldHSBackGroundImage"] = EditorGUILayout.TextField("    Field HS BG", stage["FieldHSBackGroundImage"]?.ToString());
		stage["BGM"] = EditorGUILayout.TextField("    BGM", stage["BGM"]?.ToString());

		stage["AI_level"] = EditorGUILayout.FloatField("    AI Level", (float)stage["AI_level"]);

		JObject freq = stage["AI_AttackFrequency"] as JObject ?? new JObject();
		EditorGUILayout.LabelField("    AI Attack Frequency");
		EditorGUILayout.BeginHorizontal();
		freq["x"] = EditorGUILayout.FloatField("X", (float)(freq["x"] ?? 0));
		freq["y"] = EditorGUILayout.FloatField("Y", (float)(freq["y"] ?? 0));
		EditorGUILayout.EndHorizontal();
		stage["AI_AttackFrequency"] = freq;

		stage["StartHP"] = EditorGUILayout.IntField("    Start HP", (int)stage["StartHP"]);
		stage["StartATK"] = EditorGUILayout.IntField("    Start ATK", (int)stage["StartATK"]);
		stage["Player2Skin"] = EditorGUILayout.TextField("    Player2 Skin", stage["Player2Skin"]?.ToString());
		stage["Player2PuckSkin"] = EditorGUILayout.TextField("    Player2 Puck", stage["Player2PuckSkin"]?.ToString());

		DrawGroupArray(stage);
		DrawSpawnObjects(stage);
	}

	private void DrawGroupArray(JObject stage)
	{
		EditorGUILayout.LabelField("    Player2 Group");
		var groupArray = stage["player2_Group"] as JArray ?? new JArray();
		int removeIndex = -1;
		for (int i = 0; i < groupArray.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();
			groupArray[i] = EditorGUILayout.TextField($"      Group {i + 1}", groupArray[i]?.ToString());
			if (GUILayout.Button("-", GUILayout.Width(20)))
				removeIndex = i;
			EditorGUILayout.EndHorizontal();
		}
		if (removeIndex >= 0)
			groupArray.RemoveAt(removeIndex);
		if (GUILayout.Button("      + Add Group"))
			groupArray.Add("MS00");
		stage["player2_Group"] = groupArray;
	}

	private void DrawSpawnObjects(JObject stage)
	{
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("    Spawn Objects:");
		var spawnObjects = stage["spawnObjects"] as JArray ?? new JArray();
		stage["spawnObjects"] = spawnObjects;

		int removeIndex = -1;
		for (int j = 0; j < spawnObjects.Count; j++)
		{
			var obj = spawnObjects[j] as JObject;
			EditorGUILayout.BeginVertical("helpbox");

			obj["prefabName"] = EditorGUILayout.TextField("      Prefab Name", obj["prefabName"]?.ToString());

			EditorGUILayout.LabelField("      Position");
			Vector3 pos = JObjectToVector3(obj["position"] as JObject);
			pos = EditorGUILayout.Vector3Field("", pos);
			obj["position"] = Vector3ToJObject(pos);

			EditorGUILayout.LabelField("      Rotation");
			Vector3 rot = JObjectToVector3(obj["rotation"] as JObject);
			rot = EditorGUILayout.Vector3Field("", rot);
			obj["rotation"] = Vector3ToJObject(rot);

			if (GUILayout.Button("      Remove Object"))
				removeIndex = j;

			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
		}

		if (removeIndex >= 0)
			spawnObjects.RemoveAt(removeIndex);

		if (GUILayout.Button("    + Add Spawn Object"))
		{
			var newObj = new JObject
			{
				["prefabName"] = "NewPrefab",
				["position"] = Vector3ToJObject(Vector3.zero),
				["rotation"] = Vector3ToJObject(Vector3.zero)
			};
			spawnObjects.Add(newObj);
		}
	}

	private Vector3 JObjectToVector3(JObject obj)
	{
		if (obj == null)
			return Vector3.zero;
		float x = (float)(obj["x"] ?? 0);
		float y = (float)(obj["y"] ?? 0);
		float z = (float)(obj["z"] ?? 0);
		return new Vector3(x, y, z);
	}

	private JObject Vector3ToJObject(Vector3 v)
	{
		return new JObject
		{
			["x"] = v.x,
			["y"] = v.y,
			["z"] = v.z
		};
	}
}
