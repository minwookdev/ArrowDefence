namespace ActionCat {
    using UnityEngine;
    using System.Collections.Generic;

    public class TouchWorldPosDetector : MonoBehaviour {
        [Header("COMPONENT")]
        [SerializeField] RectTransform rectTr  = null;
        [SerializeField] CircleCollider2D coll = null;
        [SerializeField] List<Collider2D> collisionList = new List<Collider2D>();

        public RectTransform RectTr {
            get => rectTr;
        }

        private void OnEnable() {
            coll.enabled = true;
        }

        private void OnDisable() {
            coll.enabled = false;
            collisionList.Clear();
        }
        private void OnTriggerExit2D(Collider2D collision) {
            collisionList.Remove(collision);
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            collisionList.Add(collision);
        }

        public Collider2D[] GetColliders() {
            return collisionList.ToArray();
        }

        public Collider2D[] GetCollidersWithLength(out int length) {
            var array = collisionList.ToArray();
            length = array.Length;
            return array;
        }

        public void SetRadius(float radius) {
            coll.radius = radius;
        }
    }
}
