namespace ActionCat
{
    using UnityEngine;

    public class Skill_Empty : AD_BowSkill
    {
        public Skill_Empty() : base() { }

        public Skill_Empty(string name, string desc, SKILL_LEVEL level, BOWSKILL_TYPE type)
            : base(name, desc, level, type)
        {

        }

        public override void BowSpecialSkill(float facingVec, float arrowSpreadAngle, byte numOfArrows,
            Transform arrowParent, AD_BowController adBow, Vector3 initScale, Vector3 arrowInitPos, Vector2 force, LOAD_ARROW_TYPE arrowType)
        {

        }
    }
}
