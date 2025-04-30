using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class PlayerSkillManager : MonoBehaviour
{
	private string savePath = "/PlayerSkillBag.json";
	private List<string> skillList = new();

	public static PlayerSkillManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogWarning("Found more than one PlayerSkillManager in the scene.");
		}
		Instance = this;

		string path = Application.persistentDataPath + savePath;
		if (!File.Exists(path))
		{
			InitJsonFile();
		}

		LoadSkills();
	}

	/// 初始化預設技能資料
	public void InitJsonFile()
	{
		skillList = new List<string> { "SK00", "SK01" };
		SaveSkills();
		Debug.Log("已初始化預設技能清單: " + string.Join(", ", skillList));
	}

	/// 讀取技能資料
	public void LoadSkills()
	{
		string path = Application.persistentDataPath + savePath;

		if (!File.Exists(path))
		{
			Debug.LogWarning("技能存檔不存在，初始化為空清單。");
			skillList = new List<string>();
			SaveSkills();
			return;
		}

		string json = File.ReadAllText(path);
		skillList = JsonConvert.DeserializeObject<List<string>>(json);
		Debug.Log("技能清單已載入: " + string.Join(", ", skillList));
	}

	/// 儲存技能資料
	public void SaveSkills()
	{
		string path = Application.persistentDataPath + savePath;
		string json = JsonConvert.SerializeObject(skillList, Formatting.Indented);
		File.WriteAllText(path, json);
		Debug.Log("技能清單已存檔: " + path);
	}

	/// 取得所有技能 ID
	public List<string> GetAllSkills()
	{
		return new List<string>(skillList);
	}

	/// 新增技能 ID
	public void AddSkill(string skillId)
	{
		if (!skillList.Contains(skillId))
		{
			skillList.Add(skillId);
			SaveSkills();
		}
	}

	/// 移除技能 ID
	public void RemoveSkill(string skillId)
	{
		if (skillList.Contains(skillId))
		{
			skillList.Remove(skillId);
			SaveSkills();
		}
	}
}
