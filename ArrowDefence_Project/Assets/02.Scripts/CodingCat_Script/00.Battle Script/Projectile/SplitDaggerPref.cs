namespace ActionCat {
    using UnityEngine;
    using ActionCat.Interface;

    public class SplitDaggerPref : ProjectilePref {
        [SerializeField] private Rigidbody2D rigidBody  = null;
        [SerializeField] private CapsuleCollider2D coll = null;

        [Header("FORCE")]
        [SerializeField] [RangeEx(8f, 20f, 1f)] 
        float force = 10f;

        private void Start() {
            CheckComponent();
            coll.enabled = false;
        }

        private void OnDisable() {
            coll.enabled = false;
        }

        public override void Shot(DamageStruct damage, short projectileDamage = 0) {
            rigidBody.AddForce(tr.up * force, ForceMode2D.Impulse);

            // Update Damage Count
            damageStruct = damage;
            damageStruct.SetDamage(finCalcDamage);

            coll.enabled = true;
        }

        private void Update() {
            CheckBounds();
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            //if(collision.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_MONSTER)) {
            //    if(collision.TryGetComponent<IDamageable>(out IDamageable target)) {
            //        Vector2 contact = collision.ClosestPoint(tr.position);
            //        target.OnHit(ref damageStruct, contact, tr.eulerAngles);
            //    }
            //}
        }

        protected override void CheckComponent() {
            if (tr == null)        throw new System.Exception("Component Not Cached.");
            if (rigidBody == null) throw new System.Exception("Component Not Cached.");
            if (coll == null)      throw new System.Exception("Component Not Cached.");
        }

        public override void SetProjectileValue(PlayerAbilitySlot ability) {
            finCalcDamage = System.Convert.ToInt16(baseDamage * ability.DamageIncRate);
        }
    }
}
