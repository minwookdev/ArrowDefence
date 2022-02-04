using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("MainArrowObject", "LessArrowObject", "arrowSkillInfoFst", "arrowSkillInfoSec", "effects", "EquipType", "abilities", "Item_Id", "Item_Name", "Item_Desc", "Item_Amount", "Item_Sprite", "Item_Type", "Item_Grade")]
	public class ES3UserType_Item_Arrow : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Item_Arrow() : base(typeof(ActionCat.Item_Arrow)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.Item_Arrow)obj;
			
			writer.WritePrivateFieldByRef("MainArrowObject", instance);
			writer.WritePrivateFieldByRef("LessArrowObject", instance);
			writer.WritePrivateField("arrowSkillInfoFst", instance);
			writer.WritePrivateField("arrowSkillInfoSec", instance);
			writer.WritePrivateField("effects", instance);
			writer.WritePrivateField("EquipType", instance);
			writer.WritePrivateField("abilities", instance);
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
			var instance = (ActionCat.Item_Arrow)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "MainArrowObject":
					reader.SetPrivateField("MainArrowObject", reader.Read<UnityEngine.GameObject>(), instance);
					break;
					case "LessArrowObject":
					reader.SetPrivateField("LessArrowObject", reader.Read<UnityEngine.GameObject>(), instance);
					break;
					case "arrowSkillInfoFst":
					reader.SetPrivateField("arrowSkillInfoFst", reader.Read<ActionCat.ASInfo>(), instance);
					break;
					case "arrowSkillInfoSec":
					reader.SetPrivateField("arrowSkillInfoSec", reader.Read<ActionCat.ASInfo>(), instance);
					break;
					case "effects":
					reader.SetPrivateField("effects", reader.Read<ActionCat.ACEffector2D[]>(), instance);
					break;
					case "EquipType":
					reader.SetPrivateField("EquipType", reader.Read<ActionCat.EQUIP_ITEMTYPE>(), instance);
					break;
					case "abilities":
					reader.SetPrivateField("abilities", reader.Read<ActionCat.Ability[]>(), instance);
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
			var instance = new ActionCat.Item_Arrow();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_Item_ArrowArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_Item_ArrowArray() : base(typeof(ActionCat.Item_Arrow[]), ES3UserType_Item_Arrow.Instance)
		{
			Instance = this;
		}
	}
}