using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroBag : MonoBehaviour
{
	public string selectedHeroID;

	[Header("Gameobject")]
	[SerializeField] private GameObject heroInfoPanel; // 璣动狾
	[SerializeField] private GameObject heroUpgradePanel; // 璣动狾

	[SerializeField] private GameObject heroSlotPrefab;
	[SerializeField] private GameObject levelUpButtonObject;
	[Header("UI")]
	[SerializeField] private Image advanturePanelHeroImage;
	[SerializeField] private TextMeshProUGUI[] heroNameTexts;
	[SerializeField] private Image[] heroImages;
	[SerializeField] private TextMeshProUGUI heroATKText;
	[SerializeField] private TextMeshProUGUI heroHPText;

	[Header("Other")]
	// 璣动佳甧竟ㄒ ScrollView  Content
	[SerializeField] private Transform heroSlotContainer;
	
	private PlayerHeroManager.PlayerHero currentHero;

	void Start()
	{
		currentHero = PlayerHeroManager.Instance.GetHeroByID("HR00");
		Debug.Log("current hero:" + currentHero.id);
		selectedHeroID = currentHero.id;
		OnClickSelectHero();
	}

	
	/// <summary>
	/// 穝ネΘ┮Τ璣动 UI 佳
	/// </summary>
	public void RefreshBagUI()
	{
		SwitchCurrentHero("HR00");
		// 睲埃甧竟い侣兜ヘ
		foreach (Transform child in heroSlotContainer)
		{
			Destroy(child.gameObject);
		}

		// 眖 PlayerHeroManager 眔璣动计沮
		List<PlayerHeroManager.PlayerHero> heroList = PlayerHeroManager.Instance.GetAllHeroData();

		// –璣动ネΘ UI 佳
		foreach (var hero in heroList)
		{
			GameObject slot = Instantiate(heroSlotPrefab, heroSlotContainer);

			slot.name = hero.id;

			// 秙 OnClick 
			Button btn = slot.GetComponent<Button>();
			btn.onClick.AddListener(() => SwitchCurrentHero(btn.gameObject.name));

			// 眔璣动单 TextMeshProUGUI じン砞﹚单
			TextMeshProUGUI levelText = slot.transform.Find("LevelText")?.GetComponent<TextMeshProUGUI>();
			if (levelText != null)
			{
				levelText.text = "Lv. " + hero.currentLevel;
			}


			// 璝Τㄤ UI じンㄒ磞瓃璣动瓜ボ单硂柑砞﹚

			Image image = slot.GetComponent<Image>();
			image.sprite = Resources.Load<Sprite>("Arts/FightScene/Field/FieldObjects/" + hero.id);
			if (!hero.owned)
			{
				image.sprite = Resources.Load<Sprite>("Arts/FightScene/Field/FieldObjects/HR00");
				image.color = new Vector4(0, 0, 0, 0.8f);
				btn.interactable = false;
			}
		}
	}

	public void OnClickOpenHeroInfoPanel()
	{
		heroInfoPanel.SetActive(true);
		PlayerHeroManager.PlayerHero heroData = PlayerHeroManager.Instance.GetHeroByID(currentHero.id);
		Debug.Log("Opened Hero Panel from: " + currentHero.id);
	}

	public void OnClickOpenHeroUpgradePanel()
	{
		heroUpgradePanel.SetActive(true);
		PlayerHeroManager.PlayerHero heroData = PlayerHeroManager.Instance.GetHeroByID(currentHero.id);

		heroATKText.text = currentHero.baseATK.ToString();
		heroHPText.text = currentHero.baseHP.ToString();
		Debug.Log("Opened Hero Panel from: " + currentHero.id);	
	}

	public void OnClickLevelUp()
	{
		//if(canLevelUp)
		//{
		//	currentHero.currentLevel += 1;
		//	PlayerHeroManager.Instance.SaveHeroes();
		//}
	}
	public void SwitchCurrentHero(string heroID)
	{
		currentHero = PlayerHeroManager.Instance.GetHeroByID(heroID);

        if (currentHero.owned)
        {
			levelUpButtonObject.GetComponent<Image>().color = new Vector4(255, 255, 255, 255);
			levelUpButtonObject.GetComponent<Button>().interactable = true;
		}
		else
		{
			levelUpButtonObject.GetComponent<Image>().color = new Vector4(80, 80, 80, 255);
			levelUpButtonObject.GetComponent<Button>().interactable = false;
		}

		foreach (var tmp in heroNameTexts)
		{
			tmp.text = currentHero.name;
		}
		foreach (var image in heroImages)
		{
			image.sprite = Resources.Load<Sprite>("Arts/HeroImages/HeroIllustrations/" + currentHero.id);
		}
	}

	public void OnClickSelectHero()
	{
		selectedHeroID = currentHero.id;
		advanturePanelHeroImage.sprite = Resources.Load<Sprite>("Arts/HeroImages/HeroIllustrations/" + currentHero.id);
		Debug.Log(advanturePanelHeroImage.sprite);


		GetComponent<BattleDataCalculator>().CalculateBattleData(selectedHeroID);
		GetComponent<BattleDataCalculator>().ApplyToFightPlayerConfig();

		RefreshBagUI();
		GetComponent<EquipmentBag>().InitEquipmentBag();
	}

}
