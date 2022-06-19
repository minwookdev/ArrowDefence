using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("arrowCount", "shotDelay", "id", "iconSprite", "level", "skillType", "termsName", "termsDesc", "soundEffects")]
	public class ES3UserType_Skill_Arrow_Rain : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Skill_Arrow_Rain() : base(typeof(ActionCat.Skill_Arrow_Rain)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.Skill_Arrow_Rain)obj;
			
			writer.WritePrivateField("arrowCount", instance);
			writer.WritePrivateField("shotDelay", instance);
			writer.WritePrivateField("id", instance);
			writer.WritePrivateFieldByRef("iconSprite", instance);
			writer.WritePrivateField("level", instance);
			writer.WritePrivateField("skillType", instance);
			writer.WritePrivateField("termsName", instance);
			writer.WritePrivateField("termsDesc", instance);
			writer.WritePrivateField("soundEffects", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.Skill_Arrow_Rain)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "arrowCount":
					instance = (ActionCat.Skill_Arrow_Rain)reader.SetPrivateField("arrowCount", reader.Read<System.Byte>(), instance);
					break;
					case "shotDelay":
					instance = (ActionCat.Skill_Arrow_Rain)reader.SetPrivateField("shotDelay", reader.Read<System.Single>(), instance);
					break;
					case "id":
					instance = (ActionCat.Skill_Arrow_Rain)reader.SetPrivateField("id", reader.Read<System.String>(), instance);
					break;
					case "iconSprite":
					instance = (ActionCat.Skill_Arrow_Rain)reader.SetPrivateField("iconSprite", reader.Read<UnityEngine.Sprite>(), instance);
					break;
					case "level":
					instance = (ActionCat.Skill_Arrow_Rain)reader.SetPrivateField("level", reader.Read<ActionCat.SKILL_LEVEL>(), instance);
					break;
					case "skillType":
					instance = (ActionCat.Skill_Arrow_Rain)reader.SetPrivateField("skillType", reader.Read<ActionCat.BOWSKILL_TYPE>(), instance);
					break;
					case "termsName":
					instance = (ActionCat.Skill_Arrow_Rain)reader.SetPrivateField("termsName", reader.Read<System.String>(), instance);
					break;
					case "termsDesc":
					instance = (ActionCat.Skill_Arrow_Rain)reader.SetPrivateField("termsDesc", reader.Read<System.String>(), instance);
					break;
					case "soundEffects":
					instance = (ActionCat.Skill_Arrow_Rain)reader.SetPrivateField("soundEffects", reader.Read<UnityEngine.AudioClip[]>(), instance);
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