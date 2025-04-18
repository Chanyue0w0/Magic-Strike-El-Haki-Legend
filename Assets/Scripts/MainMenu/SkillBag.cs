using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;

public class SkillBag : MonoBehaviour
{
	[SerializeField] private GameObject skillSlotPrefab;
	[SerializeField] private Transform skillSlotContainer;
	[SerializeField] private MainMenuButtonController mainMenuButtonController;
	[SerializeField] private EquipmentBag equipmentBag;

	[Header("Skill info panel")]
	[SerializeField] private GameObject skillInfoPanel;
	[SerializeField] private Image skillIconImage;
	[SerializeField] private TextMeshProUGUI skillNameText;
	[SerializeField] private TextMeshProUGUI skillDescriptionText;

	// 遊戲開始時自動生成技能槽
	private void Start()
	{
		GenerateSkillSlots();
	}

	// 根據玩家擁有的技能動態生成技能槽按鈕
	private void GenerateSkillSlots()
	{
		// 先清除舊有的技能槽按鈕
		foreach (Transform child in skillSlotContainer)
		{
			Destroy(child.gameObject);
		}

		// 從 SkillData 取得所有技能定義
		JObject skillJson = SkillData.Instance.jsonData;

		// 取得玩家目前擁有的所有技能 ID 清單
		List<string> ownedSkills = PlayerSkillManager.Instance.GetAllSkills();

		// 為每個技能生成一個按鈕
		foreach (var skillProp in skillJson)
		{
			string skillID = skillProp.Key;

			// 產生技能槽 UI 並命名
			GameObject skillSlot = Instantiate(skillSlotPrefab, skillSlotContainer);
			skillSlot.name = skillID;

			// 設定技能圖示（若找不到對應圖片則跳過）
			Image icon = skillSlot.GetComponentInChildren<Image>();
			string imagePath = $"Arts/MainScenes/SkillImage/{skillID}";
			Sprite skillSprite = Resources.Load<Sprite>(imagePath);
			if (skillSprite != null)
				icon.sprite = skillSprite;

			// 取得按鈕元件並設定其可否互動
			Button button = skillSlot.GetComponentInChildren<Button>();
			if (ownedSkills.Contains(skillID))
			{
				button.interactable = true;
				icon.color = Color.white;

				// 綁定點擊事件：開啟技能資訊與播放音效
				button.onClick.AddListener(() => OnClickSkill(skillID));
				button.onClick.AddListener(() => mainMenuButtonController.SoundFlipping());
			}
			else
			{
				button.interactable = false;
				icon.color = Color.gray;
			}
		}
	}

	// 玩家點擊某個技能後，顯示技能詳情資訊
	private void OnClickSkill(string skillID)
	{
		// 開啟詳情面板
		skillInfoPanel.SetActive(true);

		// 設定圖示
		string imagePath = $"Arts/MainScenes/SkillImage/{skillID}";
		Sprite skillSprite = Resources.Load<Sprite>(imagePath);
		skillIconImage.sprite = skillSprite;

		// 設定名稱與描述
		skillNameText.text = SkillData.Instance.GetSkillName(skillID);
		skillDescriptionText.text = SkillData.Instance.GetSkillDescribe(skillID);
	}
}
