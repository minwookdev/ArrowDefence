namespace ActionCat
{
    using UnityEngine;

    public class Skill_Empty : AD_BowSkill
    {
        public Skill_Empty() : base() { }

        public Skill_Empty(string id, string name, string desc, SKILL_LEVEL level, BOWSKILL_TYPE type)
            : base(id, name, desc, level, type)
        {

        }

        public override void BowSpecialSkill(float facingVec, Transform arrowParent, MonoBehaviour mono, 
                                             Vector3 initScale, Vector3 arrowInitPos, Vector2 force, LOAD_ARROW_TYPE arrowType)
        {
            return;
        }
    }
}
