//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusManager : MonoBehaviour
{
    [Header("----------------- Status Data ------------------")]
    [SerializeField] private int healthPoint;
    [SerializeField] private int attackDamage;
    [SerializeField] private int currentMagicPoint;

    [Header("----------------- Variable Reference ------------------")]
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
        //magicPoint = 0;
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
        return currentMagicPoint;
	}

    public void SetMagicPoint(int point)
    {
        currentMagicPoint = point;
    }

    public void GetOnePointMP()
    {
		currentMagicPoint += 1;
    }

    public void SetSkills(string[] skillArray)
    {
        skills = skillArray;
    }
}
