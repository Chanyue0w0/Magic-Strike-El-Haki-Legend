using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class StageButtonController : MonoBehaviour
{
	[SerializeField] private GameObject stageButtonPrefab;
	[SerializeField] private Transform stageButtonContainer;
	[SerializeField] private GameObject stagePanel;
	[SerializeField] private Text chapterText;

	private string selectedChapter;
	private string selectedLevel;

	private void Start()
	{
		GenerateStageButtons();

		// 自動選擇當前關卡
		
	}

	private void GenerateStageButtons()
	{
		var (totalChapters, levelsPerChapter, _) = StageData.Instance.GetStageCounts();

		int currentChapter = PlayerDataManager.Instance.GetPlayerChapter();
		int currentLevel = PlayerDataManager.Instance.GetPlayerCurrentLevel();
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
				else if (currentLevel == chapter && currentLevel > level) isPassed = true;
				bool isUnLocked = (currentLevel == level) && (currentChapter == chapter);

				// **設定背景圖片**
				Image bgImage = newButton.transform.Find("bg Image")?.GetComponent<Image>();
				if (bgImage != null)
				{
					string bgImagePath = $"Arts/MainScenes/StagePanel/StageImage/ch{chapter}-{level}";
					bgImage.sprite = Resources.Load<Sprite>(bgImagePath);
				}

				// **設定狀態圖片**
				Image statusImage = newButton.transform.Find("status Image")?.GetComponent<Image>();
				if (statusImage != null)
				{
					string statusImagePath;
					if (isPassed)
					{
						statusImagePath = "Arts/MainScenes/StagePanel/StageStatusIcon/PassedStage";
					}
					else if (isUnLocked)
					{
						statusImagePath = "Arts/MainScenes/StagePanel/StageStatusIcon/UnPassedStage";
					}
					else
					{
						statusImagePath = "Arts/MainScenes/StagePanel/StageStatusIcon/LockStage";
					}

					statusImage.sprite = Resources.Load<Sprite>(statusImagePath);
				}

				Button buttonComponent = newButton.GetComponent<Button>();
				if (isUnLocked)
				{
					buttonComponent.onClick.AddListener(() => SelectStage(chapterLevelName));
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

		selectedChapter = $"Chapter_{match.Groups[1].Value}";
		selectedLevel = $"Level_{match.Groups[2].Value}";

		chapterText.text = $"{match.Groups[1].Value} - {match.Groups[2].Value}";

		ApplySelectedStageToConfig();
		stagePanel.SetActive(false);
	}

	public void ApplySelectedStageToConfig()
	{
		FightStageConfig.ChapterNumber = selectedChapter;
		FightStageConfig.LevelsNumber = selectedLevel;
	}
}
