//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class SFXAudioClips : MonoBehaviour
{
	[field: Header("---------- UI SFX ----------")]
	[field: Header("Card Sound")]
	[field: SerializeField] public AudioClip ClickButton { get; private set; }
	[field: SerializeField] public AudioClip FilpCard { get; private set; }
	public static SFXAudioClips Instance { get; private set; }
	void Awake()
	{
		if (Instance != null)
		{
			Debug.Log("Found more than one FOMD Events in the sence");
		}
		Instance = this;
	}
}
