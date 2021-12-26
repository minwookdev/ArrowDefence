using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("incDamageRate")]
	public class ES3UserType_AbilityIncDamageRate : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_AbilityIncDamageRate() : base(typeof(ActionCat.AbilityIncDamageRate)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.AbilityIncDamageRate)obj;
			
			writer.WritePrivateField("incDamageRate", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.AbilityIncDamageRate)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "incDamageRate":
					reader.SetPrivateField("incDamageRate", reader.Read<System.Single>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.AbilityIncDamageRate();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_AbilityIncDamageRateArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_AbilityIncDamageRateArray() : base(typeof(ActionCat.AbilityIncDamageRate[]), ES3UserType_AbilityIncDamageRate.Instance)
		{
			Instance = this;
		}
	}
}