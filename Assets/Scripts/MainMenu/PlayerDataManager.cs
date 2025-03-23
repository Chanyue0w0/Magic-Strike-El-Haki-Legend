using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
	private const string PLAYER_NAME_KEY = "PlayerName";
	private const string PLAYER_LEVEL_KEY = "PlayerLevel";
	private const string PLAYER_EXP_KEY = "PlayerExp";
	private const string PLAYER_GEM_KEY = "PlayerGem";
	private const string PLAYER_COIN_KEY = "PlayerCoin";
	private const string PLAYER_ENERGY_KEY = "PlayerEnergy";
	private const string PLAYER_MAX_ENERGY_KEY = "PlayerMaxEnergy";

	public static PlayerDataManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		LoadPlayerData();
	}

	// 讀取玩家資料，若無則設定預設值
	private void LoadPlayerData()
	{
		if (!PlayerPrefs.HasKey(PLAYER_NAME_KEY))
		{
			PlayerPrefs.SetString(PLAYER_NAME_KEY, "chenyue");
			PlayerPrefs.SetInt(PLAYER_LEVEL_KEY, 1);
			PlayerPrefs.SetInt(PLAYER_EXP_KEY, 0);
			PlayerPrefs.SetInt(PLAYER_GEM_KEY, 100);
			PlayerPrefs.SetInt(PLAYER_COIN_KEY, 100);
			PlayerPrefs.SetInt(PLAYER_ENERGY_KEY, 35);
			PlayerPrefs.SetInt(PLAYER_MAX_ENERGY_KEY, 35);
			PlayerPrefs.Save();
		}
	}

	// 取得玩家名稱
	public string GetPlayerName() => PlayerPrefs.GetString(PLAYER_NAME_KEY);

	// 設定玩家名稱
	public void SetPlayerName(string name)
	{
		PlayerPrefs.SetString(PLAYER_NAME_KEY, name);
		PlayerPrefs.Save();
	}

	// 取得玩家等級
	public int GetPlayerLevel() => PlayerPrefs.GetInt(PLAYER_LEVEL_KEY);

	// 設定玩家等級
	public void SetPlayerLevel(int level)
	{
		PlayerPrefs.SetInt(PLAYER_LEVEL_KEY, Mathf.Max(1, level));
		PlayerPrefs.Save();
	}

	// 取得玩家經驗值
	public int GetPlayerExp() => PlayerPrefs.GetInt(PLAYER_EXP_KEY);

	// 設定玩家經驗值
	public void SetPlayerExp(int exp)
	{
		PlayerPrefs.SetInt(PLAYER_EXP_KEY, Mathf.Max(0, exp));
		PlayerPrefs.Save();
	}

	// 取得寶石數量
	public int GetPlayerGem() => PlayerPrefs.GetInt(PLAYER_GEM_KEY);

	// 設定寶石數量
	public void SetPlayerGem(int gem)
	{
		PlayerPrefs.SetInt(PLAYER_GEM_KEY, Mathf.Max(0, gem));
		PlayerPrefs.Save();
	}

	// 取得金幣數量
	public int GetPlayerCoin() => PlayerPrefs.GetInt(PLAYER_COIN_KEY);

	// 設定金幣數量
	public void SetPlayerCoin(int coin)
	{
		PlayerPrefs.SetInt(PLAYER_COIN_KEY, Mathf.Max(0, coin));
		PlayerPrefs.Save();
	}

	// 取得目前電力
	public int GetPlayerEnergy() => PlayerPrefs.GetInt(PLAYER_ENERGY_KEY);

	// 設定目前電力數量
	public void SetPlayerEnergy(int energy)
	{
		int maxEnergy = GetPlayerMaxEnergy();
		PlayerPrefs.SetInt(PLAYER_ENERGY_KEY, Mathf.Clamp(energy, 0, maxEnergy));
		PlayerPrefs.Save();
	}

	// 取得電力上限
	public int GetPlayerMaxEnergy() => PlayerPrefs.GetInt(PLAYER_MAX_ENERGY_KEY);

	// 設定電力上限
	public void SetPlayerMaxEnergy(int maxEnergy)
	{
		PlayerPrefs.SetInt(PLAYER_MAX_ENERGY_KEY, Mathf.Max(1, maxEnergy));

		// 確保當前電力不超過上限
		int currentEnergy = GetPlayerEnergy();
		if (currentEnergy > maxEnergy)
		{
			SetPlayerEnergy(maxEnergy);
		}

		PlayerPrefs.Save();
	}

	// 清除玩家資料（用於測試）
	public void ResetPlayerData()
	{
		PlayerPrefs.DeleteAll();
		LoadPlayerData();
	}
}
