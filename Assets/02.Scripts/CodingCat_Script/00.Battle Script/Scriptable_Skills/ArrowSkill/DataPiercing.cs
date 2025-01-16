namespace ActionCat {
    using UnityEngine;

    public class DataPiercing : ArrowSkillData {
        [Range(1, 3)] public byte MaxChainCount = 2;

        public DataPiercing() {
            ActiveType = ARROWSKILL_ACTIVETYPE.ATTACK;
            SkillType  = ARROWSKILL.SKILL_PIERCING;
        }

        public void OnEnable() {
            skillData = new PiercingArrow(this);
        }
    }
}
