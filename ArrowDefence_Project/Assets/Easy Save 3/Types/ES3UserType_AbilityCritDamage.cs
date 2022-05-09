using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("increaseValue", "abilityType")]
	public class ES3UserType_AbilityCritDamage : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_AbilityCritDamage() : base(typeof(ActionCat.AbilityCritDamage)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.AbilityCritDamage)obj;
			
			writer.WritePrivateField("increaseValue", instance);
			writer.WritePrivateField("abilityType", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.AbilityCritDamage)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "increaseValue":
					reader.SetPrivateField("increaseValue", reader.Read<System.Single>(), instance);
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
			var instance = new ActionCat.AbilityCritDamage();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_AbilityCritDamageArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_AbilityCritDamageArray() : base(typeof(ActionCat.AbilityCritDamage[]), ES3UserType_AbilityCritDamage.Instance)
		{
			Instance = this;
		}
	}
}