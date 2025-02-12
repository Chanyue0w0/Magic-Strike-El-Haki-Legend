//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class FightStartInitalizeData : MonoBehaviour
{
	[SerializeField] private PlayerStatusManager player1Status;
	[SerializeField] private PlayerStatusManager player2Status;


	private JArray characterData;
	// Start is called before the first frame update
	void Start()
    {
		characterData = JArray.Parse(Resources.Load<TextAsset>("CharacterSetting").text);

		player1Status.SetSkills(FightConfigGlobalIndex.player1_Group);
		JToken p1Data = GetCharacterData(FightConfigGlobalIndex.player1_Group[0]);
		player1Status.SetHP(p1Data["healthPoint"].ToObject<int>());

		player2Status.SetSkills(FightConfigGlobalIndex.player2_Group);

	}

    // Update is called once per frame
    void Update()
    {
        
    }

	private JToken GetCharacterData(string code)
	{
		foreach (var data in characterData)
		{
			if (data["code"].ToString() == code)
			{
				return data;
			}
		}
		return null;
	}
}
