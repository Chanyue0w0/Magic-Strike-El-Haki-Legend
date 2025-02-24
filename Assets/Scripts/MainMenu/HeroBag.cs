using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;

public class HeroBag : MonoBehaviour
{
	[SerializeField] private GameObject heroSlotPrefab;
	// �^���Ѯe���]�Ҧp ScrollView �� Content�^
	[SerializeField] private Transform heroSlotContainer;

	void Start()
	{
		RefreshUI();
	}

	
	/// <summary>
	/// ���s�ͦ��Ҧ��^���� UI ��
	/// </summary>
	public void RefreshUI()
	{
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

			//// ���o�^���W�٪� TextMeshProUGUI ����ó]�w�W��
			//TextMeshProUGUI nameTextMeshProUGUI = slot.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
			//if (nameTextMeshProUGUI != null)
			//{
			//	nameTextMeshProUGUI.text = hero.name;
			//}

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
}
