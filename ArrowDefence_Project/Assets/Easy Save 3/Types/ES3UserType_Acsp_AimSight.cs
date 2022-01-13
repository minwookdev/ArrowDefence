using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("lineMaterial", "lineWidth", "id", "name", "desc", "effectType", "level", "iconSprite")]
	public class ES3UserType_Acsp_AimSight : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Acsp_AimSight() : base(typeof(ActionCat.Acsp_AimSight)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.Acsp_AimSight)obj;
			
			writer.WritePropertyByRef("lineMaterial", instance.lineMaterial);
			writer.WriteProperty("lineWidth", instance.lineWidth, ES3Type_float.Instance);
			writer.WritePrivateField("id", instance);
			writer.WritePrivateField("name", instance);
			writer.WritePrivateField("desc", instance);
			writer.WritePrivateField("effectType", instance);
			writer.WritePrivateField("level", instance);
			writer.WritePrivateFieldByRef("iconSprite", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.Acsp_AimSight)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "lineMaterial":
						instance.lineMaterial = reader.Read<UnityEngine.Material>(ES3Type_Material.Instance);
						break;
					case "lineWidth":
						instance.lineWidth = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "id":
					reader.SetPrivateField("id", reader.Read<System.String>(), instance);
					break;
					case "name":
					reader.SetPrivateField("name", reader.Read<System.String>(), instance);
					break;
					case "desc":
					reader.SetPrivateField("desc", reader.Read<System.String>(), instance);
					break;
					case "effectType":
					reader.SetPrivateField("effectType", reader.Read<ActionCat.ACSP_TYPE>(), instance);
					break;
					case "level":
					reader.SetPrivateField("level", reader.Read<ActionCat.SKILL_LEVEL>(), instance);
					break;
					case "iconSprite":
					reader.SetPrivateField("iconSprite", reader.Read<UnityEngine.Sprite>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.Acsp_AimSight();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_Acsp_AimSightArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_Acsp_AimSightArray() : base(typeof(ActionCat.Acsp_AimSight[]), ES3UserType_Acsp_AimSight.Instance)
		{
			Instance = this;
		}
	}
}