namespace ActionCat {
    using UnityEngine;
    using ActionCat.Interface;

    public abstract class ProjectileType : ArrowSkill {
        protected ProjectilePref projectilePref = null;
        public abstract void OnHit(Vector2 point);
        public virtual bool TryGetPrefab(out ProjectilePref pref) {
            if(projectilePref == null) {
                pref = null;
                return false;
            }

            pref = projectilePref;
            return true;
        }

        public virtual int DefaultSpawnSize() => 10;
    }

    public class SplitArrow : ProjectileType {
        public override void Clear() {
            throw new System.NotImplementedException();
        }

        public override void OnHit(Vector2 point) {
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

        //Not Saved
        private float intervalAngle;

        public override void Init(Transform tr, Rigidbody2D rigid, IArrowObject arrowInter) {
            intervalAngle = StNum.DegreeFull / projectileCount;
        }

        public override void OnHit(Vector2 point) {
            float randomAngle = Random.Range(0f, StNum.DegreeFull);
            for (int i = 1; i <= projectileCount; i++) {
                Quaternion randomRotation = Quaternion.AngleAxis(randomAngle + (intervalAngle * i), Vector3.forward);
                var dagger = CCPooler.SpawnFromPool<ProjectilePref>(projectilePref.PrefName, point, randomRotation);
                if (dagger) {
                    dagger.Shot();
                }
            }
            CatLog.Break();
        }

        public override void Clear() { }

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

        public override void OnHit(Vector2 point) {
            throw new System.NotImplementedException();
        }
        public override void Clear() { }

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
