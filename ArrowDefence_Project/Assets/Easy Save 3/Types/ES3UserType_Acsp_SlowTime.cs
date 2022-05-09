using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("timeSlowRatio", "duration", "id", "name", "desc", "effectType", "level", "iconSprite", "nameTerms", "descTerms", "condition", "<IsStartingPrepared>k__BackingField")]
	public class ES3UserType_Acsp_SlowTime : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Acsp_SlowTime() : base(typeof(ActionCat.Acsp_SlowTime)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.Acsp_SlowTime)obj;
			
			writer.WritePrivateField("timeSlowRatio", instance);
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
			writer.WritePrivateField("<IsStartingPrepared>k__BackingField", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.Acsp_SlowTime)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "timeSlowRatio":
					reader.SetPrivateField("timeSlowRatio", reader.Read<System.Single>(), instance);
					break;
					case "duration":
					reader.SetPrivateField("duration", reader.Read<System.Single>(), instance);
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
					case "effectType":
					reader.SetPrivateField("effectType", reader.Read<ActionCat.ACSP_TYPE>(), instance);
					break;
					case "level":
					reader.SetPrivateField("level", reader.Read<ActionCat.SKILL_LEVEL>(), instance);
					break;
					case "iconSprite":
					reader.SetPrivateField("iconSprite", reader.Read<UnityEngine.Sprite>(), instance);
					break;
					case "nameTerms":
					reader.SetPrivateField("nameTerms", reader.Read<System.String>(), instance);
					break;
					case "descTerms":
					reader.SetPrivateField("descTerms", reader.Read<System.String>(), instance);
					break;
					case "condition":
					reader.SetPrivateField("condition", reader.Read<ActionCat.ArtifactCondition>(), instance);
					break;
					case "<IsStartingPrepared>k__BackingField":
					reader.SetPrivateField("<IsStartingPrepared>k__BackingField", reader.Read<System.Boolean>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.Acsp_SlowTime();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_Acsp_SlowTimeArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_Acsp_SlowTimeArray() : base(typeof(ActionCat.Acsp_SlowTime[]), ES3UserType_Acsp_SlowTime.Instance)
		{
			Instance = this;
		}
	}
}