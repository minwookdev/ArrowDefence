using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("maxChainCount", "effects")]
	public class ES3UserType_PiercingArrow : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_PiercingArrow() : base(typeof(ActionCat.PiercingArrow)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.PiercingArrow)obj;
			
			writer.WriteProperty("maxChainCount", instance.maxChainCount, ES3Type_byte.Instance);
			writer.WritePrivateField("effects", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.PiercingArrow)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "maxChainCount":
						instance.maxChainCount = reader.Read<System.Byte>(ES3Type_byte.Instance);
						break;
					case "effects":
					reader.SetPrivateField("effects", reader.Read<ActionCat.ACEffector2D[]>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.PiercingArrow();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_PiercingArrowArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_PiercingArrowArray() : base(typeof(ActionCat.PiercingArrow[]), ES3UserType_PiercingArrow.Instance)
		{
			Instance = this;
		}
	}
}