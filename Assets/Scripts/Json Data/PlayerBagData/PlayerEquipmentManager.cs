using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerEquipmentManager : MonoBehaviour
{

	[SerializeField] private bool resetJsonFile;
	private string savePath = "/playerEquipment.json";
	private List<PlayerEquipment> equipmentList = new();

	public static PlayerEquipmentManager Instance { get; private set; }

	private static readonly Dictionary<string, List<string>> buffOptions = new()
	{
		{ "TotalAttackIncrease", new List<string> { "3%", "5%", "10%" } },
		{ "TotalHealthIncrease", new List<string> { "3%", "5%", "10%" } },
		{ "CriticalRateIncrease", new List<string> { "3%", "5%", "10%" } },
		{ "SkillDamageIncrease", new List<string> { "3%", "5%", "10%" } },
		{ "PoisonDamageIncrease", new List<string> { "3%", "5%", "10%" } },
		{ "ControlDurationIncrease", new List<string> { "3%", "5%", "10%" } },
		{ "ReduceSkillBubbleGenerationTime", new List<string> { "1s", "2s", "3s" } }
	};

	[System.Serializable]
	public class PlayerEquipment
	{
		public string name;
		public string id;
		public string typeID;
		public string equipmentType;
		public string rarity;
		public string setType;
		public int currentLevel;
		public int attackPower;
		public int healthPoints;
		public string description;
		public Dictionary<string, string> buffs;
		public string equippedByHero;

		public PlayerEquipment(string name, string id, string typeID, string equipmentType, string rarity, string setType,
			int currentLevel, int attackPower, int healthPoints, string description,
			Dictionary<string, string> buffs, string equippedByHero)
		{
			this.name = name;
			this.id = id;
			this.equipmentType = equipmentType;
			this.typeID = typeID;
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
		if (resetJsonFile)
		{
			File.Delete(FilePath());
		}

		LoadEquipment();
	}
	private void InitEquipmentFile()
	{
		equipmentList = new List<PlayerEquipment>();
		Debug.LogWarning("Creat new Equipment List!");
		SaveEquipment();
		CreateEquipmentFromData("HT00");
		CreateEquipmentFromData("HT00");
		CreateEquipmentFromData("HT00");
		CreateEquipmentFromData("BD00");
		CreateEquipmentFromData("BD00");
		CreateEquipmentFromData("SH00");
		CreateEquipmentFromData("SH00");
		SaveEquipment();
	}

	private Dictionary<string, string> GenerateRandomBuffs(int numberOfBuffs)
	{
		Dictionary<string, string> selectedBuffs = new Dictionary<string, string>();
		List<string> keys = new(buffOptions.Keys);
		System.Random random = new();

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

	public PlayerEquipment CreateEquipmentFromData(string id)
	{
		if (EquipmentData.Instance == null)
		{
			Debug.LogError("EquipmentData Instance is not initialized!");
			return null;
		}

		EquipmentData.Equipment data = EquipmentData.Instance.GetEquipment(id);
		if (data == null)
		{
			Debug.LogError("Equipment data not found for ID: " + id);
			return null;
		}

		// 創建 PlayerEquipment
		PlayerEquipment newEquipment = new PlayerEquipment(
			data.Name,
			System.Guid.NewGuid().ToString(),
			data.ID,
			data.Type,
			data.Rarity,
			data.SetType,
			1, // 初始等級
			data.AttackPower,
			data.HealthPoints,
			data.Description,
			GenerateRandomBuffs(4), // 這裡設置為隨機 2 個 Buff，可根據需求更改
			"None" // 初始未被英雄裝備
		);

		bool isReloadData = equipmentList.Count > 0;
		// 添加設備到列表並存檔
		AddEquipment(newEquipment, isReloadData);
		Debug.Log("New equipment created and added: " + newEquipment.name);

		return newEquipment;
	}


	public void UpdateEquipment(PlayerEquipment updatedEquipment)
	{
		//LoadEquipment(); // Load the latest data

		for (int i = 0; i < equipmentList.Count; i++)
		{
			if (equipmentList[i].id == updatedEquipment.id)
			{
				equipmentList[i] = updatedEquipment;
				SaveEquipment(); // Save the updated data
				Debug.Log($"Equipment updated: {updatedEquipment.name}, {updatedEquipment.typeID}, id: {updatedEquipment.id}");
				return;
			}
		}

		Debug.LogWarning("Equipment not found for update: " + updatedEquipment.id);
	}

	public void AddEquipment(PlayerEquipment equipment, bool isReloadData)
	{
		if (equipment == null) return;
		if (isReloadData) LoadEquipment();
		equipmentList.Add(equipment);
		SaveEquipment();
	}

	public void LoadEquipment()
	{
		// 初始化
		if (!File.Exists(FilePath()))
		{
			InitEquipmentFile();
			return;
		}

		// 讀取檔案
		string json = File.ReadAllText(FilePath());
		if (string.IsNullOrEmpty(json))
		{
			equipmentList = new List<PlayerEquipment>();
			Debug.LogWarning("Save file is empty, creating new equipment list!");
		}
		else
		{
			equipmentList = JsonConvert.DeserializeObject<List<PlayerEquipment>>(json);
		}
		SaveEquipment();
	}

	public void SaveEquipment()
	{
		JArray json = JArray.FromObject(equipmentList);
		string jsonTxt = json.ToString();
		File.WriteAllText(FilePath(), jsonTxt);
		//Debug.Log("Equipment data saved: " + FinePath());	
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

	public PlayerEquipment GetEquipmentByID(string id)
	{
		LoadEquipment(); // Ensure the latest data is loaded
		return (id != null) ? equipmentList.Find(eq => eq.id == id) : null;
	}
	public List<PlayerEquipment> GetAllEquipmentData()
	{
		LoadEquipment();
		return equipmentList;
	}

	private string FilePath()
	{
		return Application.persistentDataPath + savePath;
	}

	public bool IsFileEixt()
	{
		if (File.Exists(FilePath())) return true;
		
		LoadEquipment();
		return false;
	}
}