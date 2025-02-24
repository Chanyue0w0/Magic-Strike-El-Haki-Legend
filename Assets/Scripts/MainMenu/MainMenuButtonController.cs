//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtonController : MonoBehaviour
{
	[Header("Panels")]
	[SerializeField] private GameObject settingPanel;
	[SerializeField] private GameObject heroPanel;
	[SerializeField] private GameObject shopPanel;
	[SerializeField] private GameObject equipmentPanel;
	[SerializeField] private GameObject adventurePanel;

	private GameObject[] panels = new GameObject[5];
	// Start is called before the first frame update
	void Start()
    {
		AudioManager.Instance.PlayBGM(MusicAudioClips.Instance.HallBGM);

		settingPanel.SetActive(false);
		heroPanel.SetActive(false);
		shopPanel.SetActive(false);
		equipmentPanel.SetActive(false);

		adventurePanel.SetActive(true);

		panels[0] = settingPanel;
		panels[1] = heroPanel;
		panels[2] = shopPanel;
		panels[3] = equipmentPanel;
		panels[4] = adventurePanel;
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

	public void OnClickExitPanel()
	{
		settingPanel.SetActive(false);
	}

	public void OnClickOpenPanel(GameObject panel)
	{
		foreach (GameObject p in panels)
			p.SetActive(false);

		panel.SetActive(true);
	}
}
