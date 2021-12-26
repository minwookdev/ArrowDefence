using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("damageCount")]
	public class ES3UserType_AbilityDamage : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_AbilityDamage() : base(typeof(ActionCat.AbilityDamage)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.AbilityDamage)obj;
			
			writer.WritePrivateField("damageCount", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.AbilityDamage)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "damageCount":
					reader.SetPrivateField("damageCount", reader.Read<System.Int16>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.AbilityDamage();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_AbilityDamageArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_AbilityDamageArray() : base(typeof(ActionCat.AbilityDamage[]), ES3UserType_AbilityDamage.Instance)
		{
			Instance = this;
		}
	}
}