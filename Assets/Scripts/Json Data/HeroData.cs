//using System.Collections;
//using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
public class HeroData : MonoBehaviour
{
	[System.Serializable]
	public class HeroStats
	{
		public int BaseATK;
		public int BaseHP;
		public int UltimateSkillDamage;
	}

	[System.Serializable]
	public class Hero
	{
		public string Name;
		public string ID;
		public string Rarity; // Normal, Common, Rare, Special, Legendary
		public Dictionary<int, HeroStats> LevelStats;
		public string Description;
	}

	public static HeroData Instance { get; private set; }

	[SerializeField] private string filePath = "jsonData/HeroData";
	[HideInInspector] public JObject jsonData;
	private void Awake()
	{
		if (Instance != null)
		{
			Debug.Log("Found more than one Hero data object in the sence");
		}
		Instance = this;

		jsonData = JObject.Parse(Resources.Load<TextAsset>(filePath).text);
	}

	public JToken GetHeroData(string id)
	{
		if (jsonData[id] == null)
			Debug.Log("id: " + id + " is not found in json data!");
		return jsonData[id];
	}

	public Hero GetHero(string id)
	{
		JToken heroData = GetHeroData(id);

		Hero hero = JsonConvert.DeserializeObject<Hero>(heroData.ToString());
		return hero;
	}
}
