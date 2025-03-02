using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static HeroData;

public class HeroBag : MonoBehaviour
{
	[SerializeField] private GameObject heroInfoPanel; // �^�����O
	[SerializeField] private GameObject heroUpgradePanel; // �^�����O

	[SerializeField] private GameObject heroSlotPrefab;
	// �^���Ѯe���]�Ҧp ScrollView �� Content�^
	[SerializeField] private Transform heroSlotContainer;

	[SerializeField] private TextMeshProUGUI[] heroNameTexts;
	[SerializeField] private Image[] heroImages;

	private PlayerHeroManager.PlayerHero currentHero;

	void Start()
	{
		RefreshUI();
	}

	
	/// <summary>
	/// ���s�ͦ��Ҧ��^���� UI ��
	/// </summary>
	public void RefreshUI()
	{
		SwitchCurrentHero("HR00");
		// �M���e�������¶���
		foreach (Transform child in heroSlotContainer)
		{
			Destroy(child.gameObject);
		}

		// �q PlayerHeroManager ���o�^���ƾڦC��
		List<PlayerHeroManager.PlayerHero> heroList = PlayerHeroManager.Instance.GetAllHeroData();

		// ���C�ӭ^���ͦ��@�� UI ��
		foreach (var hero in heroList)
		{
			GameObject slot = Instantiate(heroSlotPrefab, heroSlotContainer);

			slot.name = hero.id;
			//// ���o�^���W�٪� TextMeshProUGUI ����ó]�w�W��
			//TextMeshProUGUI nameTextMeshProUGUI = slot.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
			//if (nameTextMeshProUGUI != null)
			//{
			//	nameTextMeshProUGUI.text = hero.name;
			//}


			// ���s OnClick ���O
			Button btn = slot.GetComponent<Button>();
			btn.onClick.AddListener(() => SwitchCurrentHero(btn.gameObject.name));

			// ���o�^�����Ū� TextMeshProUGUI ����ó]�w����
			TextMeshProUGUI levelText = slot.transform.Find("LevelText")?.GetComponent<TextMeshProUGUI>();
			if (levelText != null)
			{
				levelText.text = "Lv. " + hero.currentLevel;
			}

			//// ���o�^���}���ת� TextMeshProUGUI ����ó]�w�}����
			//TextMeshProUGUI rarityTextMeshProUGUI = slot.transform.Find("RarityText")?.GetComponent<TextMeshProUGUI>();
			//if (rarityTextMeshProUGUI != null)
			//{
			//	rarityTextMeshProUGUI.text = hero.rarity;
			//}

			// �Y����L UI ����A�Ҧp�y�z�B�^���ϥܵ��A�]�i�H�b�o�̳]�w
		}
	}

	public void OnClickOpenHeroInfoPanel()
	{
		if (heroInfoPanel != null)
		{
			heroInfoPanel.SetActive(true);
			PlayerHeroManager.PlayerHero heroData = PlayerHeroManager.Instance.GetHeroByID(currentHero.id);
			Debug.Log("Opened Hero Panel from: " + currentHero.id);
		}
		else
		{
			Debug.LogWarning("Hero Panel is not assigned!");
		}
	}

	public void OnClickOpenHeroUpgradePanel()
	{
		if (heroUpgradePanel != null)
		{
			heroUpgradePanel.SetActive(true);
			PlayerHeroManager.PlayerHero heroData = PlayerHeroManager.Instance.GetHeroByID(currentHero.id);
			Debug.Log("Opened Hero Panel from: " + currentHero.id);
		}
		else
		{
			Debug.LogWarning("Hero Panel is not assigned!");
		}
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
		//ChangeHeroNames(heroID);
		foreach (var tmp in heroNameTexts)
		{
			tmp.text = currentHero.name;
		}
		//ChangeHeroImages(heroID);
		//foreach (var image in heroImages)
		//{
		//	//image.sprite = PlayerHeroManager.Instance.GetHeroByID(heroID).name;
		//}
	}
}
