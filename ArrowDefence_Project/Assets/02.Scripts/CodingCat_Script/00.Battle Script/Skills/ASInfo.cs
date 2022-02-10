namespace ActionCat
{
    public class ASInfo {
        //FIELD
        string id;
        string name;
        string desc;
        SKILL_LEVEL level;
        ARROWSKILL type;
        ARROWSKILL_ACTIVETYPE activeType;
        UnityEngine.Sprite iconSprite;
        ArrowSkill skillData;

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
        public string SkillName { get => name; }
        public string SkillDesc { get => desc; }
        public SKILL_LEVEL SkillLevel { get => level; }
        public ARROWSKILL SkillType { get => type; }
        public ARROWSKILL_ACTIVETYPE ActiveType { get => activeType; }
        public UnityEngine.Sprite IconSprite { get => iconSprite; }

        /// <summary>
        /// Create New Arrow Skill Information to Item
        /// </summary>
        /// <param name="so"></param>
        public ASInfo(ArrowSkillData so) {
            id         = so.SkillId;
            name       = so.SkillName;
            desc       = so.SkillDesc;
            level      = so.SkillLevel;
            type       = so.SkillType;
            activeType = so.ActiveType;
            iconSprite = so.IconSprite;
            skillData  = GetNewSkill(so.ArrowSkill);
        }

        /// <summary>
        /// Copy Arrow Skill Information to Item
        /// </summary>
        /// <param name="origin"></param>
        public ASInfo(ASInfo origin) {
            id         = origin.id;
            name       = origin.name;
            desc       = origin.desc;
            level      = origin.level;
            type       = origin.type;
            activeType = origin.activeType;
            iconSprite = origin.iconSprite;
            skillData  = origin.skillData;
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
                //======================================================================================
                default: throw new System.NotImplementedException("this type is Not Implemented !");
            }
        }

        public void Release() {
            skillData.Release();
        }

        public ASInfo() { }
        ~ASInfo() { }
    }
}
