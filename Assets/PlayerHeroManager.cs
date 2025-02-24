using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerHeroManager : MonoBehaviour
{
	[SerializeField] private string savePath = "/playerHero.json";
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

	void Start()
	{
		//// 弄璣动
		//List<PlayerHero> heroList = PlayerHeroManager.Instance.GetAllHeroData();
		//foreach (var loadedHero in heroList)
		//	if (loadedHero != null)
		//	{
		//		Debug.Log($"更璣动: {loadedHero.name}, 单: {loadedHero.currentLevel}");
		//	}
		//// 穝糤
		//PlayerHero newHero = new PlayerHero("豴豴", "HR03", "肚弧", 1, 250, 1500, 1000, true, 25, "豴豴豴");
		//PlayerHeroManager.Instance.AddHero(newHero);

		//// 穝璣动戈
		//PlayerHero updatedHero = PlayerHeroManager.Instance.GetHeroById("HR03");
		//updatedHero.description = "↙↙↙";
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
			heroList = new List<PlayerHero>();
			SaveHeroes();
			Debug.LogWarning("тぃ璣动郎!");	
		}

		string json = File.ReadAllText(SavePath());
		if (string.IsNullOrEmpty(json))
		{
			heroList = new List<PlayerHero>();
			Debug.LogWarning("郎ず甧ミ穝璣动!");
		}
		else
		{
			heroList = JsonConvert.DeserializeObject<List<PlayerHero>>(json);
			Debug.Log("璣动计沮更!");
		}
	}

	public void SaveHeroes()
	{
		JArray json = JArray.FromObject(heroList);
		string jsonTxt = json.ToString();
		File.WriteAllText(SavePath(), jsonTxt);
		Debug.Log("璣动计沮郎: " + SavePath());
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
			Debug.Log("璣动计沮穝: " + updatedHero.name + " " + updatedHero.id);
		}
		else
		{
			Debug.LogWarning("тぃ璶穝璣动: " + updatedHero.id);
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
			Debug.Log("璣动埃: " + hero.name);
		}
		else
		{
			Debug.LogWarning("тぃ璶埃璣动: " + heroID);
		}
	}

	public PlayerHero GetHeroById(string heroID)
	{
		// 弄程穝璣动计沮狦惠璶
		LoadHeroes();

		// 沮 heroID т癸莱璣动
		PlayerHero hero = heroList.Find(h => h.id == heroID);
		if (hero == null)
		{
			Debug.LogWarning("тぃ﹚璣动: " + heroID);
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
