using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("effectShockWave", "addExplosionPref", "skillLevel", "explosionRange", "addExplosionRange", "addExplosionDamage", "projectilePref", "projectileDamage", "sounds")]
	public class ES3UserType_Explosion : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Explosion() : base(typeof(ActionCat.Explosion)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ActionCat.Explosion)obj;
			
			writer.WritePrivateFieldByRef("effectShockWave", instance);
			writer.WritePrivateFieldByRef("addExplosionPref", instance);
			writer.WritePrivateField("skillLevel", instance);
			writer.WritePrivateField("explosionRange", instance);
			writer.WritePrivateField("addExplosionRange", instance);
			writer.WritePrivateField("addExplosionDamage", instance);
			writer.WritePrivateFieldByRef("projectilePref", instance);
			writer.WritePrivateField("projectileDamage", instance);
			writer.WritePrivateField("sounds", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ActionCat.Explosion)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "effectShockWave":
					instance = (ActionCat.Explosion)reader.SetPrivateField("effectShockWave", reader.Read<ActionCat.ACEffector2D>(), instance);
					break;
					case "addExplosionPref":
					instance = (ActionCat.Explosion)reader.SetPrivateField("addExplosionPref", reader.Read<ActionCat.ProjectilePref>(), instance);
					break;
					case "skillLevel":
					instance = (ActionCat.Explosion)reader.SetPrivateField("skillLevel", reader.Read<System.Byte>(), instance);
					break;
					case "explosionRange":
					instance = (ActionCat.Explosion)reader.SetPrivateField("explosionRange", reader.Read<System.Single>(), instance);
					break;
					case "addExplosionRange":
					instance = (ActionCat.Explosion)reader.SetPrivateField("addExplosionRange", reader.Read<System.Single>(), instance);
					break;
					case "addExplosionDamage":
					instance = (ActionCat.Explosion)reader.SetPrivateField("addExplosionDamage", reader.Read<System.Int16>(), instance);
					break;
					case "projectilePref":
					instance = (ActionCat.Explosion)reader.SetPrivateField("projectilePref", reader.Read<ActionCat.ProjectilePref>(), instance);
					break;
					case "projectileDamage":
					instance = (ActionCat.Explosion)reader.SetPrivateField("projectileDamage", reader.Read<System.Int16>(), instance);
					break;
					case "sounds":
					instance = (ActionCat.Explosion)reader.SetPrivateField("sounds", reader.Read<UnityEngine.AudioClip[]>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ActionCat.Explosion();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_ExplosionArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ExplosionArray() : base(typeof(ActionCat.Explosion[]), ES3UserType_Explosion.Instance)
		{
			Instance = this;
		}
	}
}