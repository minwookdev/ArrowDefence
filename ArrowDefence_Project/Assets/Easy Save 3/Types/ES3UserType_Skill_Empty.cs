using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("id", "name", "desc", "iconSprite", "level", "skillType")]
	public class ES3UserType_Skill_Empty : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Skill_Empty() : base(typeof(ActionCat.Skill_Empty)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.Skill_Empty)obj;
			
			writer.WritePrivateField("id", instance);
			writer.WritePrivateField("name", instance);
			writer.WritePrivateField("desc", instance);
			writer.WritePrivateFieldByRef("iconSprite", instance);
			writer.WritePrivateField("level", instance);
			writer.WritePrivateField("skillType", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.Skill_Empty)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "id":
					reader.SetPrivateField("id", reader.Read<System.String>(), instance);
					break;
					case "name":
					reader.SetPrivateField("name", reader.Read<System.String>(), instance);
					break;
					case "desc":
					reader.SetPrivateField("desc", reader.Read<System.String>(), instance);
					break;
					case "iconSprite":
					reader.SetPrivateField("iconSprite", reader.Read<UnityEngine.Sprite>(), instance);
					break;
					case "level":
					reader.SetPrivateField("level", reader.Read<ActionCat.SKILL_LEVEL>(), instance);
					break;
					case "skillType":
					reader.SetPrivateField("skillType", reader.Read<ActionCat.BOWSKILL_TYPE>(), instance);
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