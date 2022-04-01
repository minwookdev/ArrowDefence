namespace ActionCat {
    using UnityEngine;

    public abstract class AD_BowSkill {
        protected string id;
        protected Sprite iconSprite;
        protected SKILL_LEVEL level;
        protected BOWSKILL_TYPE skillType;

        protected string termsName;
        protected string termsDesc;

        #region PROPERTY
        public string Id { get => id; }
        public Sprite IconSprite
        {
            get
            {
                if (this.iconSprite != null)
                    return iconSprite;
                else
                    return null;
            }
        }
        public SKILL_LEVEL Level { get => level; }
        public BOWSKILL_TYPE Type { get => skillType; }
        public string NameTerms {
            get => termsName;
        }
        public string DescTerms {
            get => termsDesc;
        }
        public string NameByTerms {
            get {
                I2.Loc.LocalizedString loc = termsName;
                return loc;
            }
        }
        public string DescByTerms {
            get {
                I2.Loc.LocalizedString loc = termsDesc;
                return loc;
            }
        }
        #endregion

        /// <summary>
        /// Constructor With no Parameters. (Used Saving Function. Don't Delete this) 
        /// </summary>
        public AD_BowSkill() { }
        ~AD_BowSkill() { }

        protected AD_BowSkill(BowSkillData entity) {
            this.id         = entity.SkillId;
            this.level      = entity.SkillLevel;
            this.skillType  = entity.SkillType;
            this.iconSprite = entity.SkillIconSprite;
            this.termsName  = entity.NameTerms;
            this.termsDesc  = entity.DescTerms;
        }

        public abstract void Init();

        public abstract void BowSpecialSkill(Transform bowTr, AD_BowController controller, ref DamageStruct damage, Vector3 initPos, ARROWTYPE type);
        
        protected virtual bool TryGetTag(ARROWTYPE type, out string tag) {
            switch (type) {
                case ARROWTYPE.ARROW_MAIN:    tag = AD_Data.POOLTAG_MAINARROW_LESS;          return true;
                case ARROWTYPE.ARROW_SUB:     tag = AD_Data.POOLTAG_SUBARROW_LESS;           return true;
                case ARROWTYPE.ARROW_SPECIAL: tag = null;                                    return false;
                default: throw new System.NotImplementedException($"This Arrow Type is Not Implemented. (TYPE: {type})");
            }
        }

        public virtual string GetNameByTerms() {
            I2.Loc.LocalizedString loc = termsName;
            return loc;
        }

        public virtual string GetDescByTerms() {
            I2.Loc.LocalizedString loc = termsDesc;
            return loc;
        }
    }
}
