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

		// �M���C�ӭ^���N���]�Ҧp HR00, HR01, ...�^
		foreach (var heroProp in heroDataObj)
		{
			JObject heroJson = (JObject)heroProp.Value;

			// �q LevelStats �� "1" ��Ū���򥻼ƭ�
			JObject levelStats = (JObject)heroJson["LevelStats"];
			JObject level1Stats = (JObject)levelStats["1"];
			int baseATK = level1Stats["BaseATK"].Value<int>();
			int baseHP = level1Stats["BaseHP"].Value<int>();
			int ultimateDamage = level1Stats["UltimateSkillDamage"].Value<int>();

			// �إ߷s�� PlayerHero�A�ھڻݨD�мg�������G
			// - currentLevel �T�w�� 1
			// - owned �T�w�� false
			// - rarity �T�w�� "�ǻ�"�]�A�]�i�H�ھڹ�ڻݨD�վ�^ 
			// - heroShards �T�w�� 0
			// - description �T�w�� "�֦��B��ĤH����O�A�i�H��w�ĤH��ʡC"
			PlayerHero newHero = new PlayerHero
			(
				heroJson["Name"]?.ToString(),         // �i�O�d��l�W��
				heroJson["ID"]?.ToString(),             // �i�O�d��l ID
				heroJson["Rarity"].ToString(),                             // �j��]�w���u�ǻ��v
				1,                            // �_�l���� 1
				baseATK,
				baseHP,
				ultimateDamage,
				false,                               // �w�]���֦�
				0,                              // �w�]�H���ƶq�� 0
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
		//// Ū���^��
		//List<PlayerHero> heroList = PlayerHeroManager.Instance.GetAllHeroData();
		//foreach (var loadedHero in heroList)
		//	if (loadedHero != null)
		//	{
		//		Debug.Log($"���J�^��: {loadedHero.name}, ����: {loadedHero.currentLevel}");
		//	}
		//// �s�W
		//PlayerHero newHero = new PlayerHero("�p�p", "HR03", "�ǻ�", 1, 250, 1500, 1000, true, 25, "�p�p�p");
		//PlayerHeroManager.Instance.AddHero(newHero);

		//// ��s�^�����
		//PlayerHero updatedHero = PlayerHeroManager.Instance.GetHeroById("HR03");
		//updatedHero.description = "�L�L�L";
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
			Debug.LogWarning("�䤣��^���s��!");
			return;
		}

		string json = File.ReadAllText(SavePath());
		if (string.IsNullOrEmpty(json))
		{
			heroList = new List<PlayerHero>();
			Debug.LogWarning("�s�ɤ��e���šA�إ߷s���^���C��!");
		}
		else
		{
			heroList = JsonConvert.DeserializeObject<List<PlayerHero>>(json);
			Debug.Log("�^���ƾڤw���J!");
		}
	}

	public void SaveHeroes()
	{
		JArray json = JArray.FromObject(heroList);
		string jsonTxt = json.ToString();
		File.WriteAllText(SavePath(), jsonTxt);
		Debug.Log("�^���ƾڤw�s��: " + SavePath());
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
			Debug.Log("�^���ƾڤw��s: " + updatedHero.name + " " + updatedHero.id);
		}
		else
		{
			Debug.LogWarning("�䤣��n��s���^��: " + updatedHero.id);
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
			Debug.Log("�^���w�R��: " + hero.name);
		}
		else
		{
			Debug.LogWarning("�䤣��n�R�����^��: " + heroID);
		}
	}

	public PlayerHero GetHeroById(string heroID)
	{
		// ��Ū���̷s���^���ƾڡ]�p�G�ݭn�^�G
		LoadHeroes();

		// �ھ� heroID ���������^��
		PlayerHero hero = heroList.Find(h => h.id == heroID);
		if (hero == null)
		{
			Debug.LogWarning("�䤣����w���^��: " + heroID);
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
