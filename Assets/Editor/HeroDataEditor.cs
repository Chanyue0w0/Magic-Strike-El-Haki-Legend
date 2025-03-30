using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

public class HeroDataEditor : EditorWindow
{
	private string jsonFilePath = "Assets/Resources/jsonData/HeroData.json";
	private JObject heroData;
	private Vector2 scrollPos;
	private List<string> heroIDs = new List<string>();
	private Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

	[MenuItem("JsonEditor/HeroData")]
	public static void ShowWindow()
	{
		GetWindow<HeroDataEditor>("Hero Data Editor");
	}

	private void OnEnable()
	{
		LoadJson();
	}

	private void LoadJson()
	{
		if (File.Exists(jsonFilePath))
		{
			heroData = JObject.Parse(File.ReadAllText(jsonFilePath));
			heroIDs = new List<string>(heroData.Properties().Select(p => p.Name));
		}
		else
		{
			heroData = new JObject();
			heroIDs = new List<string>();
		}
	}

	private void SaveJson()
	{
		File.WriteAllText(jsonFilePath, heroData.ToString());
		AssetDatabase.Refresh();
	}

	private void OnGUI()
	{
		EditorGUILayout.BeginHorizontal();
		jsonFilePath = EditorGUILayout.TextField("JSON Path", jsonFilePath);
		if (GUILayout.Button("Refresh", GUILayout.Width(80)))
		{
			LoadJson();
		}
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(10);
		if (heroData == null) return;

		// Extra controls
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("↕ Expand All"))
		{
			foldoutStates.Clear();
			foreach (string id in heroIDs)
				foldoutStates[id] = true;
		}
		if (GUILayout.Button("⇣ Collapse All"))
		{
			foldoutStates.Clear();
			foreach (string id in heroIDs)
				foldoutStates[id] = false;
		}
		if (GUILayout.Button("⬆ Sort Levels"))
		{
			foreach (var id in heroIDs)
			{
				JObject hero = (JObject)heroData[id];
				JObject levels = hero["LevelStats"] as JObject;
				if (levels == null) continue;

				var sorted = new SortedDictionary<int, JToken>();
				foreach (var kv in levels)
				{
					if (int.TryParse(kv.Key, out int lvl))
						sorted[lvl] = kv.Value;
				}
				hero["LevelStats"] = JObject.FromObject(sorted);
			}
		}
		EditorGUILayout.EndHorizontal();

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

		int removeIndex = -1;
		for (int i = 0; i < heroIDs.Count; i++)
		{
			string id = heroIDs[i];
			JObject hero = (JObject)heroData[id];
			EditorGUILayout.BeginVertical("box");
			if (!foldoutStates.ContainsKey(id)) foldoutStates[id] = true;
			foldoutStates[id] = EditorGUILayout.Foldout(foldoutStates[id], id + " - " + (hero["Name"] ?? "Unnamed"), true);
			if (!foldoutStates[id]) { EditorGUILayout.EndVertical(); continue; }

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(id, EditorStyles.boldLabel);
			if (GUILayout.Button("Remove", GUILayout.Width(80))) removeIndex = i;
			EditorGUILayout.EndHorizontal();

			hero["Name"] = EditorGUILayout.TextField("Name", hero["Name"]?.ToString());
			hero["Rarity"] = EditorGUILayout.TextField("Rarity", hero["Rarity"]?.ToString());
			hero["Description"] = EditorGUILayout.TextField("Description", hero["Description"]?.ToString());

			GUILayout.Space(5);
			string imagePath = $"Assets/Resources/Arts/FightScene/HeadStickers/{id}.png";
			Texture2D img = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath);
			if (img) GUILayout.Label(img, GUILayout.Width(64), GUILayout.Height(64));
			else GUILayout.Label("No Preview Found");

			GUILayout.Space(5);
			EditorGUILayout.LabelField("Level Stats:", EditorStyles.boldLabel);
			JObject levels = hero["LevelStats"] as JObject;
			List<string> levelKeys = new List<string>(levels?.Properties().Select(p => p.Name) ?? new string[0]);
			string levelToRemove = null;
			foreach (var level in levelKeys)
			{
				JObject stats = (JObject)levels[level];
				EditorGUILayout.BeginVertical("helpbox");
				EditorGUILayout.LabelField("Level " + level);
				stats["BaseATK"] = EditorGUILayout.IntField("Base ATK", (int)stats["BaseATK"]);
				stats["BaseHP"] = EditorGUILayout.IntField("Base HP", (int)stats["BaseHP"]);
				stats["UltimateSkillDamage"] = EditorGUILayout.IntField("Ultimate Damage", (int)stats["UltimateSkillDamage"]);
				if (GUILayout.Button("Remove Level")) levelToRemove = level;
				EditorGUILayout.EndVertical();
			}
			if (levelToRemove != null) levels.Remove(levelToRemove);

			GUILayout.Space(5);
			if (GUILayout.Button("+ Add Level"))
			{
				string newLevel = (levels.Count + 1).ToString();
				levels[newLevel] = new JObject
				{
					["BaseATK"] = 100,
					["BaseHP"] = 1000,
					["UltimateSkillDamage"] = 500
				};
			}

			EditorGUILayout.EndVertical();
		}

		if (removeIndex >= 0)
		{
			heroData.Remove(heroIDs[removeIndex]);
			heroIDs.RemoveAt(removeIndex);
			UpdateHeroIDs();
		}

		if (GUILayout.Button("+ Add Hero"))
		{
			int nextIndex = heroData.Count;
			string newID = "HR" + nextIndex.ToString("D2");
			heroData[newID] = new JObject
			{
				["ID"] = newID,
				["Name"] = "New Hero",
				["Rarity"] = "Common",
				["Description"] = "",
				["LevelStats"] = new JObject()
			};
			heroIDs.Add(newID);
		}



		EditorGUILayout.EndScrollView();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("💾 Save")) SaveJson();
		if (GUILayout.Button("📁 Export JSON")) EditorUtility.RevealInFinder(jsonFilePath);
		EditorGUILayout.EndHorizontal();
	}

	private void UpdateHeroIDs()
	{
		heroIDs = new List<string>(heroData.Properties().Select(p => p.Name));
		heroIDs.Sort();
		for (int i = 0; i < heroIDs.Count; i++)
		{
			string newID = "HR" + i.ToString("D2");
			JObject hero = (JObject)heroData[heroIDs[i]];
			hero.Remove("ID");
			hero["ID"] = newID;
			heroData.Remove(heroIDs[i]);
			heroData[newID] = hero;
			heroIDs[i] = newID;
		}
	}
}
