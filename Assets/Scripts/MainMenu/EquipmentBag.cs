using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentBag : MonoBehaviour
{

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
	[SerializeField] private Image buff3LockIcon;
	[SerializeField] private Image buff4LockIcon;

	[Header("-------------------- Current Hero GUI -------------------- ")]
	[SerializeField] private Image heroImage;
	[SerializeField] private TextMeshProUGUI heroNameText;
	[SerializeField] private Image skill1Icon;
	[SerializeField] private Image skill2Icon;
	[SerializeField] private Image ultimateSkillIcon;
	[SerializeField] private Image headEquipmentIcon;
	[SerializeField] private Image bodyEquipmentIcon;
	[SerializeField] private Image shoesEquipmentIcon;
	[SerializeField] private TextMeshProUGUI totalATKText;
	[SerializeField] private TextMeshProUGUI totalHPText;


	[Header("Script")]
	[SerializeField] private HeroBag heroBag;
	private PlayerEquipmentManager.PlayerEquipment currentEquipment;
	private PlayerHeroManager.PlayerHero currentHero;
	private int currentHeroIndex;


	void Start()
	{
		currentHeroIndex = 0;
		currentHero = PlayerHeroManager.Instance.GetHeroByIndex(0);
		RefreshBagUI();
	}

	public void RefreshBagUI()
	{
		foreach (Transform child in equipmentSlotContainer)
		{
			Destroy(child.gameObject);
		}

		List<PlayerEquipmentManager.PlayerEquipment> equipmentList = PlayerEquipmentManager.Instance.GetAllEquipmentData();

		int index = 0;
		foreach (var equipment in equipmentList)
		{
			GameObject slot = Instantiate(equipmentSlotPrefab, equipmentSlotContainer);
			slot.name = index.ToString();
			Button btn = slot.GetComponent<Button>();
			btn.onClick.AddListener(() => OnClickOpenEquipmentPanel(btn.gameObject.name));

			TextMeshProUGUI levelText = slot.transform.Find("LevelText")?.GetComponent<TextMeshProUGUI>();
			if (levelText != null)
			{
				levelText.text = "Lv. " + equipment.currentLevel;
			}
			index++;
		}
	}

	//public void OnClickOpenEquipmentPanel(string thisGameObjectName)
	//{
	//	equipmentInfoPanel.SetActive(true);
	//	currentEquipment = PlayerEquipmentManager.Instance.GetEquipmentByIndex(int.Parse(thisGameObjectName));
	//	equipmentNameText.text = currentEquipment.name;
	//	equipmentDescribe.text = currentEquipment.description;

	//	RefreshCurrentInfoUI();
	//}

	public void OnClickOpenEquipmentPanel(string thisGameObjectName)
	{
		equipmentInfoPanel.SetActive(true);
		currentEquipment = PlayerEquipmentManager.Instance.GetEquipmentByIndex(int.Parse(thisGameObjectName));
		
		RefreshEquipmentInfo();
	}

	public void OnClickUseEquipment()
	{
		// cancel equip
		if (currentEquipment.equippedByHero != "None")
		{
			CancelUseEquipment(currentEquipment);
			return;
		}


		// Determine equipment type and assign it to the correct slot
		int slotIndex = -1;
		switch (currentEquipment.equipmentType)
		{
			case "Head":
				slotIndex = 0;
				break;
			case "Armor":
				slotIndex = 1;
				break;
			case "Shoes":
				slotIndex = 2;
				break;
		}

		if (slotIndex == -1)
		{
			Debug.LogWarning("Invalid equipment type: " + currentEquipment.equipmentType);
			return;
		}
		// use
		CancelUseEquipment(PlayerEquipmentManager.Instance.GetEquipmentByID(currentHero.equippedItems[slotIndex]));
		currentEquipment.equippedByHero = currentHero.id;
		PlayerEquipmentManager.Instance.UpdateEquipment(currentEquipment);

		currentHero.equippedItems[slotIndex] = currentEquipment.id;
		PlayerHeroManager.Instance.UpdateHero(currentHero);
		Debug.Log("update hero: " + currentHero.name);
		RefreshEquipmentInfo();
		RefreshCurrentHeroInfo();
	}

	private void CancelUseEquipment(PlayerEquipmentManager.PlayerEquipment eq)
	{
		if (eq == null || eq.equippedByHero == "None") return;

		PlayerHeroManager.PlayerHero eqHero = PlayerHeroManager.Instance.GetHeroByID(eq.equippedByHero);

		// Determine equipment type and assign it to the correct slot
		int slotIndex = -1;
		switch (eq.equipmentType)
		{
			case "Head":
				slotIndex = 0;
				break;
			case "Armor":
				slotIndex = 1;
				break;
			case "Shoes":
				slotIndex = 2;
				break;
		}

		if (slotIndex == -1)
		{
			Debug.LogWarning("Invalid equipment type: " + currentEquipment.equipmentType);
			return;
		}
		eqHero.equippedItems[slotIndex] = "";
		PlayerHeroManager.Instance.UpdateHero(eqHero);


		eq.equippedByHero = "None";
		PlayerEquipmentManager.Instance.UpdateEquipment(eq);

		RefreshEquipmentInfo();
		RefreshCurrentHeroInfo();
	}

	public void OnClickChangeCurrentHero(int next)
	{
		var allHeroData = PlayerHeroManager.Instance.GetAllHeroData();
		do
		{
			currentHeroIndex += next;
			if (currentHeroIndex >= allHeroData.Count) currentHeroIndex = 0;
			else if (currentHeroIndex < 0) currentHeroIndex = allHeroData.Count - 1;
		} while (!allHeroData[currentHeroIndex].owned);

		currentHero = PlayerHeroManager.Instance.GetHeroByIndex(currentHeroIndex);

		RefreshCurrentHeroInfo();
	}

	public void RefreshCurrentHeroInfo()
	{
		if (currentHero == null)
		{
			Debug.LogWarning("No current hero selected.");
			return;
		}

		heroNameText.text = currentHero.name;
		heroImage.sprite = Resources.Load<Sprite>("HeroImages/" + currentHero.id);
		ultimateSkillIcon.sprite = Resources.Load<Sprite>("SkillIcons/Ultimate/" + currentHero.id);
		skill1Icon.sprite = Resources.Load<Sprite>("SkillIcons/Skill1/" + currentHero.id);
		skill2Icon.sprite = Resources.Load<Sprite>("SkillIcons/Skill2/" + currentHero.id);

		headEquipmentIcon.sprite = Resources.Load<Sprite>("EquipmentIcons/" + currentHero.id);
		bodyEquipmentIcon.sprite = Resources.Load<Sprite>("EquipmentIcons/" + currentHero.id);
		shoesEquipmentIcon.sprite = Resources.Load<Sprite>("EquipmentIcons/" + currentHero.id);
	}

	public void RefreshEquipmentInfo()
	{
		if (currentEquipment == null) return;

		equipmentNameText.text = currentEquipment.name;
		equipmentDescribe.text = currentEquipment.description;
		HPText.text = "HP: " + currentEquipment.healthPoints;
		ATKText.text = "ATK: " + currentEquipment.attackPower;
		levelText.text = "Lv. " + currentEquipment.currentLevel + "/30";
		equipmentTypeText.text = currentEquipment.equipmentType;
		equipmentIcon.sprite = Resources.Load<Sprite>("EquipmentIcons/" + currentEquipment.id);

		//List<string> buffKeys = new List<string>(currentEquipment.buffs.Keys);
		//buff1Text.text = buffKeys.Count > 0 ? buffKeys[0] + ": " + currentEquipment.buffs[buffKeys[0]] : "";
		//buff2Text.text = buffKeys.Count > 1 ? buffKeys[1] + ": " + currentEquipment.buffs[buffKeys[1]] : "";
		//buff3Text.text = buffKeys.Count > 2 ? buffKeys[2] + ": " + currentEquipment.buffs[buffKeys[2]] : "";
		//buff4Text.text = buffKeys.Count > 3 ? buffKeys[3] + ": " + currentEquipment.buffs[buffKeys[3]] : "";
	}


}
