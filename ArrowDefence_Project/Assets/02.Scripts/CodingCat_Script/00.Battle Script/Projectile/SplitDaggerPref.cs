namespace ActionCat {
    using UnityEngine;

    public class SplitDaggerPref : ProjectilePref {
        [SerializeField] private Rigidbody2D rigidBody  = null;
        [SerializeField] private CapsuleCollider2D coll = null;

        [Header("FORCE")]
        [SerializeField] [RangeEx(8f, 20f, 1f)] 
        float force = 10f;

        private void Start() {
            CheckComponent();
            screenOffset = GameGlobal.ScreenOffset;
        }

        public override void Shot() {
            rigidBody.AddForce(tr.up * force, ForceMode2D.Impulse);
        }

        private void FixedUpdate() {
            CheckBounds();
        }

        protected override void CheckComponent() {
            if (tr = null)         throw new System.Exception("Component Not Cached.");
            if (rigidBody == null) throw new System.Exception("Component Not Cached.");
            if (coll == null)      throw new System.Exception("Component Not Cached.");
        }
    }
}
