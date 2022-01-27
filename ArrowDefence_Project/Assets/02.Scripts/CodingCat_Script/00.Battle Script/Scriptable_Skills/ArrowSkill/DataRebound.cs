namespace ActionCat
{
    using UnityEngine;

    public class DataRebound : ArrowSkillData
    {
        [Range(1f, 10f)] public float ScanRadius  = 5f;    //Default 5f
        [Range(1, 3)]    public int MaxChainCount = 2;     //Default 2

        public DataRebound()
        {
            ActiveType = ARROWSKILL_ACTIVETYPE.ATTACK;
            SkillType  = ARROWSKILL.SKILL_REBOUND;
        }

        public void OnEnable()
        {
            this.skillData = new ReboundArrow(this);
        }
    }
}
