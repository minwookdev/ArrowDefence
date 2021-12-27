namespace ActionCat.Experimantal {
    using UnityEngine;

    public class RotateTemp : MonoBehaviour {
        Transform tr = null;

        private void Start() {
            tr = GetComponent<Transform>();
        }

        private void Update() {
            if(Input.GetKeyDown(KeyCode.A)) {
                CatLog.Log($"EulerAngles.z = {tr.eulerAngles.z}");
                CatLog.Log($"Rotation.z    = {tr.rotation.z}");
            }
        }
    }
}
