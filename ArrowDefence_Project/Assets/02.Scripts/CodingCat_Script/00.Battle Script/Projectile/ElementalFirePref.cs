namespace ActionCat {
    using UnityEngine;
    using System.Collections;
    using ActionCat.Interface;

    public class ElementalFirePref : ProjectilePref {
        [SerializeField] CircleCollider2D coll = null;
        [SerializeField] ACEffector2D effector = null;
        WaitUntil EndWait = null;
        Quaternion randomRotation;
        float collisionTime = 0.15f;

        public override void Shot(DamageStruct damage, short projectileDamage) {
            finCalcDamage = projectileDamage;
            damageStruct  = damage;
            damageStruct.SetDamage(projectileDamage);
            StartCoroutine(Play());
        }

        private void Start() {
            CheckComponent();
            EndWait = new WaitUntil(() => effector.IsPlaying() == false);
        }

        protected override void CheckComponent() {
            if (tr == null)       throw new System.Exception("Component Not Cached.");
            if (coll == null)     throw new System.Exception("Component Not Cached.");
            if (effector == null) throw new System.Exception("Component Not Cached.");
        }

        public override void SetProjectileValue(PlayerAbilitySlot slot) {
            //finCalcDamage = System.Convert.ToInt16(baseDamage * slot.DamageIncRate);
        }

        public void SetProjectileValue(short finalCalculatedDamage) {
            finCalcDamage = finalCalculatedDamage;
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_MONSTER)) {
                if (collision.TryGetComponent<IDamageable>(out IDamageable target)) {
                    randomRotation = Quaternion.Euler(new Vector3(0f, 0f, Random.Range(0f, 360f)));
                    target.OnHitElemental(ref damageStruct, collision.ClosestPoint(tr.position), randomRotation.eulerAngles);
                }
            }
        }

        IEnumerator CollisionCo() {
            coll.enabled = true;
            float collisionTimer = 0f;
            while (collisionTimer < collisionTime) {
                collisionTimer += Time.deltaTime;
                yield return null;
            }
            coll.enabled = false;
        }

        IEnumerator Play() {
            effector.Play(true);
            yield return StartCoroutine(CollisionCo());
            yield return EndWait;
            DisableRequest(); //Coroutine End
        }
    }
}
