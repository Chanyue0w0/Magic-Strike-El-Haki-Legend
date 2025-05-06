using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSkillManager : MonoBehaviour
{
    public static PassiveSkillManager Instance { get; private set; }

    [SerializeField] private GameObject iceBallEffectPrefab;
    [SerializeField] private GameObject burningBallEffectPrefab;

    private GameObject ball;
    private bool iceBall = false;
    private bool burningBall = false;
    private bool iceGroundExplosion = false;
    private bool burnStrengthen = false;

    void Awake()
    {
        // 確保 Singleton 唯一性
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");

        // 假設 FightPlayer1Config 是 Singleton 且有 PassiveSkillGroup 屬性

        string[] passiveSkills = FightPlayer1Config.PassiveEffectGroup;

        //IceBall
        if (System.Array.Exists(passiveSkills, s => s == "PS01"))
        {
            iceBall = true;
            if (iceBallEffectPrefab != null && ball != null)
            {
                GameObject obj = Instantiate(iceBallEffectPrefab, ball.transform.position, Quaternion.identity, ball.transform);
                obj.transform.SetParent(ball.transform);
            }
        }

        //BurningBall
        if (System.Array.Exists(passiveSkills, s => s == "PS02"))
        {
            burningBall = true;
            if (burningBallEffectPrefab != null && ball != null)
            {
                GameObject obj = Instantiate(burningBallEffectPrefab, ball.transform.position, Quaternion.identity, ball.transform);
                obj.transform.SetParent(ball.transform);
            }
        }

        //IceGroundExplosion
        if (System.Array.Exists(passiveSkills, s => s == "PS03"))
        {
            Debug.Log("Has PS03");
            iceGroundExplosion = true;
        }

        //BurnStrengthen
        if (System.Array.Exists(passiveSkills, s => s == "PS04"))
        {
            Debug.Log("Has PS04");
            burnStrengthen = true;
            FightPlayer1Config.BurnDamageIncrease += 1f;
        }
    }

    //重製RogueLike所有效果
    public void ResetAllEffect()
    {
        iceBall = false;
        burningBall = false;
        iceGroundExplosion = false;
        burnStrengthen = false;
        FightPlayer1Config.BurnDamageIncrease = 0;
        FightPlayer1Config.PassiveEffectGroup = new string[] { "PS00", "PS00" };
    }

    public bool HasIceBall()
    {
        return iceBall;
    }

    public bool HasBurningBall()
    {
        return burningBall;
    }
    
    public bool HasIceGroundExplosion()
    {
        return iceGroundExplosion;
    }

    public bool HasBurnStrengthen()
    {
        return burnStrengthen;
    }
}
