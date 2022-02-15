using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("activationProbability", "projectilePref", "projectileDamage")]
	public class ES3UserType_ElementalFire : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_ElementalFire() : base(typeof(ActionCat.ElementalFire)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.ElementalFire)obj;
			
			writer.WritePrivateField("activationProbability", instance);
			writer.WritePrivateFieldByRef("projectilePref", instance);
			writer.WritePrivateField("projectileDamage", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.ElementalFire)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "activationProbability":
					reader.SetPrivateField("activationProbability", reader.Read<System.Single>(), instance);
					break;
					case "projectilePref":
					reader.SetPrivateField("projectilePref", reader.Read<ActionCat.ProjectilePref>(), instance);
					break;
					case "projectileDamage":
					reader.SetPrivateField("projectileDamage", reader.Read<System.Int16>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.ElementalFire();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_ElementalFireArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ElementalFireArray() : base(typeof(ActionCat.ElementalFire[]), ES3UserType_ElementalFire.Instance)
		{
			Instance = this;
		}
	}
}