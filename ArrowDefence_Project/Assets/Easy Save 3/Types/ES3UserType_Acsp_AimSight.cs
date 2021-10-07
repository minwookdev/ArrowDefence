using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("lineMaterial", "LineWidth")]
	public class ES3UserType_Acsp_AimSight : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Acsp_AimSight() : base(typeof(CodingCat_Games.Acsp_AimSight)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (CodingCat_Games.Acsp_AimSight)obj;
			
			writer.WritePropertyByRef("lineMaterial", instance.lineMaterial);
			writer.WriteProperty("LineWidth", instance.LineWidth, ES3Type_float.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (CodingCat_Games.Acsp_AimSight)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "lineMaterial":
						instance.lineMaterial = reader.Read<UnityEngine.Material>(ES3Type_Material.Instance);
						break;
					case "LineWidth":
						instance.LineWidth = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new CodingCat_Games.Acsp_AimSight();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_Acsp_AimSightArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_Acsp_AimSightArray() : base(typeof(CodingCat_Games.Acsp_AimSight[]), ES3UserType_Acsp_AimSight.Instance)
		{
			Instance = this;
		}
	}
}