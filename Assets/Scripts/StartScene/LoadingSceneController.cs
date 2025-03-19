using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
	[SerializeField] GameObject loadingImage;
	[SerializeField] Slider slider;
	[SerializeField] Text progressText;


	[Header("------------- Load Image Sprite ------------------")]
	[SerializeField] Sprite[] loadingImageSprite;

	private void Start()
	{
		loadingImage.SetActive(false);
	}

	public void LoadStage(int sceneIndex)
	{
		StartCoroutine(LoadAsunchronously(sceneIndex));
	}

	public void LoadStage(string sceneName)
	{
		StartCoroutine(LoadAsunchronously(sceneName));
	}

	IEnumerator LoadAsunchronously(int sceneIndex)
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

		int randomIndex = Random.Range(0, loadingImageSprite.Length);
		loadingImage.GetComponent<Image>().sprite = loadingImageSprite[randomIndex];
		loadingImage.SetActive(true);


		while (!operation.isDone)
		{
			float progress = Mathf.Clamp01(operation.progress / 0.9f);

			slider.value = progress;
			progressText.text = progress * 10000 * 1f / 100 + "%";
			//Debug.Log(progress);
			yield return null;
		}

	}

	IEnumerator LoadAsunchronously(string sceneName)
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

		int randomIndex = Random.Range(0, loadingImageSprite.Length);
		loadingImage.GetComponent<Image>().sprite = loadingImageSprite[randomIndex];
		loadingImage.SetActive(true);


		while (!operation.isDone)
		{
			float progress = Mathf.Clamp01(operation.progress / 0.9f);

			slider.value = progress;
			progressText.text = progress * 10000 * 1f / 100 + "%";
			//Debug.Log(progress);
			yield return null;
		}

	}
}
