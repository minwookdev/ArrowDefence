using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("conditionType", "maxStack", "maxCost", "maxCoolDownTime")]
	public class ES3UserType_ArtCondition_Debuff : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_ArtCondition_Debuff() : base(typeof(ActionCat.ArtCondition_Debuff)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.ArtCondition_Debuff)obj;
			
			writer.WritePrivateField("conditionType", instance);
			writer.WritePrivateField("maxStack", instance);
			writer.WritePrivateField("maxCost", instance);
			writer.WritePrivateField("maxCoolDownTime", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.ArtCondition_Debuff)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
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
			var instance = new ActionCat.ArtCondition_Debuff();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_ArtCondition_DebuffArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ArtCondition_DebuffArray() : base(typeof(ActionCat.ArtCondition_Debuff[]), ES3UserType_ArtCondition_Debuff.Instance)
		{
			Instance = this;
		}
	}
}