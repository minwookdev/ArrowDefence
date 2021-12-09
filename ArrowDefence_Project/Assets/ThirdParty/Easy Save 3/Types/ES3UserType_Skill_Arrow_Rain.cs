using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("arrowCount", "shotDelay", "isActiveDebugLine", "isDrawLine", "debugLineRender", "id", "name", "desc", "iconSprite", "level", "skillType")]
	public class ES3UserType_Skill_Arrow_Rain : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Skill_Arrow_Rain() : base(typeof(ActionCat.Skill_Arrow_Rain)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.Skill_Arrow_Rain)obj;
			
			writer.WritePrivateField("arrowCount", instance);
			writer.WritePrivateField("shotDelay", instance);
			writer.WritePrivateField("isActiveDebugLine", instance);
			writer.WritePrivateField("isDrawLine", instance);
			writer.WritePrivateFieldByRef("debugLineRender", instance);
			writer.WritePrivateField("id", instance);
			writer.WritePrivateField("name", instance);
			writer.WritePrivateField("desc", instance);
			writer.WritePrivateFieldByRef("iconSprite", instance);
			writer.WritePrivateField("level", instance);
			writer.WritePrivateField("skillType", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.Skill_Arrow_Rain)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "arrowCount":
					reader.SetPrivateField("arrowCount", reader.Read<System.Byte>(), instance);
					break;
					case "shotDelay":
					reader.SetPrivateField("shotDelay", reader.Read<System.Single>(), instance);
					break;
					case "isActiveDebugLine":
					reader.SetPrivateField("isActiveDebugLine", reader.Read<System.Boolean>(), instance);
					break;
					case "isDrawLine":
					reader.SetPrivateField("isDrawLine", reader.Read<System.Boolean>(), instance);
					break;
					case "debugLineRender":
					reader.SetPrivateField("debugLineRender", reader.Read<ActionCat.ArrowRain_LineRender>(), instance);
					break;
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
			var instance = new ActionCat.Skill_Arrow_Rain();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_Skill_Arrow_RainArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_Skill_Arrow_RainArray() : base(typeof(ActionCat.Skill_Arrow_Rain[]), ES3UserType_Skill_Arrow_Rain.Instance)
		{
			Instance = this;
		}
	}
}