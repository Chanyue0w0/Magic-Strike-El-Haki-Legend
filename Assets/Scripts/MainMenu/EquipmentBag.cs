using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentBag : MonoBehaviour
{
	[SerializeField] private GameObject equipmentInfoPanel;
	// �˳Ƽѹw�s
	[SerializeField] private GameObject equipmentSlotPrefab;
	// �˳ƼѮe���]�Ҧp ScrollView �� Content�^
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
	/// ���s�ͦ��Ҧ��˳ƪ� UI ��
	/// </summary>
	public void RefreshUI()
	{
		// �M���e�������¶���
		foreach (Transform child in equipmentSlotContainer)
		{
			Destroy(child.gameObject);
		}

		// �q PlayerEquipmentManager ����˳ƦC��
		List<PlayerEquipmentManager.PlayerEquipment> equipmentList = PlayerEquipmentManager.Instance.GetAllEquipmentData();

		// ���C�Ӹ˳ƥͦ��@�� UI ��
		int index = 0;
		foreach (var equipment in equipmentList)
		{
			GameObject slot = Instantiate(equipmentSlotPrefab, equipmentSlotContainer);
			slot.name = index.ToString();
			//// ���o�w�s�����W�� Text ����A�ó]�w�˳ƦW��
			//Text nameText = slot.transform.Find("NameText")?.GetComponent<Text>();
			//if (nameText != null)
			//{
			//	nameText.text = equipment.name;
			//}

			// ���s OnClick ���O
			Button btn = slot.GetComponent<Button>();
			btn.onClick.AddListener(() => OnClickOpenEquipmentPanel(btn.gameObject.name));

			// ���o�w�s�������� Text ����A�ó]�w�˳Ƶ���
			TextMeshProUGUI levelText = slot.transform.Find("LevelText")?.GetComponent<TextMeshProUGUI>();
			if (levelText != null)
			{
				levelText.text = "Lv. " + equipment.currentLevel;
			}

			// �Y����L UI ����A�Ҧp��ܧ����O�B�y�z���A�]�i�̦��]�w

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
