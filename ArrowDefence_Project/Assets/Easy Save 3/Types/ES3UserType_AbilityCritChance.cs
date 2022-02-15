using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("critHitChance", "abilityType")]
	public class ES3UserType_AbilityCritChance : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_AbilityCritChance() : base(typeof(ActionCat.AbilityCritChance)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.AbilityCritChance)obj;
			
			writer.WritePrivateField("critHitChance", instance);
			writer.WritePrivateField("abilityType", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.AbilityCritChance)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "critHitChance":
					reader.SetPrivateField("critHitChance", reader.Read<System.Byte>(), instance);
					break;
					case "abilityType":
					reader.SetPrivateField("abilityType", reader.Read<ActionCat.ABILITY_TYPE>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.AbilityCritChance();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_AbilityCritChanceArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_AbilityCritChanceArray() : base(typeof(ActionCat.AbilityCritChance[]), ES3UserType_AbilityCritChance.Instance)
		{
			Instance = this;
		}
	}
}