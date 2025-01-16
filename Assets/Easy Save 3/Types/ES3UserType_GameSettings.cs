using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("stageSettings", "bgmVolumeParamsValue", "seVolumeParamsValue", "<PullingType>k__BackingField", "<LanguageCode>k__BackingField")]
	public class ES3UserType_GameSettings : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_GameSettings() : base(typeof(ActionCat.Data.GameSettings)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.Data.GameSettings)obj;
			
			writer.WritePrivateField("stageSettings", instance);
			writer.WritePrivateField("bgmVolumeParamsValue", instance);
			writer.WritePrivateField("seVolumeParamsValue", instance);
			writer.WritePrivateField("<PullingType>k__BackingField", instance);
			writer.WritePrivateField("<LanguageCode>k__BackingField", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.Data.GameSettings)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "stageSettings":
					instance = (ActionCat.Data.GameSettings)reader.SetPrivateField("stageSettings", reader.Read<System.Collections.Generic.Dictionary<System.String, ActionCat.Data.StageData.StageSetting>>(), instance);
					break;
					case "bgmVolumeParamsValue":
					instance = (ActionCat.Data.GameSettings)reader.SetPrivateField("bgmVolumeParamsValue", reader.Read<System.Single>(), instance);
					break;
					case "seVolumeParamsValue":
					instance = (ActionCat.Data.GameSettings)reader.SetPrivateField("seVolumeParamsValue", reader.Read<System.Single>(), instance);
					break;
					case "<PullingType>k__BackingField":
					instance = (ActionCat.Data.GameSettings)reader.SetPrivateField("<PullingType>k__BackingField", reader.Read<ActionCat.PULLINGTYPE>(), instance);
					break;
					case "<LanguageCode>k__BackingField":
					instance = (ActionCat.Data.GameSettings)reader.SetPrivateField("<LanguageCode>k__BackingField", reader.Read<System.String>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.Data.GameSettings();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_GameSettingsArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_GameSettingsArray() : base(typeof(ActionCat.Data.GameSettings[]), ES3UserType_GameSettings.Instance)
		{
			Instance = this;
		}
	}
}