//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusManager : MonoBehaviour
{
    [Header("----------------- Status Data ------------------")]
    [SerializeField] private int healthPoint;
    [SerializeField] private int attackDamage;
    [SerializeField] private int magicPoint;

    [Header("----------------- Status Data ------------------")]
    [SerializeField] private string[] skills;
	//[Header("----------------- Script Reference ------------------")]
	// Start is called before the first frame update
	void Start()
    {
        InitStatus();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitStatus()
    {
        // health = get hp
        // dameage = get hp
        magicPoint = 4;
        // get skills
    }

    public void GetDamage(int atk)
    {
        healthPoint -= atk;
    }

    public void GetRecoverHP(int recoverHp)
    {
        healthPoint += recoverHp;
    }

    public int GetHP()
    {
        return healthPoint;
    }

    public void SetHP(int hp)
    {
        healthPoint = hp;
    }
    public int GetBasicATK()
    {
        return attackDamage;
    }

    public int GetMagicPoint()
    {
        return magicPoint;
    }

    public void SetMagicPoint(int point)
    {
        magicPoint = point;
    }

    public void GetOnePointMP()
    {
        magicPoint += 1;
    }
}
