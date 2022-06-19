using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("maxChainCount", "scanRange", "effects", "sounds")]
	public class ES3UserType_ReboundArrow : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_ReboundArrow() : base(typeof(ActionCat.ReboundArrow)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.ReboundArrow)obj;
			
			writer.WritePrivateField("maxChainCount", instance);
			writer.WritePrivateField("scanRange", instance);
			writer.WritePrivateField("effects", instance);
			writer.WritePrivateField("sounds", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.ReboundArrow)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "maxChainCount":
					instance = (ActionCat.ReboundArrow)reader.SetPrivateField("maxChainCount", reader.Read<System.Int32>(), instance);
					break;
					case "scanRange":
					instance = (ActionCat.ReboundArrow)reader.SetPrivateField("scanRange", reader.Read<System.Single>(), instance);
					break;
					case "effects":
					instance = (ActionCat.ReboundArrow)reader.SetPrivateField("effects", reader.Read<ActionCat.ACEffector2D[]>(), instance);
					break;
					case "sounds":
					instance = (ActionCat.ReboundArrow)reader.SetPrivateField("sounds", reader.Read<UnityEngine.AudioClip[]>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.ReboundArrow();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_ReboundArrowArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ReboundArrowArray() : base(typeof(ActionCat.ReboundArrow[]), ES3UserType_ReboundArrow.Instance)
		{
			Instance = this;
		}
	}
}