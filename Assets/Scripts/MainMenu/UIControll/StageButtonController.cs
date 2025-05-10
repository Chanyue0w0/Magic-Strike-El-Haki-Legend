using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public class StageButtonController : MonoBehaviour
{
	[SerializeField] private GameObject stageButtonPrefab;
	[SerializeField] private Transform stageButtonContainer;
	[SerializeField] private GameObject stagePanel;
	[SerializeField] private TextMeshProUGUI chapterText;
	[SerializeField] private Image chapterBackgroundImage;

	[SerializeField] MainMenuButtonController mainMenuButtonController;

	private int selectedChapter;
	private int selectedLevel;

	private GameObject preFrame;

	private void Start()
	{
		GenerateStageButtons();
	}

	public void GenerateStageButtons()
	{
		var (totalChapters, levelsPerChapter, _) = StageData.Instance.GetStageCounts();

		int currentChapter = PlayerDataManager.Instance.GetCurrentChapter();
		selectedChapter = currentChapter;
		int currentLevel = PlayerDataManager.Instance.GetCurrentLevel();
		selectedLevel = currentLevel;
		// 如果 chapter 或 level 小於1，則預設為1
		if (currentChapter < 1) currentChapter = 1;
		if (currentLevel < 1) currentLevel = 1;
		SelectStage($"Chapter_{currentChapter}_Level_{currentLevel}");


		foreach (Transform child in stageButtonContainer)
		{
			Destroy(child.gameObject);
		}

		// 隱形按鈕，調整Contain大小用
		//CreatTempButton();
		//CreatTempButton();

		// 倒著加入
		for (int chapter = totalChapters; chapter >= 1; chapter--)
		{
			if (!levelsPerChapter.ContainsKey(chapter))
				continue;

			for (int level = levelsPerChapter[chapter]; level >= 1; level--)
			{
				GameObject newButton = Instantiate(stageButtonPrefab, stageButtonContainer);
				string chapterLevelName = $"Chapter_{chapter}_Level_{level}";
				newButton.name = chapterLevelName;

				bool isPassed = false;
				if (currentChapter > chapter) isPassed = true;
				else if (currentChapter == chapter && currentLevel > level) isPassed = true;
				bool isUnLocked = (currentLevel == level) && (currentChapter == chapter);

				// 設定外框
				newButton.transform.Find("frame Image").gameObject.SetActive(false);
				// **設定背景圖片**
				Image bgImage = newButton.transform.Find("bg Image")?.GetComponent<Image>();
				if (bgImage != null)
				{
					string bgImagePath = $"Arts/MainScenes/StagePanel/StageImage/ch{chapter}-{level}";
					bgImage.sprite = Resources.Load<Sprite>(bgImagePath);
				}

				// **設定狀態圖片**
				Button buttonComponent = newButton.GetComponent<Button>();
				Image statusImage = newButton.transform.Find("status Image")?.GetComponent<Image>();
				if (statusImage != null)
				{
					string statusImagePath;
					if (isPassed)
					{
						buttonComponent.onClick.AddListener(() => SelectStage(chapterLevelName));
						buttonComponent.onClick.AddListener(() => mainMenuButtonController.SoundClick());
						statusImagePath = "Arts/MainScenes/StagePanel/StageStatusIcon/PassedStage";
					}
					else if (isUnLocked)
					{
						buttonComponent.onClick.AddListener(() => SelectStage(chapterLevelName));
						buttonComponent.onClick.AddListener(() => mainMenuButtonController.SoundClick());
						statusImagePath = "Arts/MainScenes/StagePanel/StageStatusIcon/UnPassedStage";
					}
					else
					{
						bgImage.color = Color.gray;
						buttonComponent.onClick.AddListener(() => mainMenuButtonController.SoundClick());
						statusImagePath = "Arts/MainScenes/StagePanel/StageStatusIcon/LockStage";
					}

					statusImage.sprite = Resources.Load<Sprite>(statusImagePath);
				}

			}
		}

		// 隱形按鈕，調整Contain大小用
		//CreatTempButton();
		//CreatTempButton();

	}


	//private void CreatTempButton()
	//{
	//	GameObject tempButton = Instantiate(stageButtonPrefab, stageButtonContainer);
	//	tempButton.GetComponent<Image>().enabled = false;

	//	foreach (Transform child in tempButton.transform)
	//	{
	//		Destroy(child.gameObject);
	//	}
	//}
	public void SelectStage(string chapterLevelName)
	{
		Match match = Regex.Match(chapterLevelName, @"Chapter_(\d+)_Level_(\d+)");
		if (!match.Success)
		{
			Debug.LogError("關卡名稱格式不正確: " + chapterLevelName);
			return;
		}

		// 將匹配到的章節與關卡字串轉換成整數
		if (!int.TryParse(match.Groups[1].Value, out selectedChapter))
		{
			Debug.LogError("無法解析章節數: " + match.Groups[1].Value);
			return;
		}
		if (!int.TryParse(match.Groups[2].Value, out selectedLevel))
		{
			Debug.LogError("無法解析關卡數: " + match.Groups[2].Value);
			return;
		}

		// 更新介面文字顯示
		chapterText.text = $"{selectedChapter}-{selectedLevel} " + GetLocalizedText(chapterText, $"{selectedChapter}-{selectedLevel}");
		chapterBackgroundImage.sprite = Resources.Load<Sprite>($"Arts/MainScenes/BackgroundImage/Chapter{selectedChapter}BackGround");
		if (chapterBackgroundImage.sprite == null)
			chapterBackgroundImage.sprite = Resources.Load<Sprite>("Arts/MainScenes/BackgroundImage/Chapter1BackGround");
		ApplySelectedStageToConfig();
		stagePanel.SetActive(false);
	}


	public void ApplySelectedStageToConfig()
	{
		FightPlayer1Config.CurrentChapter = selectedChapter;
		FightPlayer1Config.CurrentLevel = selectedLevel;
		FightPlayer1Config.isFirstTimeEnter = true;
	}

	// 放在 StageButtonController.cs
	public void OnClickFixCenterStage()
	{
		// 1. 先找出目前進度的關卡物件 --------------------------
		int cha = selectedChapter;
		int lv = selectedLevel;

		Transform target = stageButtonContainer.Find($"Chapter_{cha}_Level_{lv}");
		if (target == null)
		{
			Debug.LogWarning($"OnClickFixCenterStage : 找不到 Chapter_{cha}_Level_{lv}。");
			return;
		}

		// 更新鎖定框
		preFrame?.SetActive(false);
		preFrame = target.transform.Find("frame Image")?.gameObject;
		preFrame.SetActive(true);

		// 2. 抓到 ScrollRect（通常跟 viewport 同一層） ----------
		ScrollRect sr = stageButtonContainer.GetComponentInParent<ScrollRect>();
		if (sr == null)
		{
			Debug.Log("OnClickFixCenterStage 需要 ScrollRect，但在父物件中找不到。");
			return;
		}

		// 3. 確保 Layout 已更新，避免尺寸還沒算完 ---------------
		Canvas.ForceUpdateCanvases();

		RectTransform content = sr.content;    // = stageButtonContainer
		RectTransform viewport = sr.viewport;   // Inspector 指到的 Viewport
		RectTransform trgRT = target.GetComponent<RectTransform>();

		// ─────  垂直方向 (verticalNormalizedPosition)  ─────
		float contentH = content.rect.height;
		float viewH = viewport.rect.height;

		// anchoredPosition.y < 0 代表距離頂端的正值
		float distTop = -trgRT.anchoredPosition.y + trgRT.rect.height * (1f - trgRT.pivot.y);
		float wantedTop = distTop - viewH * 0.5f;                        // 讓按鈕中心 = 視窗中心
		float vNorm = 1f;                                                // 預設頂端
		if (contentH > viewH)
			vNorm = 1f - Mathf.Clamp01(wantedTop / (contentH - viewH));  // 轉換成 0~1

		// ─────  水平方向 (horizontalNormalizedPosition)  ─────
		float contentW = content.rect.width;
		float viewW = viewport.rect.width;

		float distLeft = trgRT.anchoredPosition.x + trgRT.rect.width * trgRT.pivot.x + trgRT.rect.width * 0.5f;
		float wantedLeft = distLeft - viewW * 0.5f;
		float hNorm = 0f;
		if (contentW > viewW)
			hNorm = Mathf.Clamp01(wantedLeft / (contentW - viewW));

		// 4. 套用 -------------------------------------------------
		sr.normalizedPosition = new Vector2(hNorm, vNorm);
	}

	public void OnClickStagePanel()
	{
		stagePanel.SetActive(true);

		Animator animator = stagePanel.GetComponent<Animator>();
		Debug.Log(animator.GetBool("Entry"));
		animator.SetBool("Entry", !animator.GetBool("Entry"));
	}

	private string GetLocalizedText(TextMeshProUGUI targetText, string key)
	{
		LocalizeStringEvent localizedEvent = targetText.GetComponent<LocalizeStringEvent>();
		if (localizedEvent == null) return "";
		var loadingResult = LocalizationSettings.StringDatabase.GetTableEntry(localizedEvent.StringReference.TableReference, key);
		//targetText.text = loadingResult.Entry.GetLocalizedString();
		if (loadingResult.Entry == null)
		{
			Debug.LogWarning($"String table \"{localizedEvent.StringReference.TableReference}\" not found key: {key}");
			return "";
		}

		targetText.text = loadingResult.Entry.GetLocalizedString();
		return targetText.text;
	}
}
