using UnityEngine;
using TMPro;

public class TopStickBarUIControll : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI playerNameText;
	[SerializeField] private TextMeshProUGUI energyText;
	[SerializeField] private TextMeshProUGUI coinText;
	[SerializeField] private TextMeshProUGUI gemText;

	private void Start()
	{
	}


	private void Update()
	{
		UpdateUI();
	}
	// 更新 UI 內容
	public void UpdateUI()
	{
		if (PlayerDataManager.Instance == null)
		{
			Debug.LogError("PlayerDataManager is not initialized!");
			return;
		}

		playerNameText.text = PlayerDataManager.Instance.GetPlayerName();
		int currentEnergy = PlayerDataManager.Instance.GetPlayerEnergy();
		int maxEnergy = PlayerDataManager.Instance.GetPlayerMaxEnergy();
		energyText.text = $"{currentEnergy}/{maxEnergy}";
		coinText.text = PlayerDataManager.Instance.GetPlayerCoin().ToString();
		gemText.text = PlayerDataManager.Instance.GetPlayerGem().ToString();
	}

	// 如果數據變更，手動調用此方法
	public void RefreshData()
	{
		UpdateUI();
	}
}
