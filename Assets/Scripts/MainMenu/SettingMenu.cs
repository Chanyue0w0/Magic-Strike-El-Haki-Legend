//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.Localization.Settings;

public class SettingsMenu : MonoBehaviour
{
	[SerializeField] private float currentFPS;
	[SerializeField] private int targetFPS = 60;
	[SerializeField] private float musicVolume;
	[SerializeField] private float sfxVolume;
	[SerializeField] private string language = "Chinese (Traditional) (zh-TW)";

	[Header("------------- Other ------------------")]
	[SerializeField] private Slider musicSlider;
	[SerializeField] private Slider sfxSlider;

	[Header("------------- Text ------------------")]
	[SerializeField] private TextMeshProUGUI musicVolumeText;
	[SerializeField] private TextMeshProUGUI sfxVolumeText;
	[SerializeField] private TextMeshProUGUI fpsText;

	[Header("------------- gameboject ------------------")]

	[SerializeField] private GameObject settingPanel;
	[SerializeField] private GameObject volumePanel;
	[SerializeField] private GameObject staffPanel;
	[SerializeField] private GameObject graphicsPanel;
	[SerializeField] private GameObject fpsList;
	[SerializeField] private GameObject languageList;

	// Start is called before the first frame update
	void Start()
	{
		InitSetting();
	}

	// Update is called once per frame
	void Update()
	{
		//currentFPS = Time.frameCount / Time.time;
		currentFPS = 1.0f / Time.deltaTime;
	}

	private void InitSetting()
	{
		staffPanel.SetActive(false);
		fpsList.SetActive(false);
		languageList.SetActive(false);

		if (PlayerPrefs.HasKey("MusicVolume"))
		{
			LoadMusicVolume();
		}
		else
		{
			SetMusicVolume();
		}

		if (PlayerPrefs.HasKey("SFXVolume"))
		{
			LoadSFXVolume();
		}
		else
		{
			SetSFXVolume();
		}

		if (PlayerPrefs.HasKey("FPS"))
		{
			LoadTargetFPS();
		}
		else
		{
			SetTargetFPS(targetFPS);
		}

		if (PlayerPrefs.HasKey("Language"))
		{
			LoadTargetFPS();
		}
		else
		{
			SetTargetFPS(targetFPS);
		}
	}

	public void SetMusicVolume()
	{
		float volume = musicSlider.value;
		musicVolume = volume;
		musicVolumeText.text = ((int)(volume * 100f)).ToString();
		PlayerPrefs.SetFloat("MusicVolume", volume);
		AudioManager.Instance.musicVolume = volume;
	}

	private void LoadMusicVolume()
	{
		musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");

		musicVolumeText.text = ((int)(musicSlider.value * 100f)).ToString();
		SetMusicVolume();
	}

	public void SetSFXVolume()
	{
		float volume = sfxSlider.value;
		sfxVolume = volume;
		sfxVolumeText.text = ((int)(volume * 100f)).ToString();
		PlayerPrefs.SetFloat("SFXVolume", volume);
		AudioManager.Instance.sfxVolume = volume;
	}

	private void LoadSFXVolume()
	{
		sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
		sfxVolumeText.text = ((int)(sfxSlider.value * 100f)).ToString();
		SetSFXVolume();
	}

	public void SetTargetFPS(int fps)
	{
		fpsText.text = fps.ToString();
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = fps;
		PlayerPrefs.SetInt("FPS", fps);
		targetFPS = fps;

		fpsList.SetActive(false);
	}

	private void LoadTargetFPS()
	{
		SetTargetFPS(PlayerPrefs.GetInt("FPS"));
	}


	public void SetLanguage(string language)
	{
		//var loc = LocalizationSettings.AvailableLocales.Locales.Find(local => local.LocaleName == language);
		//LocalizationSettings.Instance.SetSelectedLocale(loc);
		LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales.Find(local => local.LocaleName == language);


		PlayerPrefs.SetString("Language", language);
		languageList.SetActive(false);
	}

	private void LoadLanguage()
	{
		SetLanguage(PlayerPrefs.GetString("Language"));
	}

	public void OpenList(GameObject list)
	{
		list.SetActive(!list.activeSelf);
	}


	public void OnClickVolumePanel()
	{
		volumePanel.SetActive(true);
		staffPanel.SetActive(false);
		graphicsPanel.SetActive(false);
	}
	public void OnClickstaffPanel()
	{
		volumePanel.SetActive(false);
		staffPanel.SetActive(true);
		graphicsPanel.SetActive(false);
	}

	public void OnClickGraphicsPanel()
	{

		volumePanel.SetActive(false);
		staffPanel.SetActive(false);
		graphicsPanel.SetActive(true);
	}

	public void OnClickIsEnableSettingPanel(bool isEnable)
	{
		settingPanel.SetActive(isEnable);
	}
}
