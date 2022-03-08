namespace ActionCat {
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using TMPro;

    public class BluePrintSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler {
        [Header("COMPONENT")]
        [SerializeField] Image imageIcon  = null;
        [SerializeField] Image imageFrame = null;
        [SerializeField] TextMeshProUGUI textAmount = null;

        public void SetSlot(AD_item item) {

        }
    }
}
