namespace ActionCat
{
    using UnityEngine;

    public enum ACSP_TYPE
    { 
        SPEFFECT_NONE,
        SPEFFECT_AIMSIGHT,
        SPEEFECT_SLOWTIME
    }

    [System.Serializable]
    public abstract class AccessorySPEffect
    {
        protected string id;
        protected string name;
        protected string desc;
        protected ACSP_TYPE effectType;
        protected SKILL_LEVEL level;

        #region PROPERTY
        public string ID { get => id; }
        public string Name { get => name; }
        public string Description { get => desc; }
        public ACSP_TYPE SpEffectType { get => effectType; }
        public SKILL_LEVEL Level { get => level; }
        #endregion

        protected AccessorySPEffect(string id, string name, string desc, ACSP_TYPE type, SKILL_LEVEL level)
        {
            this.id         = id;
            this.name       = name;
            this.desc       = desc;
            this.effectType = type;
            this.level      = level;
        }

        public AccessorySPEffect() { }

        public abstract void Setup();
    }

    public class Acsp_AimSight : AccessorySPEffect, IToString
    {
        public Material lineMaterial;
        public float LineWidth = 0.050f;

        public override void Setup()
        {
            //Battle Scene이 시작되면 Bow GameObject에 Accessory Effect Component를 Add 해줌
            GameGlobal.GetBowGameObjectInScene().AddComponent<SPEffect_AimSight>().Initialize(lineMaterial, LineWidth);
        }

        public override string ToString() => "Aim Sight";

        public Acsp_AimSight(string skillid, string skillname, string skilldesc, ACSP_TYPE effecttype, SKILL_LEVEL skilllevel, Material lineMat, float width) : 
            base(skillid, skillname, skilldesc, effecttype, skilllevel)
        {
            lineMaterial = lineMat;
            LineWidth    = width;
        }

        /// <summary>
        /// No Parameter Constructor For ES3 Utility (Don't Destory this)
        /// </summary>
        public Acsp_AimSight() : base() { }
    }

    public class Acsp_SlowTime : AccessorySPEffect, IToString
    {
        public override void Setup()
        {
            return;
        }

        public override string ToString() => "Slow Time";


        public Acsp_SlowTime(string id, string name, string desc, ACSP_TYPE type, SKILL_LEVEL level) : 
            base(id, name, desc, type, level)
        {

        }

        public Acsp_SlowTime() : base() { }
    }
}
