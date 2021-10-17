using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("invenList")]
	public class ES3UserType_AD_Inventory : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_AD_Inventory() : base(typeof(ActionCat.AD_Inventory)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.AD_Inventory)obj;
			
			writer.WritePrivateField("invenList", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.AD_Inventory)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "invenList":
					reader.SetPrivateField("invenList", reader.Read<System.Collections.Generic.List<ActionCat.AD_item>>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.AD_Inventory();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_AD_InventoryArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_AD_InventoryArray() : base(typeof(ActionCat.AD_Inventory[]), ES3UserType_AD_Inventory.Instance)
		{
			Instance = this;
		}
	}
}