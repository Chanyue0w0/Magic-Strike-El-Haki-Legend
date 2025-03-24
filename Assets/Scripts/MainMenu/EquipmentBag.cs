using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EquipmentData;

public class EquipmentBag : MonoBehaviour
{
	// 升級所需消耗的金幣、以及每次升級增加的 HP 與 ATK 數值（預設為 100）
	[SerializeField] private int upgradeCost = 1;
	[SerializeField] private int upgradeHPIncrement = 100;
	[SerializeField] private int upgradeATKIncrement = 100;

	[Header("UI Panels and Containers")]
	[SerializeField] private GameObject equipmentInfoPanel;
	[SerializeField] private GameObject equipmentSlotPrefab;
	[SerializeField] private Transform equipmentSlotContainer;

	[Header("-------------------- Equipment Info GUI -------------------- ")]
	[SerializeField] private TextMeshProUGUI equipmentNameText;
	[SerializeField] private Image equipmentImage;
	[SerializeField] private TextMeshProUGUI equipmentDescribe;
	[SerializeField] private TextMeshProUGUI useButtonText;
	[SerializeField] private TextMeshProUGUI HPText;
	[SerializeField] private TextMeshProUGUI ATKText;
	[SerializeField] private TextMeshProUGUI levelText;
	[SerializeField] private TextMeshProUGUI equipmentTypeText;
	[SerializeField] private TextMeshProUGUI buff1Text;
	[SerializeField] private TextMeshProUGUI buff2Text;
	[SerializeField] private TextMeshProUGUI buff3Text;
	[SerializeField] private TextMeshProUGUI buff4Text;
	[SerializeField] private Image equipmentIcon;
	[SerializeField] private Image buff1LockIcon;
	[SerializeField] private Image buff2LockIcon;
	[SerializeField] private Image buff3LockIcon;
	[SerializeField] private Image buff4LockIcon;
	// 顯示升級所需花費與目前玩家金幣資訊
	[SerializeField] private TextMeshProUGUI costCoin;

	[Header("-------------------- Current Hero GUI -------------------- ")]
	[SerializeField] private Image heroImage;
	[SerializeField] private TextMeshProUGUI heroNameText;
	[SerializeField] private Image skill1Icon;
	[SerializeField] private Image skill2Icon;
	[SerializeField] private Image ultimateSkillIcon;
	[SerializeField] private GameObject headEquipmentIcon;
	[SerializeField] private GameObject bodyEquipmentIcon;
	[SerializeField] private GameObject shoesEquipmentIcon;
	[SerializeField] private TextMeshProUGUI totalATKText;
	[SerializeField] private TextMeshProUGUI totalHPText;

	[SerializeField] private Button headEquipmentButton;
	[SerializeField] private Button bodyEquipmentButton;
	[SerializeField] private Button shoesEquipmentButton;

	[Header("Scripts")]
	[SerializeField] private BattleDataCalculator battleDataCalculator;
	[SerializeField] private HeroBag heroBag;

	// 當前選中的裝備與英雄
	private PlayerEquipmentManager.PlayerEquipment currentEquipment;
	private PlayerHeroManager.PlayerHero currentHero;
	private int currentHeroIndex;

	private void Start()
	{
		currentHeroIndex = 0;
		currentHero = PlayerHeroManager.Instance.GetHeroByIndex(0);
		OnClickChangeCurrentHero(1);
		OnClickChangeCurrentHero(-1);
		RefreshCurrentHeroInfoUI();
		RefreshBagUI();
	}

	public void InitEquipmentBag()
	{
		currentHero = PlayerHeroManager.Instance.GetHeroByIndex(currentHeroIndex);

		OnClickChangeCurrentHero(1);
		OnClickChangeCurrentHero(-1);
		RefreshCurrentHeroInfoUI();
		RefreshBagUI();
	}

	private void RefreshBagUI()
	{
		// 清除容器中所有舊的裝備槽
		foreach (Transform child in equipmentSlotContainer)
		{
			Destroy(child.gameObject);
		}

		List<PlayerEquipmentManager.PlayerEquipment> equipmentList = PlayerEquipmentManager.Instance.GetAllEquipmentData();

		// 使用 for 迴圈以捕捉正確的索引值
		for (int i = 0; i < equipmentList.Count; i++)
		{
			var equipment = equipmentList[i];
			GameObject slot = Instantiate(equipmentSlotPrefab, equipmentSlotContainer);
			slot.name = i.ToString();

			// 透過資源路徑載入裝備圖片
			Image slotImage = slot.GetComponent<Image>();
			slotImage.sprite = Resources.Load<Sprite>("Arts/EquipmentImgaes/" + equipment.name);

			// 設定按鈕點擊事件，利用捕捉到的 index 傳入 OnClickOpenEquipmentPanel
			Button btn = slot.GetComponent<Button>();
			btn.onClick.AddListener(() => OnClickOpenEquipmentPanel(equipment.id));

			// 若有子物件 LevelText，則更新其文字內容
			TextMeshProUGUI slotLevelText = slot.transform.Find("LevelText")?.GetComponent<TextMeshProUGUI>();
			if (slotLevelText != null)
			{
				slotLevelText.text = "Lv. " + equipment.currentLevel;
			}
		}
	}

