using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("<Item_Id>k__BackingField", "<Item_Name>k__BackingField", "<Item_Desc>k__BackingField", "<Item_Amount>k__BackingField", "<Item_Sprite>k__BackingField", "<Item_Type>k__BackingField", "<Item_Grade>k__BackingField")]
	public class ES3UserType_Item_Material : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Item_Material() : base(typeof(CodingCat_Games.Item_Material)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (CodingCat_Games.Item_Material)obj;
			
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
			var instance = (CodingCat_Games.Item_Material)obj;
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
			var instance = new CodingCat_Games.Item_Material();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_Item_MaterialArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_Item_MaterialArray() : base(typeof(CodingCat_Games.Item_Material[]), ES3UserType_Item_Material.Instance)
		{
			Instance = this;
		}
	}
}