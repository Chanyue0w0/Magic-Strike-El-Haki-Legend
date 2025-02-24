using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;

public class HeroBag : MonoBehaviour
{
	[SerializeField] private GameObject heroSlotPrefab;
	// 璣动佳甧竟ㄒ ScrollView  Content
	[SerializeField] private Transform heroSlotContainer;

	void Start()
	{
		RefreshUI();
	}

	
	/// <summary>
	/// 穝ネΘ┮Τ璣动 UI 佳
	/// </summary>
	public void RefreshUI()
	{
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

			//// 眔璣动嘿 TextMeshProUGUI じン砞﹚嘿
			//TextMeshProUGUI nameTextMeshProUGUI = slot.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
			//if (nameTextMeshProUGUI != null)
			//{
			//	nameTextMeshProUGUI.text = hero.name;
			//}

			// 眔璣动单 TextMeshProUGUI じン砞﹚单
			TextMeshProUGUI levelText = slot.transform.Find("LevelText")?.GetComponent<TextMeshProUGUI>();
			if (levelText != null)
			{
				levelText.text = "Lv. " + hero.currentLevel;
			}

			//// 眔璣动祡Τ TextMeshProUGUI じン砞﹚祡Τ
			//TextMeshProUGUI rarityTextMeshProUGUI = slot.transform.Find("RarityText")?.GetComponent<TextMeshProUGUI>();
			//if (rarityTextMeshProUGUI != null)
			//{
			//	rarityTextMeshProUGUI.text = hero.rarity;
			//}

			// 璝Τㄤ UI じンㄒ磞瓃璣动瓜ボ单硂柑砞﹚
		}
	}
}
