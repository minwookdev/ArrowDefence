using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("stageInfo", "craftingInfoList", "gold", "stone")]
	public class ES3UserType_PlayerInfo : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_PlayerInfo() : base(typeof(ActionCat.Data.PlayerInfo)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.Data.PlayerInfo)obj;
			
			writer.WritePrivateField("stageInfo", instance);
			writer.WritePrivateField("craftingInfoList", instance);
			writer.WritePrivateField("gold", instance);
			writer.WritePrivateField("stone", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.Data.PlayerInfo)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "stageInfo":
					instance = (ActionCat.Data.PlayerInfo)reader.SetPrivateField("stageInfo", reader.Read<System.Collections.Generic.Dictionary<System.String, ActionCat.Data.StageInfo>>(), instance);
					break;
					case "craftingInfoList":
					instance = (ActionCat.Data.PlayerInfo)reader.SetPrivateField("craftingInfoList", reader.Read<System.Collections.Generic.List<ActionCat.Data.CraftingInfo>>(), instance);
					break;
					case "gold":
					instance = (ActionCat.Data.PlayerInfo)reader.SetPrivateField("gold", reader.Read<System.Int32>(), instance);
					break;
					case "stone":
					instance = (ActionCat.Data.PlayerInfo)reader.SetPrivateField("stone", reader.Read<System.Int32>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.Data.PlayerInfo();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_PlayerInfoArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_PlayerInfoArray() : base(typeof(ActionCat.Data.PlayerInfo[]), ES3UserType_PlayerInfo.Instance)
		{
			Instance = this;
		}
	}
}