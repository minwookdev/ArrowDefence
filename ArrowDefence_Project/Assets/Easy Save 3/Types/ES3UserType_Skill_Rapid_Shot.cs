using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("arrowCount")]
	public class ES3UserType_Skill_Rapid_Shot : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Skill_Rapid_Shot() : base(typeof(ActionCat.Skill_Rapid_Shot)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.Skill_Rapid_Shot)obj;
			
			writer.WritePrivateField("arrowCount", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.Skill_Rapid_Shot)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "arrowCount":
					reader.SetPrivateField("arrowCount", reader.Read<System.Byte>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.Skill_Rapid_Shot();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_Skill_Rapid_ShotArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_Skill_Rapid_ShotArray() : base(typeof(ActionCat.Skill_Rapid_Shot[]), ES3UserType_Skill_Rapid_Shot.Instance)
		{
			Instance = this;
		}
	}
}