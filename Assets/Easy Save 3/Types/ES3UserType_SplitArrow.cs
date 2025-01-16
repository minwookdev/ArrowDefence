using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("projectilePref", "projectileDamage")]
	public class ES3UserType_SplitArrow : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_SplitArrow() : base(typeof(ActionCat.SplitArrow)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.SplitArrow)obj;
			
			writer.WritePrivateFieldByRef("projectilePref", instance);
			writer.WritePrivateField("projectileDamage", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.SplitArrow)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "projectilePref":
					reader.SetPrivateField("projectilePref", reader.Read<ActionCat.ProjectilePref>(), instance);
					break;
					case "projectileDamage":
					reader.SetPrivateField("projectileDamage", reader.Read<System.Int16>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.SplitArrow();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_SplitArrowArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_SplitArrowArray() : base(typeof(ActionCat.SplitArrow[]), ES3UserType_SplitArrow.Instance)
		{
			Instance = this;
		}
	}
}