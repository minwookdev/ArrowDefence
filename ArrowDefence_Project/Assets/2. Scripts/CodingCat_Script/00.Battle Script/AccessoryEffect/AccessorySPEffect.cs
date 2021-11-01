namespace ActionCat
{
    using UnityEngine;

    [System.Serializable]
    public abstract class AccessorySPEffect
    {
        protected string id;
        protected string name;
        protected string desc;
        protected ACSP_TYPE effectType;
        protected SKILL_LEVEL level;
        protected Sprite iconSprite;

        #region PROPERTY
        public string ID { get => id; }
        public string Name { get => name; }
        public string Description { get => desc; }
        public ACSP_TYPE SpEffectType { get => effectType; }
        public SKILL_LEVEL Level { get => level; }
        public Sprite IconSprite
        {
            get
            {
                if (iconSprite != null)
                    return iconSprite;
                else
                    return null;
            }
        }
        #endregion

        protected AccessorySPEffect(string id, string name, string desc, ACSP_TYPE type, SKILL_LEVEL level, Sprite sprite)
        {
            this.id         = id;
            this.name       = name;
            this.desc       = desc;
            this.effectType = type;
            this.level      = level;
            this.iconSprite = sprite;
        }

        public AccessorySPEffect() { }

        public abstract void Init();
    }

    public class Acsp_AimSight : AccessorySPEffect, IToString
    {
        public Material lineMaterial;
        public float lineWidth = 0.050f;

        public override void Init()
        {
            //Battle Scene이 시작되면 Bow GameObject에 Accessory Effect Component를 Add 해줌
            GameGlobal.GetBowGameObjectInScene().AddComponent<SPEffect_AimSight>().Initialize(lineMaterial, lineWidth);
        }

        public override string ToString() => "Aim Sight";

        public Acsp_AimSight(string id, string name, string desc, ACSP_TYPE type, SKILL_LEVEL level, Sprite sprite, Material lineMat, float width) : 
            base(id, name, desc, type, level, sprite)
        {
            lineMaterial = lineMat;
            lineWidth    = width;
        }

        /// <summary>
        /// Constructor using Skill Data Scriptableobject. (Main)
        /// </summary>
        /// <param name="data"></param>
        public Acsp_AimSight(SkillDataAimSight data)
            : base(data.SkillId, data.SkillName, data.SkillDesc, data.EffectType, data.SkillLevel, data.SkillIconSprite)
        {
            lineMaterial = data.LineRenderMat;
            lineWidth    = data.LineWidth;
        }

        /// <summary>
        /// No Parameter Constructor For ES3 Utility (Don't Destory this)
        /// </summary>
        public Acsp_AimSight() : base() { }
    }

    public class Acsp_SlowTime : AccessorySPEffect, IToString
    {
        float timeSlowRatio;
        float duration;
        float cooldown;

        public float Cooldown { get => cooldown; }

        public override void Init()
        {
            return;
        }

        public override string ToString() => "Slow Time";

        public Acsp_SlowTime(SkillDataSlowTime data) : 
            base(data.SkillId, data.SkillName, data.SkillDesc, data.EffectType, data.SkillLevel, data.SkillIconSprite)
        {
            this.timeSlowRatio = data.TimeSlowRatio;
            this.duration      = data.Duration;
            this.cooldown      = data.CoolDown;
        }

        public Acsp_SlowTime() : base() { }

        public float ActiveSkill(MonoBehaviour mono)
        {
            mono.StartCoroutine(SlowTimeCo());
            return duration;
        }

        System.Collections.IEnumerator SlowTimeCo()
        {
            GameManager.Instance.TimeScaleSet(timeSlowRatio);

            yield return new WaitForSecondsRealtime(duration);

            CatLog.Log("슬로우 타임 종료");
            GameManager.Instance.TimeDefault();
        }
    }
}
