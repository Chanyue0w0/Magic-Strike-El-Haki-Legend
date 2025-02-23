//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using static HeroData;

public class EquipmentData : MonoBehaviour
{

	[System.Serializable]
	public class Equipment
	{
		public string Name;
		public string ID; // e.g., HT00 (Head), BD00 (Body), SH00 (Shoes)
		public string Type; // Head, Body, Shoes
		public string Rarity; // Normal, Common, Rare, Special, Legendary
		public string SetType; // None, Warrior, Archer, Magician, Healer
		public int AttackPower;
		public int HealthPoints;
		public string Description;
	}

	public static EquipmentData Instance { get; private set; }

	[SerializeField] private string filePath = "jsonData/EquipmentData";
	[HideInInspector] public JObject jsonData;
	private void Awake()
	{
		if (Instance != null)
		{
			Debug.Log("Found more than EquipmentData object in the sence");
		}
		Instance = this;

		jsonData = JObject.Parse(Resources.Load<TextAsset>(filePath).text);
	}

	public JToken GetEquipmentData(string id)
	{

		if (jsonData[id] == null)
			Debug.Log("id: " + id + " is not found in json data!");
		return jsonData[id];
	}

	public Equipment GetEquipment(string id)
	{
		JToken equiData = GetEquipmentData(id);

		Equipment hero = JsonConvert.DeserializeObject<Equipment>(equiData.ToString());
		return hero;
	}
}
