namespace ActionCat
{
    public class ASInfo
    {
        //FIELD
        string id;
        string name;
        string desc;
        SKILL_LEVEL level;
        ARROWSKILL type;
        ARROWSKILL_ACTIVETYPE activeType;
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

        public ARROWSKILL_ACTIVETYPE ActiveType{
            get{
                return activeType;
            }
        }

        /// <summary>
        /// Create New Arrow Skill Information to Item
        /// </summary>
        /// <param name="so"></param>
        public ASInfo(ArrowSkillData so)
        {
            id         = so.SkillId;
            name       = so.SkillName;
            desc       = so.SkillDesc;
            level      = so.SkillLevel;
            type       = so.SkillType;
            activeType = so.ActiveType;
            skillData  = so.ArrowSkill;
        }

        /// <summary>
        /// Copy Arrow Skill Information to Item
        /// </summary>
        /// <param name="origin"></param>
        public ASInfo(ASInfo origin)
        {
            id         = origin.id;
            name       = origin.name;
            desc       = origin.desc;
            level      = origin.level;
            type       = origin.type;
            activeType = origin.activeType;
            skillData  = origin.skillData;
        }

        public ASInfo() { }
        ~ASInfo() { }
    }
}
