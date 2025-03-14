using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentBag : MonoBehaviour
{
	[SerializeField] private GameObject equipmentInfoPanel;
	[SerializeField] private GameObject equipmentSlotPrefab;
	[SerializeField] private Transform equipmentSlotContainer;

	[SerializeField] private TextMeshProUGUI equipmentNameText;
	[SerializeField] private Image equipmentImage;
	[SerializeField] private TextMeshProUGUI equipmentDescribe;

	[SerializeField] private TextMeshProUGUI totalATKText;
	[SerializeField] private TextMeshProUGUI totalHPText;

	[SerializeField] private HeroBag heroBag;
	private PlayerEquipmentManager.PlayerEquipment currentEquipment;
	private PlayerHeroManager.PlayerHero currentHero;
	private int currentHeroIndex;

	[SerializeField] private Image heroIcon;
	[SerializeField] private Image heroImage;
	[SerializeField] private TextMeshProUGUI heroNameText;
	[SerializeField] private Image skill1Icon;
	[SerializeField] private Image skill2Icon;
	[SerializeField] private Image ultimateSkillIcon;
	[SerializeField] private Image headEquipmentIcon;
	[SerializeField] private Image bodyEquipmentIcon;
	[SerializeField] private Image shoesEquipmentIcon;

	void Start()
	{
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

	public void OnClickOpenEquipmentPanel(string thisGameObjectName)
	{
		equipmentInfoPanel.SetActive(true);
		currentEquipment = PlayerEquipmentManager.Instance.GetEquipmentByIndex(int.Parse(thisGameObjectName));
		equipmentNameText.text = currentEquipment.name;
		equipmentDescribe.text = currentEquipment.description;
	}

	public void OnClickUseEquipment()
	{
		PlayerEquipmentManager.Instance.SaveEquipment();
	}

	public void OnClickChangeCurrentHero(int next)
	{
		currentHeroIndex += next;
		currentHero = PlayerHeroManager.Instance.GetHeroByIndex(currentHeroIndex);
		RefreshCurrentInfoUI();
	}

	private void RefreshCurrentInfoUI()
	{
		if (currentHero == null)
		{
			Debug.LogWarning("No current hero selected.");
			return;
		}

		heroNameText.text = currentHero.name;
		heroIcon.sprite = Resources.Load<Sprite>("HeroIcons/" + currentHero.id);
		heroImage.sprite = Resources.Load<Sprite>("HeroImages/" + currentHero.id);
		ultimateSkillIcon.sprite = Resources.Load<Sprite>("SkillIcons/Ultimate/" + currentHero.id);
		skill1Icon.sprite = Resources.Load<Sprite>("SkillIcons/Skill1/" + currentHero.id);
		skill2Icon.sprite = Resources.Load<Sprite>("SkillIcons/Skill2/" + currentHero.id);

		headEquipmentIcon.sprite = Resources.Load<Sprite>("EquipmentIcons/" + currentHero.id);
		bodyEquipmentIcon.sprite = Resources.Load<Sprite>("EquipmentIcons/" + currentHero.id);
		shoesEquipmentIcon.sprite = Resources.Load<Sprite>("EquipmentIcons/" + currentHero.id);
	}
}
