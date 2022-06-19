using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("projectileCount", "projectilePref", "projectileDamage", "sounds")]
	public class ES3UserType_SplitDagger : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_SplitDagger() : base(typeof(ActionCat.SplitDagger)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.SplitDagger)obj;
			
			writer.WritePrivateField("projectileCount", instance);
			writer.WritePrivateFieldByRef("projectilePref", instance);
			writer.WritePrivateField("projectileDamage", instance);
			writer.WritePrivateField("sounds", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.SplitDagger)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "projectileCount":
					instance = (ActionCat.SplitDagger)reader.SetPrivateField("projectileCount", reader.Read<System.Int32>(), instance);
					break;
					case "projectilePref":
					instance = (ActionCat.SplitDagger)reader.SetPrivateField("projectilePref", reader.Read<ActionCat.ProjectilePref>(), instance);
					break;
					case "projectileDamage":
					instance = (ActionCat.SplitDagger)reader.SetPrivateField("projectileDamage", reader.Read<System.Int16>(), instance);
					break;
					case "sounds":
					instance = (ActionCat.SplitDagger)reader.SetPrivateField("sounds", reader.Read<UnityEngine.AudioClip[]>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.SplitDagger();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_SplitDaggerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_SplitDaggerArray() : base(typeof(ActionCat.SplitDagger[]), ES3UserType_SplitDagger.Instance)
		{
			Instance = this;
		}
	}
}