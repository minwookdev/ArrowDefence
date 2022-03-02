namespace ActionCat {
    using UnityEngine;
    using System.Collections;
    using ActionCat.Interface;

    public class ExplosionSmallPref : ProjectilePref {
        [SerializeField] ACEffector2D effector = null;
        [SerializeField] CircleCollider2D circleColl;

        //Coroutine
        WaitUntil waitEffectStop = null;
        WaitUntil waitProjectileReady = null;

        //Fields
        bool isReady = false;

        private void Awake() {
            waitEffectStop      = new WaitUntil(() => effector.IsPlaying() == false);
            waitProjectileReady = new WaitUntil(() => isReady == true);
            CheckComponent();
        }

        private void OnEnable() {
            StartCoroutine(ExplosionCo());
        }

        public override void DisableRequest() {
            isReady = false;
            base.DisableRequest();
        }

        public override void Shot(DamageStruct damage, short projectileDamage = 0) {
            finCalcDamage = projectileDamage;
            damageStruct  = damage;
            damageStruct.SetDamage(finCalcDamage);

            isReady = true;
        }

        protected override void CheckComponent() {
            if (tr == null)       CatLog.ComponentMent();
            if (effector == null) CatLog.ComponentMent();
        }

        IEnumerator ExplosionCo() {
            yield return waitProjectileReady;
            effector.Play(true);

            if(GameGlobal.TryGetOverlapCircleAll2D(out Collider2D[] colliders, tr.position, circleColl.radius, AD_Data.LAYER_MONSTER)) {
                foreach (var collider in colliders) {
                    if(collider.TryGetComponent<IDamageable>(out IDamageable target)) {
                        target.OnHitElemental(ref damageStruct, collider.ClosestPoint(tr.position), direction: (collider.transform.position - tr.position));
                    }
                }
            }

            yield return waitEffectStop;
            DisableRequest();
        }
    }
}
