namespace ActionCat {
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using TMPro;

    public class ArtifactSlot_Debuff : AccessorySkillSlot, ITouchPosReceiver {
        [Header("DE-BUFF TYPE SLOT")]
        [SerializeField] Image[] imagesStackBar = null;
        [Space(10f)]
        [SerializeField] Image imageCooldownMask = null;
        [SerializeField] TextMeshProUGUI tmpCoolDownTime = null;
        [Space(10f)]
        [SerializeField] Color disableColor = new Color();
        [SerializeField] Color enableColor  = new Color();

        [Header("DE-BUFF")]
        [SerializeField] [ReadOnly] int maxStackCount = 0;
        [SerializeField] [ReadOnly] int currentStackCount = 0;
        [SerializeField] [ReadOnly] float maxCost = 0f;
        [SerializeField] [ReadOnly] float currentCost = 0f;
        [SerializeField] [ReadOnly] float currentCoolDownTime = 0f;
        [SerializeField] [ReadOnly] float maxCoolDownTime = 0f;
        [SerializeField] [ReadOnly] float findRadius = 0f;
        [SerializeField] [ReadOnly] bool isRunning    = false;
        [SerializeField] [ReadOnly] bool isActiveable = false;
        System.Action<float, ITouchPosReceiver> requestWorldPosition = null;
        System.Collections.Generic.List<MonsterStatus> monsterStatusList = null;

        public ArtifactSlot_Debuff Init(AccessorySPEffect effect, System.Action notify, System.Action<float, ITouchPosReceiver> request) {
            artifactEffect   = effect;
            notifyPlayAction = notify;
            requestWorldPosition = request;

            eventTrigger     = GetComponent<EventTrigger>();
            skillIcon.sprite = artifactEffect.IconSprite;

            maxCoolDownTime = artifactEffect.Condition.CoolDown;
            maxStackCount   = artifactEffect.Condition.MaxStack;
            maxCost         = artifactEffect.Condition.MaxCost;
            findRadius      = artifactEffect.GetRange();

            if (maxStackCount > 4) {
                throw new System.Exception("디버프 슬롯은 현재 4이상의 Stack을 지원하지 않습니다.");
            }

            for (int i = 0; i < imagesStackBar.Length; i++) {
                bool enable = (maxStackCount - 1 >= i);
                imagesStackBar[i].color      = enable ? enableColor : disableColor;
                imagesStackBar[i].fillAmount = enable ? 0f : 1f;
            }

            SlotStop();
            GameManager.Instance.AddListnerInBattle(SlotRunning); // <-- Cost Increase Start Trigger
            GameManager.Instance.AddListnerEndBattle(SlotStop);   // <-- Cost Increase Stop Trigger
            GameManager.Instance.AddListnerGameOver(SlotStop);    // <-- Cost Increase Stop Trigger
            GameManager.Instance.AddListnerPause(SlotStop);       // <-- Cost Increase Stop Trigger

            monsterStatusList = new System.Collections.Generic.List<MonsterStatus>();
            return this;
        }

        void SlotRunning() {
            isRunning    = true;
            isActiveable = true;
            imageCooldownMask.fillAmount = 0f;
        }

        void SlotStop() {
            isRunning    = false;
            isActiveable = false;
            imageCooldownMask.fillAmount = 1f;
            tmpCoolDownTime.text = "";
        }

        void Update() {
            if (!isRunning) {
                return;
            }
            UpdateCost();
            UpdateStackCount();
            UpdateCoolDown();
        }

        void UpdateCost() {
            //Stack이 Max에 도달한 상태에서는 Cost 쌓이지 않음
            if (currentStackCount >= maxStackCount) {
                return;
            }

            currentCost += Time.unscaledDeltaTime;
            if (currentCost > maxCost) {
                currentCost = 0f;
                currentStackCount++;
                notifyPlayAction();
            }
        }

        void UpdateCoolDown() {
            if(maxCoolDownTime <= 0f) { return;
                //CoolDown Time이 등록되지 않은 Debuff Skill의 경우 업데이트 하지 않음
                // ※ CurrentCoolDown / MaxCoolDown의 상황이 [0/0] 이 되버리는 경우 오류값이 들어가버림
            }

            if(currentCoolDownTime > 0f) {
                currentCoolDownTime -= Time.unscaledDeltaTime;
            }

            tmpCoolDownTime.text = (currentCoolDownTime > 0f && currentCoolDownTime < maxCoolDownTime) ? Mathf.CeilToInt(currentCoolDownTime).ToString() : "";
            imageCooldownMask.fillAmount = currentCoolDownTime / maxCoolDownTime;
        }

        void UpdateStackCount() {
            if(currentStackCount >= maxStackCount) {
                //Over Index 방지를 위한 return
                return;
            }

            //stack ui update. 쌓여있는 StackCount에 맞는 인덱스 넘버 Bar 업데이트 해줌
            imagesStackBar[currentStackCount].fillAmount = currentCost / maxCost;
        }

        void ITouchPosReceiver.SendWorldPos(Vector2 position) {
            throw new System.NotImplementedException();
        }

        void ITouchPosReceiver.SendColliders(Collider2D[] colliders) {
            colliders.Foreach((coll) => {
                if (coll.TryGetComponent<MonsterStatus>(out MonsterStatus status)) {
                    monsterStatusList.Add(status);
                }
            });
            artifactEffect.ActiveDebuff(monsterStatusList.ToArray());
            monsterStatusList.Clear();
        }

        public void BE_ACTIVE() {
            bool result = (isActiveable && currentStackCount > 0 && currentCoolDownTime <= 0f);
            if (!result) {
                Notify.Inst.Message("Artifact Not Prepared !");
                return;
            }

            isRunning = false;                                // Re-Draw 방지
            currentStackCount--;                              // Reduce Current Stack Count
            currentCoolDownTime = maxCoolDownTime;            // CoolDown Time Init
            for (int i = 0; i < maxStackCount; i++) {         // Re-Draw Stack Bar's
                imagesStackBar[i].fillAmount = (i < currentStackCount) ? 1f : 0f;
            }

            //Call Touch Position Detector
            requestWorldPosition(findRadius, this);
            isRunning = true;
        }
    }
}
