using UnityEngine;

namespace I2.Loc
{
	public static class ScriptLocalization
	{

		public static string Battle 		{ get{ return LocalizationManager.GetTranslation ("Battle"); } }
		public static string Craft 		{ get{ return LocalizationManager.GetTranslation ("Craft"); } }
		public static string Items 		{ get{ return LocalizationManager.GetTranslation ("Items"); } }
		public static string Shop 		{ get{ return LocalizationManager.GetTranslation ("Shop"); } }
	}

    public static class ScriptTerms
	{

		public const string Battle = "Battle";
		public const string Craft = "Craft";
		public const string Items = "Items";
		public const string Shop = "Shop";
	}
}