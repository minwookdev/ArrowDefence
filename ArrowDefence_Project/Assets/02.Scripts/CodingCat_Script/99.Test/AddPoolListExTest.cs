namespace ActionCat {
    using UnityEngine;

    public class AddPoolListExTest : MonoBehaviour {
        float testFloat = 0f;
        public float TestFloat {
            get {
                return testFloat;
            }
            set {
                testFloat = value;
            }
        }

        private void Start() {
            CatLog.Log($"Test Float: {testFloat}");
        }
    }
}
