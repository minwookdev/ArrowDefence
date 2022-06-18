namespace ActionCat {
    using UnityEngine;
    using ActionCat.Interface;

    public class SplitDaggerPref : ProjectilePref {
        [SerializeField] private Rigidbody2D rigidBody  = null;
        [SerializeField] private CapsuleCollider2D coll = null;

        [Header("FORCE")]
        [SerializeField] [RangeEx(8f, 50f, 1f)] 
        float force = 10f;

        [Header("SOUND")]
        [SerializeField] [ReadOnly] Audio.ACSound audioSource = null;
        [SerializeField] AudioClip[] daggerHitClips = null;

        private Vector3 tempEulerAngels;
        private float damageFloaterRange = 2f;

        private void Start() {
            CheckComponent();
            SetScreenBound();
            coll.enabled = false;

            //Get Audio Channel
            audioSource = SoundManager.Instance.TryGetChannel(CHANNELTYPE.PROJECTILE, out Audio.ACSound result) ? result : audioSource; 
        }

        private void OnDisable() {
            coll.enabled = false;
        }

        public override void Shot(DamageStruct damage, short projectileDamage) {
            rigidBody.AddForce(tr.up * force, ForceMode2D.Impulse);

            // Update Damage Count
            finCalcDamage = projectileDamage;
            damageStruct  = damage;
            damageStruct.SetDamage(projectileDamage);

            coll.enabled = true;
        }

        private void Update() {
            CheckBounds();
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if(collision.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_MONSTER)) {
                if(collision.TryGetComponent<IDamageable>(out IDamageable target)) {
                    Vector2 contact   = collision.ClosestPoint(tr.position);
                    tempEulerAngels   = tr.localEulerAngles;
                    tempEulerAngels.z += (Random.Range(-damageFloaterRange, damageFloaterRange + GameGlobal.RandomRangeCorrection));
                    target.OnHit(ref damageStruct, contact, tempEulerAngels);
                    audioSource.PlayOneShot(daggerHitClips.RandIndex());
                }
            }
        }

        protected override void CheckComponent() {
            if (tr == null)        throw new System.Exception("Component Not Cached.");
            if (rigidBody == null) throw new System.Exception("Component Not Cached.");
            if (coll == null)      throw new System.Exception("Component Not Cached.");
        }
    }
}
