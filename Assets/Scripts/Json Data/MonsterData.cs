//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class MonsterData : MonoBehaviour
{
	public static MonsterData Instance { get; private set; }

	[SerializeField] private string filePath = "jsonData/MonsterData";
	[HideInInspector] public JObject jsonData;
	private void Awake()
	{
		if (Instance != null)
		{
			Debug.Log("Found more than one MonsterData object in the sence");
		}
		Instance = this;

		jsonData = JObject.Parse(Resources.Load<TextAsset>(filePath).text);
	}

	public JToken GetMonsterData(string id)
	{
		return jsonData[id];
	}

	public string GetScriptPath(string id)
	{
		if (jsonData[id] == null)
			Debug.Log("id: " + id + " is not found in json data!");
		return jsonData[id]["ScriptPath"].ToString();
	}
}
