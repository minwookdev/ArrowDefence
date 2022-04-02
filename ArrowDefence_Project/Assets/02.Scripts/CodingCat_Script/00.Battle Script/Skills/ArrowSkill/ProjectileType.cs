namespace ActionCat {
    using System.Collections.Generic;
    using UnityEngine;
    using ActionCat.Interface;
    //==================================================================== [ PARENT ] =========================================================================
    public abstract class ProjectileType : ArrowSkill {
        protected ProjectilePref projectilePref = null;
        protected short projectileDamage;

        //NON-SAVED
        protected PlayerAbilitySlot ability = null;
        protected string[] poolTags = null;

        public abstract void OnHit(Vector2 point, ref DamageStruct damage);

        /// <summary>
        /// 두 개 이상의 Projectile Prefab을 사용하는 Skill에 한해서 override해서 사용
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, GameObject> GetProjectileDic() {
            var dictionary = new Dictionary<string, GameObject>();
            dictionary.Add(GetUniqueTags()[0], projectilePref.gameObject);
            poolTags = new string[0]; // Assign New Empty PoolTags Array.
            return dictionary;
        }

        public virtual int DefaultSpawnSize() => 10;

        public void AddPoolTag(string tag) {
            poolTags = GameGlobal.AddArray<string>(poolTags, tag);
            CatLog.Log($"New Projectile PoolTag: {tag}");
        }

        public virtual void SetAbility(PlayerAbilitySlot address) {
            ability = address;
        }

        protected abstract string[] GetUniqueTags();

        /// <summary>
        /// End Battle, Clear Variables
        /// </summary>
        public override void Release() {
            poolTags = null;
            ability  = null;
        }

        public ProjectileType() { }

        /// <summary>
        /// Clone Constructor
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="address"></param>
        protected ProjectileType(string[] tags, PlayerAbilitySlot address) {
            poolTags = tags;
            ability  = address;

        }

        ///public virtual bool TryGetPrefab(out ProjectilePref pref) {
        ///    if(projectilePref == null) {
        ///        pref = null;
        ///        return false;
        ///    }
        ///
        ///    pref = projectilePref;
        ///    return true;
        ///}
    }
    //=========================================================================================================================================================
    //================================================================= [ SPLIT ARROW ] =======================================================================
    public class SplitArrow : ProjectileType {
        public override string GetDescription(string localizedString) {
            throw new System.NotImplementedException();
        }

        public override void Clear() {
            throw new System.NotImplementedException();
        }

        public override void OnHit(Vector2 point, ref DamageStruct damage) {
            throw new System.NotImplementedException();
        }

        protected override string[] GetUniqueTags() {
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
        //SAVED
        private int projectileCount;

        //Not Saved
        private float intervalAngle;

        public override string GetDescription(string localizedString) {
            throw new System.NotImplementedException();
        }

        protected override string[] GetUniqueTags() {
            return new string[1] { "splitdagger" };
        }

        public override void Init(Transform tr, Rigidbody2D rigid, IArrowObject arrowInter) {
            intervalAngle = StNum.DegreeFull / projectileCount;
        }

        public override void OnHit(Vector2 point, ref DamageStruct damage) {
            float randomAngle = Random.Range(0f, StNum.DegreeFull);
            for (int i = 1; i <= projectileCount; i++) {
                Quaternion randomRotation = Quaternion.AngleAxis(randomAngle + (intervalAngle * i), Vector3.forward);
                var dagger = CCPooler.SpawnFromPool<ProjectilePref>(poolTags[0], point, randomRotation);
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
        public SplitDagger(SplitDagger origin) : base(origin.poolTags, origin.ability) {
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
        public override string GetDescription(string localizedString) {
            throw new System.NotImplementedException();
        }

        private float activationProbability;

        protected override string[] GetUniqueTags() {
            return new string[1] { "elemental_fire" };
        }

        public override void OnHit(Vector2 point, ref DamageStruct damage) {
            if (Random.Range(0f, 100f) < activationProbability + ability.ElementalActivationRateIncrease) {
                var fire = CCPooler.SpawnFromPool<ElementalFirePref>(poolTags[0], point, Quaternion.identity);
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
        public ElementalFire(ElementalFire origin): base(origin.poolTags, origin.ability) {
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
    //================================================================== [ EXPLOSION ] ========================================================================
    public class Explosion : ProjectileType {
        ACEffector2D effectShockWave;
        ProjectilePref addExplosionPref;
        private byte skillLevel;
        private float explosionRange;
        private float addExplosionRange;
        private short addExplosionDamage;

        public override int DefaultSpawnSize() => 3;

        public override string GetDescription(string localizedString) {
            throw new System.NotImplementedException();
        }

        protected override string[] GetUniqueTags() {
            return new string[3] { "explosion", "smallEx", "shockwave" };
        }

        public override Dictionary<string, GameObject> GetProjectileDic() {
            poolTags = new string[0];
            var dictionary = new Dictionary<string, GameObject>();
            dictionary.Add(GetUniqueTags()[0], projectilePref.gameObject);
            dictionary.Add(GetUniqueTags()[1], addExplosionPref.gameObject);
            dictionary.Add(GetUniqueTags()[2], effectShockWave.gameObject);
            return dictionary;
        }

        public override void OnHit(Vector2 point, ref DamageStruct damage) {
            var explosionPref = CCPooler.SpawnFromPool<ExplosionPref>(poolTags[0], point, Quaternion.identity);
            if (explosionPref) {
                explosionPref.SetValue(poolTags[1], explosionRange, addExplosionRange, addExplosionDamage, skillLevel);
                explosionPref.Shot(damage, ability.GetProjectileDamage(projectileDamage));
            }
            var effectShockWave = CCPooler.SpawnFromPool<ACEffector2D>(poolTags[2], point, Quaternion.identity);
            if (effectShockWave) {
                effectShockWave.PlayOnce();
            }
        }

        public override void Clear() {
            throw new System.NotImplementedException();
        }

        public Explosion(Dt_Explosion data) {
            bool isExplosionPref = data.ExplosionPref.TryGetComponent<ProjectilePref>(out ProjectilePref explosionPref);
            bool isSmallExPref   = data.SmallExPref.TryGetComponent<ProjectilePref>(out ProjectilePref smallExPref);

            if(!isExplosionPref || !isSmallExPref) {
                throw new System.Exception("Explosion Prefab is Missing Projectile Component.");
            }
            // Prefab
            projectilePref    = explosionPref;
            addExplosionPref  = smallExPref;
            effectShockWave   = data.effects[0];
            // Counts
            explosionRange     = data.ExplosionRange;
            addExplosionRange  = data.AddExplosionRange;
            projectileDamage   = data.ExplosionDamage;
            addExplosionDamage = data.AddExDamage;
            switch (data.SkillLevel) {
                case SKILL_LEVEL.LEVEL_MEDIUM: skillLevel = 1; break;
                case SKILL_LEVEL.LEVEL_HIGH:   skillLevel = 2; break;
                case SKILL_LEVEL.LEVEL_UNIQUE: skillLevel = 3; break;
            }
        }

        public Explosion(Explosion origin) : base(origin.poolTags, origin.ability) {
            projectilePref     = origin.projectilePref;
            addExplosionPref   = origin.addExplosionPref;
            explosionRange     = origin.explosionRange;
            addExplosionRange  = origin.addExplosionRange;
            projectileDamage   = origin.projectileDamage;
            addExplosionDamage = origin.addExplosionDamage;
            skillLevel         = origin.skillLevel;
            effectShockWave    = origin.effectShockWave;
        }
        #region ES3
        public Explosion() { }
        #endregion
    }
    //=========================================================================================================================================================
}
