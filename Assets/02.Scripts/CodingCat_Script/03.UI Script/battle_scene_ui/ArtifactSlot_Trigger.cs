namespace ActionCat {
    using UnityEngine;
    using UnityEngine.UI;

    public class ArtifactSlot_Trigger : AccessorySkillSlot {
        [Header("TRIGGER TYPE SLOT")]
        public Image[] activeableBars = null; // <- private멤버로 깔면 워닝띄워서 당장은 일케둠 !

        [Header("SOUND")]
        [SerializeField] Audio.ACSound audioSource = null;

        public ArtifactSlot_Trigger Init(AccessorySPEffect effect) {
            throw new System.NotImplementedException();
        }
    }
}
