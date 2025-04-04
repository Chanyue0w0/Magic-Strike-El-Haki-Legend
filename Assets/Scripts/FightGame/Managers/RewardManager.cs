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


    }

    private void RewardCoins(int chapter,int level)
    {
        int coins = chapter * level * 10;
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
