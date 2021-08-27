using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("MainBow", "MainArrow", "SubArrow")]
	public class ES3UserType_Player_Equipments : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Player_Equipments() : base(typeof(CodingCat_Games.Player_Equipments)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (CodingCat_Games.Player_Equipments)obj;
			
			writer.WritePrivateField("MainBow", instance);
			writer.WritePrivateField("MainArrow", instance);
			writer.WritePrivateField("SubArrow", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (CodingCat_Games.Player_Equipments)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "MainBow":
					reader.SetPrivateField("MainBow", reader.Read<CodingCat_Games.Item_Bow>(), instance);
					break;
					case "MainArrow":
					reader.SetPrivateField("MainArrow", reader.Read<CodingCat_Games.Item_Arrow>(), instance);
					break;
					case "SubArrow":
					reader.SetPrivateField("SubArrow", reader.Read<CodingCat_Games.Item_Arrow>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new CodingCat_Games.Player_Equipments();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_Player_EquipmentsArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_Player_EquipmentsArray() : base(typeof(CodingCat_Games.Player_Equipments[]), ES3UserType_Player_Equipments.Instance)
		{
			Instance = this;
		}
	}
}