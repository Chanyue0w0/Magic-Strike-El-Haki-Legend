using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerEquipmentManager : MonoBehaviour
{
	private string savePath = "/playerEquipment.json";
	private List<PlayerEquipment> equipmentList = new();

	public static PlayerEquipmentManager Instance { get; private set; }

	private static readonly Dictionary<string, List<string>> buffOptions = new()
	{
		{ "Total Attack Increase%", new List<string> { "3%", "5%", "10%" } },
		{ "Total Health Increase%", new List<string> { "3%", "5%", "10%" } },
		{ "Critical Rate Increase%", new List<string> { "3%", "5%", "10%" } },
		{ "Skill Damage Increase%", new List<string> { "3%", "5%", "10%" } },
		{ "Poison Damage Increase%", new List<string> { "3%", "5%", "10%" } },
		{ "Control Duration Increase%", new List<string> { "3%", "5%", "10%" } },
		{ "Reduce Skill Bubble Generation Time", new List<string> { "1s", "2s", "3s" } }
	};

	[System.Serializable]
	public class PlayerEquipment
	{
		public string name;
		public string id;
		public string equipmentType;
		public string rarity;
		public string setType;
		public int currentLevel;
		public int attackPower;
		public int healthPoints;
		public string description;
		public Dictionary<string, string> buffs;
		public string equippedByHero;

		public PlayerEquipment(string name, string id, string equipmentType, string rarity, string setType,
			int currentLevel, int attackPower, int healthPoints, string description,
			Dictionary<string, string> buffs, string equippedByHero)
		{
			this.name = name;
			this.id = id;
			this.equipmentType = equipmentType;
			this.rarity = rarity;
			this.setType = setType;
			this.currentLevel = currentLevel;
			this.attackPower = attackPower;
			this.healthPoints = healthPoints;
			this.description = description;
			this.buffs = buffs ?? new Dictionary<string, string>();
			this.equippedByHero = equippedByHero;
		}
	}

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.Log("Found more than one PlayerEquipmentManager object in the scene");
		}
		Instance = this;
	}

	private void Start()
	{
		//PlayerEquipment newEquipment = new PlayerEquipment(
		//	"Legendary Sword", System.Guid.NewGuid().ToString(), "Weapon", "Legendary", "Warrior",
		//	1, 100, 200, "A powerful warrior's sword", GenerateRandomBuffs(4), "None"
		//);
		//AddEquipment(newEquipment);
		if(!File.Exists(SavePath()))
		{
			CreateEquipmentFromData("EM00");
			CreateEquipmentFromData("EM01");
			CreateEquipmentFromData("EM02");
		}
	}

	private Dictionary<string, string> GenerateRandomBuffs(int numberOfBuffs)
	{
		Dictionary<string, string> selectedBuffs = new Dictionary<string, string>();
		List<string> keys = new List<string>(buffOptions.Keys);
		System.Random random = new System.Random();

		while (selectedBuffs.Count < numberOfBuffs && keys.Count > 0)
		{
			int index = random.Next(keys.Count);
			string key = keys[index];
			string value = buffOptions[key][random.Next(buffOptions[key].Count)];

			selectedBuffs[key] = value;
			keys.RemoveAt(index);
		}
		return selectedBuffs;
	}

	public void CreateEquipmentFromData(string id)
	{
		if (EquipmentData.Instance == null)
		{
			Debug.LogError("EquipmentData Instance is not initialized!");
			return;
		}

		EquipmentData.Equipment data = EquipmentData.Instance.GetEquipment(id);
		if (data == null)
		{
			Debug.LogError("Equipment data not found for ID: " + id);
			return;
		}

		// 創建 PlayerEquipment
		PlayerEquipment newEquipment = new PlayerEquipment(
			data.Name,
			data.ID,
			data.Type,
			data.Rarity,
			data.SetType,
			1, // 初始等級
			data.AttackPower,
			data.HealthPoints,
			data.Description,
			GenerateRandomBuffs(2), // 這裡設置為隨機 2 個 Buff，可根據需求更改
			"None" // 初始未被英雄裝備
		);

		// 添加設備到列表並存檔
		AddEquipment(newEquipment);
		Debug.Log("New equipment created and added: " + newEquipment.name);
	}


	public void AddEquipment(PlayerEquipment equipment)
	{
		if (equipment == null) return;
		LoadEquipment();
		equipmentList.Add(equipment);
		SaveEquipment();
	}

	public void LoadEquipment()
	{
		if (!File.Exists(SavePath()))
		{
			equipmentList = new List<PlayerEquipment>();
			Debug.LogWarning("Equipment save file not found!");
			SaveEquipment();
		}
		else
		{
			string json = File.ReadAllText(SavePath());
			if (string.IsNullOrEmpty(json))
			{
				equipmentList = new List<PlayerEquipment>();
				Debug.LogWarning("Save file is empty, creating new equipment list!");
			}
			else
			{
				equipmentList = JsonConvert.DeserializeObject<List<PlayerEquipment>>(json);
				Debug.Log("Equipment data loaded!");
			}
		}
	}

	public void SaveEquipment()
	{
		JArray json = JArray.FromObject(equipmentList);
		string jsonTxt = json.ToString();
		File.WriteAllText(SavePath(), jsonTxt);
		Debug.Log("Equipment data saved: " + SavePath());
	}
	public PlayerEquipment GetEquipmentByIndex(int index)
	{
		LoadEquipment(); // Ensure the latest data is loaded

		if (index >= 0 && index < equipmentList.Count)
		{
			return equipmentList[index];
		}

		Debug.LogWarning("Invalid index for fetching equipment: " + index);
		return null;
	}

	public List<PlayerEquipment> GetAllEquipmentData()
	{
		LoadEquipment();
		return equipmentList;
	}

	private string SavePath()
	{
		return Application.persistentDataPath + savePath;
	}
}