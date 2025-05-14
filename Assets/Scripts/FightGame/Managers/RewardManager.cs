using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance { get; private set; }

    public Transform rewardContainer; // 在 Inspector 指定 UI 的容器

    private GameObject coinPrefab;
    private GameObject diamondPrefab;
    private GameObject helmetPrefab;
    private GameObject armorPrefab;
    private GameObject shoesPrefab;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // 從 Resources 載入三種獎勵 prefab
        coinPrefab = Resources.Load<GameObject>("Prefabs/RewardIcon/CoinRewardIcon");
        diamondPrefab = Resources.Load<GameObject>("Prefabs/RewardIcon/DiamondRewardIcon");
        helmetPrefab = Resources.Load<GameObject>("Prefabs/RewardIcon/HelmetRewardIcon");
        armorPrefab = Resources.Load<GameObject>("Prefabs/RewardIcon/ArmorRewardIcon");
        shoesPrefab = Resources.Load<GameObject>("Prefabs/RewardIcon/ShoesRewardIcon");

        if (coinPrefab == null || diamondPrefab == null || helmetPrefab == null)
        {
            Debug.LogError("RewardIcon prefabs missing! Check Resources/Prefabs/RewardIcon folder.");
        }
    }

    public void GenerateReward()
    {
        if (coinPrefab == null) return;

        int chapter = FightPlayer1Config.CurrentChapter;
        int level = FightPlayer1Config.CurrentLevel;
        
        RewardCoins(chapter,level);

        RewardEquipment(chapter, level);

        //PlayerDataManager.Instance.SetPlayerChapter();
    }

    private void RewardEquipment(int chapter, int level)
    {
        // 1/3 機率判斷
        if (Random.Range(0, 0) != 0)
        {
            Debug.Log("未獲得裝備");
            return;
        }

        // 隨機選裝備類型
        int type = Random.Range(0, 3); // 0 = helmet, 1 = armor, 2 = shoes
        GameObject prefabToUse = null;
        string equipmentID = "HT00";

        switch (type)
        {
            case 0:
                prefabToUse = helmetPrefab;
                equipmentID = "HT00";
                break;
            case 1:
                prefabToUse = armorPrefab;
                equipmentID = "BD00";
                break;
            case 2:
                prefabToUse = shoesPrefab;
                equipmentID = "SH00";
                break;
        }

        if (prefabToUse == null)
        {
            Debug.LogWarning("裝備 prefab 尚未設定！");
            return;
        }

        GameObject rewardObj = Instantiate(prefabToUse, rewardContainer);

        Text amountText = rewardObj.transform.Find("AmountText")?.GetComponent<Text>();
        if (amountText != null)
        {
            amountText.text = "1";
        }
        else
        {
            Debug.LogWarning("AmountText not found in CoinRewardIcon prefab.");
        }

        // 儲存給玩家（假設你有這個方法）
        PlayerEquipmentManager.Instance.CreateEquipmentFromData(equipmentID);
    }


    private void RewardCoins(int chapter,int level)
    {
        //int coins = chapter * level * 10 * (2^FightPlayer1Config.coinBonus);
        int coins = chapter * level * 10 * (int)Mathf.Pow(2, FightPlayer1Config.coinBonus);

        FightPlayer1Config.coinBonus = 0; //在RewardManager 重製
        //int coins = chapter * level * 10 * (1 << FightPlayer1Config.coinBonus);


        GameObject rewardObj = Instantiate(coinPrefab, rewardContainer);

        // 修改子物件中的文字
        Text amountText = rewardObj.transform.Find("AmountText")?.GetComponent<Text>();
        if (amountText != null)
        {
            amountText.text = coins.ToString();
        }
        else
        {
            Debug.LogWarning("AmountText not found in CoinRewardIcon prefab.");
        }

        PlayerDataManager.Instance.AddPlayerCoin(coins);  //存入玩家資料
    }

}
