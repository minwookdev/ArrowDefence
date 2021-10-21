namespace ActionCat
{
    using UnityEngine;

    public abstract class AD_BowSkill
    {
        protected string id;
        protected string name;
        protected string desc;
        protected SKILL_LEVEL level;
        protected BOWSKILL_TYPE skillType;

        #region PROPERTY
        public string Id { get => id; }
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

        protected AD_BowSkill(string skillid, string skillname, string skilldesc, SKILL_LEVEL level, BOWSKILL_TYPE type)
        {
            this.id        = skillid;
            this.name      = skillname;
            this.desc      = skilldesc;
            this.level     = level;
            this.skillType = type;
        }

        public abstract void BowSpecialSkill(float anglez, Transform arrowParent, MonoBehaviour mono, 
                                             Vector3 initscale, Vector3 arrowInitPos, Vector2 force, LOAD_ARROW_TYPE arrowType);
    }
}
