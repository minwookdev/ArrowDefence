using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("costIncreaseValue", "conditionType", "maxStack", "maxCost", "maxCoolDownTime")]
	public class ES3UserType_ArtCondition_Trigger : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_ArtCondition_Trigger() : base(typeof(ActionCat.ArtCondition_Trigger)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.ArtCondition_Trigger)obj;
			
			writer.WritePrivateField("costIncreaseValue", instance);
			writer.WritePrivateField("conditionType", instance);
			writer.WritePrivateField("maxStack", instance);
			writer.WritePrivateField("maxCost", instance);
			writer.WritePrivateField("maxCoolDownTime", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.ArtCondition_Trigger)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "costIncreaseValue":
					reader.SetPrivateField("costIncreaseValue", reader.Read<System.Single>(), instance);
					break;
					case "conditionType":
					reader.SetPrivateField("conditionType", reader.Read<ActionCat.ARTCONDITION>(), instance);
					break;
					case "maxStack":
					reader.SetPrivateField("maxStack", reader.Read<System.Int32>(), instance);
					break;
					case "maxCost":
					reader.SetPrivateField("maxCost", reader.Read<System.Single>(), instance);
					break;
					case "maxCoolDownTime":
					reader.SetPrivateField("maxCoolDownTime", reader.Read<System.Single>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.ArtCondition_Trigger();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_ArtCondition_TriggerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ArtCondition_TriggerArray() : base(typeof(ActionCat.ArtCondition_Trigger[]), ES3UserType_ArtCondition_Trigger.Instance)
		{
			Instance = this;
		}
	}
}