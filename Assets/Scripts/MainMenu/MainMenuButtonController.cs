//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtonController : MonoBehaviour
{
	[Header("Panels")]
	[SerializeField] private GameObject settingPanel;
	[SerializeField] private GameObject heroPanel;
	[SerializeField] private GameObject shopPanel;
	[SerializeField] private GameObject equipmentPanel;
	[SerializeField] private GameObject adventurePanel;
	[SerializeField] private GameObject rankPanel;

	[SerializeField] private GameObject chapterPanel;
	[SerializeField] private GameObject[] infoPanels;


	private GameObject[] panels = new GameObject[6];
	[Header("Other")]
	[SerializeField] private Image buttomBarImage;
	[SerializeField] private LoadingSceneController loadingSceneController;
	[SerializeField] private BattleDataCalculator battleDataCalculator;
	// Start is called before the first frame update
	void Start()
    {
		AudioManager.Instance.PlayBGM(MusicAudioClips.Instance.HallBGM);

		settingPanel.SetActive(false);
		heroPanel.SetActive(false);
		shopPanel.SetActive(false);
		equipmentPanel.SetActive(false);
		chapterPanel.SetActive(false);

		adventurePanel.SetActive(true);

		panels[0] = settingPanel;
		panels[1] = heroPanel;
		panels[2] = shopPanel;
		panels[3] = equipmentPanel;
		panels[4] = adventurePanel;
		panels[5] = rankPanel;
		OnClickCloseInfoPanel();
	}

    // Update is called once per frame
    void Update()
    {
        
    }


	public void SoundFlipping()
	{
		AudioManager.Instance.PlaySFX(SFXAudioClips.Instance.FilpCard);
	}
	public void SoundClick()
	{
		AudioManager.Instance.PlaySFX(SFXAudioClips.Instance.ClickButton);
	}

	

	public void OnClickOpenPanel(GameObject panel)
	{
		foreach (GameObject p in panels)
			p.SetActive(false);

		panel.SetActive(true);
	}

	public void OnClickCloseInfoPanel()
	{
		foreach(var panel in infoPanels)
		{
			panel.SetActive(false);
		}
	}

	public void OnClickClosePanel(GameObject panel)
	{
		panel?.SetActive(false);
	}
	public void OnClickChapterPanel(bool isOpen)
	{
		chapterPanel.SetActive(isOpen);
	}

	public void OnClickButtomButton(Sprite sprite)
	{
		buttomBarImage.sprite = sprite;
	}

	public void OnClickGameStart()
	{
		//var (chapter, level, stage) = StageButtonController.Instance.GetSelectedStage();
		//FightStageConfig.ChapterNumber = chapter;
		//FightStageConfig.LevelsNumber = level;
		//FightStageConfig.StageNumber = stage;

		battleDataCalculator.CalculateBattleData(GetComponent<HeroBag>().selectedHeroID);
		battleDataCalculator.ApplyToFightPlayerConfig();
		loadingSceneController.LoadStage("FightScene");
	}
}
