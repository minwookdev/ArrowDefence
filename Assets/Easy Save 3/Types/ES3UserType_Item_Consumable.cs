using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("Item_Id", "Item_Amount", "Item_Sprite", "Item_Type", "Item_Grade", "termsName", "termsDesc")]
	public class ES3UserType_Item_Consumable : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Item_Consumable() : base(typeof(ActionCat.Item_Consumable)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.Item_Consumable)obj;
			
			writer.WritePrivateField("Item_Id", instance);
			writer.WritePrivateField("Item_Amount", instance);
			writer.WritePrivateFieldByRef("Item_Sprite", instance);
			writer.WritePrivateField("Item_Type", instance);
			writer.WritePrivateField("Item_Grade", instance);
			writer.WritePrivateField("termsName", instance);
			writer.WritePrivateField("termsDesc", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.Item_Consumable)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "Item_Id":
					reader.SetPrivateField("Item_Id", reader.Read<System.String>(), instance);
					break;
					case "Item_Amount":
					reader.SetPrivateField("Item_Amount", reader.Read<System.Int32>(), instance);
					break;
					case "Item_Sprite":
					reader.SetPrivateField("Item_Sprite", reader.Read<UnityEngine.Sprite>(), instance);
					break;
					case "Item_Type":
					reader.SetPrivateField("Item_Type", reader.Read<ActionCat.ITEMTYPE>(), instance);
					break;
					case "Item_Grade":
					reader.SetPrivateField("Item_Grade", reader.Read<ActionCat.ITEMGRADE>(), instance);
					break;
					case "termsName":
					reader.SetPrivateField("termsName", reader.Read<System.String>(), instance);
					break;
					case "termsDesc":
					reader.SetPrivateField("termsDesc", reader.Read<System.String>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.Item_Consumable();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_Item_ConsumableArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_Item_ConsumableArray() : base(typeof(ActionCat.Item_Consumable[]), ES3UserType_Item_Consumable.Instance)
		{
			Instance = this;
		}
	}
}