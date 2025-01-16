using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("increaseRate", "abilityType")]
	public class ES3UserType_IncArrowDamageRate : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_IncArrowDamageRate() : base(typeof(ActionCat.IncArrowDamageRate)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.IncArrowDamageRate)obj;
			
			writer.WritePrivateField("increaseRate", instance);
			writer.WritePrivateField("abilityType", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.IncArrowDamageRate)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "increaseRate":
					reader.SetPrivateField("increaseRate", reader.Read<System.Single>(), instance);
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
			var instance = new ActionCat.IncArrowDamageRate();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_IncArrowDamageRateArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_IncArrowDamageRateArray() : base(typeof(ActionCat.IncArrowDamageRate[]), ES3UserType_IncArrowDamageRate.Instance)
		{
			Instance = this;
		}
	}
}