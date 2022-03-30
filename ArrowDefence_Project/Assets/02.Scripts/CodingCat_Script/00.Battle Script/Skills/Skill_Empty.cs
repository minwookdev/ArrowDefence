namespace ActionCat {
    using UnityEngine;

    public class Skill_Empty : AD_BowSkill {
        public override string GetNameByTerms() {
            throw new System.NotImplementedException();
        }

        public override string GetDescByTerms() {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Constructor using Skill Data Scriptableobject. (Main)
        /// </summary>
        /// <param name="data"></param>
        public Skill_Empty(SkillData_Empty data) : base(data) {

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
