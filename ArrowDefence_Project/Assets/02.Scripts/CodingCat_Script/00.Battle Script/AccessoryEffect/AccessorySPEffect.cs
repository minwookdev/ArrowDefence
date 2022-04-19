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
        protected string nameTerms;
        protected string descTerms;
        protected ArtifactCondition condition = null;
        #region PROPERTY
        public string ID { get => id; }
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
        public string NameByTerms {
            get {
                I2.Loc.LocalizedString loc = nameTerms;
                return loc;
            }
        }
        public string DescByTerms {
            get {
                I2.Loc.LocalizedString loc = descTerms;
                return loc;
            }
        }
        public string TermsName {
            get {
                return nameTerms;
            }
        }
        public string TermsDesc {
            get {
                return descTerms;
            }
        }
        #endregion

        public virtual string GetNameByTerms() {
            I2.Loc.LocalizedString loc = nameTerms;
            return loc;
        }

        public abstract string GetDescByTerms();

        protected AccessorySPEffect(AccessorySkillData entity) {
            this.id         = entity.SkillId;
            this.effectType = entity.EffectType;
            this.level      = entity.SkillLevel;
            this.iconSprite = entity.SkillIconSprite;
            this.nameTerms  = entity.NameTerms;
            this.descTerms  = entity.DescTerms;

            //assignment artifact condition
            switch (entity.ConditionType) {
                case ARTCONDITION.NONE:    break;
                case ARTCONDITION.TRIGGER: condition = new ArtCondition_Trigger(entity.MaxStack, entity.MaxCost, entity.CoolDownTime, entity.IncreaseCostCount); break;
                case ARTCONDITION.BUFF:    condition = new ArtCondition_Buff(entity.MaxCost, entity.CoolDownTime); break;
                case ARTCONDITION.DEBUFF:  condition = new ArtCondition_Debuff(entity.MaxStack, entity.MaxCost, entity.CoolDownTime); break;
                case ARTCONDITION.PASSIVE: break;
                default: throw new System.NotImplementedException();
            }
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

        public override string GetDescByTerms() {
            throw new System.NotImplementedException();
        }

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
        /// <param name="entity"></param>
        public Acsp_AimSight(SkillDataAimSight entity) : base(entity) {
            aimSightPref = entity.AimSightPref;
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

        public override string GetDescByTerms() {
            throw new System.NotImplementedException();
        }

        public override void Init() {
            return;
        }

        public override string ToString() => "Slow Time";

        public Acsp_SlowTime(SkillDataSlowTime entity) : base(entity) {
            this.timeSlowRatio = entity.TimeSlowRatio;
            this.duration      = entity.Duration;
            this.cooldown      = entity.CoolDown;
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
    //=================================================================== << CURE >> ==================================================================
    public class Acsp_Cure : AccessorySPEffect {
        public override string GetDescByTerms() {
            throw new System.NotImplementedException();
        }

        public override void Init() {
            throw new System.NotImplementedException();
        }
    }
    //=================================================================================================================================================
    //=============================================================== << CURSED SLOW >> ===============================================================
    public class Acsp_CursedSlow : AccessorySPEffect {
        public override string GetDescByTerms() {
            throw new System.NotImplementedException();
        }

        public override void Init() {
            throw new System.NotImplementedException();
        }
    }
    //=================================================================================================================================================
}
