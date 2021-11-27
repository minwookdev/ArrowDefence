using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("id", "name", "desc", "level", "type", "activeType", "iconSprite", "skillData")]
	public class ES3UserType_ASInfo : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_ASInfo() : base(typeof(ActionCat.ASInfo)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.ASInfo)obj;
			
			writer.WritePrivateField("id", instance);
			writer.WritePrivateField("name", instance);
			writer.WritePrivateField("desc", instance);
			writer.WritePrivateField("level", instance);
			writer.WritePrivateField("type", instance);
			writer.WritePrivateField("activeType", instance);
			writer.WritePrivateFieldByRef("iconSprite", instance);
			writer.WritePrivateField("skillData", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.ASInfo)obj;
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
					case "level":
					reader.SetPrivateField("level", reader.Read<ActionCat.SKILL_LEVEL>(), instance);
					break;
					case "type":
					reader.SetPrivateField("type", reader.Read<ActionCat.ARROWSKILL>(), instance);
					break;
					case "activeType":
					reader.SetPrivateField("activeType", reader.Read<ActionCat.ARROWSKILL_ACTIVETYPE>(), instance);
					break;
					case "iconSprite":
					reader.SetPrivateField("iconSprite", reader.Read<UnityEngine.Sprite>(), instance);
					break;
					case "skillData":
					reader.SetPrivateField("skillData", reader.Read<ActionCat.ArrowSkill>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.ASInfo();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_ASInfoArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ASInfoArray() : base(typeof(ActionCat.ASInfo[]), ES3UserType_ASInfo.Instance)
		{
			Instance = this;
		}
	}
}