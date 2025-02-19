//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	[Header("Volume")]
	[Range(0.0001f, 1)]
	public float masterVolume = 1;
	[Range(0.0001f, 1)]
	public float musicVolume = 1;
	[Range(0.0001f, 1)]
	public float sfxVolume = 1;


	[Header("Reference")]
	[SerializeField] private AudioMixer audioMixer;
	[SerializeField] private AudioSource sfxSource; // �Ω󼽩񭵮�
	[SerializeField] private AudioSource musicSource;

	public static AudioManager Instance { get; private set; }
	void Awake()
	{
		if (Instance != null)
		{
			Debug.Log("Found more than one Audio Manager in the sence");
		}
		Instance = this;


		if (PlayerPrefs.HasKey("MasterVolume")) masterVolume = PlayerPrefs.GetFloat("MasterVolume");
		if (PlayerPrefs.HasKey("MusicVolume")) musicVolume = PlayerPrefs.GetFloat("MusicVolume");
		if (PlayerPrefs.HasKey("SFXVolume")) sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
	}

	private void Update()
	{
		audioMixer.SetFloat("Master", Mathf.Log10(masterVolume) * 20);
		audioMixer.SetFloat("SFX", Mathf.Log10(sfxVolume) * 20);
		audioMixer.SetFloat("Music", Mathf.Log10(musicVolume) * 20);

		//Debug.Log(bgmSource.volume);
	}

	// ����I������
	public void PlayBGM(AudioClip bgm)
	{
		if (musicSource.clip != bgm)
		{
			musicSource.clip = bgm;
			musicSource.loop = true;
			musicSource.Play();
		}
	}

	// ����I������
	public void StopBGM()
	{
		musicSource.Stop();
	}

	// ���񭵮�
	public void PlaySFX(AudioClip clip)
	{
		sfxSource.PlayOneShot(clip);
	}

	// �b���w��m���񭵮�
	public void PlaySFXAtPosition(AudioClip clip, Vector3 position)
	{
		AudioSource.PlayClipAtPoint(clip, position);
	}
}
