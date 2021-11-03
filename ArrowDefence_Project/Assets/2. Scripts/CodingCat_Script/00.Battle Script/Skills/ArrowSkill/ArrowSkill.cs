namespace ActionCat
{
    using UnityEngine;

    public abstract class ArrowSkill
    {
        public abstract void OnHitSkillActive();
        public abstract void OnAirSkillActive();
    }

    public class ReboundArrow : ArrowSkill
    {
        public override void OnAirSkillActive()
        {
            throw new System.NotImplementedException();
        }

        public override void OnHitSkillActive()
        {
            throw new System.NotImplementedException();
        }
    }

    public class GuidanceArrow : ArrowSkill
    {
        public override void OnAirSkillActive()
        {
            throw new System.NotImplementedException();
        }

        public override void OnHitSkillActive()
        {
            throw new System.NotImplementedException();
        }
    }

    public class SplitArrow : ArrowSkill
    {
        public override void OnAirSkillActive()
        {
            throw new System.NotImplementedException();
        }

        public override void OnHitSkillActive()
        {
            throw new System.NotImplementedException();
        }
    }
}
