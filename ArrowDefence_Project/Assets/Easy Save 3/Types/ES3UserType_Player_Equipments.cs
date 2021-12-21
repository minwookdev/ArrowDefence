using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("EquippedBow", "EquippedArrow_f", "EquippedArrow_s", "EquippedAccessory_f", "EquippedAccessory_s", "EquippedAccessory_t")]
	public class ES3UserType_Player_Equipments : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Player_Equipments() : base(typeof(ActionCat.Player_Equipments)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.Player_Equipments)obj;
			
			writer.WritePrivateField("EquippedBow", instance);
			writer.WritePrivateField("EquippedArrow_f", instance);
			writer.WritePrivateField("EquippedArrow_s", instance);
			writer.WritePrivateField("EquippedAccessory_f", instance);
			writer.WritePrivateField("EquippedAccessory_s", instance);
			writer.WritePrivateField("EquippedAccessory_t", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.Player_Equipments)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "EquippedBow":
					reader.SetPrivateField("EquippedBow", reader.Read<ActionCat.Item_Bow>(), instance);
					break;
					case "EquippedArrow_f":
					reader.SetPrivateField("EquippedArrow_f", reader.Read<ActionCat.Item_Arrow>(), instance);
					break;
					case "EquippedArrow_s":
					reader.SetPrivateField("EquippedArrow_s", reader.Read<ActionCat.Item_Arrow>(), instance);
					break;
					case "EquippedAccessory_f":
					reader.SetPrivateField("EquippedAccessory_f", reader.Read<ActionCat.Item_Accessory>(), instance);
					break;
					case "EquippedAccessory_s":
					reader.SetPrivateField("EquippedAccessory_s", reader.Read<ActionCat.Item_Accessory>(), instance);
					break;
					case "EquippedAccessory_t":
					reader.SetPrivateField("EquippedAccessory_t", reader.Read<ActionCat.Item_Accessory>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.Player_Equipments();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_Player_EquipmentsArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_Player_EquipmentsArray() : base(typeof(ActionCat.Player_Equipments[]), ES3UserType_Player_Equipments.Instance)
		{
			Instance = this;
		}
	}
}