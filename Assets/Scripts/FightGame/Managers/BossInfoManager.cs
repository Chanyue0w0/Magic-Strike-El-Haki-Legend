using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossInfoManager : MonoBehaviour
{

	public static BossInfoManager Instance { get; private set; }
	[Header("----------------- Slime Boss Panel ------------------")]
    [SerializeField] private GameObject slimeBossPanel;
    [SerializeField] private Image slimeBossImage;
	[SerializeField] private Text slimeBossNameText;
	[SerializeField] private Text slimeBossSkillText;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			//DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private Dictionary<string, string> slimeBossNames = new Dictionary<string, string>
{
	{ "OriginSlime", "史萊姆" },
	{ "GrassSlime", "雜草史萊姆" },
	{ "WoodBarrelSlime", "滾筒史萊姆" },
	{ "WindmillSlime", "風車史萊姆" },
	{ "PoisonFlowerSlime", "毒花朵史萊姆" },
	{ "VineSlime", "藤蔓史萊姆" },
	{ "FairySlime", "妖精史萊姆" },
	{ "LavaSlime", "熔岩史萊姆" },
	{ "QuartzSlime", "石英史萊姆" }
};

	private Dictionary<string, string> slimeBossSkills = new Dictionary<string, string>
{
	{ "OriginSlime", "無" },
	{ "GrassSlime", "分裂多個史萊姆\n不讓你進!" },
	{ "WoodBarrelSlime", "分裂多個滾筒\n躲阿!" },
	{ "WindmillSlime", "吹出強風\n這球有風阻!" },
	{ "PoisonFlowerSlime", "噴出毒霧\n咳咳..." },
	{ "VineSlime", "伸長藤蔓\n勾走你心!" },
	{ "FairySlime", "閃現並射出妖火\n很快很快!" },
	{ "LavaSlime", "投擲岩漿\n燒燒燙燙!" },
	{ "QuartzSlime", "照耀直線區域\n閃亮閃亮!" }
};


	public void PlayBossAnimation()
	{
		StartCoroutine(SlimeBossAnimationCoroutine());
	}

	private IEnumerator SlimeBossAnimationCoroutine()
	{
		// 載入圖片資源
		string resourcePath = "Arts/FightScene/SlimeBoss/" + FightPlayer2Config.PlayerSkin + "Boss";
		Debug.Log("嘗試讀取圖片路徑：" + resourcePath);
		Sprite deathSprite = Resources.Load<Sprite>(resourcePath);

		// 顯示面板
		slimeBossPanel.SetActive(true);

		if (deathSprite != null)
		{
			slimeBossImage.sprite = deathSprite;
		}
		else
		{
			Debug.LogWarning("找不到死亡圖片資源：" + resourcePath);
		}

		// 根據 PlayerSkin 決定名稱與技能
		string slimeKey = FightPlayer2Config.PlayerSkin;
		if (slimeBossNames.ContainsKey(slimeKey))
		{
			slimeBossNameText.text = slimeBossNames[slimeKey];
			slimeBossSkillText.text = slimeBossSkills[slimeKey];
		}
		else
		{
			slimeBossNameText.text = "未知史萊姆";
			slimeBossSkillText.text = "未知技能";
		}



		// 等待 1 秒（非受 Time.timeScale 影響）
		yield return new WaitForSecondsRealtime(2f);

		// 關閉面板
		slimeBossPanel.SetActive(false);
	}
}
