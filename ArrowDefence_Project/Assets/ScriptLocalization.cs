using UnityEngine;

namespace I2.Loc
{
	public static class ScriptLocalization
	{

		public static class ABILITY
		{
			public static string DAMAGE 		{ get{ return LocalizationManager.GetTranslation ("ABILITY/DAMAGE"); } }
			public static string CHARGEDSHOT_DMG 		{ get{ return LocalizationManager.GetTranslation ("ABILITY/CHARGEDSHOT_DMG"); } }
			public static string CRIT_CHANCE 		{ get{ return LocalizationManager.GetTranslation ("ABILITY/CRIT_CHANCE"); } }
			public static string CRIT_DAMAGE 		{ get{ return LocalizationManager.GetTranslation ("ABILITY/CRIT_DAMAGE"); } }
			public static string ARROWSPEED 		{ get{ return LocalizationManager.GetTranslation ("ABILITY/ARROWSPEED"); } }
			public static string INC_DAMAGE_RATE 		{ get{ return LocalizationManager.GetTranslation ("ABILITY/INC_DAMAGE_RATE"); } }
			public static string PROJECTILE_DAMAGE 		{ get{ return LocalizationManager.GetTranslation ("ABILITY/PROJECTILE_DAMAGE"); } }
			public static string SPELL_DAMAGE 		{ get{ return LocalizationManager.GetTranslation ("ABILITY/SPELL_DAMAGE"); } }
		}
	}

    public static class ScriptTerms
	{

		public static class ABILITY
		{
		    public const string DAMAGE = "ABILITY/DAMAGE";
		    public const string CHARGEDSHOT_DMG = "ABILITY/CHARGEDSHOT_DMG";
		    public const string CRIT_CHANCE = "ABILITY/CRIT_CHANCE";
		    public const string CRIT_DAMAGE = "ABILITY/CRIT_DAMAGE";
		    public const string ARROWSPEED = "ABILITY/ARROWSPEED";
		    public const string INC_DAMAGE_RATE = "ABILITY/INC_DAMAGE_RATE";
		    public const string PROJECTILE_DAMAGE = "ABILITY/PROJECTILE_DAMAGE";
		    public const string SPELL_DAMAGE = "ABILITY/SPELL_DAMAGE";
		}
	}
}