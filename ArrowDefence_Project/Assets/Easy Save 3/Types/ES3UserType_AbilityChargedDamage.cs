using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("chargedShotDamageMulti", "abilityType")]
	public class ES3UserType_AbilityChargedDamage : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_AbilityChargedDamage() : base(typeof(ActionCat.AbilityChargedDamage)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.AbilityChargedDamage)obj;
			
			writer.WritePrivateField("chargedShotDamageMulti", instance);
			writer.WritePrivateField("abilityType", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.AbilityChargedDamage)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "chargedShotDamageMulti":
					reader.SetPrivateField("chargedShotDamageMulti", reader.Read<System.Single>(), instance);
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
			var instance = new ActionCat.AbilityChargedDamage();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_AbilityChargedDamageArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_AbilityChargedDamageArray() : base(typeof(ActionCat.AbilityChargedDamage[]), ES3UserType_AbilityChargedDamage.Instance)
		{
			Instance = this;
		}
	}
}