using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("bowSkill_Fst", "bowSkill_Sec", "bowGameObject", "EquipType", "Item_Id", "Item_Name", "Item_Desc", "Item_Amount", "Item_Sprite", "Item_Type", "Item_Grade")]
	public class ES3UserType_Item_Bow : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Item_Bow() : base(typeof(ActionCat.Item_Bow)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.Item_Bow)obj;
			
			writer.WritePrivateField("bowSkill_Fst", instance);
			writer.WritePrivateField("bowSkill_Sec", instance);
			writer.WritePrivateFieldByRef("bowGameObject", instance);
			writer.WritePrivateField("EquipType", instance);
			writer.WritePrivateField("Item_Id", instance);
			writer.WritePrivateField("Item_Name", instance);
			writer.WritePrivateField("Item_Desc", instance);
			writer.WritePrivateField("Item_Amount", instance);
			writer.WritePrivateFieldByRef("Item_Sprite", instance);
			writer.WritePrivateField("Item_Type", instance);
			writer.WritePrivateField("Item_Grade", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.Item_Bow)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "bowSkill_Fst":
					reader.SetPrivateField("bowSkill_Fst", reader.Read<ActionCat.AD_BowSkill>(), instance);
					break;
					case "bowSkill_Sec":
					reader.SetPrivateField("bowSkill_Sec", reader.Read<ActionCat.AD_BowSkill>(), instance);
					break;
					case "bowGameObject":
					reader.SetPrivateField("bowGameObject", reader.Read<UnityEngine.GameObject>(), instance);
					break;
					case "EquipType":
					reader.SetPrivateField("EquipType", reader.Read<ActionCat.EQUIP_ITEMTYPE>(), instance);
					break;
					case "Item_Id":
					reader.SetPrivateField("Item_Id", reader.Read<System.Int32>(), instance);
					break;
					case "Item_Name":
					reader.SetPrivateField("Item_Name", reader.Read<System.String>(), instance);
					break;
					case "Item_Desc":
					reader.SetPrivateField("Item_Desc", reader.Read<System.String>(), instance);
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
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.Item_Bow();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_Item_BowArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_Item_BowArray() : base(typeof(ActionCat.Item_Bow[]), ES3UserType_Item_Bow.Instance)
		{
			Instance = this;
		}
	}
}