using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("<MaxComboCount>k__BackingField", "<KilledCount>k__BackingField", "<ClearedCount>k__BackingField", "<IsUsedResurrect>k__BackingField", "<IsStageCleared>k__BackingField")]
	public class ES3UserType_StageInfo : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_StageInfo() : base(typeof(ActionCat.Data.StageInfo)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.Data.StageInfo)obj;
			
			writer.WritePrivateField("<MaxComboCount>k__BackingField", instance);
			writer.WritePrivateField("<KilledCount>k__BackingField", instance);
			writer.WritePrivateField("<ClearedCount>k__BackingField", instance);
			writer.WritePrivateField("<IsUsedResurrect>k__BackingField", instance);
			writer.WritePrivateField("<IsStageCleared>k__BackingField", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.Data.StageInfo)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "<MaxComboCount>k__BackingField":
					reader.SetPrivateField("<MaxComboCount>k__BackingField", reader.Read<System.Int16>(), instance);
					break;
					case "<KilledCount>k__BackingField":
					reader.SetPrivateField("<KilledCount>k__BackingField", reader.Read<System.Int16>(), instance);
					break;
					case "<ClearedCount>k__BackingField":
					reader.SetPrivateField("<ClearedCount>k__BackingField", reader.Read<System.Byte>(), instance);
					break;
					case "<IsUsedResurrect>k__BackingField":
					reader.SetPrivateField("<IsUsedResurrect>k__BackingField", reader.Read<System.Boolean>(), instance);
					break;
					case "<IsStageCleared>k__BackingField":
					reader.SetPrivateField("<IsStageCleared>k__BackingField", reader.Read<System.Boolean>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.Data.StageInfo();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_StageInfoArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_StageInfoArray() : base(typeof(ActionCat.Data.StageInfo[]), ES3UserType_StageInfo.Instance)
		{
			Instance = this;
		}
	}
}