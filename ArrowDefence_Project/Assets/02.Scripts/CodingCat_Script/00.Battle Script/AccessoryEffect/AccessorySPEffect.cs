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
        public ArtifactCondition Condition {
            get {
                return condition;
            }
        }
        #endregion
        public bool IsPrepared {
            get; private set;
        } = false;

        public virtual string GetNameByTerms() {
            I2.Loc.LocalizedString loc = nameTerms;
            return loc;
        }

        public virtual string GetDescByTerms() {
            I2.Loc.LocalizedString loc = descTerms;
            return loc;
        }

        protected AccessorySPEffect(AccessorySkillData entity) {
            this.id         = entity.SkillId;
            this.effectType = entity.EffectType;
            this.level      = entity.SkillLevel;
            this.iconSprite = entity.SkillIconSprite;
            this.nameTerms  = entity.NameTerms;
            this.descTerms  = entity.DescTerms;

            //assignment artifact condition
            switch (entity.ConditionType) {
                case ARTCONDITION.NONE:    condition = new ArtCondition_Empty(); break;
                case ARTCONDITION.TRIGGER: condition = new ArtCondition_Trigger(entity.MaxStack, entity.MaxCost, entity.CoolDownTime, entity.IncreaseCostCount); break;
                case ARTCONDITION.BUFF:    condition = new ArtCondition_Buff(entity.CoolDownTime); break;
                case ARTCONDITION.DEBUFF:  condition = new ArtCondition_Debuff(entity.MaxStack, entity.MaxCost, entity.CoolDownTime); break;
                case ARTCONDITION.PASSIVE: condition = new ArtCondition_Empty(); break;
                default: throw new System.NotImplementedException();
            }
        }
        #region ES3
        public AccessorySPEffect() { }
        #endregion

        /// <summary>
        /// 특수효과 발동 중 배틀 종료처리 되었을 때 효과 중지처리.
        /// </summary>
        public virtual void OnStop() { return; }

        public abstract void OnActive();

        public abstract void Init();

        /// <summary>
        /// 무한 루프 주의. return float: 지속시간/간격, activatingCount: 발동횟수
        /// </summary>
        /// <param name="activatingCount"></param>
        /// <returns></returns>
        public virtual float GetDuration(out int activatingCount) {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// ### Artifact Grade 선적용
        /// </summary>
        /// <returns></returns>
        protected string GetGradeString() {
            switch (this.level) {
                case SKILL_LEVEL.LEVEL_LOW:    return  " [1T]";
                case SKILL_LEVEL.LEVEL_MEDIUM: return  " [2T]";
                case SKILL_LEVEL.LEVEL_HIGH:   return  " [3T]";
                case SKILL_LEVEL.LEVEL_UNIQUE: return  " [U]";
                default: throw new System.NotImplementedException();
            }
        }
    }
    //=================================================================================================================================================
    //================================================================ << AIM SIGHT >> ================================================================
    public class Acsp_AimSight : AccessorySPEffect {
        private GameObject aimSightPref;

        public override void Init() {
            var controller = GameGlobal.GetControllerByTag().GetComponent<AD_BowController>();
            var parent     = controller.GetSlotTrOrNull;
            if (parent) {
                var aimsighter = GameObject.Instantiate(aimSightPref, parent.position, Quaternion.identity, parent).GetComponent<SPEffect_AimSight>();
                aimsighter.Initialize(controller);
            }
        }

        public override void OnActive() {
            throw new System.NotImplementedException();
        }

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
    public class Acsp_SlowTime : AccessorySPEffect {
        float timeSlowRatio;
        float duration;

        public override void OnActive() {
            GameManager.Instance.TimeScaleSet(StNum.floatOne - timeSlowRatio);
        }

        public override float GetDuration(out int activatingCount) {
            activatingCount = 1;
            return duration;
        }

        public override void OnStop() {
            GameManager.Instance.TimeToDefault();
        }

        public override void Init() {
            return;
        }

        public override string GetDescByTerms() {
            I2.Loc.LocalizedString loc = descTerms;
            return string.Format(loc, (timeSlowRatio * 100f).ToString().GetColor(StringColor.GREEN), duration.ToString().GetColor(StringColor.GREEN));
        }

        public Acsp_SlowTime(SkillDataSlowTime entity) : base(entity) {
            this.timeSlowRatio = entity.TimeSlowRatio;
            this.duration      = entity.Duration;
        }
        #region ES3
        public Acsp_SlowTime() : base() { }
        #endregion
    }
    //=================================================================================================================================================
    //=================================================================== << CURE >> ==================================================================
    public class Acsp_Cure : AccessorySPEffect {
        float healAmount = 0f;
        int healRepeatTime = 0;
        float repeatIntervalTime = 0f;

        public override void OnActive() {
            BattleProgresser.OnIncPlayerHealth(healAmount);
        }

        public override void OnStop() {
            CatLog.Log("OnStop Artifact Cure Effect.");
            return;
        }

        public override float GetDuration(out int activatingCount) {
            activatingCount = healRepeatTime;
            return repeatIntervalTime;
        }

        public override string GetDescByTerms() {
            I2.Loc.LocalizedString loc = descTerms;
            return string.Format(loc, (healAmount * healRepeatTime).ToString().GetColor(StringColor.GREEN), healRepeatTime.ToString().GetColor(StringColor.GREEN));
        }

        public Acsp_Cure(SkillEntity_Cure entity) : base(entity) {
            this.healAmount         = entity.HealAmount;
            this.healRepeatTime     = entity.HealRepeatTime;
            this.repeatIntervalTime = entity.RepeatIntervalTime;
        }
        #region ES3
        public Acsp_Cure() { }
        #endregion

        public override void Init() {
            return;
        }
    }
    //=================================================================================================================================================
    //=============================================================== << CURSED SLOW >> ===============================================================
    public class Acsp_CursedSlow : AccessorySPEffect {
        float radius = 0f;
        float slowRatio = 0f;
        float duration = 0f;

        public override void Init() {
            return;
        }

        public override void OnActive() {
            throw new System.NotImplementedException();
        }

        public override string GetDescByTerms() {
            I2.Loc.LocalizedString loc = descTerms;
            return string.Format(loc, (slowRatio * 100).ToString().GetColor(StringColor.GREEN));
        }
        public Acsp_CursedSlow(SkillEntity_CurseSlow entity) : base(entity) {
            this.radius    = entity.RangeRadius;
            this.slowRatio = entity.SlowRatio;
            this.duration  = entity.Duration;
        }
        #region ES3
        public Acsp_CursedSlow() { }
        #endregion
    }
    //=================================================================================================================================================
}
