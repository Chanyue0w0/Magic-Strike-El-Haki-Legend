//using System.Collections;
//using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System.Collections;

public class PlayerStatusManager : MonoBehaviour
{
	private enum UserPosition { player1, player2 };
    [Header("----------------- Status Data ------------------")]
    [SerializeField] private int healthPoint;
    [SerializeField] private int attackDamage;
    [SerializeField] private int currentMagicPoint;
    [SerializeField] private bool isBurning = false; // 是否正在燃燒

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
            RegisterPlayerNotification(notification);//訂閱通知
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
            RegisterPlayerNotification(notification);//訂閱通知
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

    //訂閱通知
    public void RegisterPlayerNotification(PlayerNotification playerNotification)
    {
        Debug.Log($"{gameObject.name} 收到 傷害");
        playerNotification.OnDamageReceived += HandleDamageNotification;
    }

    // 接收 `PlayerNotification` 的受到攻擊通知
    private void HandleDamageNotification(int damage, GameObject player)
    {
        GetDamage(damage);
    }

    // 接收 `PlayerNotification` 的受到效果通知
    private void HandleStatusEffectApplied(StatusEffect effect, GameObject player)
    {
        if (player == gameObject)
        {
            Debug.Log($"{gameObject.name} 觸發狀態效果：{effect}");

            if (effect == StatusEffect.Burn && !isBurning)
            {
                StartCoroutine(BurnEffect()); // 在這裡觸發燃燒效果
            }
        }
    }
    private IEnumerator BurnEffect()//燃燒效果
    {
        isBurning = true;
        for (int i = 0; i < 5; i++) // 燃燒 5 秒，每秒扣 5 點血
        {
            GetDamage(5);
            yield return new WaitForSeconds(1);
        }
        isBurning = false;
    }

    public void GetDamage(int damage)
    {
        healthPoint -= damage;
        healthBar.SetHealth(healthPoint); // 更新血條
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
