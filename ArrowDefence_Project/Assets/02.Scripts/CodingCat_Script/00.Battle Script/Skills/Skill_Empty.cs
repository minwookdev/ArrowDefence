namespace ActionCat
{
    using UnityEngine;

    public class Skill_Empty : AD_BowSkill
    {
        /// <summary>
        /// Constructor using Skill Data Scriptableobject. (Main)
        /// </summary>
        /// <param name="data"></param>
        public Skill_Empty(SkillData_Empty data)
            : base(data.SkillId, data.SkillName, data.SkillDesc, data.SkillLevel, data.SkillType, data.SkillIconSprite) {

        }
        #region ES3
        public Skill_Empty() : base() { }
        #endregion

        public override void Init() {
            throw new System.NotImplementedException();
        }

        public override void BowSpecialSkill(Transform bowTr, AD_BowController controller, ref DamageStruct damage, Vector3 initPos, ARROWTYPE type) {
            throw new System.NotImplementedException();
        }
    }
}
