using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentBag : MonoBehaviour
{
	[SerializeField] private GameObject equipmentInfoPanel;
	// 裝備槽預製
	[SerializeField] private GameObject equipmentSlotPrefab;
	// 裝備槽容器（例如 ScrollView 的 Content）
	[SerializeField] private Transform equipmentSlotContainer;


	[SerializeField] private TextMeshProUGUI equipmentNameText;
	[SerializeField] private Image equipmentImage;
	
	private PlayerHeroManager.PlayerHero currentHeroData;
	private PlayerEquipmentManager.PlayerEquipment currentEquipment;

	void Start()
	{
		RefreshUI();
	}

	/// <summary>
	/// 重新生成所有裝備的 UI 槽
	/// </summary>
	public void RefreshUI()
	{
		// 清除容器中的舊項目
		foreach (Transform child in equipmentSlotContainer)
		{
			Destroy(child.gameObject);
		}

		// 從 PlayerEquipmentManager 獲取裝備列表
		List<PlayerEquipmentManager.PlayerEquipment> equipmentList = PlayerEquipmentManager.Instance.GetAllEquipmentData();

		// 為每個裝備生成一個 UI 槽
		int index = 0;
		foreach (var equipment in equipmentList)
		{
			GameObject slot = Instantiate(equipmentSlotPrefab, equipmentSlotContainer);
			slot.name = index.ToString();
			//// 取得預製中的名稱 Text 元件，並設定裝備名稱
			//Text nameText = slot.transform.Find("NameText")?.GetComponent<Text>();
			//if (nameText != null)
			//{
			//	nameText.text = equipment.name;
			//}

			// 按鈕 OnClick 指令
			Button btn = slot.GetComponent<Button>();
			btn.onClick.AddListener(() => OnClickOpenEquipmentPanel(btn.gameObject.name));

			// 取得預製中的等級 Text 元件，並設定裝備等級
			TextMeshProUGUI levelText = slot.transform.Find("LevelText")?.GetComponent<TextMeshProUGUI>();
			if (levelText != null)
			{
				levelText.text = "Lv. " + equipment.currentLevel;
			}

			// 若有其他 UI 元件，例如顯示攻擊力、描述等，也可依此設定

			index++;
		}
	}

	public void OnClickOpenEquipmentPanel(string thisGameObjectName)
	{
		equipmentInfoPanel.SetActive(true);
		currentEquipment = PlayerEquipmentManager.Instance.GetEquipmentByIndex(int.Parse(thisGameObjectName));
		equipmentNameText.text = currentEquipment.name;
		//equipmentImage.sprite = currentEquipment
	}

	public void OnClickUseEquipment()
	{
		currentEquipment.equippedByHero = currentHeroData.name;
		currentHeroData.equippedItems[0] = currentEquipment.id;

		PlayerHeroManager.Instance.SaveHeroes();
		PlayerEquipmentManager.Instance.SaveEquipment();
	}
}
