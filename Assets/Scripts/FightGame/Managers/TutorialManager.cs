//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [SerializeField] private Button exitButton;

    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GifPlayerController gifPlayerController;
    [SerializeField] private Text tutorialText;
    [SerializeField] private Text pageIndicatorText; // 顯示頁數
    [SerializeField] private string[] tutorialContents;

    [SerializeField] private GameObject[] stepPanel;

    private int currentStep = 0;
    private int currentIndex = 0;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        tutorialContents = new string[]
       {
            "拖曳移動玩家，打擊球體",
            "「進球將自動攻擊」對手，\n造成傷害",
            "擊破「技能泡泡」\n施放【輔助技能】",
            "每次進球獲得1點魔力值",
            "魔力值集滿，\n《快速點擊兩下》施放大招"
       };

        // 初始將 Exit 按鈕設為不可按
        if (exitButton != null)
        {
            exitButton.interactable = false;
        }

        currentIndex = 0;
        currentIndex = 0;
        StepOn();

		ShowTutorialText();
    }

    private void ShowTutorialText()
    {
        if (tutorialContents != null && tutorialContents.Length > 0)
        {
            tutorialText.text = tutorialContents[currentIndex];

            if (pageIndicatorText != null)
            {
                pageIndicatorText.text = $"{currentIndex + 1} / {tutorialContents.Length}";
            }

            // Exit Button：只有在最後一頁才可點
            if (exitButton != null)
            {
                exitButton.interactable = (currentIndex == tutorialContents.Length - 1);
            }
        }
    }


    public void NextTutorial()
    {
        if (tutorialContents == null || tutorialContents.Length == 0)
            return;

        currentIndex++;
        if (currentIndex >= tutorialContents.Length) currentIndex = tutorialContents.Length - 1;
        gifPlayerController.PlayGIF(currentIndex);
        ShowTutorialText();
    }

	public void LastTutorial()
	{
		if (tutorialContents == null || tutorialContents.Length == 0)
			return;

		currentIndex--;
		if (currentIndex < 0) currentIndex = 0;
		gifPlayerController.PlayGIF(currentIndex);
		ShowTutorialText();
	}

	public void CloseTutorial()
    {
        StepOn();
	}

    public void OpenTutorial()
    {
        RoundController.Instance.SetTimeScale(0);
        tutorialPanel.SetActive(true);
        ShowTutorialText();
    }

    private void StepOn()
    {
        foreach (var p in stepPanel)
        {
            p.SetActive(false);
        }

        stepPanel[currentStep].SetActive(true);
        currentStep++;
    }

    public void OnClickNextMask()
    {
        switch (currentStep)
        {
            case 1:
                StepOn(); 
                break;
            case 2:
                StepOn();
                break;
			default:
				tutorialPanel.SetActive(false);
				RoundController.Instance.SetTimeScale(1);
				Debug.Log("Tutorial step Complete");
                break;
		}
	}
}
