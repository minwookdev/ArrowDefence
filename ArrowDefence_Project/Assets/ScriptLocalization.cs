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
			public static string ADDITIONAL_FIRE 		{ get{ return LocalizationManager.GetTranslation ("ABILITY/ADDITIONAL_FIRE"); } }
			public static string ARMOR_PENETRATION 		{ get{ return LocalizationManager.GetTranslation ("ABILITY/ARMOR_PENETRATION"); } }
			public static string ELEMENTAL_ACTIVATION 		{ get{ return LocalizationManager.GetTranslation ("ABILITY/ELEMENTAL_ACTIVATION"); } }
		}

		public static class Common
		{
			public static string RANGE_LARGE 		{ get{ return LocalizationManager.GetTranslation ("Common/RANGE_LARGE"); } }
			public static string RANGE_MEDIUM 		{ get{ return LocalizationManager.GetTranslation ("Common/RANGE_MEDIUM"); } }
			public static string RANGE_SMALL 		{ get{ return LocalizationManager.GetTranslation ("Common/RANGE_SMALL"); } }
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
		    public const string ADDITIONAL_FIRE = "ABILITY/ADDITIONAL_FIRE";
		    public const string ARMOR_PENETRATION = "ABILITY/ARMOR_PENETRATION";
		    public const string ELEMENTAL_ACTIVATION = "ABILITY/ELEMENTAL_ACTIVATION";
		}

		public static class Common
		{
		    public const string RANGE_LARGE = "Common/RANGE_LARGE";
		    public const string RANGE_MEDIUM = "Common/RANGE_MEDIUM";
		    public const string RANGE_SMALL = "Common/RANGE_SMALL";
		}
	}
}