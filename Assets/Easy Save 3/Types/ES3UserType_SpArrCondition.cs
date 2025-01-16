using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("chargeType", "costIncrease", "maxCost", "maxStackCount")]
	public class ES3UserType_SpArrCondition : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_SpArrCondition() : base(typeof(ActionCat.SpArrCondition)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.SpArrCondition)obj;
			
			writer.WritePrivateField("chargeType", instance);
			writer.WritePrivateField("costIncrease", instance);
			writer.WritePrivateField("maxCost", instance);
			writer.WritePrivateField("maxStackCount", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.SpArrCondition)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "chargeType":
					reader.SetPrivateField("chargeType", reader.Read<ActionCat.CHARGETYPE>(), instance);
					break;
					case "costIncrease":
					reader.SetPrivateField("costIncrease", reader.Read<System.Single>(), instance);
					break;
					case "maxCost":
					reader.SetPrivateField("maxCost", reader.Read<System.Int32>(), instance);
					break;
					case "maxStackCount":
					reader.SetPrivateField("maxStackCount", reader.Read<System.Int32>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.SpArrCondition();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_SpArrConditionArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_SpArrConditionArray() : base(typeof(ActionCat.SpArrCondition[]), ES3UserType_SpArrCondition.Instance)
		{
			Instance = this;
		}
	}
}