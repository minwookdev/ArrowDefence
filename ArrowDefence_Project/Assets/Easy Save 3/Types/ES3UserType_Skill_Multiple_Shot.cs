using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("arrowCount", "spreadAngle")]
	public class ES3UserType_Skill_Multiple_Shot : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Skill_Multiple_Shot() : base(typeof(CodingCat_Games.Skill_Multiple_Shot)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (CodingCat_Games.Skill_Multiple_Shot)obj;
			
			writer.WritePrivateField("arrowCount", instance);
			writer.WritePrivateField("spreadAngle", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (CodingCat_Games.Skill_Multiple_Shot)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "arrowCount":
					reader.SetPrivateField("arrowCount", reader.Read<System.Byte>(), instance);
					break;
					case "spreadAngle":
					reader.SetPrivateField("spreadAngle", reader.Read<System.Single>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new CodingCat_Games.Skill_Multiple_Shot();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_Skill_Multiple_ShotArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_Skill_Multiple_ShotArray() : base(typeof(CodingCat_Games.Skill_Multiple_Shot[]), ES3UserType_Skill_Multiple_Shot.Instance)
		{
			Instance = this;
		}
	}
}