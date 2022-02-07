namespace ActionCat {
    using UnityEngine;

    public abstract class ProjectileType : ArrowSkill {
        protected GameObject projectilePref = null;
        public abstract void OnHit();
    }

    public class SplitArrow : ProjectileType {
        public override void Clear() {
            throw new System.NotImplementedException();
        }

        public override void OnHit() {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Copy Class Constructor
        /// </summary>
        /// <param name="splitArrow"></param>
        public SplitArrow(DataSplit data) {

        }

        public SplitArrow(SplitArrow origin) {

        }
        #region ES3
        public SplitArrow() { }
        ~SplitArrow() { }
        #endregion
    }

    public class SplitDagger : ProjectileType {
        private int projectileCount;

        public override void OnHit() {
            throw new System.NotImplementedException();
        }

        public override void Clear() {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Constructor for Skill Data ScriptableObject
        /// </summary>
        /// <param name="data"></param>
        public SplitDagger(DataSplitDagger data) {
            projectilePref  = data.daggerPref;
            projectileCount = data.projectileCount;
        }
        /// <summary>
        /// Constructor for Skill Clone
        /// </summary>
        /// <param name="origin"></param>
        public SplitDagger(SplitDagger origin) {
            projectilePref  = origin.projectilePref;
            projectileCount = origin.projectileCount;
        }
        #region ES3
        public SplitDagger() { }
        ~SplitDagger() { }
        #endregion
    }

    public class ElementalFire : ProjectileType {
        private float activationProbability;

        public override void OnHit() {
            throw new System.NotImplementedException();
        }
        public override void Clear() {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Constructor for Skill Data ScriptableObject
        /// </summary>
        /// <param name="data"></param>
        public ElementalFire(DataEltalFire data) {
            projectilePref = data.firePref;
            activationProbability = data.ActivationProbability;
        }
        /// <summary>
        /// Constructor for Skill Clone
        /// </summary>
        /// <param name="origin"></param>
        public ElementalFire(ElementalFire origin) {
            projectilePref = origin.projectilePref;
            activationProbability = origin.activationProbability;
        }
        #region ES3
        public ElementalFire() { }
        ~ElementalFire() { }
        #endregion
    }
}
