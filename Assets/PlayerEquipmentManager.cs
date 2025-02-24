using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerEquipmentManager : MonoBehaviour
{
	[SerializeField] private string savePath = "/playerEquipment.json";
	private List<PlayerEquipment> equipmentList = new();

	public static PlayerEquipmentManager Instance { get; private set; }

	[System.Serializable]
	public class PlayerEquipment
	{
		public string name;
		public string id;
		public string equipmentType; // 例如：頭、身體、鞋子
		public string rarity;        // 普通、常見、稀有、特殊、傳說
		public string setType;       // None, Warrior, Archer, Magician, Healer
		public int currentLevel;
		public int attackPower;
		public int healthPoints;
		public string description;
		public string buff1;
		public string buff2;
		public string buff3;
		public string buff4;
		public string equippedByHero; // 給予裝備的英雄代號

		public PlayerEquipment(string name, string id, string equipmentType, string rarity, string setType, int currentLevel, int attackPower, int healthPoints, string description, string buff1, string buff2, string buff3, string buff4, string equippedByHero)
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
			this.buff1 = buff1;
			this.buff2 = buff2;
			this.buff3 = buff3;
			this.buff4 = buff4;
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

	void Start()
	{
		// 載入所有裝備數據
		List<PlayerEquipment> list = PlayerEquipmentManager.Instance.GetAllEquipmentData();
		foreach (var eq in list)
		{
			if (eq != null)
			{
				Debug.Log($"Loaded Equipment: {eq.name}, Level: {eq.currentLevel}");
			}
		}
		// 新增一筆裝備
		PlayerEquipment newEquipment = new PlayerEquipment(
			"Flame Helmet", "HT01", "Head", "Rare", "Warrior", 1, 50, 200,
			"A helmet forged in flames", "TotalATKIncrease_5%", "CriticalRateIncrease_5%", "SkillDamageIncrease_5%", "SkillBubbleCooldownReduction_2s", "HR01"
		);
		PlayerEquipmentManager.Instance.AddEquipment(newEquipment);

		// 取得指定 ID 的裝備並更新其描述
		PlayerEquipment updatedEquipment = PlayerEquipmentManager.Instance.GetEquipmentById("HT01");
		if (updatedEquipment != null)
		{
			updatedEquipment.description = "Updated description";
			PlayerEquipmentManager.Instance.UpdateEquipment(updatedEquipment);
		}
	}

	public void AddEquipment(PlayerEquipment equipment)
	{
		LoadEquipment();
		if (!equipmentList.Exists(e => e.id == equipment.id))
		{
			equipmentList.Add(equipment);
		}
		else
		{
			UpdateEquipment(equipment);
		}
		SaveEquipment();
	}

	public void DeleteEquipment(string equipmentID)
	{
		LoadEquipment();
		PlayerEquipment equipment = equipmentList.Find(e => e.id == equipmentID);
		if (equipment != null)
		{
			equipmentList.Remove(equipment);
			SaveEquipment();
			Debug.Log("Equipment deleted: " + equipment.name);
		}
		else
		{
			Debug.LogWarning("Equipment not found to delete: " + equipmentID);
		}
	}

	public void UpdateEquipment(PlayerEquipment updatedEquipment)
	{
		PlayerEquipment equipment = equipmentList.Find(e => e.id == updatedEquipment.id);
		if (equipment != null)
		{
			equipment.currentLevel = updatedEquipment.currentLevel;
			equipment.attackPower = updatedEquipment.attackPower;
			equipment.healthPoints = updatedEquipment.healthPoints;
			equipment.description = updatedEquipment.description;
			equipment.buff1 = updatedEquipment.buff1;
			equipment.buff2 = updatedEquipment.buff2;
			equipment.buff3 = updatedEquipment.buff3;
			equipment.buff4 = updatedEquipment.buff4;
			equipment.equippedByHero = updatedEquipment.equippedByHero;
			SaveEquipment();
			Debug.Log("Equipment updated: " + updatedEquipment.name + " " + updatedEquipment.id);
		}
		else
		{
			Debug.LogWarning("Equipment not found to update: " + updatedEquipment.id);
		}
	}

	public PlayerEquipment GetEquipmentById(string equipmentID)
	{
		LoadEquipment();
		PlayerEquipment equipment = equipmentList.Find(e => e.id == equipmentID);
		if (equipment == null)
		{
			Debug.LogWarning("Equipment not found: " + equipmentID);
		}
		return equipment;
	}

	public List<PlayerEquipment> GetAllEquipmentData()
	{
		LoadEquipment();
		return equipmentList;
	}

	public void LoadEquipment()
	{
		if (!File.Exists(SavePath()))
		{
			equipmentList = new List<PlayerEquipment>();
			Debug.LogWarning("Equipment save file not found!");
			SaveEquipment();
		}

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

	public void SaveEquipment()
	{
		JArray json = JArray.FromObject(equipmentList);
		string jsonTxt = json.ToString();
		File.WriteAllText(SavePath(), jsonTxt);
		Debug.Log("Equipment data saved: " + SavePath());
	}

	private string SavePath()
	{
		return Application.persistentDataPath + savePath;
	}
}
