using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	private enum UserPosition { player1, player2 };
	[SerializeField] private UserPosition playerNumber;

	public bool isUIShaking = false;
	//[SerializeField] private Slider slider;
	//[SerializeField] private Slider delayedSlider; // 延遲血條
	[SerializeField] private Image delayedFillImage; // 延遲血條
	[SerializeField] public Vector2 UIOriginPosition;
	[SerializeField] public float UIShakeRangeOrigin = 1f;
	[SerializeField] public float UIShakeRange = 1f;
	[SerializeField] private RectTransform rt;
	[SerializeField] private Image fillImage;
	[SerializeField] private Text healthPointAmount;
	[SerializeField] private float maxHealth = 900f; // 最大血量
	[SerializeField] private float currentHealth = 900f; // 當前血量
	[SerializeField] private float delayedBarSpeed = 0.5f; // 延遲血條的更新速度

	[SerializeField] private GameObject warningEffect; // 低血量警告

	private void Start()
	{
		UIOriginPosition = rt.transform.position;

		delayedFillImage.fillAmount = fillImage.fillAmount;
	}

	private void Update()
	{
		if (isUIShaking)
		{
			UIShaking();
		}
		// 平滑減少延遲血條
		if (delayedFillImage.fillAmount != fillImage.fillAmount)
		{
			delayedFillImage.fillAmount = Mathf.Lerp(delayedFillImage.fillAmount, fillImage.fillAmount, Time.deltaTime * delayedBarSpeed);
			if (Mathf.Abs(delayedFillImage.fillAmount - fillImage.fillAmount) < 0.01f) // 確保最終值和即時血條一致
			{
				delayedFillImage.fillAmount = fillImage.fillAmount;
			}
		}

		if (playerNumber == UserPosition.player1)
        {
			if (currentHealth < maxHealth * 0.3f && !warningEffect.activeSelf)
			{
				warningEffect.SetActive(true);
			}
			else if (currentHealth >= maxHealth * 0.3f && warningEffect.activeSelf)
			{
				warningEffect.SetActive(false);
			}
		}
		
	}


	private void Awake()
	{
		//slider = gameObject.GetComponent<Slider>();
	}

	public void SetMaxHealth(int health)
	{
		fillImage.fillAmount = 1;
		//slider.maxValue = health;
		//slider.value = health;
		maxHealth = health;
		currentHealth = health;
		healthPointAmount.text = (health.ToString());// + '/' + health.ToString()  //去除最大血量數字


		delayedFillImage.fillAmount = fillImage.fillAmount; // 同步延遲血條的初始值
	}

	public void SetHealth(int health)
	{
		//UI表現鎖血，由於過低數值UI填滿則會=0，所以選擇鎖血顯示
		//if (health >= slider.maxValue * 0.1f) 
		//	slider.value = health;
		//else if (health < slider.maxValue * 0.1f && health > 0)
		//	slider.value = slider.maxValue * 0.1f;
		//else
		//	slider.value = 0;
		currentHealth = health;
		fillImage.fillAmount = currentHealth / maxHealth;
		healthPointAmount.text = (health.ToString());// +'/' + slider.maxValue.ToString() //去除最大血量數字
	}

	public void SetPoisoningUI(bool setPoisoning)
	{
		if (setPoisoning)
		{
			fillImage.color = new Color32(255, 0, 218, 255);
			//Debug.Log("Poison");
		}
		else
		{
			fillImage.color = new Color32(255, 255, 255, 255);
			//Debug.Log("PoisonEnd");
		}
	}

	public void SetIsUIShaking()
	{
		isUIShaking = true;
	}
	public void UIShaking()
	{
		//rt.localPosition = new Vector2(rt.localPosition.x + UIShakeRange * Time.deltaTime, rt.localPosition.y + UIShakeRange * Time.deltaTime);
		rt.transform.position = new Vector2(UIOriginPosition.x + (Random.Range(0f, UIShakeRange)) - UIShakeRange * 0.5f,
											UIOriginPosition.y + (Random.Range(0f, UIShakeRange)) - UIShakeRange * 0.5f);//

		UIShakeRange /= 1.05f;
		if (UIShakeRange < 0.05f)
		{
			UIShakeRange = UIShakeRangeOrigin;
			isUIShaking = false;
			rt.transform.position = UIOriginPosition;
		}
		//Debug.Log("Shaking" + rt.localPosition);
	}
}
