namespace ActionCat {
    using UnityEngine;
    using UnityEngine.UI;

    public class ArtifactSlot_Debuff : AccessorySkillSlot, ITouchPosReceiver {
        [Header("DE-BUFF TYPE SLOT")]
        [SerializeField] Image[] activeableBars = null;

        [Header("DE-BUFF")]
        [SerializeField] [ReadOnly] int maxStackCount     = 0;
        [SerializeField] [ReadOnly] int currentStackCount = 0;
        [SerializeField] [ReadOnly] float maxCost     = 0f;
        [SerializeField] [ReadOnly] float currentCost = 0f;
        System.Action<float, ITouchPosReceiver> requestWorldPosition = null;

        public ArtifactSlot_Debuff Init(AccessorySPEffect effect) {
            throw new System.NotImplementedException();
        }

        void ITouchPosReceiver.SendWorldPos(Vector2 position) {
            throw new System.NotImplementedException();
        }
    }
}
