using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public class EquipmentBag : MonoBehaviour
{
	public enum EquipmentType { Head, Armor, Shose, All , NULL};

	// 升級所需消耗的金幣、以及每次升級增加的 HP 與 ATK 數值（預設為 100）, 等級上限(30)
	[SerializeField] private const int maxLevel = 30;
	[SerializeField] int requiredEvoStones = 10; // 預設消耗量

	[Header("UI Panels and Containers")]
	[SerializeField] private GameObject SkillBagPanel;
	[SerializeField] private GameObject equipmentInfoPanel;
	[SerializeField] private GameObject evolutionPanel;
	[SerializeField] private GameObject equipmentSlotPrefab;
	[SerializeField] private Transform equipmentSlotContainer;
	[SerializeField] private List<Sprite> useButtonSprite;

	[Header("-------------------- Skill Info GUI -------------------- ")]
	[SerializeField] private Image skillUseButtonImage;
	[Header("-------------------- Equipment Info GUI -------------------- ")]
	[SerializeField] private TextMeshProUGUI equipmentNameText;
	[SerializeField] private List<Image> equipmentImages;
	[SerializeField] private List<TextMeshProUGUI> levelTexts;
	[SerializeField] private TextMeshProUGUI HPText;
	[SerializeField] private TextMeshProUGUI ATKText;
	[SerializeField] private TextMeshProUGUI equipmentTypeText;
	[SerializeField] private List<TextMeshProUGUI> buffTexts;
	[SerializeField] private List<Image> buffLockIcons;
	[SerializeField] private Image useButtonImage;
	[SerializeField] private Image bgRarity;

	[Header("------------------ Level up and Evolution GUI -------------------- ")]
	[SerializeField] private Transform levelUpEffectPoistion;
	[SerializeField] private GameObject levelUpEffectPrefab;
	// 顯示升級所需花費與目前玩家金幣資訊
	[SerializeField] private TextMeshProUGUI costCoin;
	// 新增升級按鈕參考，用來在達上限或金錢不足時更新狀態
	[SerializeField] private Button levelUpButton;
	[SerializeField] private TextMeshProUGUI evolutionCostText;

	[SerializeField] private Image evolutionButtonImage;
	[SerializeField] private Button evolutionButton;
	[SerializeField] private Image evoFrame;
	[SerializeField] private Image evoNextFrame;
	[SerializeField] private Image evoStone;
	[SerializeField] private List<Sprite> evoStoneSprite;

	[Header("-------------------- Current Hero GUI -------------------- ")]
	[SerializeField] private Image heroImage;
	[SerializeField] private TextMeshProUGUI heroNameText;
	[SerializeField] private Image skill1Icon;
	[SerializeField] private Image skill2Icon;
	[SerializeField] private Image ultimateSkillIcon;
	[SerializeField] private GameObject headEquipmentIcon;
	[SerializeField] private GameObject ArmorEquipmentIcon;
	[SerializeField] private GameObject shoesEquipmentIcon;
	[SerializeField] private TextMeshProUGUI totalATKText;
	[SerializeField] private TextMeshProUGUI totalHPText;

	[Header("------------- Scripts -----------------")]
	[SerializeField] private BattleDataCalculator battleDataCalculator;
	[SerializeField] private HeroBag heroBag;
	[SerializeField] private EquipmentLevelData equipmentLevelData;

	// 當前選中的裝備與英雄
	private PlayerEquipmentManager.PlayerEquipment currentEquipment;
	[HideInInspector] public PlayerHeroManager.PlayerHero currentHero;
	private int currentHeroIndex;
	private EquipmentType bagEquipmentType = EquipmentType.All;

	private void Start()
	{
		bagEquipmentType = EquipmentType.All;
		currentHeroIndex = 0;
		currentHero = PlayerHeroManager.Instance.GetHeroByIndex(0);
		OnClickChangeCurrentHero(1);
		OnClickChangeCurrentHero(-1);
		RefreshCurrentHeroInfoUI();
		OnClickSwitchBagType("All");
		RefreshBagUI();

		evolutionPanel.SetActive(false);
		SkillBagPanel.SetActive(false);
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

		for (int i = 0; i < equipmentList.Count; i++)
		{
			// 目前選擇的裝備欄
			var equipment = equipmentList[i];
			EquipmentType type = GetEquipmentType(equipment.equipmentType);
			if (type != bagEquipmentType && bagEquipmentType != EquipmentType.All) continue;


			GameObject slot = Instantiate(equipmentSlotPrefab, equipmentSlotContainer);
			slot.name = i.ToString();

			// 透過資源路徑載入裝備圖片
			Image slotImage = slot.transform.Find("EquipmentImage")?.GetComponent<Image>();
			slotImage.sprite = Resources.Load<Sprite>("Arts/EquipmentImgaes/" + equipment.name);

			// 設定按鈕點擊事件，利用捕捉到的 index 傳入 OnClickOpenEquipmentPanel
			Button btn = slot.GetComponent<Button>();
			btn.onClick.AddListener(() => OnClickOpenEquipmentPanel(equipment.id));

			// 若有子物件 LevelText，則更新其文字內容
			TextMeshProUGUI slotLevelText = slot.transform.Find("LevelText")?.GetComponent<TextMeshProUGUI>();
			if (slotLevelText != null)
			{
				slotLevelText.text = "Lv." + equipment.currentLevel;
			}

			Image frame = slot.transform.Find("FrameImage")?.GetComponent<Image>();
			if (frame != null) frame.sprite = Resources.Load<Sprite>($"Arts/EquipmentImgaes/{equipment.rarity}_{equipment.equipmentType}");
		}
	}

	private void RefreshEquipmentInfoUI()
	{
		if (currentEquipment == null)
			return;

		equipmentNameText.text = GetLocalizedString(equipmentNameText, $"{currentEquipment.typeID}_Name");
		Debug.Log($"{currentEquipment.typeID}_Name");
		HPText.text = currentEquipment.healthPoints.ToString();
		ATKText.text = currentEquipment.attackPower.ToString();
		equipmentTypeText.text = currentEquipment.equipmentType;

		foreach (var image in equipmentImages)
		{
			image.sprite = Resources.Load<Sprite>("Arts/EquipmentImgaes/" + currentEquipment.name);
		}
		foreach (var tmp in levelTexts)
		{
			tmp.text = "Lv. " + currentEquipment.currentLevel + "/" + maxLevel;
		}

		// 使用 buffTexts 來設定 buff 資訊
		List<string> buffKeys = new List<string>(currentEquipment.buffs.Keys);
		for (int i = 0; i < buffTexts.Count; i++)
		{
			buffTexts[i].text = i < buffKeys.Count ? GetLocalizedString(buffTexts[i], buffKeys[i]) + ": " + currentEquipment.buffs[buffKeys[i]] : "";
		}

		// 使用 buffLockIcons 來設定 buff 鎖定狀態
		int unlockedBuffCount = GetEquipmentRarity(currentEquipment.rarity);
		for (int i = 0; i < buffLockIcons.Count; i++)
		{
			string spritePath = (i < unlockedBuffCount) ? ("Arts/MainScenes/dots/" + (i + 1)) : ("Arts/MainScenes/locks/" + (i + 1));
			buffLockIcons[i].sprite = Resources.Load<Sprite>(spritePath);
		}

		useButtonImage.sprite = (currentEquipment.equippedByHero != "None") ? useButtonSprite[0] : useButtonSprite[1];
		bgRarity.sprite = Resources.Load<Sprite>("Arts/MainScenes/EqipmentInfoBackground/" + currentEquipment.rarity);

		// 升級金幣部分：若金錢不足則文字變紅
		int upgradeCost = equipmentLevelData.GetCostMoney(currentEquipment.typeID, currentEquipment.currentLevel);
		int playerCoin = PlayerDataManager.Instance.GetPlayerCoin();
		costCoin.text = $"{upgradeCost} / {playerCoin}";
		if (playerCoin < upgradeCost)
			costCoin.color = Color.red;
		else
			costCoin.color = Color.white;

		// 當裝備等級已達上限，將升級按鈕設為不可點擊且灰階
		if (currentEquipment.currentLevel >= maxLevel)
		{
			levelUpButton.interactable = false;
			levelUpButton.image.color = Color.gray;
		}
		else
		{
			levelUpButton.interactable = true;
			levelUpButton.image.color = Color.white;
		}
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
		//ultimateSkillIcon.sprite = Resources.Load<Sprite>("SkillIcons/Ultimate/" + currentHero.id);
		if (currentHero.equippedItems[3] == "") skill1Icon.color = Color.clear;
		else
		{
			skill1Icon.color = Color.white;
			skill1Icon.sprite = Resources.Load<Sprite>("Arts/MainScenes/SkillImage/" + currentHero.equippedItems[3]);
		}
		if (currentHero.equippedItems[4] == "") skill2Icon.color = Color.clear;
		else
		{
			skill2Icon.color = Color.white;
			skill2Icon.sprite = Resources.Load<Sprite>("Arts/MainScenes/SkillImage/" + currentHero.equippedItems[4]);
		}

		// 更新英雄裝備圖示、按鈕功能 (若裝備資料存在則載入圖片，否則設為 null)
		GetEquipmentSpriteData(currentHero.equippedItems, 0, headEquipmentIcon);
		GetEquipmentSpriteData(currentHero.equippedItems, 1, ArmorEquipmentIcon);
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
		// 特效動畫
		Instantiate(levelUpEffectPrefab, levelUpEffectPoistion);

		// 檢查裝備是否已達最高等級 (maxLevel)
		if (currentEquipment.currentLevel >= maxLevel)
		{
			Debug.Log("該裝備已達到最高等級。");
			// 保險起見也同步更新按鈕狀態
			levelUpButton.interactable = false;
			levelUpButton.image.color = Color.gray;
			return;
		}

		// 檢查玩家金幣是否足夠
		int playerCoin = PlayerDataManager.Instance.GetPlayerCoin();
		int upgradeCost = equipmentLevelData.GetCostMoney(currentEquipment.typeID, currentEquipment.currentLevel);
		if (playerCoin < upgradeCost)
		{
			Debug.Log("玩家金幣不足，無法升級裝備。");
			return;
		}

		// 扣除金幣並升級裝備（等級 +1，HP 與 ATK 分別增加預設數值）
		PlayerDataManager.Instance.SetPlayerCoin(playerCoin - upgradeCost);
		currentEquipment.currentLevel += 1;
		currentEquipment.healthPoints = equipmentLevelData.GetHealthPoints(currentEquipment.typeID, currentEquipment.currentLevel);
		currentEquipment.attackPower += equipmentLevelData.GetAttackPower(currentEquipment.typeID, currentEquipment.currentLevel);

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
		EquipmentType eqType = GetEquipmentType(currentEquipment.equipmentType);

		int slotIndex = -1;

		switch(eqType)
		{
			case EquipmentType.Head:
				slotIndex = 0; 
				break;
			case EquipmentType.Armor:
				slotIndex = 1;
				break;
			case EquipmentType.Shose:
				slotIndex = 2;
				break;
			default:
				slotIndex = -1;
				break;
		}

		if (slotIndex == -1)
		{
			Debug.LogWarning("Invalid equipment type: " + currentEquipment.equipmentType);
			return;
		}

		// cancel equipment
		if (currentEquipment.equippedByHero != "None")
		{
			CancelUseEquipment(currentEquipment, slotIndex);
		}
		else
		{
			// use
			// cancle origin eq on hero
			CancelUseEquipment(PlayerEquipmentManager.Instance.GetEquipmentByID(currentHero.equippedItems[slotIndex]), slotIndex);

			currentEquipment.equippedByHero = currentHero.id;
			PlayerEquipmentManager.Instance.UpdateEquipment(currentEquipment);

			currentHero.equippedItems[slotIndex] = currentEquipment.id;
			PlayerHeroManager.Instance.UpdateHero(currentHero);

			Debug.Log("update hero: " + currentHero.name);
		}

		// update UI
		RefreshEquipmentInfoUI();
		RefreshCurrentHeroInfoUI();
		RefreshBagUI();
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

	private void GetEquipmentSpriteData(List<string> equippedItems, int slotIndex, GameObject obj)
	{
		Image frameImage = obj.GetComponent<Image>();
		Image image = obj.transform.GetChild(0).GetComponent<Image>();
		Button btn = obj.GetComponent<Button>();

		btn.onClick.RemoveAllListeners();
		frameImage.sprite = Resources.Load<Sprite>("Arts/EquipmentImgaes/" + slotIndex.ToString());
		btn.onClick.AddListener(() => GetComponent<MainMenuButtonController>().SoundClick());
		image.sprite = null;
		image.color = Color.clear;


		if (slotIndex >= 0 && slotIndex < equippedItems.Count && !string.IsNullOrEmpty(equippedItems[slotIndex]))
		{
			var equipment = PlayerEquipmentManager.Instance.GetEquipmentByID(equippedItems[slotIndex]);
			if (equipment != null)
			{
				frameImage.sprite = Resources.Load<Sprite>($"Arts/EquipmentImgaes/{equipment.rarity}_{equipment.equipmentType}");
				image.color = Color.white;
				image.sprite = Resources.Load<Sprite>("Arts/EquipmentImgaes/" + equipment.name);
				btn.onClick.AddListener(() => OnClickOpenEquipmentPanel(equipment.id));
				return;
			}
		}
		return;
	}

	public void OnClickEvolutionPanel()
	{
		if (currentEquipment == null)
		{
			Debug.LogWarning("沒有選擇要進化的裝備。");
			return;
		}

		evoFrame.sprite = Resources.Load<Sprite>($"Arts/EquipmentImgaes/{currentEquipment.rarity}_{currentEquipment.equipmentType}");
		int playerEvoStones = 0;
		switch (currentEquipment.rarity)
		{
			case "Normal":
				playerEvoStones = PlayerDataManager.Instance.GetCommonEvoStone();
				evoNextFrame.sprite = Resources.Load<Sprite>($"Arts/EquipmentImgaes/Common_{currentEquipment.equipmentType}");
				evoStone.sprite = evoStoneSprite[0];
				break;
			case "Common":
				playerEvoStones = PlayerDataManager.Instance.GetRareEvoStone();
				evoNextFrame.sprite = Resources.Load<Sprite>($"Arts/EquipmentImgaes/Rare_{currentEquipment.equipmentType}");
				evoStone.sprite = evoStoneSprite[1];
				break;
			case "Rare":
				playerEvoStones = PlayerDataManager.Instance.GetSpecialEvoStone();
				evoNextFrame.sprite = Resources.Load<Sprite>($"Arts/EquipmentImgaes/Special_{currentEquipment.equipmentType}");
				evoStone.sprite = evoStoneSprite[2];
				break;
			case "Special":
				playerEvoStones = PlayerDataManager.Instance.GetLegendaryEvoStone();
				evoNextFrame.sprite = Resources.Load<Sprite>($"Arts/EquipmentImgaes/Legendary_{currentEquipment.equipmentType}");
				evoStone.sprite = evoStoneSprite[3];
				break;
			case "Legendary":
				evoNextFrame.sprite = Resources.Load<Sprite>($"Arts/EquipmentImgaes/Legendary_{currentEquipment.equipmentType}");
				evoStone.sprite = evoStoneSprite[3];
				Debug.Log("該裝備已達最高稀有度，無法進化。");
				return;
			default:
				Debug.LogWarning("裝備稀有度無法進化。");
				return;
		}

		// 啟用介面
		evolutionPanel.SetActive(true);
		// 如果裝備等級未達上限，則禁用進化按鈕並更新提示文字
		if (currentEquipment.currentLevel < maxLevel)
		{
			evolutionButton.interactable = false;
			evolutionButtonImage.color = Color.gray;
			evolutionCostText.text = (currentEquipment.rarity == "Legendary") ? "MAX" : "Not Max LV";
			evolutionCostText.color = Color.red;
			return;
		}

		// 更新 UI 顯示進化石數量，若不足則文字變紅
		string evoText = (currentEquipment.rarity == "Legendary") ? "MAX" : $"{playerEvoStones} / {requiredEvoStones}";
		evolutionCostText.text = evoText;
		evolutionCostText.color = (playerEvoStones < requiredEvoStones) ? Color.red : Color.white;

		// 啟用或禁用進化按鈕
		bool canEvolve = playerEvoStones >= requiredEvoStones && evoText != "MAX" && currentEquipment.currentLevel >= maxLevel;
		evolutionButton.interactable = canEvolve;
		evolutionButtonImage.color = canEvolve ? Color.white : Color.gray;
	}


	public void OnClickEvolution()
	{
		if (currentEquipment == null)
		{
			Debug.LogWarning("沒有選擇要進化的裝備。");
			return;
		}


		int requiredEvoStones = 10; // 預設消耗量
		int playerEvoStones = 0;

		switch (currentEquipment.rarity)
		{
			case "Normal":
				playerEvoStones = PlayerDataManager.Instance.GetCommonEvoStone();
				PlayerDataManager.Instance.SetCommonEvoStone(playerEvoStones - requiredEvoStones);
				currentEquipment.rarity = "Common";
				break;
			case "Common":
				playerEvoStones = PlayerDataManager.Instance.GetRareEvoStone();
				PlayerDataManager.Instance.SetRareEvoStone(playerEvoStones - requiredEvoStones);
				currentEquipment.rarity = "Rare";
				break;
			case "Rare":
				playerEvoStones = PlayerDataManager.Instance.GetSpecialEvoStone();
				PlayerDataManager.Instance.SetSpecialEvoStone(playerEvoStones - requiredEvoStones);
				currentEquipment.rarity = "Special";
				break;
			case "Special":
				playerEvoStones = PlayerDataManager.Instance.GetLegendaryEvoStone();
				PlayerDataManager.Instance.SetLegendaryEvoStone(playerEvoStones - requiredEvoStones);
				currentEquipment.rarity = "Legendary";
				break;
			case "Legendary":
				Debug.Log("該裝備已達最高稀有度，無法進化。");
				evolutionCostText.text = "MAX";
				return;
			default:
				Debug.LogWarning("裝備稀有度無法進化。");
				return;
		}

		// 重置等級
		currentEquipment.currentLevel = 0;
		PlayerEquipmentManager.Instance.UpdateEquipment(currentEquipment);

		Debug.Log("裝備 " + currentEquipment.name + " 已進化至 " + currentEquipment.rarity);
		OnClickEvolutionPanel();

		RefreshBagUI();
		RefreshCurrentHeroInfoUI();
		RefreshEquipmentInfoUI();
	}

	public void OnClickSwitchBagType(string eqType)
	{
		bagEquipmentType = GetEquipmentType(eqType);
		RefreshBagUI();
		return;
	}

	public void OnClickSelectSkill(Image image)
	{
		string skillID = image.sprite.name;

		// on gear (cancle)
		if (currentHero.equippedItems[3] == skillID)
		{
			skillUseButtonImage.sprite = useButtonSprite[1];
			currentHero.equippedItems[3] = "";
		}
		else if (currentHero.equippedItems[4] == skillID)
		{
			skillUseButtonImage.sprite = useButtonSprite[1];
			currentHero.equippedItems[4] = "";
		}
		else if (currentHero.equippedItems[4] == "")
		{
			skillUseButtonImage.sprite = useButtonSprite[0];
			currentHero.equippedItems[4] = skillID;
		}
		else
		{
			skillUseButtonImage.sprite = useButtonSprite[0];
			currentHero.equippedItems[3] = skillID;
		}

		PlayerHeroManager.Instance.UpdateHero(currentHero);

		RefreshCurrentHeroInfoUI();
	}

	public void OnClickCancleSkill(int skillNumber)
	{
		currentHero.equippedItems[skillNumber] = "";

		RefreshCurrentHeroInfoUI();
	}

	private int GetEquipmentRarity(string rarity)
	{
		switch (rarity)
		{
			case "Normal":
				return 0;
			case "Common":
				return 1;
			case "Rare":
				return 2;
			case "Special":
				return 3;
			case "Legendary":
				return 4;
			default:
				return -1;
		}
	}

	private EquipmentType GetEquipmentType(string equipmentType)
	{
		switch (equipmentType)
		{
			case "Head":
				return EquipmentType.Head;
			case "Armor":
				return EquipmentType.Armor;
			case "Shoes":
				return EquipmentType.Shose;
			case "All":
				return EquipmentType.All;
			default:
				return EquipmentType.NULL;
		}
	}

	static string GetLocalizedString(TextMeshProUGUI targetText, string key)
	{
		LocalizeStringEvent localizedEvent = targetText.GetComponent<LocalizeStringEvent>();
		if (localizedEvent == null) return "";
		var loadingResult = LocalizationSettings.StringDatabase.GetTableEntry(localizedEvent.StringReference.TableReference, key);
		targetText.text = loadingResult.Entry.GetLocalizedString();
		return targetText.text;
	}

}
