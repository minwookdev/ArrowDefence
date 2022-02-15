namespace ActionCat {
    using UnityEngine;
    using ActionCat.Interface;

    //================================================================= << PARENT >> ==================================================================
    [System.Serializable]
    public abstract class AccessorySPEffect {
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
        public Sprite IconSprite {
            get {
                if (iconSprite != null)
                    return iconSprite;
                else
                    return null;
            }
        }
        #endregion

        protected AccessorySPEffect(string id, string name, string desc, ACSP_TYPE type, SKILL_LEVEL level, Sprite sprite) {
            this.id         = id;
            this.name       = name;
            this.desc       = desc;
            this.effectType = type;
            this.level      = level;
            this.iconSprite = sprite;
        }
        #region ES3
        public AccessorySPEffect() { }
        #endregion

        /// <summary>
        /// 특수효과 슬롯에 이벤트로 할당될 함수.
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// 특수효과 발동 중 배틀 종료처리 되었을 때 효과 중지처리.
        /// </summary>
        public virtual void OnStop() { return; }
    }
    //=================================================================================================================================================
    //================================================================ << AIM SIGHT >> ================================================================
    public class Acsp_AimSight : AccessorySPEffect, IToString {
        private GameObject aimSightPref;

        public override void Init() {
            var controller = GameGlobal.GetControllerByTag().GetComponent<AD_BowController>();
            var parent     = controller.GetSlotTrOrNull;
            if (parent) {
                var aimsighter = GameObject.Instantiate(aimSightPref, parent.position, Quaternion.identity, parent).GetComponent<SPEffect_AimSight>();
                aimsighter.Initialize(controller);
            }
        }

        public override string ToString() => "Aim Sight";

        /// <summary>
        /// Constructor using Skill Data Scriptableobject. (Main)
        /// </summary>
        /// <param name="data"></param>
        public Acsp_AimSight(SkillDataAimSight data)
            : base(data.SkillId, data.SkillName, data.SkillDesc, data.EffectType, data.SkillLevel, data.SkillIconSprite) {
            aimSightPref = data.AimSightPref;
        }
        #region ES3
        public Acsp_AimSight() : base() { }
        #endregion
    }
    //=================================================================================================================================================
    //================================================================ << SLOW TIME >> ================================================================
    public class Acsp_SlowTime : AccessorySPEffect, IToString {
        float timeSlowRatio;
        float duration;
        float cooldown;

        public float Cooldown { get => cooldown; }

        public override void Init() {
            return;
        }

        public override string ToString() => "Slow Time";

        public Acsp_SlowTime(SkillDataSlowTime data) : 
            base(data.SkillId, data.SkillName, data.SkillDesc, data.EffectType, data.SkillLevel, data.SkillIconSprite) {
            this.timeSlowRatio = data.TimeSlowRatio;
            this.duration      = data.Duration;
            this.cooldown      = data.CoolDown;
        }
        #region ES3
        public Acsp_SlowTime() : base() { }
        #endregion

        public float ActiveSkill(MonoBehaviour mono) {
            //mono.StartCoroutine(SlowTimeCo());

            GameManager.Instance.TimeScaleSet(timeSlowRatio);
            return duration;
        }
        public override void OnStop() {
            GameManager.Instance.TimeToDefault();
        }

        System.Collections.IEnumerator SlowTimeCo() { // <- Not Use this type
            GameManager.Instance.TimeScaleSet(timeSlowRatio);

            yield return new WaitForSecondsRealtime(duration);

            CatLog.Log("슬로우 타임 종료");
            GameManager.Instance.TimeToDefault();
        }
    }
    //=================================================================================================================================================
}
