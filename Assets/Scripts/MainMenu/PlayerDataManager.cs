using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
	[SerializeField] private bool resetPlayerData = false;
	private const string PLAYER_NAME_KEY = "PlayerName";
	private const string PLAYER_LEVEL_KEY = "PlayerLevel";
	private const string PLAYER_EXP_KEY = "PlayerExp";
	private const string PLAYER_GEM_KEY = "PlayerGem";
	private const string PLAYER_COIN_KEY = "PlayerCoin";
	private const string PLAYER_ENERGY_KEY = "PlayerEnergy";
	private const string PLAYER_MAX_ENERGY_KEY = "PlayerMaxEnergy";
	// 新增玩家目前的 Chapter 與 Level
	private const string PLAYER_CHAPTER_KEY = "PlayerChapter";
	private const string PLAYER_CURRENT_LEVEL_KEY = "PlayerCurrentLevel";

	// 進化石數量
	private const string COMMON_EVOSTONE_KEY = "CommonEvoStone";
	private const string RARE_EVOSTONE_KEY = "RareEvoStone";
	private const string SPECIAL_EVOSTONE_KEY = "SpecialEvoStone";
	private const string LEGENDARY_EVOSTONE_KEY = "LegendaryEvoStone";

	public static PlayerDataManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		if(resetPlayerData) ResetPlayerData();
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
			PlayerPrefs.SetInt(PLAYER_GEM_KEY, 1000);
			PlayerPrefs.SetInt(PLAYER_COIN_KEY, 5000);
			PlayerPrefs.SetInt(PLAYER_ENERGY_KEY, 35);
			PlayerPrefs.SetInt(PLAYER_MAX_ENERGY_KEY, 35);
			// 章節
			PlayerPrefs.SetInt(PLAYER_CHAPTER_KEY, 1);
			PlayerPrefs.SetInt(PLAYER_CURRENT_LEVEL_KEY, 1);
			// 石頭
			PlayerPrefs.SetInt(COMMON_EVOSTONE_KEY, 100);
			PlayerPrefs.SetInt(RARE_EVOSTONE_KEY, 1000);
			PlayerPrefs.SetInt(SPECIAL_EVOSTONE_KEY, 10);
			PlayerPrefs.SetInt(LEGENDARY_EVOSTONE_KEY, 10);
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

	// 取得玩家目前的 Chapter
	public int GetPlayerChapter() => PlayerPrefs.GetInt(PLAYER_CHAPTER_KEY);

	// 設定玩家目前的 Chapter
	public void SetPlayerChapter(int chapter)
	{
		PlayerPrefs.SetInt(PLAYER_CHAPTER_KEY, chapter);
		PlayerPrefs.Save();
	}

	// 取得玩家目前的 Level (遊戲中的關卡等級)
	public int GetPlayerCurrentLevel() => PlayerPrefs.GetInt(PLAYER_CURRENT_LEVEL_KEY);

	// 設定玩家目前的 Level
	public void SetPlayerCurrentLevel(int level)
	{
		PlayerPrefs.SetInt(PLAYER_CURRENT_LEVEL_KEY, level);
		PlayerPrefs.Save();
	}

	

	// 取得與設定進化石數量
	public int GetCommonEvoStone() => PlayerPrefs.GetInt(COMMON_EVOSTONE_KEY);
	public void SetCommonEvoStone(int amount)
	{
		PlayerPrefs.SetInt(COMMON_EVOSTONE_KEY, Mathf.Max(0, amount));
		PlayerPrefs.Save();
	}

	public int GetRareEvoStone() => PlayerPrefs.GetInt(RARE_EVOSTONE_KEY);
	public void SetRareEvoStone(int amount)
	{
		PlayerPrefs.SetInt(RARE_EVOSTONE_KEY, Mathf.Max(0, amount));
		PlayerPrefs.Save();
	}

	public int GetSpecialEvoStone() => PlayerPrefs.GetInt(SPECIAL_EVOSTONE_KEY);
	public void SetSpecialEvoStone(int amount)
	{
		PlayerPrefs.SetInt(SPECIAL_EVOSTONE_KEY, Mathf.Max(0, amount));
		PlayerPrefs.Save();
	}

	public int GetLegendaryEvoStone() => PlayerPrefs.GetInt(LEGENDARY_EVOSTONE_KEY);
	public void SetLegendaryEvoStone(int amount)
	{
		PlayerPrefs.SetInt(LEGENDARY_EVOSTONE_KEY, Mathf.Max(0, amount));
		PlayerPrefs.Save();
	}

	// 增加金幣
	public void AddPlayerCoin(int amount)
	{
		int current = GetPlayerCoin();
		SetPlayerCoin(current + amount);
	}

	// 增加進化石（四種）
	public void AddCommonEvoStone(int amount)
	{
		int current = GetCommonEvoStone();
		SetCommonEvoStone(current + amount);
	}

	public void AddRareEvoStone(int amount)
	{
		int current = GetRareEvoStone();
		SetRareEvoStone(current + amount);
	}

	public void AddSpecialEvoStone(int amount)
	{
		int current = GetSpecialEvoStone();
		SetSpecialEvoStone(current + amount);
	}

	public void AddLegendaryEvoStone(int amount)
	{
		int current = GetLegendaryEvoStone();
		SetLegendaryEvoStone(current + amount);
	}


	// 清除玩家資料（用於測試）
	public void ResetPlayerData()
	{
		PlayerPrefs.DeleteAll();
		LoadPlayerData();
	}
}
