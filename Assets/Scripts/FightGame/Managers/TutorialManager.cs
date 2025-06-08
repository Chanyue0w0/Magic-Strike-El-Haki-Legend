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
    [SerializeField] private Text pageIndicatorText; // ��ܭ���
	[SerializeField] private Image nextButtonImage;
	[SerializeField] private Image lastButtonImage;
	[SerializeField] private GameObject[] stepPanel;

    private int currentStep = 0;
    private int currentIndex = 0;
    private string[] tutorialContents;

    private void Awake()
    {
        tutorialPanel.SetActive(false);
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
            "�즲���ʪ��a�A�����y��",
            "�u�i�y�N�۰ʧ����v���A\n�y���ˮ`",
            "���}�u�ޯ�w�w�v\n�I��i�D�ʧޯ�j",

            "�C���i�y��o1�I�]�O��",
            "�]�O�ȶ����A\n�m�ֳt�I����U�n�I��j��"
       };

        // ��l�N Exit ���s�]�����i��
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

            // Exit Button�G�u���b�̫�@���~�i�I
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


		AudioManager.Instance.PlaySFX(SFXAudioClips.Instance.FilpCard);
		nextButtonImage.color = Color.white;
		lastButtonImage.color = Color.white;
		lastButtonImage.transform.GetComponent<Button>().interactable = true;
		currentIndex++;
        if (currentIndex >= tutorialContents.Length-1)
        {

			nextButtonImage.transform.GetComponent<Button>().interactable = false;
			nextButtonImage.color = Color.gray;
			currentIndex = tutorialContents.Length - 1;
		}
        gifPlayerController.PlayGIF(currentIndex);
        ShowTutorialText();
    }

	public void LastTutorial()
	{
		if (tutorialContents == null || tutorialContents.Length == 0)
			return;


		AudioManager.Instance.PlaySFX(SFXAudioClips.Instance.FilpCard);
		lastButtonImage.color = Color.white;
		nextButtonImage.color = Color.white;
		nextButtonImage.transform.GetComponent<Button>().interactable = true;
		currentIndex--;
		if (currentIndex <= 0)
        {
            lastButtonImage.transform.GetComponent<Button>().interactable = false;
            lastButtonImage.color = Color.gray;
			currentIndex = 0;
		}
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

		AudioManager.Instance.PlaySFX(SFXAudioClips.Instance.ClickButton);
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
