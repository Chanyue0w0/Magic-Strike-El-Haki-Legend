//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

public static class FightPlayer2Config
{
	public static string[] Group = { "MS00", "SK01", "SK01" };
	public static string NormalAttack = "OriginSlimeNormalAttack";
	public static string Ult = "HammerUlt";

	public static string PlayerSkin;
	public static int StartHP = 1000;
	public static int NowHP = 1000;
	public static int StartATK = 80;
	public static int NowATK = 80;
	public static float CriticalPercentage = 0.0f;
	public static float SkillDamageIncrease = 0.0f;
	public static float PoisonDamageIncrease = 0.0f;
	public static float BurnDamageIncrease = 0.0f;
	public static float CC_SkillTimeIncrease = 0.0f;
	public static float SkillBubbleTimeDecrease = 0.0f;
	public static string EquipSet = "None";

	public static float ShieldPercentage = 0.0f;//減傷比例，戰鬥中才會取得
	public static bool instSkillP2 = false;//P2生成技能開啟
}
