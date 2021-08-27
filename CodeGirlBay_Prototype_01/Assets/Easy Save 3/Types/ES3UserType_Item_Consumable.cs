using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("<Item_Id>k__BackingField", "<Item_Name>k__BackingField", "<Item_Desc>k__BackingField", "<Item_Amount>k__BackingField", "<Item_Sprite>k__BackingField", "<Item_Type>k__BackingField", "<Item_Grade>k__BackingField")]
	public class ES3UserType_Item_Consumable : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Item_Consumable() : base(typeof(CodingCat_Games.Item_Consumable)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (CodingCat_Games.Item_Consumable)obj;
			
			writer.WritePrivateField("<Item_Id>k__BackingField", instance);
			writer.WritePrivateField("<Item_Name>k__BackingField", instance);
			writer.WritePrivateField("<Item_Desc>k__BackingField", instance);
			writer.WritePrivateField("<Item_Amount>k__BackingField", instance);
			writer.WritePrivateFieldByRef("<Item_Sprite>k__BackingField", instance);
			writer.WritePrivateField("<Item_Type>k__BackingField", instance);
			writer.WritePrivateField("<Item_Grade>k__BackingField", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (CodingCat_Games.Item_Consumable)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "<Item_Id>k__BackingField":
					reader.SetPrivateField("<Item_Id>k__BackingField", reader.Read<System.Int32>(), instance);
					break;
					case "<Item_Name>k__BackingField":
					reader.SetPrivateField("<Item_Name>k__BackingField", reader.Read<System.String>(), instance);
					break;
					case "<Item_Desc>k__BackingField":
					reader.SetPrivateField("<Item_Desc>k__BackingField", reader.Read<System.String>(), instance);
					break;
					case "<Item_Amount>k__BackingField":
					reader.SetPrivateField("<Item_Amount>k__BackingField", reader.Read<System.Int32>(), instance);
					break;
					case "<Item_Sprite>k__BackingField":
					reader.SetPrivateField("<Item_Sprite>k__BackingField", reader.Read<UnityEngine.Sprite>(), instance);
					break;
					case "<Item_Type>k__BackingField":
					reader.SetPrivateField("<Item_Type>k__BackingField", reader.Read<CodingCat_Games.ITEMTYPE>(), instance);
					break;
					case "<Item_Grade>k__BackingField":
					reader.SetPrivateField("<Item_Grade>k__BackingField", reader.Read<CodingCat_Games.ITEMGRADE>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new CodingCat_Games.Item_Consumable();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_Item_ConsumableArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_Item_ConsumableArray() : base(typeof(CodingCat_Games.Item_Consumable[]), ES3UserType_Item_Consumable.Instance)
		{
			Instance = this;
		}
	}
}