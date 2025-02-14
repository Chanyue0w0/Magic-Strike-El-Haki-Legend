//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class EquipmentData : MonoBehaviour
{
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
}
