//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class MusicAudioClips : MonoBehaviour
{

	[field: Header("-------------------- BGM --------------------")]

	[field: SerializeField] public AudioClip MainBGM { get; private set; }
	[field: SerializeField] public AudioClip HallBGM { get; private set; }
	[field: SerializeField] public AudioClip BasicBattleBGM { get; private set; }

	public static MusicAudioClips Instance { get; private set; }
	void Awake()
	{
		if (Instance != null)
		{
			Debug.Log("Found more than one FOMD Events in the sence");
		}
		Instance = this;
	}
}
