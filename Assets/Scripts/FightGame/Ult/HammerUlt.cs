using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerUlt : MonoBehaviour
{
    [Header("----------------- PlayerGameObject ------------------")]
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    [Header("----------------- Data ------------------")]
    [SerializeField] private int playerNumber;
    [SerializeField] private int targetNumber;
    [SerializeField] private int skillDamage = 0; // 傷害值
    [SerializeField] private bool canInstAttack = false; // 可生成攻擊物
    [SerializeField] private int instAttackAmount = 1; // 攻擊物生成數量
    [SerializeField] private bool destroyGameObject = false; // 攻擊物生成數量

    [Header("----------------- GameObjects ------------------")]
    [SerializeField] private GameObject locationAttack;

    void Start()
    {
        locationAttack = Resources.Load<GameObject>("Prefabs/Ult/HammerRippleLarge");
    }

    // Update is called once per frame
    void Update()
    {
        if (canInstAttack)
        {
            InstAttack();
        }

        if(destroyGameObject)
        { 
            Destroy(gameObject);
        }
    }

    public void SetPlayerNumber(int pNumber)
    {
        playerNumber = pNumber;
    }

    public void SetDamage(int damage)
    {
        skillDamage = damage;
    }

    public void SetTargetNumber(int tNumber)
    {
        targetNumber = tNumber;
    }

    public void InstAttack()//透過動畫call此函數生成
    {        
        if(instAttackAmount > 0)
        {
            instAttackAmount--;
            GameObject obj = Instantiate(locationAttack, gameObject.transform.position, Quaternion.identity);
            obj.GetComponent<HammerRippleAttack>().SetDamage(skillDamage);
            obj.GetComponent<HammerRippleAttack>().SetTargetNumber(targetNumber);
        }
    }
}
