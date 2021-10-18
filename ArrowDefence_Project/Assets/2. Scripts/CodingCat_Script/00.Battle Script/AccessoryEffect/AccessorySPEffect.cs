namespace ActionCat
{
    using UnityEngine;

    public enum ACCESSORY_SPEFFECT_TYPE
    { 
        SPEFFECT_NONE,
        SPEFFECT_AIMSIGHT,
        SPEEFECT_SLOWTIME
    }

    [System.Serializable]
    public abstract class AccessorySPEffect
    {
        public abstract void Setup();
    }

    public class Acsp_AimSight : AccessorySPEffect, IToString
    {
        public Material lineMaterial;
        public float LineWidth = 0.050f;

        public override void Setup() => GameGlobal.GetBowGameObjectInScene().AddComponent<SPEffect_AimSight>().Initialize(lineMaterial, LineWidth);

        public override string ToString() => "Aim Sight";

        public Acsp_AimSight(Material lineMat, float width)
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
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return "Slow Time";
        }
    }
}
