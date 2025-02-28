//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

public static class FightPlayer1Config
{
	public static string[] Group = { "HR00", "C05", "B02" };
	public static string NormalAttack = "HammerNormalAttack";
	public static string Ult = "HammerUlt";

	public static string PlayerSkin;
	public static int StartHP = 1500;
	public static int NowHP = 1500;
	public static int StartATK = 100;
	public static int NowATK = 100;
	public static float CriticalPercentage = 0.0f;
	public static float SkillDamageIncrease = 0.0f;
	public static float PoisonDamageIncrease = 0.0f;
	public static float BurnDamageIncrease = 0.0f;
	public static float CC_SkillTimeIncrease = 0.0f;
	public static float SkillBubbleTimeDecrease = 0.0f;
	public static string EquipSet = "None";

	public static bool instSkillP1 = true;//P1生成技能開啟
}
