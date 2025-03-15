using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static HeroData;

public class PlayerHeroManager : MonoBehaviour
{
	private string savePath = "/playerHero.json";
	private List<PlayerHero> heroList = new();

	public static PlayerHeroManager Instance { get; private set; }
	[System.Serializable]
	public class PlayerHero
	{
		public string name;
		public string id;
		public string rarity;
		public int currentLevel;
		public int baseATK;
		public int baseHP;
		public int ultimateDamage;
		public bool owned;
		public int heroShards;
		public string description;
		public List<string> equippedItems; // List of equipment IDs

		public PlayerHero(string name, string id, string rarity, int level, int atk, int hp, int ultDamage, bool isOwned, int shards, string desc, List<string> equippedItems = null)
		{
			this.name = name;
			this.id = id;
			this.rarity = rarity;
			currentLevel = level;
			baseATK = atk;
			baseHP = hp;
			ultimateDamage = ultDamage;
			owned = isOwned;
			heroShards = shards;
			description = desc;
			this.equippedItems = equippedItems ?? new List<string>();
		}
	}

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.Log("Found more than PlayerHeroManager object in the scene");
		}
		Instance = this;
	}

	private void InitJsonFile()
	{
		JObject heroDataObj = HeroData.Instance.jsonData;

		foreach (var heroProp in heroDataObj)
		{
			JObject heroJson = (JObject)heroProp.Value;
			JObject levelStats = (JObject)heroJson["LevelStats"];
			JObject level1Stats = (JObject)levelStats["1"];
			int baseATK = level1Stats["BaseATK"].Value<int>();
			int baseHP = level1Stats["BaseHP"].Value<int>();
			int ultimateDamage = level1Stats["UltimateSkillDamage"].Value<int>();

			PlayerHero newHero = new PlayerHero
			(
				heroJson["Name"]?.ToString(),
				heroJson["ID"]?.ToString(),
				heroJson["Rarity"].ToString(),
				1,
				baseATK,
				baseHP,
				ultimateDamage,
				false,
				0,
				heroJson["Description"].ToString(),
				new List<string> { "", "", ""}
			);
			heroList.Add(newHero);
		}

		heroList[0].owned = true;
		SaveHeroes();
	}

	void Start()
	{
		if (!File.Exists(FilePath()))
			InitJsonFile();

		// Add a new test hero on start
		PlayerHero newHero = new PlayerHero("Test Hero", "HR99", "Legendary", 1, 300, 2000, 1500, false, 50, "A powerful test hero", new List<string> { "", "", "" });
		AddHero(newHero);
	}

	public void AddHero(PlayerHero hero)
	{
		LoadHeroes();
		if (!heroList.Exists(h => h.id == hero.id))
		{
			heroList.Add(hero);
		}
		else
		{
			UpdateHero(hero);
		}
		SaveHeroes();
	}

	public void LoadHeroes()
	{
		if (!File.Exists(FilePath()))
		{
			Debug.LogWarning("Hero save file not found!");
			return;
		}

		string json = File.ReadAllText(FilePath());
		if (string.IsNullOrEmpty(json))
		{
			heroList = new List<PlayerHero>();
			Debug.LogWarning("Save file is empty, creating new hero list!");
		}
		else
		{
			heroList = JsonConvert.DeserializeObject<List<PlayerHero>>(json);
			Debug.Log("Hero data loaded!");
		}
	}

	public void SaveHeroes()
	{
		SortHeroes();
		JArray json = JArray.FromObject(heroList);
		string jsonTxt = json.ToString();
		File.WriteAllText(FilePath(), jsonTxt);
		Debug.Log("Hero data saved: " + FilePath());
	}

	public void UpdateHero(PlayerHero updatedHero)
	{
		for (int i = 0; i < heroList.Count; i++)
		{
			if (heroList[i].id == updatedHero.id)
			{
				heroList[i] = updatedHero;
				SaveHeroes(); // Save the updated data
				Debug.Log("Equipment updated: " + updatedHero.name);
				return;
			}
		}

		Debug.LogWarning("Equipment not found for update: " + updatedHero.id);
	}

	public PlayerHero GetHeroByID(string heroID)
	{
		LoadHeroes(); // Ensure the latest data is loaded
		return heroList.Find(hero => hero.id == heroID);
	}

	public PlayerHero GetHeroByIndex(int index)
	{
		LoadHeroes(); // Ensure the latest data is loaded
		return heroList[index];
	}

	public List<PlayerHero> GetAllHeroData()
	{
		LoadHeroes();
		return heroList;
	}

	private string FilePath()
	{
		return Application.persistentDataPath + savePath;
	}

	private void SortHeroes()
	{
		heroList.Sort((a, b) => a.owned.CompareTo(b.owned));
	}

}