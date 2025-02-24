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


    [Header("----------------- Health Bar Setting ------------------")]
    [SerializeField] private HealthBar healthBar;

    [Header("----------------- PlayerGameObject ------------------")]
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;


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
            healthBar.SetMaxHealth(FightPlayer1Config.StartHP);
            // 在 Start 時嘗試找到 PlayerNotification 並綁定事件
            PlayerNotification notification = player1.GetComponent<PlayerNotification>();
        }

        if (player == UserPosition.player2)
        {
            skills = FightPlayer2Config.Group;
            SetHP(FightPlayer2Config.StartHP);
            SetATK(FightPlayer2Config.StartATK);
            SetMagicPoint(0);
            healthBar.SetMaxHealth(FightPlayer2Config.StartHP);
            // 在 Start 時嘗試找到 PlayerNotification 並綁定事件
            PlayerNotification notification = player2.GetComponent<PlayerNotification>();
        }

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

    public void RegisterPlayerNotification(PlayerNotification playerNotification)
    {
        playerNotification.OnDamageReceived += HandleDamageNotification;
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
