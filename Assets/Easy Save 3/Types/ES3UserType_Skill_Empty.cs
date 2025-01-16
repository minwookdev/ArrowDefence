using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("id", "iconSprite", "level", "skillType", "termsName", "termsDesc")]
	public class ES3UserType_Skill_Empty : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Skill_Empty() : base(typeof(ActionCat.Skill_Empty)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.Skill_Empty)obj;
			
			writer.WritePrivateField("id", instance);
			writer.WritePrivateFieldByRef("iconSprite", instance);
			writer.WritePrivateField("level", instance);
			writer.WritePrivateField("skillType", instance);
			writer.WritePrivateField("termsName", instance);
			writer.WritePrivateField("termsDesc", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.Skill_Empty)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "id":
					instance = (ActionCat.Skill_Empty)reader.SetPrivateField("id", reader.Read<System.String>(), instance);
					break;
					case "iconSprite":
					instance = (ActionCat.Skill_Empty)reader.SetPrivateField("iconSprite", reader.Read<UnityEngine.Sprite>(), instance);
					break;
					case "level":
					instance = (ActionCat.Skill_Empty)reader.SetPrivateField("level", reader.Read<ActionCat.SKILL_LEVEL>(), instance);
					break;
					case "skillType":
					instance = (ActionCat.Skill_Empty)reader.SetPrivateField("skillType", reader.Read<ActionCat.BOWSKILL_TYPE>(), instance);
					break;
					case "termsName":
					instance = (ActionCat.Skill_Empty)reader.SetPrivateField("termsName", reader.Read<System.String>(), instance);
					break;
					case "termsDesc":
					instance = (ActionCat.Skill_Empty)reader.SetPrivateField("termsDesc", reader.Read<System.String>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.Skill_Empty();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_Skill_EmptyArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_Skill_EmptyArray() : base(typeof(ActionCat.Skill_Empty[]), ES3UserType_Skill_Empty.Instance)
		{
			Instance = this;
		}
	}
}