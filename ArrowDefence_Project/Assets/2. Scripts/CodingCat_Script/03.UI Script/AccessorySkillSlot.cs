namespace ActionCat
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using TMPro;

    public class AccessorySkillSlot : MonoBehaviour, IPointerClickHandler
    {
        /// <summary>
        /// ACSP SLOT INIT DATA
        /// </summary>
        public class SkillSlotInitData
        {
            public Sprite SkillIconSprite { get; private set; } = null;
            public float MaxCount { get; private set; } = 0f;
            public bool IsPrepared { get; private set; } = false;
            public System.Action SkillCallback { get; private set; } = null;

            public void InitSkillData(Sprite iconsprite, float maxcount, bool isprepared, System.Action callback)
            {
                SkillIconSprite = iconsprite;
                MaxCount = maxcount;
                IsPrepared = isprepared;
                SkillCallback = callback;
            }
        }

        [SerializeField] Image skillIcon;
        [SerializeField] Image coolDownMask;
        [SerializeField] Image[] activationsImages;
        [SerializeField] TextMeshProUGUI coolDownTmp;

        private SKILL_ACTIVATIONS_TYPE skillActiveType;
        private float currentCoolDown;
        private float maxCoolDown;
        private float currentKillStack;
        private float maxKillStack;
        private bool isPreparedSkillActive = false;
        private EventTrigger eventTrigger;

        //Skill Callback
        System.Action skillCallback;

        void Init(SKILL_ACTIVATIONS_TYPE type, Sprite skillicon)
        {
            eventTrigger = GetComponent<EventTrigger>();
            skillActiveType = type;
            if (skillIcon == null)
                return;
            skillIcon.sprite = skillicon;
        }

        #region INIT

        public void InitCoolDownSkillButton(Sprite skillicon, float maxcooldowncount, bool isprepared,
                                            System.Action skillcallback)
        {
            Init(SKILL_ACTIVATIONS_TYPE.COOLDOWN_ACTIVE, skillicon);

            if (coolDownMask == null || coolDownTmp == null)
            {
                CatLog.WLog("CoolDown Mask or CoolDown Count Text is null");
                return;
            }

            //Init-Cool Down variables
            maxCoolDown = maxcooldowncount;
            currentCoolDown = (isprepared) ? 0f : maxCoolDown;

            //Init-Cool Down UI Element
            coolDownMask.fillAmount = 0f;
            coolDownTmp.text = "";

            InitEventTriggerCallback(skillcallback);
        }

        public void InitCoolDownSkillButton(SkillSlotInitData data)
        {
            Init(SKILL_ACTIVATIONS_TYPE.COOLDOWN_ACTIVE, data.SkillIconSprite);

            if (coolDownMask == null || coolDownTmp == null)
            {
                CatLog.WLog("CoolDown Mask or CoolDown Count Text is null");
                return;
            }

            //Init-Cool Down variables
            maxCoolDown = data.MaxCount;
            currentCoolDown = (data.IsPrepared) ? 0f : maxCoolDown;

            //Init-Cool Down UI Element
            coolDownMask.fillAmount = 0f;
            coolDownTmp.text = "";

            InitEventTriggerCallback(data.SkillCallback);
        }

        public void InitStackingSkillButton()
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
            coolDownTmp.text = (currentCoolDown > 0f) ? currentCoolDown.ToString("{0:F0}") : "";
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

        public void DecreaseSkillCoolDown()
        {

        }

        public void IncreaseSkillGauge()
        {

        }

        #endregion 

        void InitEventTriggerCallback(System.Action callback)
        {
            //Script GameObject is having EventTrigger
            //EventTrigger.Entry skillSlotEntry = new EventTrigger.Entry();
            //skillSlotEntry.eventID = EventTriggerType.PointerClick;
            //skillSlotEntry.callback.AddListener((data) => callback());
            //eventTrigger.triggers.Add(skillSlotEntry);

            skillCallback = callback;
            GameManager.Instance.PreventionPulling(eventTrigger);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            //스킬 발동 가능여부 판단해서 스킬 발동해줌
            if (isPreparedSkillActive)
            {
                skillCallback();
                isPreparedSkillActive = false;
            }
            else
                CatLog.Log("Skill Not Prepared !");
        }
    }
}
