//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
	[SerializeField] private int targetFPS = 60;
	[SerializeField] private float currentFPS;
	[SerializeField] private float musicVolume;
	[SerializeField] private float sfxVolume;

	[Header("------------- Other ------------------")]
	[SerializeField] private Slider musicSlider;
	[SerializeField] private Slider sfxSlider;

	[Header("------------- Text ------------------")]
	[SerializeField] private TextMeshProUGUI musicVolumeText;
	[SerializeField] private TextMeshProUGUI sfxVolumeText;
	[SerializeField] private TextMeshProUGUI fpsText;

	[Header("------------- gameboject ------------------")]
	[SerializeField] private GameObject volumePanel;
	[SerializeField] private GameObject staffPanel;
	[SerializeField] private GameObject graphicsPanel;
	[SerializeField] private GameObject fpsList;


	private bool isOpenFPSList = false;

	// Start is called before the first frame update
	void Start()
	{
		volumePanel.SetActive(true);
		staffPanel.SetActive(false);
		graphicsPanel.SetActive(false);
		fpsList.SetActive(false);
		isOpenFPSList = false;

		initSetting();
	}

	// Update is called once per frame
	void Update()
	{
		//currentFPS = Time.frameCount / Time.time;
		currentFPS = 1.0f / Time.deltaTime;
	}

	private void initSetting()
	{
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
	}

	public void SetMusicVolume()
	{
		float volume = musicSlider.value;
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

		isOpenFPSList = false;
		fpsList.SetActive(false);
	}

	private void LoadTargetFPS()
	{
		SetTargetFPS(PlayerPrefs.GetInt("FPS"));
	}



	public void OpenFPSList()
	{
		isOpenFPSList = !isOpenFPSList;
		fpsList.SetActive(isOpenFPSList);
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
}
