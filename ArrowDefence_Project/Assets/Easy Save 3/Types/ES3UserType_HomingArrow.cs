using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("searchInterval", "scanRadius", "speed", "rotateSpeed")]
	public class ES3UserType_HomingArrow : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_HomingArrow() : base(typeof(ActionCat.HomingArrow)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.HomingArrow)obj;
			
			writer.WritePrivateField("searchInterval", instance);
			writer.WritePrivateField("scanRadius", instance);
			writer.WritePrivateField("speed", instance);
			writer.WritePrivateField("rotateSpeed", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.HomingArrow)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "searchInterval":
					reader.SetPrivateField("searchInterval", reader.Read<System.Single>(), instance);
					break;
					case "scanRadius":
					reader.SetPrivateField("scanRadius", reader.Read<System.Single>(), instance);
					break;
					case "speed":
					reader.SetPrivateField("speed", reader.Read<System.Single>(), instance);
					break;
					case "rotateSpeed":
					reader.SetPrivateField("rotateSpeed", reader.Read<System.Single>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.HomingArrow();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_HomingArrowArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_HomingArrowArray() : base(typeof(ActionCat.HomingArrow[]), ES3UserType_HomingArrow.Instance)
		{
			Instance = this;
		}
	}
}