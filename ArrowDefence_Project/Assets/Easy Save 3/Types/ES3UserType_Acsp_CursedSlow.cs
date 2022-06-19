using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("radius", "slowRatio", "duration", "id", "name", "desc", "effectType", "level", "iconSprite", "nameTerms", "descTerms", "condition", "soundEffect", "<IsStartingPrepared>k__BackingField")]
	public class ES3UserType_Acsp_CursedSlow : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Acsp_CursedSlow() : base(typeof(ActionCat.Acsp_CursedSlow)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.Acsp_CursedSlow)obj;
			
			writer.WritePrivateField("radius", instance);
			writer.WritePrivateField("slowRatio", instance);
			writer.WritePrivateField("duration", instance);
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
			var instance = (ActionCat.Acsp_CursedSlow)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "radius":
					instance = (ActionCat.Acsp_CursedSlow)reader.SetPrivateField("radius", reader.Read<System.Single>(), instance);
					break;
					case "slowRatio":
					instance = (ActionCat.Acsp_CursedSlow)reader.SetPrivateField("slowRatio", reader.Read<System.Single>(), instance);
					break;
					case "duration":
					instance = (ActionCat.Acsp_CursedSlow)reader.SetPrivateField("duration", reader.Read<System.Single>(), instance);
					break;
					case "id":
					instance = (ActionCat.Acsp_CursedSlow)reader.SetPrivateField("id", reader.Read<System.String>(), instance);
					break;
					case "name":
					instance = (ActionCat.Acsp_CursedSlow)reader.SetPrivateField("name", reader.Read<System.String>(), instance);
					break;
					case "desc":
					instance = (ActionCat.Acsp_CursedSlow)reader.SetPrivateField("desc", reader.Read<System.String>(), instance);
					break;
					case "effectType":
					instance = (ActionCat.Acsp_CursedSlow)reader.SetPrivateField("effectType", reader.Read<ActionCat.ACSP_TYPE>(), instance);
					break;
					case "level":
					instance = (ActionCat.Acsp_CursedSlow)reader.SetPrivateField("level", reader.Read<ActionCat.SKILL_LEVEL>(), instance);
					break;
					case "iconSprite":
					instance = (ActionCat.Acsp_CursedSlow)reader.SetPrivateField("iconSprite", reader.Read<UnityEngine.Sprite>(), instance);
					break;
					case "nameTerms":
					instance = (ActionCat.Acsp_CursedSlow)reader.SetPrivateField("nameTerms", reader.Read<System.String>(), instance);
					break;
					case "descTerms":
					instance = (ActionCat.Acsp_CursedSlow)reader.SetPrivateField("descTerms", reader.Read<System.String>(), instance);
					break;
					case "condition":
					instance = (ActionCat.Acsp_CursedSlow)reader.SetPrivateField("condition", reader.Read<ActionCat.ArtifactCondition>(), instance);
					break;
					case "soundEffect":
					instance = (ActionCat.Acsp_CursedSlow)reader.SetPrivateField("soundEffect", reader.Read<UnityEngine.AudioClip>(), instance);
					break;
					case "<IsStartingPrepared>k__BackingField":
					instance = (ActionCat.Acsp_CursedSlow)reader.SetPrivateField("<IsStartingPrepared>k__BackingField", reader.Read<System.Boolean>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.Acsp_CursedSlow();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_Acsp_CursedSlowArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_Acsp_CursedSlowArray() : base(typeof(ActionCat.Acsp_CursedSlow[]), ES3UserType_Acsp_CursedSlow.Instance)
		{
			Instance = this;
		}
	}
}