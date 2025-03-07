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
    [SerializeField] private bool isBurning = false; // �O�_���b�U�N

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
    [SerializeField] private float damageSpacing = 1.0f; // �ˮ`�Ʀr���j�d��վ��ܼ�
    [SerializeField] private Vector2 positionOffset = new Vector2(0, 0); // �ˮ`�Ʀr��m�~�t�վ��ܼ�

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
            // �b Start �ɹ��է�� PlayerNotification �øj�w�ƥ�
            PlayerNotification notification = player1.GetComponent<PlayerNotification>();
            RegisterPlayerNotification(notification);//�q�\�q��

            //MagicStonesUI_animator = MagicStonesUI.GetComponent<Animator>();
        }

        if (player == UserPosition.player2)
        {
            skills = FightPlayer2Config.Group;
            SetHP(FightPlayer2Config.StartHP);
            SetATK(FightPlayer2Config.StartATK);
            //SetMagicPoint(0);
            healthBar.SetMaxHealth(FightPlayer2Config.StartHP);
            // �b Start �ɹ��է�� PlayerNotification �øj�w�ƥ�
            PlayerNotification notification = player2.GetComponent<PlayerNotification>();
            RegisterPlayerNotification(notification);//�q�\�q��
        }

        if (!userIsEnemy)
        {
            characterData = HeroData.Instance.GetHeroData(skills[0]);
            if(characterData == null)
            {
                Debug.Log("not found skills[0]: " + skills[0] + "in heroData.json!!!!");
                return;
            }
            //healthPoint = characterData["LevelStats"]["1"]["BaseHP"].ToObject<int>();  //�z�LMainMenu�[�`�L��
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

    //�q�\�q��
    public void RegisterPlayerNotification(PlayerNotification playerNotification)
    {
        //Debug.Log($"{gameObject.name} ���� �ˮ`");
        playerNotification.OnDamageReceived += HandleDamageNotification;
        playerNotification.OnStatusEffectApplied += HandleStatusEffectApplied;
        //playerNotification.OnGetMagicPointApplied += HandleGetMagicPointNotification;
    }

    // ���� `PlayerNotification` ����������q��
    private void HandleDamageNotification(int damage, GameObject player)
    {
        //Debug.Log($"{gameObject.name} �������ˮ`�G{damage}");
        GetDamage(damage);
    }

    // ���� `PlayerNotification` ������ĪG�q��
    private void HandleStatusEffectApplied(StatusEffect effect, GameObject player)
    {
        
        //Debug.Log($"{gameObject.name} Ĳ�o���A�ĪG�G{effect}");

        if (effect == StatusEffect.Burn && !isBurning)
        {
            StartCoroutine(BurnEffect()); // �b�o��Ĳ�o�U�N�ĪG
        }

    }


    private IEnumerator BurnEffect()//�U�N�ĪG
    {
        isBurning = true;
        for (int i = 0; i < 5; i++) // �U�N 5 ��A�C�� 5 �I��
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
            healthPoint -= finalDamage; //������˶q
            //Debug.Log("FightPlayer1Config.ShieldPercentage" + FightPlayer1Config.ShieldPercentage);
            //Debug.Log("Final Damage 1 :" + finalDamage);
        }
        else if(player == UserPosition.player2)
        {
            finalDamage = Mathf.RoundToInt(damage * (1 - FightPlayer2Config.ShieldPercentage));
            healthPoint -= finalDamage; //������˶q
            //Debug.Log("FightPlayer2Config.ShieldPercentage" + FightPlayer2Config.ShieldPercentage);
            //Debug.Log("Final Damage 2 :" + finalDamage);
        }
        healthBar.SetHealth(healthPoint); // ��s���
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

        // �ھڪ��a�s���p��_�l��m�A�T�O����~�����
        float startX = -((scoreString.Length - 1) * damageSpacing * 0.5f); // �ѥ��ܥk; 

        // ���H
        //if (playerNumber == 2 && GlobalIndex.playerGroupCount == 2)
        //    startX = ((scoreString.Length - 1) * damageSpacing * 0.5f);  // �ѥk�ܥ�

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
                    // �ѥ��ܥk�ƦC
                    adjustedPosition = player1.transform.position +
                                    new Vector3(startX + i * damageSpacing, 0, 0) +
                                    new Vector3(positionOffset.x, positionOffset.y, 0);

                    SetParticleFlip(digitObject, 0, 0); // �]�w Flip.x = 0 Flip.y = 0
                }
                else 
                {
                    
                    // �ѥ��ܥk�ƦC
                    adjustedPosition = player2.transform.position +
                                    new Vector3(startX + i * damageSpacing, 0, 0) +
                                    new Vector3(positionOffset.x, -positionOffset.y, 0);

                    SetParticleFlip(digitObject, 0, 0); // �]�w Flip.x = 0 Flip.y = 0
                }
                    

                // ���H
                //if (playerNumber == 2 && GlobalIndex.playerGroupCount == 2)
                //{
                //    // �ѥk�ܥ��ƦC (�ϥέt�� spacing)
                //    adjustedPosition = gameObject.transform.position +
                //                       new Vector3(startX - i * damageSpacing, 0, 0) +
                //                       new Vector3(positionOffset.x, positionOffset.y, 0);

                //    SetParticleFlip(digitObject, 1, 1); // �]�w Flip.x = 0 Flip.y = 1
                //}

                digitObject.transform.position = adjustedPosition; // �]�w�Ʀr��m

                StartCoroutine(ReturnToPoolAfterDelay(digitObject, digit, 1.0f)); // 1���N�����^�����
            }
        }
    }

    private IEnumerator ReturnToPoolAfterDelay(GameObject obj, int digit, float delay)
    {
        yield return new WaitForSeconds(delay);
        DamageNumberPoolManager.Instance.ReturnToPool(obj, digit);
    }

    // �W�U½��S�ġA�]�wParticleSystem��Render��Flip.y�ݩ�
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
