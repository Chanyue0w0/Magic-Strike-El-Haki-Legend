using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

		public PlayerHero(string name, string id, string rarity, int level, int atk, int hp, int ultDamage, bool isOwned, int shards, string desc)
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
		}
	}

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.Log("Found more than PlayerHeroManager object in the sence");
		}
		Instance = this;
	}

	private void InitJsonFile()
	{
		JObject heroDataObj = HeroData.Instance.jsonData;

		// 遍歷每個英雄代號（例如 HR00, HR01, ...）
		foreach (var heroProp in heroDataObj)
		{
			JObject heroJson = (JObject)heroProp.Value;

			// 從 LevelStats 的 "1" 級讀取基本數值
			JObject levelStats = (JObject)heroJson["LevelStats"];
			JObject level1Stats = (JObject)levelStats["1"];
			int baseATK = level1Stats["BaseATK"].Value<int>();
			int baseHP = level1Stats["BaseHP"].Value<int>();
			int ultimateDamage = level1Stats["UltimateSkillDamage"].Value<int>();

			// 建立新的 PlayerHero，根據需求覆寫部分欄位：
			// - currentLevel 固定為 1
			// - owned 固定為 false
			// - rarity 固定為 "傳說"（你也可以根據實際需求調整） 
			// - heroShards 固定為 0
			// - description 固定為 "擁有冰凍敵人的能力，可以減緩敵人行動。"
			PlayerHero newHero = new PlayerHero
			(
				heroJson["Name"]?.ToString(),         // 可保留原始名稱
				heroJson["ID"]?.ToString(),             // 可保留原始 ID
				heroJson["Rarity"].ToString(),                             // 強制設定為「傳說」
				1,                            // 起始等級 1
				baseATK,
				baseHP,
				ultimateDamage,
				false,                               // 預設未擁有
				0,                              // 預設碎片數量為 0
				heroJson["Description"].ToString()
			);

			heroList.Add(newHero);
		}
		SaveHeroes();
	}
	void Start()
	{
		if (!File.Exists(SavePath()))
			InitJsonFile();
		// test
		//// 讀取英雄
		//List<PlayerHero> heroList = PlayerHeroManager.Instance.GetAllHeroData();
		//foreach (var loadedHero in heroList)
		//	if (loadedHero != null)
		//	{
		//		Debug.Log($"載入英雄: {loadedHero.name}, 等級: {loadedHero.currentLevel}");
		//	}
		//// 新增
		//PlayerHero newHero = new PlayerHero("喵喵", "HR03", "傳說", 1, 250, 1500, 1000, true, 25, "喵喵喵");
		//PlayerHeroManager.Instance.AddHero(newHero);

		//// 更新英雄資料
		//PlayerHero updatedHero = PlayerHeroManager.Instance.GetHeroById("HR03");
		//updatedHero.description = "汪汪汪";
		//PlayerHeroManager.Instance.UpdateHero(updatedHero);
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
		if (!File.Exists(SavePath()))
		{
			Debug.LogWarning("找不到英雄存檔!");
			return;
		}

		string json = File.ReadAllText(SavePath());
		if (string.IsNullOrEmpty(json))
		{
			heroList = new List<PlayerHero>();
			Debug.LogWarning("存檔內容為空，建立新的英雄列表!");
		}
		else
		{
			heroList = JsonConvert.DeserializeObject<List<PlayerHero>>(json);
			Debug.Log("英雄數據已載入!");
		}
	}

	public void SaveHeroes()
	{
		JArray json = JArray.FromObject(heroList);
		string jsonTxt = json.ToString();
		File.WriteAllText(SavePath(), jsonTxt);
		Debug.Log("英雄數據已存檔: " + SavePath());
	}

	public void UpdateHero(PlayerHero updatedHero)
	{
		PlayerHero hero = heroList.Find(h => h.id == updatedHero.id);
		if (hero != null)
		{
			hero.currentLevel = updatedHero.currentLevel;
			hero.baseATK = updatedHero.baseATK;
			hero.baseHP = updatedHero.baseHP;
			hero.ultimateDamage = updatedHero.ultimateDamage;
			hero.owned = updatedHero.owned;
			hero.heroShards = updatedHero.heroShards;
			hero.description = updatedHero.description;
			SaveHeroes();
			Debug.Log("英雄數據已更新: " + updatedHero.name + " " + updatedHero.id);
		}
		else
		{
			Debug.LogWarning("找不到要更新的英雄: " + updatedHero.id);
		}
	}

	public void DeleteHero(string heroID)
	{
		LoadHeroes();
		PlayerHero hero = heroList.Find(h => h.id == heroID);
		if (hero != null)
		{
			heroList.Remove(hero);
			SaveHeroes();
			Debug.Log("英雄已刪除: " + hero.name);
		}
		else
		{
			Debug.LogWarning("找不到要刪除的英雄: " + heroID);
		}
	}

	public PlayerHero GetHeroById(string heroID)
	{
		// 先讀取最新的英雄數據（如果需要）：
		LoadHeroes();

		// 根據 heroID 找到對應的英雄
		PlayerHero hero = heroList.Find(h => h.id == heroID);
		if (hero == null)
		{
			Debug.LogWarning("找不到指定的英雄: " + heroID);
		}
		return hero;
	}

	public List<PlayerHero> GetAllHeroData()
	{
		LoadHeroes();
		return heroList;
	}

	private string SavePath()
	{
		return Application.persistentDataPath + savePath;
	}
}
