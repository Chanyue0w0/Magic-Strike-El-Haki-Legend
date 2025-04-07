using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class StageButtonController : MonoBehaviour
{
	[SerializeField] private GameObject stageButtonPrefab;
	[SerializeField] private Transform stageButtonContainer;
	[SerializeField] private GameObject stagePanel;
	[SerializeField] private Text chapterText;
	[SerializeField] private Image chapterBackgroundImage;

	[SerializeField] MainMenuButtonController mainMenuButtonController;

	private int selectedChapter;
	private int selectedLevel;

	private void Start()
	{
		GenerateStageButtons();
	}

	public void GenerateStageButtons()
	{
		var (totalChapters, levelsPerChapter, _) = StageData.Instance.GetStageCounts();

		int currentChapter = PlayerDataManager.Instance.GetCurrentChapter();
		int currentLevel = PlayerDataManager.Instance.GetCurrentLevel();
		// 如果 chapter 或 level 小於1，則預設為1
		if (currentChapter < 1) currentChapter = 1;
		if (currentLevel < 1) currentLevel = 1;
		SelectStage($"Chapter_{currentChapter}_Level_{currentLevel}");


		foreach (Transform child in stageButtonContainer)
		{
			Destroy(child.gameObject);
		}

		for (int chapter = 1; chapter <= totalChapters; chapter++)
		{
			if (!levelsPerChapter.ContainsKey(chapter))
				continue;

			for (int level = 1; level <= levelsPerChapter[chapter]; level++)
			{
				GameObject newButton = Instantiate(stageButtonPrefab, stageButtonContainer);
				string chapterLevelName = $"Chapter_{chapter}_Level_{level}";
				newButton.name = chapterLevelName;

				bool isPassed = false;
				if (currentChapter > chapter) isPassed = true;
				else if (currentChapter == chapter && currentLevel > level) isPassed = true;
				bool isUnLocked = (currentLevel == level) && (currentChapter == chapter);

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
	}


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
		chapterText.text = $"{selectedChapter} - {selectedLevel}";
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
}
