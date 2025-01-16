namespace ActionCat {
    using I2.Loc;
    public class ASInfo {
        //FIELD
        string id;
        SKILL_LEVEL level;
        ARROWSKILL type;
        ARROWSKILL_ACTIVETYPE activeType;
        UnityEngine.Sprite iconSprite;
        ArrowSkill skillData;
        string nameTerms;
        string descTerms;

        //PROPERTIES
        public ArrowSkill SkillData {
            get {
                if (skillData != null)
                    return skillData;
                else
                    return null;
            }
        }

        public string SkillId { get => id; }
        public SKILL_LEVEL SkillLevel { get => level; }
        public ARROWSKILL SkillType { get => type; }
        public ARROWSKILL_ACTIVETYPE ActiveType { get => activeType; }
        public UnityEngine.Sprite IconSprite { get => iconSprite; }
        public string NameByTerms {
            get {
                LocalizedString loc = nameTerms;
                return loc;
            }
        }
        public string DescByTerms {
            get {
                LocalizedString loc = descTerms;
                return loc;
            }
        }

        /// <summary>
        /// Create New Arrow Skill Information to Item
        /// </summary>
        /// <param name="entity"></param>
        public ASInfo(ArrowSkillData entity) {
            id         = entity.SkillId;
            level      = entity.SkillLevel;
            type       = entity.SkillType;
            activeType = entity.ActiveType;
            iconSprite = entity.IconSprite;
            skillData  = GetNewSkill(entity.ArrowSkill);
            nameTerms  = entity.NameTerms;
            descTerms  = entity.DescTerms;
        }

        /// <summary>
        /// Copy Arrow Skill Information to Item
        /// </summary>
        /// <param name="origin"></param>
        public ASInfo(ASInfo origin) {
            id         = origin.id;
            level      = origin.level;
            type       = origin.type;
            activeType = origin.activeType;
            iconSprite = origin.iconSprite;
            skillData  = origin.skillData;
            nameTerms  = origin.nameTerms;
            descTerms  = origin.descTerms;
        }

        public ArrowSkill GetNewSkill(ArrowSkill data) {
            switch (data) {
                //=================================== [ TYPE : HIT ] ===================================
                case ReboundArrow newskill:   return new ReboundArrow(newskill);
                case PiercingArrow newskill:  return new PiercingArrow(newskill);
                //=================================== [ TYPE : AIR ] ===================================
                case HomingArrow newskill:    return new HomingArrow(newskill);
                //================================ [ TYPE : PROJECTILE ] ===============================
                case SplitDagger newskill:    return new SplitDagger(newskill);
                case SplitArrow newskill:     return new SplitArrow(newskill);
                case ElementalFire newskill:  return new ElementalFire(newskill);
                case Explosion newskill:      return new Explosion(newskill);
                //===================================== [ BUFF ] =======================================
                case ElementalAmplification newskill: return new ElementalAmplification(newskill);
                //===================================== [ EMPTY ] ======================================
                case EmptyTypeArrowSkill newskill:    return new EmptyTypeArrowSkill(newskill);
                //======================================================================================
                default: throw new System.NotImplementedException("this type is Not Implemented !");
            }
        }

        public string GetNameByTerms() {
            I2.Loc.LocalizedString loc = nameTerms;
            return loc;
        }

        public string GetDescByTerms() {
            I2.Loc.LocalizedString loc = descTerms;
            return skillData.GetDesc(loc);
        }

        public void Release() {
            skillData.ClearOrigin();
        }

        public ASInfo() { }
        ~ASInfo() { }
    }
}
