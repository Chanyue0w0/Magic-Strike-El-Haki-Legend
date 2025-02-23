//using System.Collections;
//using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class PlayerStatusManager : MonoBehaviour
{
	private enum UserPosition { player1, player2 };
    [Header("----------------- Status Data ------------------")]
    [SerializeField] private int healthPoint;
    [SerializeField] private int attackDamage;
    [SerializeField] private int currentMagicPoint;

	[Header("----------------- Config Setting ------------------")]
    [SerializeField] private UserPosition player;
    [SerializeField] private bool userIsEnemy;

	[Header("----------------- Variable Observe ------------------")]
    [SerializeField] private string[] skills;
    //[Header("----------------- Script Reference ------------------")]

    // private variable
    private JToken characterData;

	// Start is called before the first frame update
	void Start()
    {
        //InitStatus();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitStatus()
    {
		if (player == UserPosition.player1)
        {
            skills = FightPlayer1Config.Group;
            SetHP(FightPlayer1Config.StartHP);
            SetATK(FightPlayer1Config.StartATK);
            SetMagicPoint(0);
        }
        if (player == UserPosition.player2)
        {
            skills = FightPlayer2Config.Group;
            SetHP(FightPlayer2Config.StartHP);
            SetATK(FightPlayer2Config.StartATK);
            SetMagicPoint(0);
        }
        else
            Debug.Log("user poistion is not setting!!!!");

        


        if (!userIsEnemy)
        {
            characterData = HeroData.Instance.GetHeroData(skills[0]);
            if(characterData == null)
            {
                Debug.Log("not found skills[0]: " + skills[0] + "in heroData.json!!!!");
                return;
            }
            //healthPoint = characterData["LevelStats"]["1"]["BaseHP"].ToObject<int>();  //透過MainMenu加總過來

		}
        else
        {
			characterData = MonsterData.Instance.GetMonsterData(skills[0]);
			if (characterData == null)
			{
				Debug.Log("not found skills[0]: " + skills[0] + "in heroData.json!!!!");
				return;
			}
        }
	}

	public void GetDamage(int atk)
    {
        healthPoint -= atk;
    }

    public void GetRecoverHP(int recoverHp)
    {
        healthPoint += recoverHp;
    }

    public int GetHP()
    {
        return healthPoint;
    }

    public void SetHP(int hp)
    {
        healthPoint = hp;
    }
    public int GetATK()
    {
        return attackDamage;
    }
    public void SetATK(int atk)
    {
        attackDamage = atk;
    }

    public int GetMagicPoint()
    {
        return currentMagicPoint;
	}

    public void SetMagicPoint(int point)
    {
        currentMagicPoint = point;
    }

    public void GetOnePointMP()
    {
		currentMagicPoint += 1;
    }

    public void SetSkills(string[] skillArray)
    {
        skills = skillArray;
    }
}
