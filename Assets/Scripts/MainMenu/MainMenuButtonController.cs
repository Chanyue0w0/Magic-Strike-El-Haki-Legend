//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtonController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		AudioManager.Instance.PlayBGM(MusicAudioClips.Instance.HallBGM);
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
}
