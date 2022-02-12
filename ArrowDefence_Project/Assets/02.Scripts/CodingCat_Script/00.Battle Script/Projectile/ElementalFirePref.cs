namespace ActionCat {
    using UnityEngine;
    using System.Collections;

    public class ElementalFirePref : ProjectilePref {
        [SerializeField] CircleCollider2D coll = null;
        [SerializeField] ACEffector2D effector = null;

        WaitUntil EndWait = null;

        public override void Shot(DamageStruct damage, short projectileDamage) {
            StartCoroutine(Play());
            finCalcDamage = projectileDamage;
            damage.SetDamage(finCalcDamage);
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
            if(collision.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_MONSTER)) {
                //Collision
            }
        }

        IEnumerator CollisionCo() {
            coll.enabled = true;
            yield return null;
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
