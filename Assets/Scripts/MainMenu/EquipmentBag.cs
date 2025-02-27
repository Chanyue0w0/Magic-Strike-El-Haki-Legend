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
		if (equipmentInfoPanel != null)
		{
			int x = 0;

			int.TryParse(thisGameObjectName, out x);
			equipmentInfoPanel.SetActive(true);
			PlayerEquipmentManager.PlayerEquipment EquipmentData = PlayerEquipmentManager.Instance.GetEquipmentByIndex(x);
			Debug.Log("Opened Equipment Panel from: " + thisGameObjectName);
		}
		else
		{
			Debug.LogWarning("Equipment Panel is not assigned!");
		}
	}
}
