namespace ActionCat
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using TMPro;

    public class AccessorySkillSlot : MonoBehaviour, IPointerClickHandler
    {
        /// <summary>
        /// ACSP SLOT INIT DATA
        /// </summary>
        public class ActiveSkillSlotInitData
        {
            public SKILL_ACTIVATIONS_TYPE ActiveType { get; private set; }
            public Sprite SkillIconSprite { get; private set; }
            public float MaxCount { get; private set; } = 0f;
            public bool IsPrepared { get; private set; } = false;
            public Func<MonoBehaviour, float> SkillFunc { get; private set; }
            public Action StopEffectAction { get; private set; }

            public ActiveSkillSlotInitData(Sprite iconsprite, float maxcount, bool isprepared,
                                           SKILL_ACTIVATIONS_TYPE type, Func<MonoBehaviour, float> skillfunc,
                                           Action stopskilleffect)
            {
                SkillIconSprite  = iconsprite;
                MaxCount         = maxcount;
                IsPrepared       = isprepared;
                ActiveType       = type;
                SkillFunc        = skillfunc;
                StopEffectAction = stopskilleffect;
            }
        }

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
        private SKILL_ACTIVATIONS_TYPE skillActiveType;
        private EventTrigger eventTrigger;

        //Skill Callback
        Func<MonoBehaviour, float> activeSkillFunc = null;  //Skill Effect
        Action stopSkillAction  = null;                     //Skill Effect Stop
        Coroutine skillEffectCo = null;                     //Skill Effect Coroutine

        void InitDefault(SKILL_ACTIVATIONS_TYPE type, Sprite skillicon)
        {
            eventTrigger = GetComponent<EventTrigger>();
            skillActiveType = type;
            if (skillIcon == null)
                return;
            skillIcon.sprite = skillicon;
        }

        #region INIT

        public void InitCoolDownSkillButton(ActiveSkillSlotInitData data)
        {
            InitDefault(data.ActiveType, data.SkillIconSprite);

            if (coolDownMask == null || coolDownTmp == null) {
                CatLog.WLog("CoolDown Mask or CoolDown Count Text is null");
                return;
            }

            //Init-Cool Down variables
            maxCoolDown = data.MaxCount;
            currentCoolDown = (data.IsPrepared) ? 0f : maxCoolDown;

            //Init-Cool Down UI Element
            coolDownMask.fillAmount = 1f;
            coolDownTmp.text = "";

            InitEventTriggerCallback(data.SkillFunc);
            InitEffectStop(data.StopEffectAction);
        }

        public void InitStackingSkillButton(ActiveSkillSlotInitData data)
        {

        }

        public void InitKillCountSkillButton_typeA()
        {

        }

        public void InitKillCountSkillButton_typeB()
        {

        }

        #endregion

        private void Update()
        {
            switch (skillActiveType)
            {
                case SKILL_ACTIVATIONS_TYPE.COOLDOWN_ACTIVE: UpdateCoolDownSkill(); break;
                case SKILL_ACTIVATIONS_TYPE.CHARGING_ACTIVE: UpdateChargingSkill(); break;
                case SKILL_ACTIVATIONS_TYPE.KILLCOUNT_ACTIVE: UpdateKillStackSkill(); break;
                case SKILL_ACTIVATIONS_TYPE.HITSCOUNT_ACTIVE: UpdateHitsStackSkill(); break;
            }
        }

        #region UPDATE

        void UpdateCoolDownSkill()
        {
            //Only Update Battle Scene In-Battle State
            if (GameManager.Instance.GameState != GAMESTATE.STATE_INBATTLE)
                return;

            //Prepared Skill
            if (isPreparedSkillActive == false)
            {
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
        void InitEffectStop(Action action)
        {
            stopSkillAction = action;

            //if the End Battle, Stop Effect Coroutine and Disable Effect Use
            GameManager.Instance.AddListnerEndBattle(() => {
                if (isEffectActivation == true) {
                    StopCoroutine(skillEffectCo);
                    stopSkillAction();
                    CatLog.Log("Accessory Special Effect 강제종료.");
                }

                //Slot Disable Effect
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

        void InitEventTriggerCallback(Func<MonoBehaviour, float> callback)
        {
            //Script GameObject is having EventTrigger
            //EventTrigger.Entry skillSlotEntry = new EventTrigger.Entry();
            //skillSlotEntry.eventID = EventTriggerType.PointerClick;
            //skillSlotEntry.callback.AddListener((data) => callback());
            //eventTrigger.triggers.Add(skillSlotEntry);

            activeSkillFunc = callback;
            GameManager.Instance.PreventionPulling(eventTrigger);
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

        IEnumerator ActiveSkillCo()
        {
            isEffectActivation = true;
            duration = activeSkillFunc(GameManager.Instance.Controller());
            currentCoolDown = maxCoolDown;

            //Wait For Skill Duration
            while (duration > 0)
            {
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
