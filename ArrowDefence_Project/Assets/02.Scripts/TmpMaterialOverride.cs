namespace ActionCat {
    using UnityEngine;
    using TMPro;
    public class TmpMaterialOverride : MonoBehaviour {
        [Header("MATERIAL OVERRIDE")]
        [SerializeField] Material textMaterial   = null;
        [SerializeField] ACTIVE_TIME activeTime  = ACTIVE_TIME.START;
        [SerializeField] [ReadOnly] string ActiveLanguageCode = "Korean";
        [SerializeField] TextMeshProUGUI[] texts = null; [Space(10f)]
        [SerializeField] bool isActive           = false;


        private void Awake() {
            if (ActiveLanguageCode != I2.Loc.LocalizationManager.CurrentLanguage) {
                this.enabled = false;
                return;
            }

            if (isActive && activeTime == ACTIVE_TIME.AWAKE) {
                OverrideMaterial();
                this.enabled = false;
            }
        }

        private void OnEnable() {
            if (isActive && activeTime == ACTIVE_TIME.ENABLE) {
                OverrideMaterial();
                this.enabled = false;
            }
        }

        private void Start() {
            if (isActive && activeTime == ACTIVE_TIME.START) {
                OverrideMaterial();
                this.enabled = false;
            }
        }

        private void Update() {
            if (isActive && activeTime == ACTIVE_TIME.UPDATE) {
                OverrideMaterial();
                this.enabled = false;
            }
        }

        [ContextMenu("Preview")]
        public void PreviewInEditor() {
            OverrideMaterial();
        }

        private void OverrideMaterial() {
            foreach (var text in texts) {
                text.fontMaterial = textMaterial;
            }
        }

        enum ACTIVE_TIME {
            AWAKE,
            START,
            UPDATE,
            ENABLE
        }
    }
}
