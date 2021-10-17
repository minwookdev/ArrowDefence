using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("arrowCount")]
	public class ES3UserType_Skill_Arrow_Rain : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Skill_Arrow_Rain() : base(typeof(ActionCat.Skill_Arrow_Rain)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.Skill_Arrow_Rain)obj;
			
			writer.WritePrivateField("arrowCount", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.Skill_Arrow_Rain)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "arrowCount":
					reader.SetPrivateField("arrowCount", reader.Read<System.Byte>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.Skill_Arrow_Rain();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_Skill_Arrow_RainArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_Skill_Arrow_RainArray() : base(typeof(ActionCat.Skill_Arrow_Rain[]), ES3UserType_Skill_Arrow_Rain.Instance)
		{
			Instance = this;
		}
	}
}