using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("values")]
	public class ES3UserType_EmptyTypeArrowSkill : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_EmptyTypeArrowSkill() : base(typeof(ActionCat.EmptyTypeArrowSkill)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.EmptyTypeArrowSkill)obj;
			
			writer.WritePrivateField("values", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.EmptyTypeArrowSkill)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "values":
					reader.SetPrivateField("values", reader.Read<System.Single[]>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.EmptyTypeArrowSkill();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_EmptyTypeArrowSkillArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_EmptyTypeArrowSkillArray() : base(typeof(ActionCat.EmptyTypeArrowSkill[]), ES3UserType_EmptyTypeArrowSkill.Instance)
		{
			Instance = this;
		}
	}
}