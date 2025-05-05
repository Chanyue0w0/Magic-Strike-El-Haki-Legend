using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingMagic : MonoBehaviour
{
    [SerializeField] private int playerNumber = 1;
    //[SerializeField] private float OriginalShieldPercentage = 0.0f;
    [SerializeField] private int baseSkillHealingAmount = 100;
    private int skillHealingAmount = 100;
    //[SerializeField] private int skillMoveSpeed = 10;
    [SerializeField] private GameObject healingEffect;
    [SerializeField] private GameObject healingUIEffect;
    [SerializeField] private GameObject player1MagicPointPanel;


    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;


    public void SetPlayerNumber(int pNumber) // initial
    {
        playerNumber = pNumber;
        InitializedSkillInfo();
    }

    public void InitializedSkillInfo()
    {
        //float skillDamageTMP = 0;
        //待後續新增治療量增加
        //if (playerNumber == 1)
        //{
        //    skillDamageTMP = baseSkillDamage * (1 + FightPlayer1Config.SkillDamageIncrease);
        //}
        //else
        //{
        //    skillDamageTMP = baseSkillDamage * (1 + FightPlayer2Config.SkillDamageIncrease);
        //}
        skillHealingAmount = Mathf.RoundToInt(baseSkillHealingAmount);
        healingEffect = Resources.Load<GameObject>("Prefabs/Effect/HealingEffect");
        healingUIEffect = Resources.Load<GameObject>("Prefabs/Effect/HealingUIEffect");
        player1 = GameObject.FindGameObjectWithTag("Player1");
        player2 = GameObject.FindGameObjectWithTag("Player2");

        player1MagicPointPanel = GameObject.Find("P1 Magic Point Panel");
    }

    public void Active()
    {
        if (playerNumber == 1)
        {
            IDamageable damageable = player1.GetComponent<IDamageable>();
            damageable.GetRecoverHP(skillHealingAmount);


            GameObject CFS = Instantiate(healingEffect, player1.transform.position, Quaternion.identity);
            CFS.transform.SetParent(player1.transform);

            GameObject CFS2 = Instantiate(healingUIEffect);
            CFS2.transform.SetParent(player1MagicPointPanel.transform, false); // 保持 localPosition、scale
            CFS2.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // 設定在面板中心或指定位置
            //Vector3 panelWorldPos = player1MagicPointPanel.transform.position; // UI 世界位置
            //GameObject CFS2 = Instantiate(healingUIEffect, panelWorldPos, Quaternion.identity);


        }
        //else
        //{
        //    IDamageable damageable = player2.GetComponent<IDamageable>();
        //    damageable.GetRecoverHP(skillHealingAmount);

        //    GameObject CFS = Instantiate(healingEffect, player2.transform.position, Quaternion.identity);
        //    CFS.transform.SetParent(player2.transform);
        //}
    }


}
