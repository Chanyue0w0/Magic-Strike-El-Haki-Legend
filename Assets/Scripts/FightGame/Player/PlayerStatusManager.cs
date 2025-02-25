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
    [SerializeField] private bool isBurning = false; // �O�_���b�U�N

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
            // �b Start �ɹ��է�� PlayerNotification �øj�w�ƥ�
            PlayerNotification notification = player1.GetComponent<PlayerNotification>();
            RegisterPlayerNotification(notification);//�q�\�q��
        }

        if (player == UserPosition.player2)
        {
            skills = FightPlayer2Config.Group;
            SetHP(FightPlayer2Config.StartHP);
            SetATK(FightPlayer2Config.StartATK);
            SetMagicPoint(0);
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
        Debug.Log($"{gameObject.name} ���� �ˮ`");
        playerNotification.OnDamageReceived += HandleDamageNotification;
    }

    // ���� `PlayerNotification` ����������q��
    private void HandleDamageNotification(int damage, GameObject player)
    {
        GetDamage(damage);
    }

    // ���� `PlayerNotification` ������ĪG�q��
    private void HandleStatusEffectApplied(StatusEffect effect, GameObject player)
    {
        if (player == gameObject)
        {
            Debug.Log($"{gameObject.name} Ĳ�o���A�ĪG�G{effect}");

            if (effect == StatusEffect.Burn && !isBurning)
            {
                StartCoroutine(BurnEffect()); // �b�o��Ĳ�o�U�N�ĪG
            }
        }
    }
    private IEnumerator BurnEffect()//�U�N�ĪG
    {
        isBurning = true;
        for (int i = 0; i < 5; i++) // �U�N 5 ��A�C�� 5 �I��
        {
            GetDamage(5);
            yield return new WaitForSeconds(1);
        }
        isBurning = false;
    }

    public void GetDamage(int damage)
    {
        healthPoint -= damage;
        healthBar.SetHealth(healthPoint); // ��s���
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
