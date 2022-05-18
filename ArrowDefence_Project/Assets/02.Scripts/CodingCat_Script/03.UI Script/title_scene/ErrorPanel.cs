namespace ActionCat {
    using UnityEngine;
    using TMPro;

    public class ErrorPanel : MonoBehaviour {
        [Header("COMPONENT")]
        [SerializeField] TextMeshProUGUI textErrorName = null;
        [SerializeField] TextMeshProUGUI textErrorInfo = null;
        
        public void EnablePanel(string errorInfo, string errorName = "") {
            gameObject.SetActive(true);
            textErrorName.text = (string.IsNullOrEmpty(errorName)) ? "ERROR NAME: {NULL}" : errorName;
            textErrorInfo.text = errorInfo;
        }
    }
}
