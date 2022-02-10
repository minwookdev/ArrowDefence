namespace ActionCat {
    using UnityEngine;
    using System.Collections;

    public class ElementalFirePref : ProjectilePref {
        [SerializeField] CircleCollider2D coll = null;
        [SerializeField] ACEffector2D effector = null;

        WaitUntil EndWait = null;

        public override void Shot(DamageStruct damage) {
            effector.Play(true);
            StartCoroutine(CollisionCo());
            StartCoroutine(WaitEndEffect());
        }

        private void Update() {
            
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
            throw new System.NotImplementedException();
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

        IEnumerator WaitEndEffect() {
            yield return EndWait;
            DisableRequest();
        }
    }
}
