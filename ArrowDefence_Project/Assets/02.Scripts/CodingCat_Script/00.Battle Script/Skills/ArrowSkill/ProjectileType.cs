namespace ActionCat {
    using UnityEngine;
    using ActionCat.Interface;
    //==================================================================== [ PARENT ] =========================================================================
    public abstract class ProjectileType : ArrowSkill {
        protected ProjectilePref projectilePref = null;
        protected short projectileDamage;

        //Not Saved
        protected string projectilePoolTag  = "";
        protected PlayerAbilitySlot ability = null;

        public abstract void OnHit(Vector2 point, ref DamageStruct damage);
        public virtual bool TryGetPrefab(out ProjectilePref pref) {
            if(projectilePref == null) {
                pref = null;
                return false;
            }

            pref = projectilePref;
            return true;
        }

        public virtual int DefaultSpawnSize() => 10;

        public virtual void SetPoolTag(string tag) {
            projectilePoolTag = tag;
            CatLog.Log($"Projectile PoolTag : {projectilePoolTag}");
        }

        public virtual void SetAbility(PlayerAbilitySlot address) {
            ability = address;
        }

        public abstract string GetUniqueTag();

        public override void Release() {
            projectilePoolTag = "";
            ability = null;
        }

        public ProjectileType() { }

        //이 생성자에서 ability 받아주면 된다.
        protected ProjectileType(string tag, PlayerAbilitySlot address) {
            projectilePoolTag = tag;
            ability           = address;

        }
    }
    //=========================================================================================================================================================
    //================================================================= [ SPLIT ARROW ] =======================================================================
    public class SplitArrow : ProjectileType {
        public override void Clear() {
            throw new System.NotImplementedException();
        }

        public override void OnHit(Vector2 point, ref DamageStruct damage) {
            throw new System.NotImplementedException();
        }

        public override string GetUniqueTag() {
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
    //=========================================================================================================================================================
    //================================================================= [ SPLIT DAGGER ] ======================================================================
    public class SplitDagger : ProjectileType {
        private int projectileCount;

        //Not Saved
        private float intervalAngle;

        public override string GetUniqueTag() {
            return "splitdagger";
        }

        public override void Init(Transform tr, Rigidbody2D rigid, IArrowObject arrowInter) {
            intervalAngle = StNum.DegreeFull / projectileCount;
        }

        public override void OnHit(Vector2 point, ref DamageStruct damage) {
            float randomAngle = Random.Range(0f, StNum.DegreeFull);
            for (int i = 1; i <= projectileCount; i++) {
                Quaternion randomRotation = Quaternion.AngleAxis(randomAngle + (intervalAngle * i), Vector3.forward);
                var dagger = CCPooler.SpawnFromPool<ProjectilePref>(projectilePoolTag, point, randomRotation);
                if (dagger) {
                    dagger.Shot(damage, ability.GetProjectileDamage(projectileDamage));
                }
            }
        }

        public override void Clear() { }

        public override void Release() {
            base.Release();
            intervalAngle = 0f;
        }

        /// <summary>
        /// Constructor for Skill Data ScriptableObject
        /// </summary>
        /// <param name="data"></param>
        public SplitDagger(DataSplitDagger data) {
            projectilePref   = data.daggerPref;
            projectileCount  = data.projectileCount;
            projectileDamage = data.ProjectileDamage;
        }
        /// <summary>
        /// Constructor for Skill Clone
        /// </summary>
        /// <param name="origin"></param>
        public SplitDagger(SplitDagger origin) : base(origin.projectilePoolTag, origin.ability) {
            projectilePref   = origin.projectilePref;
            projectileCount  = origin.projectileCount;
            projectileDamage = origin.projectileDamage;
        }
        #region ES3
        public SplitDagger() { }
        ~SplitDagger() { }
        #endregion
    }
    //=========================================================================================================================================================
    //=============================================================== [ ELEMENTAL - FIRE ] ====================================================================
    public class ElementalFire : ProjectileType {
        private float activationProbability;

        public override string GetUniqueTag() {
            return "elemental_fire";
        }

        public override void OnHit(Vector2 point, ref DamageStruct damage) {
            if (Random.Range(0f, 100f) < activationProbability + ability.ElementalActivationRateIncrease) {
                var fire = CCPooler.SpawnFromPool<ElementalFirePref>(projectilePoolTag, point, Quaternion.identity);
                if (fire) {
                    fire.Shot(damage, ability.GetProjectileDamage(projectileDamage)); //
                }
            }
        }
        public override void Clear() { }

        public override void Release() {
            base.Release();
        }

        /// <summary>
        /// Constructor for Skill Data ScriptableObject
        /// </summary>
        /// <param name="data"></param>
        public ElementalFire(DataEltalFire data) {
            projectilePref        = data.firePref;
            activationProbability = data.ActivationProbability;
            projectileDamage      = data.ProjectileDamage;
        }
        /// <summary>
        /// Constructor for Skill Clone
        /// </summary>
        /// <param name="origin"></param>
        public ElementalFire(ElementalFire origin): base(origin.projectilePoolTag, origin.ability) {
            projectilePref        = origin.projectilePref;
            activationProbability = origin.activationProbability;
            projectileDamage      = origin.projectileDamage;
        }
        #region ES3
        public ElementalFire() { }
        ~ElementalFire() { }
        #endregion
    }
    //=========================================================================================================================================================
}
