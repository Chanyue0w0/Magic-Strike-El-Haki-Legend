//using System.Collections;
//using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerStatusManager : MonoBehaviour
{
	private enum UserPosition { player1, player2 };
    [Header("----------------- Status Data ------------------")]
    [SerializeField] private int healthPoint;
    [SerializeField] private int maxHealthPoint;
    [SerializeField] private int attackDamage;
    //[SerializeField] private int currentMagicPoint;
    [SerializeField] private bool isBurning = false; // 是否正在燃燒
    [SerializeField] private bool isPoisoning = false; // 是否正在中毒
    [SerializeField] private bool isAlive = true; // 是否活著

    [Header("----------------- Config Setting ------------------")]
    [SerializeField] private UserPosition player;
    [SerializeField] private bool userIsEnemy;
    [SerializeField] private int playerNumber;

    [Header("----------------- Variable Observe ------------------")]
    [SerializeField] private string[] skills;
    //[Header("----------------- Script Reference ------------------")]


    [Header("----------------- Health Bar Setting ------------------")]
    [SerializeField] private HealthBar healthBar;

    [Header("----------------- Player Gamebject ------------------")]
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;

    [Header("----------------- SpriteSkin ------------------")]
    [SerializeField] private SpriteRenderer player_skin;
    [SerializeField] private SpriteRenderer playerPuck_skin;

    [Header("----------------- HeadStickers ------------------")]
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Image headStickerImage;

    [Header("----------------- Damage Number ------------------")]
    [SerializeField] private float damageSpacing = 1.0f; // 傷害數字間隔範圍調整變數
    [SerializeField] private Vector2 positionOffset = new Vector2(0, 0); // 傷害數字位置誤差調整變數

    [Header("----------------- Animator ------------------")] //Only for Player2
    [SerializeField] private Animator player_animator;


    [Header("----------------- Burn Effect ------------------")] //Only for Player2
    private Coroutine burnCoroutine;
    [SerializeField] private GameObject burnEffect; // 普通燃燒效果
    [SerializeField] private GameObject burnEffect2; // 強化燃燒效果


    // private variable
    private JToken characterData;

	// Start is called before the first frame update
	void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(player == UserPosition.player2 &&  healthPoint <= 0 && isAlive)
        {
            player_animator.SetTrigger("DiePAnimation");
            isAlive = false;
        }
    }

    public void InitStatus()
    {
        // 先確保取消舊的訂閱，避免多次觸發
        UnregisterPlayerNotification();

        FightPlayer1Config.ShieldPercentage = 0;//重製護盾狀態
        isAlive = true;
        if (player == UserPosition.player1)
        {
            playerNumber = 1;
            skills = FightPlayer1Config.Group;
            maxHealthPoint = FightPlayer1Config.StartHP;
            SetHP(FightPlayer1Config.NowHP);
            SetATK(FightPlayer1Config.StartATK);
            SetPlayerSkin(FightPlayer1Config.PlayerSkin);
            //SetMagicPoint(0);
            healthBar.SetMaxHealth(FightPlayer1Config.StartHP);
            healthBar.SetHealth(FightPlayer1Config.NowHP);
            // 在 Start 時嘗試找到 PlayerNotification 並綁定事件
            PlayerNotification notification = player1.GetComponent<PlayerNotification>();
            RegisterPlayerNotification(notification);//訂閱通知

            //MagicStonesUI_animator = MagicStonesUI.GetComponent<Animator>();
        }
        else if (player == UserPosition.player2)
        {
            playerNumber = 2;
            skills = FightPlayer2Config.Group;
            maxHealthPoint = FightPlayer2Config.StartHP;
            SetHP(FightPlayer2Config.StartHP);
            SetATK(FightPlayer2Config.StartATK);
            //SetPlayerSkin(FightPlayer2Config.PlayerSkin);  //史萊姆需要用更改生成Prefab
            //GameObject skinPrefab = Resources.Load<GameObject>("Prefabs/SlimeSkins/" + FightPlayer2Config.PlayerSkin + "Skin");
            //GameObject skinObj = Instantiate(skinPrefab, player2.transform.position, Quaternion.identity);
            //skinObj.transform.SetParent(player2.transform);
            RuntimeAnimatorController loadedController = Resources.Load<RuntimeAnimatorController>("AnimationForSkin/" 
                + FightPlayer2Config.PlayerSkin + "Skin");
            player_animator.runtimeAnimatorController = loadedController;

            SetPuckSkin(FightPlayer2Config.PuckSkin);
            //SetMagicPoint(0);
            healthBar.SetMaxHealth(FightPlayer2Config.StartHP);
            healthBar.SetHealth(FightPlayer2Config.StartHP);
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
    public void SetPlayerSkin(string playerSkin)
    {
        player_skin.sprite = Resources.Load<Sprite>("Arts/FightScene/Field/FieldObjects/" + playerSkin);
    }
    public void SetPuckSkin(string puckSkin)
    {
        playerPuck_skin.sprite = Resources.Load<Sprite>("Arts/FightScene/Field/FieldObjects/" + puckSkin);
    }

    // 取消訂閱通知，避免事件重複綁定
    public void UnregisterPlayerNotification()
    {
        if (player == UserPosition.player1)
        {
            PlayerNotification notification = player1.GetComponent<PlayerNotification>();
            if (notification != null)
            {
                notification.OnDamageReceived -= HandleDamageNotification;
                notification.OnStatusEffectApplied -= HandleStatusEffectApplied;
                notification.OnHealingReceived -= HandleHealingNotification;
            }
        }
        else if (player == UserPosition.player2)
        {
            PlayerNotification notification = player2.GetComponent<PlayerNotification>();
            if (notification != null)
            {
                notification.OnDamageReceived -= HandleDamageNotification;
                notification.OnStatusEffectApplied -= HandleStatusEffectApplied;
                notification.OnHealingReceived -= HandleHealingNotification;
            }
        }
    }


    //訂閱通知
    public void RegisterPlayerNotification(PlayerNotification playerNotification)
    {
        if (playerNotification != null)
        {
            playerNotification.OnDamageReceived -= HandleDamageNotification;
            playerNotification.OnDamageReceived += HandleDamageNotification;

            playerNotification.OnHealingReceived -= HandleHealingNotification;
            playerNotification.OnHealingReceived += HandleHealingNotification;

            playerNotification.OnStatusEffectApplied -= HandleStatusEffectApplied;
            playerNotification.OnStatusEffectApplied += HandleStatusEffectApplied;
        }
    }


    // 接收 `PlayerNotification` 的受到攻擊通知
    private void HandleDamageNotification(int damage, GameObject player)
    {
        //Debug.Log($"{gameObject.name} 受攻擊傷害：{damage}");
        GetDamage(damage);
    }

    // 接收 `PlayerNotification` 的受到治療通知
    private void HandleHealingNotification(int healingAmount, GameObject player)
    {
        //Debug.Log($"{gameObject.name} 受攻擊傷害：{damage}");
        GetRecoverHP(healingAmount);
    }

    // 接收 `PlayerNotification` 的受到效果通知
    private void HandleStatusEffectApplied(StatusEffect effect, GameObject player)
    {

        //Debug.Log($"{gameObject.name} 觸發狀態效果：{effect}");

        if (effect == StatusEffect.Burn)//中毒效果
        {
            if (playerNumber == 1)
            {
                GameObject instBurnEffect = burnEffect;
                //if (PassiveSkillManager.Instance.HasBurnStrengthen())//有強化燃燒
                //{
                //    instBurnEffect = burnEffect2;
                //}
                GameObject obj = Instantiate(instBurnEffect, player1.transform.position, Quaternion.identity);
                obj.transform.SetParent(player1.transform);
            }
            else
            {
                GameObject instBurnEffect = burnEffect;
                if (PassiveSkillManager.Instance.HasBurnStrengthen())//有強化燃燒
                {
                    instBurnEffect = burnEffect2;
                }
                GameObject obj = Instantiate(instBurnEffect, player2.transform.position, Quaternion.identity);
                obj.transform.SetParent(player2.transform);
            }

            // 若已有燒傷效果正在進行，先停止協程再重啟
            if (burnCoroutine != null)
            {
                StopCoroutine(burnCoroutine);
            }

            burnCoroutine = StartCoroutine(BurnEffect()); // 重新啟動燃燒效果
        }
        else if (effect == StatusEffect.Poison)//中毒效果
        {
            if (!isPoisoning)
            {
                StartCoroutine(PoisonEffect());
            }
        }


    }


    private IEnumerator PoisonEffect() // 中毒效果
    {
        isPoisoning = true;
        Color poisonColor = new Color(0.69f, 0, 1, 1); // 紫色
        Color poisonLightColor = new Color(0.85f, 0.5f, 1, 1); // 淡紫色
        Color normalColor = new Color(1, 1, 1, 1);      // 白色

        for (int i = 0; i < 5; i++) // 中毒 5 秒
        {
            int burnDamage = Mathf.RoundToInt(70);
            GetDamage(burnDamage);

            // 變紫色
            player_skin.color = poisonColor;
            healthBarImage.color = poisonColor;
            headStickerImage.color = poisonColor;

            yield return new WaitForSeconds(0.5f);

            // 變白色
            player_skin.color = poisonLightColor;
            healthBarImage.color = poisonLightColor;
            headStickerImage.color = poisonLightColor;

            yield return new WaitForSeconds(0.5f);
        }

        isPoisoning = false;

        // 結束時保證回復白色
        player_skin.color = normalColor;
        healthBarImage.color = normalColor;
        headStickerImage.color = normalColor;
    }


    private IEnumerator BurnEffect()//燃燒效果
    {
        isBurning = true;
        for (int i = 0; i < 5; i++) // 燃燒 5 秒，每秒扣 20 點血
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
        if(finalDamage > 0)
        {
            UIShakingManager.Instance.ShakePlayerUI(playerNumber);
            DisplayDamage(finalDamage);
        }
        Debug.Log(player + " Get Damage "+ finalDamage);

    }

    public void GetRecoverHP(int recoverHp)
    {
        int finalHealAmount = 0;
        if (player == UserPosition.player1)
        {
            finalHealAmount = Mathf.RoundToInt(recoverHp);
            healthPoint = Mathf.Min(healthPoint + recoverHp, maxHealthPoint);
            //Debug.Log("FightPlayer1Config.ShieldPercentage" + FightPlayer1Config.ShieldPercentage);
            //Debug.Log("Final Damage 1 :" + finalDamage);
        }
        else if (player == UserPosition.player2)
        {
            finalHealAmount = Mathf.RoundToInt(recoverHp);
            healthPoint = Mathf.Min(healthPoint + recoverHp, maxHealthPoint);
            //Debug.Log("FightPlayer2Config.ShieldPercentage" + FightPlayer2Config.ShieldPercentage);
            //Debug.Log("Final Damage 2 :" + finalDamage);
        }
        healthBar.SetHealth(healthPoint); // 更新血條
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