	private void RefreshEquipmentInfoUI()
	{
		if (currentEquipment == null)
			return;

		// 更新裝備基本資訊
		equipmentNameText.text = currentEquipment.name;
		//equipmentDescribe.text = currentEquipment.description;
		HPText.text = currentEquipment.healthPoints.ToString();
		ATKText.text = currentEquipment.attackPower.ToString();
		levelText.text = "Lv. " + currentEquipment.currentLevel + "/30";
		equipmentTypeText.text = currentEquipment.equipmentType;
		equipmentIcon.sprite = Resources.Load<Sprite>("Arts/EquipmentImgaes/" + currentEquipment.name);

		// 根據裝備是否已被英雄裝備更新使用按鈕文字（使用或卸下）
		//if (currentEquipment.equippedByHero != "None")
		//	useButtonText.text = "卸下裝備";
		//else
		//	useButtonText.text = "使用裝備";

		// 顯示裝備的 buff 資訊（依序顯示前四個 buff）
		List<string> buffKeys = new List<string>(currentEquipment.buffs.Keys);
		buff1Text.text = buffKeys.Count > 0 ? buffKeys[0] + ": " + currentEquipment.buffs[buffKeys[0]] : "";
		buff2Text.text = buffKeys.Count > 1 ? buffKeys[1] + ": " + currentEquipment.buffs[buffKeys[1]] : "";
		buff3Text.text = buffKeys.Count > 2 ? buffKeys[2] + ": " + currentEquipment.buffs[buffKeys[2]] : "";
		buff4Text.text = buffKeys.Count > 3 ? buffKeys[3] + ": " + currentEquipment.buffs[buffKeys[3]] : "";

		// 更新 buff 鎖/解鎖圖示：依照裝備稀有度決定可解鎖的 buff 數量
		int unlockedBuffCount = GetUnlockedBuffCount(currentEquipment.rarity);
		for (int i = 0; i < 4; i++)
		{
			Image icon = null;
			switch (i)
			{
				case 0:
					icon = buff1LockIcon;
					break;
				case 1:
					icon = buff2LockIcon;
					break;
				case 2:
					icon = buff3LockIcon;
					break;
				case 3:
					icon = buff4LockIcon;
					break;
			}
			if (icon != null)
			{
				// 若該 buff 編號小於解鎖數量，則顯示解鎖狀態，否則顯示鎖定狀態
				string spritePath = (i < unlockedBuffCount) ? ("Arts/MainScenes/dots/" + (i + 1)) : ("Arts/MainScenes/locks/" + (i + 1));
				icon.sprite = Resources.Load<Sprite>(spritePath);
			}
		}

		// 更新升級所需花費與目前金幣資訊
		int playerCoin = PlayerDataManager.Instance.GetPlayerCoin();
		costCoin.text = $"{upgradeCost} / {playerCoin}";
	}

	private void RefreshCurrentHeroInfoUI()
	{
		if (currentHero == null)
		{
			Debug.LogWarning("尚未選擇當前英雄。");
			return;
		}

		heroNameText.text = currentHero.name;
		heroImage.sprite = Resources.Load<Sprite>("Arts/HeroImages/HeroIllustrations/" + currentHero.id);
		ultimateSkillIcon.sprite = Resources.Load<Sprite>("SkillIcons/Ultimate/" + currentHero.id);
		skill1Icon.sprite = Resources.Load<Sprite>("SkillIcons/Skill1/" + currentHero.id);
		skill2Icon.sprite = Resources.Load<Sprite>("SkillIcons/Skill2/" + currentHero.id);

		// 更新英雄裝備圖示、按鈕功能 (若裝備資料存在則載入圖片，否則設為 null)
		GetEquipmentSpriteData(currentHero.equippedItems, 0, headEquipmentIcon);
		GetEquipmentSpriteData(currentHero.equippedItems, 1, bodyEquipmentIcon);
		GetEquipmentSpriteData(currentHero.equippedItems, 2, shoesEquipmentIcon);


		// 計算並更新英雄的戰鬥數據
		battleDataCalculator.CalculateBattleData(currentHero.id);
		totalATKText.text = battleDataCalculator.totalATK.ToString();
		totalHPText.text = battleDataCalculator.totalHP.ToString();
	}

	public void OnClickOpenEquipmentPanel(string id)
	{
		equipmentInfoPanel.SetActive(true);
		currentEquipment = PlayerEquipmentManager.Instance.GetEquipmentByID(id);
		RefreshEquipmentInfoUI();
	}

	public void OnClickCloseEquipmentPanel()
	{
		equipmentInfoPanel.SetActive(false);
	}

