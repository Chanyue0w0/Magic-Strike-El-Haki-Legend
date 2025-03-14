//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

public static class FightPlayer1Config
{
	public static string[] Group = { "HR00", "SK00", "SK01" };
	public static string NormalAttack = "HammerNormalAttack";
	public static string Ult = "HammerUlt";

	public static string PlayerSkin = "BigHammerLionRex";
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

	public static float ShieldPercentage = 0.0f;//減傷比例，戰鬥中才會取得
	public static bool instSkillP1 = true;//P1生成技能開啟
}
