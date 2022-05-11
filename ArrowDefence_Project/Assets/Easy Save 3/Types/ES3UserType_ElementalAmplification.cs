using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("value", "<BuffType>k__BackingField")]
	public class ES3UserType_ElementalAmplification : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_ElementalAmplification() : base(typeof(ActionCat.ElementalAmplification)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.ElementalAmplification)obj;
			
			writer.WritePrivateField("value", instance);
			writer.WritePrivateField("<BuffType>k__BackingField", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.ElementalAmplification)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "value":
					reader.SetPrivateField("value", reader.Read<System.Single>(), instance);
					break;
					case "<BuffType>k__BackingField":
					reader.SetPrivateField("<BuffType>k__BackingField", reader.Read<ActionCat.ARROWBUFFTYPE>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.ElementalAmplification();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_ElementalAmplificationArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ElementalAmplificationArray() : base(typeof(ActionCat.ElementalAmplification[]), ES3UserType_ElementalAmplification.Instance)
		{
			Instance = this;
		}
	}
}