namespace CodingCat_Games
{
    using System;
    using UnityEngine;

    public enum Enum_BowSkillType
    {
        SKILL_NULL,
        SKILL_SPREAD_SHOT,
        SKILL_RAPID_SHOT,
        SKILL_ARROW_RAIN
    }

    public class AD_BowSkill
    {
        public virtual void BowSpecialSkill(float facingVec, float arrowSpreadAngle, byte numOfArrows, Transform arrowParent,
                                  AD_BowController adBow, Vector3 initscale, Vector3 arrowInitPos, Vector2 force) { }
    }
}
