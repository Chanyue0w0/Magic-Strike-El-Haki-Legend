//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventurePanelUIControll : MonoBehaviour
{
    [SerializeField] private bool playChatBoxAnimation = false;

    [SerializeField] private GameObject chatBox;
    private Animator chatBoxAnimator;
    // Start is called before the first frame update
    void Start()
    {
		chatBoxAnimator = chatBox.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickHeroImage()
    {
        chatBoxAnimator.Play("ChatBoxEntry");
    }

}
