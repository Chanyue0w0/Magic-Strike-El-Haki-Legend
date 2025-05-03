using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaChaDoor : MonoBehaviour
{
	[SerializeField] private GameObject gaChaPanel;

	private void Start()
	{
		gaChaPanel.SetActive(false);
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Ball"))
		{
			// °õ¦æ°Êµe
			gaChaPanel.SetActive(true);
			Time.timeScale = 0;
		}
	}

	public void OncClickFinishGaCha()
	{
		Time.timeScale = 1f;
		// back to menu
	}
}
