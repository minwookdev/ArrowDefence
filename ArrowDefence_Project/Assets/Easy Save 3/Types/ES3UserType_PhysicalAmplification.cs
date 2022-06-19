using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("value", "<BuffType>k__BackingField", "sounds")]
	public class ES3UserType_PhysicalAmplification : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_PhysicalAmplification() : base(typeof(ActionCat.PhysicalAmplification)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.PhysicalAmplification)obj;
			
			writer.WritePrivateField("value", instance);
			writer.WritePrivateField("<BuffType>k__BackingField", instance);
			writer.WritePrivateField("sounds", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.PhysicalAmplification)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "value":
					instance = (ActionCat.PhysicalAmplification)reader.SetPrivateField("value", reader.Read<System.Single>(), instance);
					break;
					case "<BuffType>k__BackingField":
					instance = (ActionCat.PhysicalAmplification)reader.SetPrivateField("<BuffType>k__BackingField", reader.Read<ActionCat.ARROWBUFFTYPE>(), instance);
					break;
					case "sounds":
					instance = (ActionCat.PhysicalAmplification)reader.SetPrivateField("sounds", reader.Read<UnityEngine.AudioClip[]>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.PhysicalAmplification();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_PhysicalAmplificationArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_PhysicalAmplificationArray() : base(typeof(ActionCat.PhysicalAmplification[]), ES3UserType_PhysicalAmplification.Instance)
		{
			Instance = this;
		}
	}
}