//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class SFXAudioClips : MonoBehaviour
{
	[field: Header("---------- UI SFX ----------")]
	[field: Header("Card Sound")]
	[field: SerializeField] public AudioClip ClickButton { get; private set; }
	[field: SerializeField] public AudioClip FilpCard { get; private set; }
	[field: SerializeField] public AudioClip BallHit { get; private set; }
	[field: SerializeField] public AudioClip BallBounce { get; private set; }
	[field: SerializeField] public AudioClip SheildNormalAttack { get; private set; }
	[field: SerializeField] public AudioClip SlimeNormalAttack { get; private set; }
	[field: SerializeField] public AudioClip FireBall { get; private set; }
	[field: SerializeField] public AudioClip StunEffect { get; private set; }
	[field: SerializeField] public AudioClip SkillPickUp { get; private set; }
	[field: SerializeField] public AudioClip HammerUlt { get; private set; }
	[field: SerializeField] public AudioClip MaxMagicPoint { get; private set; }
	[field: SerializeField] public AudioClip UltAnimation { get; private set; }
	[field: SerializeField] public AudioClip GetMoney { get; private set; }
	[field: SerializeField] public AudioClip SlimeDie { get; private set; }
	[field: SerializeField] public AudioClip WinSoundEffect { get; private set; }
	[field: SerializeField] public AudioClip LoseSoundEffect { get; private set; }
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
