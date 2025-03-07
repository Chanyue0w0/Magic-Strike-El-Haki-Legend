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
    //[SerializeField] private int currentMagicPoint;
    [SerializeField] private bool isBurning = false; // 是否正在燃燒

    [Header("----------------- Config Setting ------------------")]
    [SerializeField] private UserPosition player;
    [SerializeField] private bool userIsEnemy;

	[Header("----------------- Variable Observe ------------------")]
    [SerializeField] private string[] skills;
    //[Header("----------------- Script Reference ------------------")]


    [Header("----------------- Health Bar Setting ------------------")]
    [SerializeField] private HealthBar healthBar;

    [Header("----------------- Player Gamebject ------------------")]
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;

    //[Header("----------------- MagicStonesUI ------------------")]
    //[SerializeField] private GameObject MagicStonesUI;
    //[SerializeField] private Animator MagicStonesUI_animator;
    [Header("----------------- Damage Number ------------------")]
    [SerializeField] private float damageSpacing = 1.0f; // 傷害數字間隔範圍調整變數
    [SerializeField] private Vector2 positionOffset = new Vector2(0, 0); // 傷害數字位置誤差調整變數

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
            //SetMagicPoint(0);
            healthBar.SetMaxHealth(FightPlayer1Config.StartHP);
            // 在 Start 時嘗試找到 PlayerNotification 並綁定事件
            PlayerNotification notification = player1.GetComponent<PlayerNotification>();
            RegisterPlayerNotification(notification);//訂閱通知

            //MagicStonesUI_animator = MagicStonesUI.GetComponent<Animator>();
        }

        if (player == UserPosition.player2)
        {
            skills = FightPlayer2Config.Group;
            SetHP(FightPlayer2Config.StartHP);
            SetATK(FightPlayer2Config.StartATK);
            //SetMagicPoint(0);
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
        //Debug.Log($"{gameObject.name} 收到 傷害");
        playerNotification.OnDamageReceived += HandleDamageNotification;
        playerNotification.OnStatusEffectApplied += HandleStatusEffectApplied;
        //playerNotification.OnGetMagicPointApplied += HandleGetMagicPointNotification;
    }

    // 接收 `PlayerNotification` 的受到攻擊通知
    private void HandleDamageNotification(int damage, GameObject player)
    {
        //Debug.Log($"{gameObject.name} 受攻擊傷害：{damage}");
        GetDamage(damage);
    }

    // 接收 `PlayerNotification` 的受到效果通知
    private void HandleStatusEffectApplied(StatusEffect effect, GameObject player)
    {
        
        //Debug.Log($"{gameObject.name} 觸發狀態效果：{effect}");

        if (effect == StatusEffect.Burn && !isBurning)
        {
            StartCoroutine(BurnEffect()); // 在這裡觸發燃燒效果
        }

    }


    private IEnumerator BurnEffect()//燃燒效果
    {
        isBurning = true;
        for (int i = 0; i < 5; i++) // 燃燒 5 秒，每秒扣 5 點血
        {
            int burnDamage = Mathf.RoundToInt(20 * (1 + FightPlayer1Config.BurnDamageIncrease));
            GetDamage(burnDamage);
            yield return new WaitForSeconds(1);
        }
        isBurning = false;
    }

    public void GetDamage(int damage)
    {
        VibrationPattern.Instance.StartVibrationPattern();
        int finalDamage = 0;
        if(player == UserPosition.player1)
        {
            finalDamage = Mathf.RoundToInt(damage * (1 - FightPlayer1Config.ShieldPercentage));
            healthPoint -= finalDamage; //扣除減傷量
            //Debug.Log("FightPlayer1Config.ShieldPercentage" + FightPlayer1Config.ShieldPercentage);
            //Debug.Log("Final Damage 1 :" + finalDamage);
        }
        else if(player == UserPosition.player2)
        {
            finalDamage = Mathf.RoundToInt(damage * (1 - FightPlayer2Config.ShieldPercentage));
            healthPoint -= finalDamage; //扣除減傷量
            //Debug.Log("FightPlayer2Config.ShieldPercentage" + FightPlayer2Config.ShieldPercentage);
            //Debug.Log("Final Damage 2 :" + finalDamage);
        }
        healthBar.SetHealth(healthPoint); // 更新血條
        DisplayDamage(finalDamage);
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

    public void DisplayDamage(int score)
    {
        string scoreString = score.ToString();

        // 根據玩家編號計算起始位置，確保對齊居中顯示
        float startX = -((scoreString.Length - 1) * damageSpacing * 0.5f); // 由左至右; 

        // 雙人
        //if (playerNumber == 2 && GlobalIndex.playerGroupCount == 2)
        //    startX = ((scoreString.Length - 1) * damageSpacing * 0.5f);  // 由右至左

        for (int i = 0; i < scoreString.Length; i++)
        {
            char digitChar = scoreString[i];
            int digit = int.Parse(digitChar.ToString());

            GameObject digitObject = DamageNumberPoolManager.Instance.GetDamageEffect(digit);

            if (digitObject != null)
            {
                Vector3 adjustedPosition = Vector3.zero;


                if (player == UserPosition.player1)
                {
                    // 由左至右排列
                    adjustedPosition = player1.transform.position +
                                    new Vector3(startX + i * damageSpacing, 0, 0) +
                                    new Vector3(positionOffset.x, positionOffset.y, 0);

                    SetParticleFlip(digitObject, 0, 0); // 設定 Flip.x = 0 Flip.y = 0
                }
                else 
                {
                    
                    // 由左至右排列
                    adjustedPosition = player2.transform.position +
                                    new Vector3(startX + i * damageSpacing, 0, 0) +
                                    new Vector3(positionOffset.x, -positionOffset.y, 0);

                    SetParticleFlip(digitObject, 0, 0); // 設定 Flip.x = 0 Flip.y = 0
                }
                    

                // 雙人
                //if (playerNumber == 2 && GlobalIndex.playerGroupCount == 2)
                //{
                //    // 由右至左排列 (使用負的 spacing)
                //    adjustedPosition = gameObject.transform.position +
                //                       new Vector3(startX - i * damageSpacing, 0, 0) +
                //                       new Vector3(positionOffset.x, positionOffset.y, 0);

                //    SetParticleFlip(digitObject, 1, 1); // 設定 Flip.x = 0 Flip.y = 1
                //}

                digitObject.transform.position = adjustedPosition; // 設定數字位置

                StartCoroutine(ReturnToPoolAfterDelay(digitObject, digit, 1.0f)); // 1秒後將物件返回物件池
            }
        }
    }

    private IEnumerator ReturnToPoolAfterDelay(GameObject obj, int digit, float delay)
    {
        yield return new WaitForSeconds(delay);
        DamageNumberPoolManager.Instance.ReturnToPool(obj, digit);
    }

    // 上下翻轉特效，設定ParticleSystem的Render的Flip.y屬性
    private void SetParticleFlip(GameObject digitObject, float flipX, float flipY)
    {
        ParticleSystemRenderer renderer = digitObject.GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            renderer.flip = new Vector3(flipX, flipY, renderer.flip.z);
        }
    }

    public void SetSkills(string[] skillArray)
    {
        skills = skillArray;
    }
}
