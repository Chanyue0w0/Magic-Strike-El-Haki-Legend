//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;


public class SkillData : MonoBehaviour
{
	public static SkillData Instance { get; private set; }

	[SerializeField] private string filePath = "jsonData/SkillData";
	[HideInInspector] public JObject jsonData;
	private void Awake()
	{
		if (Instance != null)
		{
			Debug.Log("Found more than one SkillData object in the sence");
		}
		Instance = this;

		jsonData = JObject.Parse(Resources.Load<TextAsset>(filePath).text);
	}

	public JToken GetSkillData(string id)
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
