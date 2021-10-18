namespace ActionCat
{
    using UnityEngine;

    public enum BOWSKILL_TYPE
    {
        SKILL_EMPTY,
        SKILL_SPREAD_SHOT,
        SKILL_RAPID_SHOT,
        SKILL_ARROW_RAIN
    }

    public enum SKILL_LEVEL
    { 
        LEVEL_LOW,
        LEVEL_MEDIUM,
        LEVEL_HIGH
    }

    public abstract class AD_BowSkill
    {
        protected string name;
        protected string desc;
        protected SKILL_LEVEL level;
        protected BOWSKILL_TYPE skillType;

        #region PROPERTY
        public string Name { get => name; }
        public string Description { get => desc; }
        public SKILL_LEVEL Level { get => level; }
        public BOWSKILL_TYPE Type { get => skillType; }
        #endregion

        /// <summary>
        /// Constructor With no Parameters. (Used Saving Function. Don't Delete this) 
        /// </summary>
        public AD_BowSkill() { }
        ~AD_BowSkill() { }

        protected AD_BowSkill(string skillname, string skilldesc, SKILL_LEVEL level, BOWSKILL_TYPE type)
        {
            this.name      = skillname;
            this.desc      = skilldesc;
            this.level     = level;
            this.skillType = type;
        }

        public abstract void BowSpecialSkill(float facingVec, float arrowSpreadAngle, byte numOfArrows, Transform arrowParent,
                                  AD_BowController adBow, Vector3 initscale, Vector3 arrowInitPos, Vector2 force, LOAD_ARROW_TYPE arrowType);
    }
}
