using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("increaseValue", "abilityType")]
	public class ES3UserType_AdditionalFire : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_AdditionalFire() : base(typeof(ActionCat.AdditionalFire)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.AdditionalFire)obj;
			
			writer.WritePrivateField("increaseValue", instance);
			writer.WritePrivateField("abilityType", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.AdditionalFire)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "increaseValue":
					reader.SetPrivateField("increaseValue", reader.Read<System.Byte>(), instance);
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
			var instance = new ActionCat.AdditionalFire();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_AdditionalFireArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_AdditionalFireArray() : base(typeof(ActionCat.AdditionalFire[]), ES3UserType_AdditionalFire.Instance)
		{
			Instance = this;
		}
	}
}