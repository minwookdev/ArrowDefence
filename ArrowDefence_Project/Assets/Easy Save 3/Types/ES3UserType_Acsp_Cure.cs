using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("healAmount", "healRepeatTime", "repeatIntervalTime", "id", "name", "desc", "effectType", "level", "iconSprite", "nameTerms", "descTerms", "condition", "soundEffect", "<IsStartingPrepared>k__BackingField")]
	public class ES3UserType_Acsp_Cure : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Acsp_Cure() : base(typeof(ActionCat.Acsp_Cure)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.Acsp_Cure)obj;
			
			writer.WritePrivateField("healAmount", instance);
			writer.WritePrivateField("healRepeatTime", instance);
			writer.WritePrivateField("repeatIntervalTime", instance);
			writer.WritePrivateField("id", instance);
			writer.WritePrivateField("name", instance);
			writer.WritePrivateField("desc", instance);
			writer.WritePrivateField("effectType", instance);
			writer.WritePrivateField("level", instance);
			writer.WritePrivateFieldByRef("iconSprite", instance);
			writer.WritePrivateField("nameTerms", instance);
			writer.WritePrivateField("descTerms", instance);
			writer.WritePrivateField("condition", instance);
			writer.WritePrivateFieldByRef("soundEffect", instance);
			writer.WritePrivateField("<IsStartingPrepared>k__BackingField", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.Acsp_Cure)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "healAmount":
					instance = (ActionCat.Acsp_Cure)reader.SetPrivateField("healAmount", reader.Read<System.Single>(), instance);
					break;
					case "healRepeatTime":
					instance = (ActionCat.Acsp_Cure)reader.SetPrivateField("healRepeatTime", reader.Read<System.Int32>(), instance);
					break;
					case "repeatIntervalTime":
					instance = (ActionCat.Acsp_Cure)reader.SetPrivateField("repeatIntervalTime", reader.Read<System.Single>(), instance);
					break;
					case "id":
					instance = (ActionCat.Acsp_Cure)reader.SetPrivateField("id", reader.Read<System.String>(), instance);
					break;
					case "name":
					instance = (ActionCat.Acsp_Cure)reader.SetPrivateField("name", reader.Read<System.String>(), instance);
					break;
					case "desc":
					instance = (ActionCat.Acsp_Cure)reader.SetPrivateField("desc", reader.Read<System.String>(), instance);
					break;
					case "effectType":
					instance = (ActionCat.Acsp_Cure)reader.SetPrivateField("effectType", reader.Read<ActionCat.ACSP_TYPE>(), instance);
					break;
					case "level":
					instance = (ActionCat.Acsp_Cure)reader.SetPrivateField("level", reader.Read<ActionCat.SKILL_LEVEL>(), instance);
					break;
					case "iconSprite":
					instance = (ActionCat.Acsp_Cure)reader.SetPrivateField("iconSprite", reader.Read<UnityEngine.Sprite>(), instance);
					break;
					case "nameTerms":
					instance = (ActionCat.Acsp_Cure)reader.SetPrivateField("nameTerms", reader.Read<System.String>(), instance);
					break;
					case "descTerms":
					instance = (ActionCat.Acsp_Cure)reader.SetPrivateField("descTerms", reader.Read<System.String>(), instance);
					break;
					case "condition":
					instance = (ActionCat.Acsp_Cure)reader.SetPrivateField("condition", reader.Read<ActionCat.ArtifactCondition>(), instance);
					break;
					case "soundEffect":
					instance = (ActionCat.Acsp_Cure)reader.SetPrivateField("soundEffect", reader.Read<UnityEngine.AudioClip>(), instance);
					break;
					case "<IsStartingPrepared>k__BackingField":
					instance = (ActionCat.Acsp_Cure)reader.SetPrivateField("<IsStartingPrepared>k__BackingField", reader.Read<System.Boolean>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.Acsp_Cure();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_Acsp_CureArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_Acsp_CureArray() : base(typeof(ActionCat.Acsp_Cure[]), ES3UserType_Acsp_Cure.Instance)
		{
			Instance = this;
		}
	}
}