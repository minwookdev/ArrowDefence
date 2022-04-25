namespace ActionCat {
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    public class ArtifactSlot_Debuff : AccessorySkillSlot, ITouchPosReceiver {
        [Header("DE-BUFF TYPE SLOT")]
        [SerializeField] Image[] activeableBars = null;

        [Header("DE-BUFF")]
        [SerializeField] [ReadOnly] int maxStackCount     = 0;
        [SerializeField] [ReadOnly] int currentStackCount = 0;
        [SerializeField] [ReadOnly] float maxCost     = 0f;
        [SerializeField] [ReadOnly] float currentCost = 0f;
        [SerializeField] [ReadOnly] float coolDownTime = 0f;
        System.Action<float, ITouchPosReceiver> requestWorldPosition = null;

        Color disableBarColor = new Color(40 / 255, 40 / 255, 40 / 255);
        Color enableBarColor  = new Color(255 / 255, 200 / 255, 85 / 255);

        public ArtifactSlot_Debuff Init(AccessorySPEffect effect, System.Action notifyAction) {
            artifactEffect   = effect;
            notifyPlayAction = notifyAction;

            eventTrigger     = GetComponent<EventTrigger>();
            skillIcon.sprite = effect.IconSprite;

            maxStackCount = effect.Condition.MaxStack;
            maxCost       = effect.Condition.MaxCost;
            coolDownTime  = effect.Condition.CoolDown;

            for (int i = 0; i < activeableBars.Length; i++) {
                activeableBars[i].color = (maxStackCount > i) ? disableBarColor : enableBarColor;
            }

            return this;
        }

        void ITouchPosReceiver.SendWorldPos(Vector2 position) {
            throw new System.NotImplementedException();
        }
    }
}
