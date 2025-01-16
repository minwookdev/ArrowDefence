using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("<IsSkipable>k__BackingField", "<Current>k__BackingField", "<Max>k__BackingField", "<Result>k__BackingField", "amount", "<IsAvailable>k__BackingField")]
	public class ES3UserType_CraftingInfo : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_CraftingInfo() : base(typeof(ActionCat.Data.CraftingInfo)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.Data.CraftingInfo)obj;
			
			writer.WritePrivateField("<IsSkipable>k__BackingField", instance);
			writer.WritePrivateField("<Current>k__BackingField", instance);
			writer.WritePrivateField("<Max>k__BackingField", instance);
			writer.WritePrivateFieldByRef("<Result>k__BackingField", instance);
			writer.WritePrivateField("amount", instance);
			writer.WritePrivateField("<IsAvailable>k__BackingField", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.Data.CraftingInfo)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "<IsSkipable>k__BackingField":
					reader.SetPrivateField("<IsSkipable>k__BackingField", reader.Read<System.Boolean>(), instance);
					break;
					case "<Current>k__BackingField":
					reader.SetPrivateField("<Current>k__BackingField", reader.Read<System.Int32>(), instance);
					break;
					case "<Max>k__BackingField":
					reader.SetPrivateField("<Max>k__BackingField", reader.Read<System.Int32>(), instance);
					break;
					case "<Result>k__BackingField":
					reader.SetPrivateField("<Result>k__BackingField", reader.Read<ActionCat.ItemData>(), instance);
					break;
					case "amount":
					reader.SetPrivateField("amount", reader.Read<System.Int32>(), instance);
					break;
					case "<IsAvailable>k__BackingField":
					reader.SetPrivateField("<IsAvailable>k__BackingField", reader.Read<System.Boolean>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.Data.CraftingInfo();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_CraftingInfoArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_CraftingInfoArray() : base(typeof(ActionCat.Data.CraftingInfo[]), ES3UserType_CraftingInfo.Instance)
		{
			Instance = this;
		}
	}
}