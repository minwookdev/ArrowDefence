namespace ActionCat.UI {
    using UnityEngine;

    public class RequirementSlot : MonoBehaviour {
        public void DisableSlot() {
            gameObject.SetActive(false);
        }

        public void EnableSlot() {
            gameObject.SetActive(true);
        }
    }
}
