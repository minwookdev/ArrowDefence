using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("<isOnAutoMode>k__BackingField", "<isOnEliteSpawn>k__BackingField")]
	public class ES3UserType_StageSetting : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_StageSetting() : base(typeof(ActionCat.Data.StageData.StageSetting)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.Data.StageData.StageSetting)obj;
			
			writer.WritePrivateField("<isOnAutoMode>k__BackingField", instance);
			writer.WritePrivateField("<isOnEliteSpawn>k__BackingField", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.Data.StageData.StageSetting)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "<isOnAutoMode>k__BackingField":
					instance = (ActionCat.Data.StageData.StageSetting)reader.SetPrivateField("<isOnAutoMode>k__BackingField", reader.Read<System.Boolean>(), instance);
					break;
					case "<isOnEliteSpawn>k__BackingField":
					instance = (ActionCat.Data.StageData.StageSetting)reader.SetPrivateField("<isOnEliteSpawn>k__BackingField", reader.Read<System.Boolean>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.Data.StageData.StageSetting();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_StageSettingArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_StageSettingArray() : base(typeof(ActionCat.Data.StageData.StageSetting[]), ES3UserType_StageSetting.Instance)
		{
			Instance = this;
		}
	}
}