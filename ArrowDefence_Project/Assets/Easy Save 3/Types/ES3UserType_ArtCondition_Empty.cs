using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute()]
	public class ES3UserType_ArtCondition_Empty : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_ArtCondition_Empty() : base(typeof(ActionCat.ArtCondition_Empty)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.ArtCondition_Empty)obj;
			
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.ArtCondition_Empty)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.ArtCondition_Empty();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_ArtCondition_EmptyArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ArtCondition_EmptyArray() : base(typeof(ActionCat.ArtCondition_Empty[]), ES3UserType_ArtCondition_Empty.Instance)
		{
			Instance = this;
		}
	}
}