namespace ActionCat {
    using UnityEngine;

    public class CreateFilePanel : MonoBehaviour {
        public bool? IsRequestCreateJson {
            get; private set;
        } = null;

        public void EnablePanel() {
            gameObject.SetActive(true);
        }

        public void BE_CREATE() {
            IsRequestCreateJson = true;
            gameObject.SetActive(false);
        }

        public void BE_CANCEL() {
            IsRequestCreateJson = false;
            gameObject.SetActive(false);
        }

        public void ClearRequestValue() {
            IsRequestCreateJson = null;
        }
    }
}
