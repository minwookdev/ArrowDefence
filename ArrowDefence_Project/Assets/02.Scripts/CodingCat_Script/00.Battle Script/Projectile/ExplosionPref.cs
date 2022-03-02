namespace ActionCat {
    using UnityEngine;
    using System.Collections;
    using ActionCat.Interface;

    public class ExplosionPref : ProjectilePref {
        [SerializeField] ACEffector2D effector = null;

        byte skillLevel = 0;
        float explosionHitRadius = 0f;
        float addExplosionRadius = 0f;
        float explosionInterval = 0.7f;
        short addExplosionDamage;
        string addExpTag   = "";
        bool isReady = false;

        //Coroutine
        WaitForSeconds waitForInterval = null;
        WaitUntil waitEffectStop       = null;
        WaitUntil waitExplosionReady   = null;

        private void Awake() {
            waitForInterval = new WaitForSeconds(explosionInterval);
            waitEffectStop  = new WaitUntil(() => effector.IsPlaying() == false);
            waitExplosionReady = new WaitUntil(() => isReady == true);
            CheckComponent();
        }

        private void OnEnable() {
            StartCoroutine(ExplosionCo());
        }

        public override void Shot(DamageStruct damage, short projectileDamage = 0) {
            finCalcDamage = projectileDamage;
            damageStruct  = damage;
            damageStruct.SetDamage(finCalcDamage);

            isReady = true;
        }

        public override void DisableRequest() {
            isReady = false;
            base.DisableRequest();
        }

        protected override void CheckComponent() {
            if (tr == null)       CatLog.ComponentMent();
            if (effector == null) CatLog.ComponentMent();
        }

        IEnumerator ExplosionCo() {
            yield return waitExplosionReady;
            effector.Play();
            CineCam.Inst.ShakeCamera(8f, .3f);

            if (GameGlobal.TryGetOverlapCircleAll2D(out Collider2D[] colliders, tr.position, explosionHitRadius, AD_Data.LAYER_MONSTER)) {
                foreach (var collider in colliders) {
                    if(collider.TryGetComponent<IDamageable>(out IDamageable target)) {
                        target.OnHitElemental(ref damageStruct, collider.ClosestPoint(tr.position), direction: (collider.transform.position - tr.position));
                    }
                }
            }

            yield return StartCoroutine(AddExplosionPhase(skillLevel));

            yield return waitEffectStop;
            DisableRequest();
        }

        public void SetValue(string smallExplosionTag, float expHitRange, float addExpRange, short addexpdamage, byte level) {
            skillLevel = level;
            addExpTag = smallExplosionTag;
            explosionHitRadius = expHitRange;
            addExplosionRadius = addExpRange;
            addExplosionDamage = addexpdamage;
        }

        IEnumerator AddExplosionPhase(byte level) {
            switch (level) {
                case 1:  yield break;
                case 2:  break;
                case 3:  break;
                default: CatLog.ELog("Over Range Explosion SkillLevel."); yield break;
            }

            yield return waitForInterval;

            float radius = (level <= 2) ? explosionHitRadius + addExplosionRadius : explosionHitRadius + (addExplosionRadius * 0.5f);
            //var targetColliderArray = GameGlobal.OverlapCircleAll2D(tr, radius, AD_Data.LAYER_MONSTER, target => Vector2.Distance(tr.position, target.transform.position) < explosionHitRadius);
            var targetColliderArray = GameGlobal.OverlapCircleAll2D(tr, radius, AD_Data.LAYER_MONSTER);
            for (int i = 0; i < targetColliderArray.Length; i++) {
                var addExPref = CCPooler.SpawnFromPool<ExplosionSmallPref>(addExpTag, targetColliderArray[i].transform.position, Quaternion.identity);
                if (addExPref) {
                    addExPref.Shot(damageStruct, addExplosionDamage);
                }
            }

            yield return StartCoroutine(AddExplosionPhaseEx(level));
        }

        IEnumerator AddExplosionPhaseEx(byte level) {
            switch (level) {
                case 3:   break;
                default:  yield break;
            }

            yield return waitForInterval;

            float radius = explosionHitRadius + addExplosionRadius;
            var targetColliderArray = GameGlobal.OverlapCircleAll2D(tr, radius, AD_Data.LAYER_MONSTER);
            for (int i = 0; i < targetColliderArray.Length; i++) {
                var addExPref = CCPooler.SpawnFromPool<ExplosionSmallPref>(addExpTag, targetColliderArray[i].transform.position, Quaternion.identity);
                if (addExPref) {
                    addExPref.Shot(damageStruct, addExplosionDamage);
                }
            }

            yield break;
        }
    }
}
