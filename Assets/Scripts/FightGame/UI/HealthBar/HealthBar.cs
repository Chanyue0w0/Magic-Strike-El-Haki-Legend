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
	//[SerializeField] private Slider delayedSlider; // ������
	[SerializeField] private Image delayedFillImage; // ������
	[SerializeField] public Vector2 UIOriginPosition;
	[SerializeField] public float UIShakeRangeOrigin = 1f;
	[SerializeField] public float UIShakeRange = 1f;
	[SerializeField] private RectTransform rt;
	[SerializeField] private Image fillImage;
	[SerializeField] private Text healthPointAmount;
	[SerializeField] private float maxHealth = 900f; // �̤j��q
	[SerializeField] private float currentHealth = 900f; // ��e��q
	[SerializeField] private float delayedBarSpeed = 0.5f; // ����������s�t��

	[SerializeField] private GameObject warningEffect; // �C��qĵ�i

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
		// ���ƴ�֩�����
		if (delayedFillImage.fillAmount != fillImage.fillAmount)
		{
			delayedFillImage.fillAmount = Mathf.Lerp(delayedFillImage.fillAmount, fillImage.fillAmount, Time.deltaTime * delayedBarSpeed);
			if (Mathf.Abs(delayedFillImage.fillAmount - fillImage.fillAmount) < 0.01f) // �T�O�̲׭ȩM�Y�ɦ���@�P
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
		healthPointAmount.text = (health.ToString());// + '/' + health.ToString()  //�h���̤j��q�Ʀr


		delayedFillImage.fillAmount = fillImage.fillAmount; // �P�B����������l��
	}

	public void SetHealth(int health)
	{
		//UI��{���A�ѩ�L�C�ƭ�UI�񺡫h�|=0�A�ҥH���������
		//if (health >= slider.maxValue * 0.1f) 
		//	slider.value = health;
		//else if (health < slider.maxValue * 0.1f && health > 0)
		//	slider.value = slider.maxValue * 0.1f;
		//else
		//	slider.value = 0;
		currentHealth = health;
		fillImage.fillAmount = currentHealth / maxHealth;
		healthPointAmount.text = (health.ToString());// +'/' + slider.maxValue.ToString() //�h���̤j��q�Ʀr
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
