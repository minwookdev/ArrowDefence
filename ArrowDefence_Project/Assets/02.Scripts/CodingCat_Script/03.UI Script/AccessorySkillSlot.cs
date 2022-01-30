namespace ActionCat {
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using TMPro;

    public class AccessorySkillSlot : MonoBehaviour, IPointerClickHandler {
        [Header("Default")]
        [SerializeField] Image skillIcon;

        [Header("CoolDown Type")]
        [SerializeField] Image coolDownMask;
        [SerializeField] TextMeshProUGUI coolDownTmp;

        [Header("Kill & Hit Type")]
        [SerializeField] Image[] activationsImages;

        //CoolDown & Charged Type variables
        private float currentCoolDown;
        private float maxCoolDown;

        //Kill & Hit Type variables
        private float currentKillStack;
        private float maxKillStack;
        private float duration;

        private bool isPreparedSkillActive = false;
        private bool isEffectActivation    = false;
        private ACSPACTIVETYPE skillActiveType;
        private EventTrigger eventTrigger;

        //Skill Callback
        Func<MonoBehaviour, float> activeSkillFunc = null;  //Skill Effect
        Action stopSkillAction  = null;                     //Skill Effect Stop
        Coroutine skillEffectCo = null;                     //Skill Effect Coroutine

        #region INIT

        public AccessorySkillSlot InitSlot(UI.ACSData data) {
            switch (data.ActiveType) {
                case ACSPACTIVETYPE.COOLDOWN:  InitCoolDownType(data); return this;
                case ACSPACTIVETYPE.CHARGING:  InitStackType(data);    return this;
                case ACSPACTIVETYPE.KILLCOUNT: InitKillType(data);     return this;
                case ACSPACTIVETYPE.HITCOUNT:  InitHitType(data);      return this;
                default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 모든 Active Type에 대한 공통적인 초기화 사항.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="skillicon"></param>
        void InitDefault(ACSPACTIVETYPE type, Sprite skillicon) {
            eventTrigger = GetComponent<EventTrigger>();
            skillActiveType = type;
            if (skillIcon == null)
                return;
            skillIcon.sprite = skillicon;
        }

        /// <summary>
        /// CoolDown Type Skill Slots
        /// </summary>
        /// <param name="data"></param>
        public void InitCoolDownType(UI.ACSData data) {
            InitDefault(data.ActiveType, data.IconSprite);

            if(coolDownMask == null || coolDownTmp == null) {
                throw new Exception("CoolDown Mask or Text Component is Null.");
            }

            //init cooldown variables
            maxCoolDown     = data.MaxCount;
            currentCoolDown = (data.IsPrepared) ? 0f : maxCoolDown;

            //init cooldown ui element
            coolDownMask.fillAmount = 1f;
            coolDownTmp.text = "";

            InitEventTriggerCallback(data.SkillFunc);
            InitEffectStop(data.SkillStopCallback);
        }

        public void InitStackType(UI.ACSData data) {
            throw new System.NotImplementedException();
        }

        public void InitKillType(UI.ACSData data) {
            throw new System.NotImplementedException();
        }

        public void InitHitType(UI.ACSData data) {
            throw new System.NotImplementedException();
        }

        #endregion

        private void Update() {
            switch (skillActiveType) {
                case ACSPACTIVETYPE.COOLDOWN:  UpdateCoolDownSkill();  break;   //시간이 흐름에 따라 사용 가능 횟수가 충전
                case ACSPACTIVETYPE.CHARGING:  UpdateChargingSkill();  break;   //
                case ACSPACTIVETYPE.KILLCOUNT: UpdateKillStackSkill(); break;
                case ACSPACTIVETYPE.HITCOUNT:  UpdateHitsStackSkill(); break;
            }
        }

        #region UPDATE

        void UpdateCoolDownSkill() {
            //Only Update Battle Scene In-Battle State
            if (GameManager.Instance.GameState != GAMESTATE.STATE_INBATTLE)
                return;

            //Prepared Skill
            if (isPreparedSkillActive == false) {
                currentCoolDown -= Time.deltaTime;
                if (currentCoolDown <= 0f)
                    isPreparedSkillActive = true;
            }

            //Update-UI-Element
            //coolDownTmp.text = (currentCoolDown > 0f) ? currentCoolDown.ToString("F0") : "";
            coolDownTmp.text = (currentCoolDown > 0f && currentCoolDown < maxCoolDown) ? Mathf.CeilToInt(currentCoolDown).ToString() : "";
            coolDownMask.fillAmount = currentCoolDown / maxCoolDown;
        }

        void UpdateChargingSkill()
        {

        }

        void UpdateKillStackSkill()
        {

        }

        void UpdateHitsStackSkill()
        {

        }

        #region STOP_SKILL [END BATTLE]

        /// <summary>
        /// Disable Activating Skill Button
        /// </summary>
        void InitEffectStop(Action action) {
            stopSkillAction = action;

            //if the End Battle, Stop Effect Coroutine and Disable Effect Use
            GameManager.Instance.AddListnerEndBattle(() => {
                if (isEffectActivation == true) {
                    StopCoroutine(skillEffectCo);
                    stopSkillAction();
                    CatLog.Log("Accessory Special Effect 강제종료.");
                }

                //Block Using Effect
                isPreparedSkillActive   = false;
                coolDownTmp.text        = "";
                coolDownMask.fillAmount = 1f;
            });

            GameManager.Instance.AddListnerGameOver(() => {
                if (isEffectActivation == true) {
                    StopCoroutine(skillEffectCo);
                    stopSkillAction();
                    CatLog.Log("Accessory Special Effect 강제 종료.");
                }

                //Block Using Effect
                isPreparedSkillActive   = false;
                coolDownTmp.text        = "";
                coolDownMask.fillAmount = 1f;
            });

        }

        #endregion

        public void DecreaseSkillCoolDown()
        {

        }

        public void IncreaseSkillGauge()
        {

        }

        #endregion 

        void InitEventTriggerCallback(Func<MonoBehaviour, float> callback) {
            //GameManager.Instance.PreventionPulling(eventTrigger); //Controller 로직 변경으로 현재 필요없음.
            activeSkillFunc = callback;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            //효과 발동 가능여부 판단해서 스킬 발동해줌
            if (isPreparedSkillActive && isEffectActivation == false) {
                skillEffectCo = StartCoroutine(ActiveSkillCo());
            }
            else
                CatLog.Log("Skill Not Prepared !");
        }

        IEnumerator ActiveSkillCo() {
            isEffectActivation = true;
            duration = activeSkillFunc(GameManager.Instance.GetControllerInstOrNull());
            currentCoolDown = maxCoolDown;

            //Wait For Skill Duration
            while (duration > 0) {
                yield return null;
                duration -= Time.unscaledDeltaTime;
            }

            CatLog.Log("Active Skill 발동 종료 초기화 진행");

            stopSkillAction();
            isPreparedSkillActive = false;
            isEffectActivation    = false;
        }
    }
}
