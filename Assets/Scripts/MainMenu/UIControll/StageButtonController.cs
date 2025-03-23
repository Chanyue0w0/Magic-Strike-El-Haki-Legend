using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class StageButtonController : MonoBehaviour
{
	[SerializeField] private GameObject stageButtonPrefab;
	[SerializeField] private Transform stageButtonContainer;
	[SerializeField] private GameObject stagePanel;
	[SerializeField] private Text chapterText;

	private string selectedChapter;
	private string selectedLevel;
	private string selectedStage;

	private void Start()
	{
		GenerateStageButtons();
	}

	private void GenerateStageButtons()
	{
		var (totalChapters, levelsPerChapter, stagesPerLevel) = StageData.Instance.GetStageCounts();

		foreach (Transform child in stageButtonContainer)
		{
			Destroy(child.gameObject);
		}

		for (int chapter = 1; chapter <= totalChapters; chapter++)
		{
			if (!levelsPerChapter.ContainsKey(chapter)) continue;

			for (int level = 1; level <= levelsPerChapter[chapter]; level++)
			{
				if (!stagesPerLevel.ContainsKey((chapter, level))) continue;

				for (int stage = 1; stage <= stagesPerLevel[(chapter, level)]; stage++)
				{
					GameObject newButton = Instantiate(stageButtonPrefab, stageButtonContainer);
					string chapterName = $"Chapter_{chapter}_Level_{level}_Stage_{stage}";
					newButton.name = chapterName;
					newButton.GetComponentInChildren<TextMeshProUGUI>().text = $"{chapter}-{level} {stage}";
					newButton.GetComponent<Button>().onClick.AddListener(() => SelectStage(chapterName));
				}
			}
		}
	}

	public void SelectStage(string chapterName)
	{
		Match match = Regex.Match(chapterName, @"Chapter_(\d+)_Level_(\d+)_Stage_(\d+)");

		selectedChapter = $"Chapter_{match.Groups[1].Value}";
		selectedLevel = $"Level_{match.Groups[2].Value}";
		selectedStage = match.Groups[3].Value;

		chapterText.text = $"{match.Groups[1].Value}-{match.Groups[2].Value} {match.Groups[3].Value}";
		ApplySelectedStageToConfig();
		stagePanel.SetActive(false);
	}

	public void ApplySelectedStageToConfig()
	{
		FightStageConfig.ChapterNumber = selectedChapter;
		FightStageConfig.LevelsNumber = selectedLevel;
		FightStageConfig.StageNumber = selectedStage;
	}
}
