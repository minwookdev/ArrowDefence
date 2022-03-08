namespace ActionCat.UI {
    using UnityEngine;
    using UnityEngine.UI;

    public class CraftingSlot : MonoBehaviour {
        [Header("COMPONENT")]
        [SerializeField] RectTransform progressPanelRectTr = null;
        [SerializeField] RectTransform choosePanelRectTr   = null;
        [SerializeField] Button chooseButton = null;

        public void AddListnerToButton(UnityEngine.Events.UnityAction unityAction) {
            if(unityAction == null) {
                throw new System.Exception("Unity Action is Null.");
            }

            chooseButton.onClick.AddListener(unityAction);
        }
    }
}
