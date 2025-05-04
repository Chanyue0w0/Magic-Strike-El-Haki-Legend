//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] private LoadingSceneController loadingSceneController;
    // Start is called before the first frame update
    void Start()
    {
		AudioManager.Instance.PlayBGM(MusicAudioClips.Instance.MainBGM);

        PlayerEquipmentManager.Instance.LoadEquipment();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) )
        {
            if (IsDataInitializeFinished())
                loadingSceneController.LoadStage("MainMenuScene");
        }

	}

    private bool IsDataInitializeFinished()
    {
        bool pass =  PlayerEquipmentManager.Instance.IsFileEixt()
            & PlayerHeroManager.Instance.IsFileEixt()
            & PlayerDataManager.Instance.IsPlayerDataInitlized();
        return pass;
    }
}