	public void OnClickLevelUp()
	{
		if (currentEquipment == null)
		{
			Debug.LogWarning("沒有選擇要升級的裝備。");
			return;
		}

		// 檢查裝備是否已達最高等級 (30)
		if (currentEquipment.currentLevel >= 30)
		{
			Debug.Log("該裝備已達到最高等級。");
			return;
		}

		// 檢查玩家金幣是否足夠
		int playerCoin = PlayerDataManager.Instance.GetPlayerCoin();
		if (playerCoin < upgradeCost)
		{
			Debug.Log("玩家金幣不足，無法升級裝備。");
			return;
		}

		// 扣除金幣並升級裝備（等級 +1，HP 與 ATK 分別增加預設數值）
		PlayerDataManager.Instance.SetPlayerCoin(playerCoin - upgradeCost);
		currentEquipment.currentLevel += 1;
		currentEquipment.healthPoints += upgradeHPIncrement;
		currentEquipment.attackPower += upgradeATKIncrement;

		// 更新裝備資料與介面
		PlayerEquipmentManager.Instance.UpdateEquipment(currentEquipment);
		RefreshEquipmentInfoUI();
		RefreshBagUI();

		Debug.Log("裝備 " + currentEquipment.name + " 已升級至等級 " + currentEquipment.currentLevel);
	}
	public void OnClickChangeCurrentHero(int next)
	{
		var allHeroData = PlayerHeroManager.Instance.GetAllHeroData();
		if (allHeroData.Count == 0)
		{
			Debug.LogWarning("目前沒有可用的英雄資料。");
			return;
		}

		// 循環切換直到找到擁有的英雄
		do
		{
			currentHeroIndex += next;
			if (currentHeroIndex >= allHeroData.Count)
				currentHeroIndex = 0;
			else if (currentHeroIndex < 0)
				currentHeroIndex = allHeroData.Count - 1;
		} while (!allHeroData[currentHeroIndex].owned);

		currentHero = PlayerHeroManager.Instance.GetHeroByIndex(currentHeroIndex);
		RefreshCurrentHeroInfoUI();
	}
	public void OnClickUseEquipment()
	{
		// Determine equipment type and assign it to the correct slot
		int slotIndex = GetEquipmentSlotIndex(currentEquipment.equipmentType);

		if (slotIndex == -1)
		{
			Debug.LogWarning("Invalid equipment type: " + currentEquipment.equipmentType);
			return;
		}

		// cancel equipment
		if (currentEquipment.equippedByHero != "None")
		{
			CancelUseEquipment(currentEquipment, slotIndex);

			RefreshEquipmentInfoUI();
			RefreshCurrentHeroInfoUI();
			return;
		}

		// use
		// cancle origin eq on hero
		CancelUseEquipment(PlayerEquipmentManager.Instance.GetEquipmentByID(currentHero.equippedItems[slotIndex]), slotIndex);

		currentEquipment.equippedByHero = currentHero.id;
		PlayerEquipmentManager.Instance.UpdateEquipment(currentEquipment);

		currentHero.equippedItems[slotIndex] = currentEquipment.id;
		PlayerHeroManager.Instance.UpdateHero(currentHero);

		Debug.Log("update hero: " + currentHero.name);
		RefreshEquipmentInfoUI();
		RefreshCurrentHeroInfoUI();
	}

	private void CancelUseEquipment(PlayerEquipmentManager.PlayerEquipment eq, int slotIndex)
	{
		if (eq == null || eq.equippedByHero == "None") return;

		PlayerHeroManager.PlayerHero eqHero = PlayerHeroManager.Instance.GetHeroByID(eq.equippedByHero);


		if (slotIndex == -1)
		{
			Debug.LogWarning("Invalid equipment type: " + currentEquipment.equipmentType);
			return;
		}
		eqHero.equippedItems[slotIndex] = "";
		PlayerHeroManager.Instance.UpdateHero(eqHero);
		currentHero = PlayerHeroManager.Instance.GetHeroByIndex(currentHeroIndex);

		eq.equippedByHero = "None";
		PlayerEquipmentManager.Instance.UpdateEquipment(eq);
	}

	private int GetEquipmentSlotIndex(string equipmentType)
	{
		switch (equipmentType)
		{
			case "Head":
				return 0;
			case "Armor":
				return 1;
			case "Shoes":
				return 2;
			default:
				return -1;
		}
	}

	private void GetEquipmentSpriteData(List<string> equippedItems, int slotIndex, GameObject obj)
	{
		obj.GetComponent<Image>().sprite = null;
		obj.GetComponent<Button>().onClick.RemoveAllListeners();
		if (slotIndex >= 0 && slotIndex < equippedItems.Count && !string.IsNullOrEmpty(equippedItems[slotIndex]))
		{
			var equipment = PlayerEquipmentManager.Instance.GetEquipmentByID(equippedItems[slotIndex]);
			if (equipment != null)
			{
				obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Arts/EquipmentImgaes/" + equipment.name);
				obj.GetComponent<Button>().onClick.AddListener(() => OnClickOpenEquipmentPanel(equipment.id));
				return;
			}
		}


		return;
	}

	private int GetUnlockedBuffCount(string rarity)
	{
		switch (rarity)
		{
			case "Common":
				return 1;
			case "稀有":
				return 2;
			case "特殊":
				return 3;
			case "傳說":
				return 4;
			default:
				return 0;
		}
	}

}
