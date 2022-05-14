namespace ActionCat {
    using UnityEngine;
    using ActionCat.Interface;

    //================================================================= << PARENT >> ==================================================================
    [System.Serializable]
    public abstract class AccessorySPEffect {
        // [ Saved-Variables ]
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
        public bool IsStartingPrepared {
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

        public virtual float GetRange() {
            throw new System.NotImplementedException();
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
        /// ARTIFACT SKILL에 만들어놓기는 했는데, SKILL에서는 등급을 string으로 표현할 필요가 없음. 아이템 등급에 적용해주면 좋을 듯
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
        //=========================================================== << PASSIVE >> ==========================================================
        public abstract void Init();
        //============================================================ << BUFF >> ============================================================
        public abstract void OnActive();

        /// <summary>
        /// 특수효과 발동 중 배틀 종료처리 되었을 때 효과 중지처리.
        /// </summary>
        public virtual void OnStop() { return; }

        /// <summary>
        /// 무한 루프 주의. return float: 지속시간/간격, activatingCount: 발동횟수
        /// </summary>
        /// <param name="activatingCount"></param>
        /// <returns></returns>
        public virtual float GetDuration(out int activatingCount) {
            throw new System.NotImplementedException();
        }

        //=========================================================== << DE-BUFF >> ==========================================================
        public virtual void ActiveDebuff(MonsterStatus[] position) {
            throw new System.NotImplementedException();
        }

        //=========================================================== << TRIGGER >> ==========================================================
    }
    //=================================================================================================================================================
    //================================================================ << AIM SIGHT >> ================================================================
    public class Acsp_AimSight : AccessorySPEffect {
        // [ Saved-Variables ]
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
        // [ Saved-Variables ]
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
        // [ Saved-Variables ]
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
        // [ Saved-Variables ]
        float radius = 0f;
        float slowRatio = 0f;
        float duration = 0f;

        public override float GetRange() {
            return radius;
        }

        public override string GetDescByTerms() {
            I2.Loc.LocalizedString loc = descTerms;
            return string.Format(loc, (slowRatio * 100).ToString().GetColor(StringColor.GREEN), duration.ToString().GetColor(StringColor.YELLOW));
        }

        public override void ActiveDebuff(MonsterStatus[] statusArray) {
            //base.ActiveDebuff(position);
            //전달받은 position에 Physics2D로 콜라이더 뿌려서 해당 반경의 몬스터 객체들 거두고 
            //걔네들한테 MonsterState 컴포넌트 배열로 잡아서 다 디버프 효과 뿌려주면 될듯?
            //CatLog.Log("Artifact SP Effect: Cursed Slow까지 잘 전달됨");

            //var isTargetExist = GameGlobal.TryGetOverlapCircleAll2D(out Collider2D[] targets, position, radius, AD_Data.LAYER_MONSTER);
            //if (isTargetExist) {
            //    foreach (var target in targets) {
            //        if (target.TryGetComponent<MonsterState>(out MonsterState state)) {
            //            state.ValActionSpeed(slowRatio, duration);
            //        }
            //        else {
            //            CatLog.WLog($"The Target Can't Exsist MonsterState Component, but this target is Monster. Name: {target.name}");
            //        }
            //    }
            //}

            foreach (var status in statusArray) {
                status.ValActionSpeed(slowRatio, duration);
            }
        }

        public Acsp_CursedSlow(SkillEntity_CurseSlow entity) : base(entity) {
            this.radius    = entity.RangeRadius;
            this.slowRatio = entity.SlowRatio;
            this.duration  = entity.Duration;
        }
        #region ES3
        public Acsp_CursedSlow() { }
        #endregion
        #region NOTUSED
        public override void Init() {
            return;
        }

        public override void OnActive() {
            throw new System.NotImplementedException();
        }
        #endregion
    }
    //=================================================================================================================================================
}
