namespace ActionCat.UI.StageBoard {
    using System.Collections.Generic;
    using Data.StageData;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class StageBoard : MonoBehaviour {
        [Header("STAGE")]
        [SerializeField] STAGETYPE stageType = STAGETYPE.NONE;

        [Header("MONSTER")]
        [SerializeField] ItemData_Mat[] monsterEntities = null;
        [SerializeField] MonsterSlot[] monsterSlots;

        [Header("DROP LIST")]
        [SerializeField] RectTransform dropListParent = null;
        [SerializeField] List<UI_ItemDataSlot> dropListSlots = null;
        [SerializeField] ItemData[] dropEntities = null;
        [SerializeField] int[] rewardedSlotIndexs;

        [Header("CHALLENGES")]
        [SerializeField] ChallengeInfos challengeInfos = null;

        [Header("SETTINGS")]
        [SerializeField] SettingsInfo settingsInfo = null;
        [SerializeField] [ReadOnly]
        bool isAchieveAll = false;

        [Header("DEBUG")]
        [SerializeField] bool isDebug = false;

        void Start() {
            //Set MonsterSlots, DropListSlots
            SetMonsterSlots();
            DropListSlotCaching();

            // Update Challenge Panel
            ChallengePanelSet(ref isAchieveAll);

            // Update Settings Panel
            SettingsPanelSet(isAchieveAll);
        }

        void OnEnable() {
            
        }

        void UpdateChallengeInfo(ref bool isAchieveAll) {
            switch (stageType) {
                case STAGETYPE.STAGE_DEV:              ChallengePanelSet(ref isAchieveAll); break;
                case STAGETYPE.STAGE_FOREST_SECLUDED:  SetChallengeStageFst(); break;
                case STAGETYPE.STAGE_DUNGEON_ENTRANCE: SetChallengeStageSec(); break;
                default: throw new System.NotImplementedException("this Stage Type is NotImplemented.");
            }
        }

        void SettingsPanelSet(bool isOpenPanel) {
            //1. Settings Panel이 열렸는지 체크
            //2-1. 열렸으면 PlayerData.GameSettings에서 키가있는지 확인
            //2-2. 키가 없으면 바로 생성해주고 데이터 가져와서 세팅해줌.
            //3-1. 3별아닌 상태면 데이터를 가져오지 않고, 바로 isOn 바로 false처리 해줘서 닫아놓음.

            if (isDebug == true) { //Enable Settings Panel [Only Debugging]
                var settings = GameManager.Instance.GetStageSetting(GameGlobal.GetStageKey(stageType));
                settingsInfo.InitSettingPanel(true, settings);
                return;
            }

            if (isOpenPanel == true) {
                var settings = GameManager.Instance.GetStageSetting(GameGlobal.GetStageKey(stageType));
                settingsInfo.InitSettingPanel(isOpenPanel, settings);
            }
            else {
                settingsInfo.InitSettingPanel(isOpenPanel);
            }
        }

        void DropListSlotCaching() {
            dropListSlots = new List<UI_ItemDataSlot>();
            for (int i = 0; i < dropListParent.childCount; i++) {
                if (dropListParent.GetChild(i).TryGetComponent<UI_ItemDataSlot>(out UI_ItemDataSlot slot)) {
                    dropListSlots.Add(slot);
                }
                else {
                    CatLog.ELog($"ERROR: ChildCount Number {i} is Not Have ItemDataSlot Component.");
                }
            }

            //부족한 슬롯 수 계산
            sbyte needSlotCount = (sbyte)(dropEntities.Length - dropListSlots.Count);
            if (needSlotCount > 0) {
                for (sbyte i = 0; i < needSlotCount; i++) {
                    dropListSlots.Add(GameObject.Instantiate(dropListSlots[0], dropListParent));
                }
            }

            byte enableSlotCount = 0;
            for (int i = 0; i < dropEntities.Length; i++) {
                dropListSlots[i].EnableDropListSlot(dropEntities[i]);
                enableSlotCount++;
            }

            //잉여슬롯 비활성화
            if (enableSlotCount < dropListSlots.Count) {
                for (int i = enableSlotCount; i < dropListSlots.Count; i++) {
                    dropListSlots[i].DisableSlot();
                }
            }
        }

        void SetMonsterSlots() {
            byte enableSlotCount = 0;
            for (int i = 0; i < monsterEntities.Length; i++) {
                monsterSlots[i].EnableSlot(monsterEntities[i]);
                enableSlotCount++;
            }

            //잉여슬롯 비활성화
            if (enableSlotCount < monsterSlots.Length) {
                for (int i = enableSlotCount; i < monsterSlots.Length; i++) {
                    monsterSlots[i].DisableSlot();
                }
            }
        }

        #region TOGGLE

        public void ToggleUpdateAutoMode() {
            settingsInfo.ToggleUpdateAutoMode();
        }

        public void ToggleUpdateMutant() {
            settingsInfo.ToggleUpdateMutant();
        }

        #endregion

        #region CHALLNEGES

        void SetChallengeStageFst() { 
            //set challenge info
        }

        void SetChallengeStageSec() {
            //set challenge info

        }

        void ChallengePanelSet(ref bool isAchieveAll) {
            challengeInfos.DisableAllStar();
            //Get Players Stage Progress Data
            if (GameManager.Instance.TryGetStageData(GameGlobal.GetStageKey(stageType), out Data.StageInfo data)) {
                var byteArray = new Data.StageAchievement().GetStarCount(stageType, data, out isAchieveAll);
                for (int i = 0; i < byteArray.Length; i++) {
                    challengeInfos.EnableStar(byteArray[i]);
                }

                if (isAchieveAll == true && data.IsUseableAuto == false) {
                    data.EnableAutoUse();
                }
            }

            if (data == null || data.IsStageCleared == false) {
                for (int i = 0; i < rewardedSlotIndexs.Length; i++) {
                    dropListSlots[rewardedSlotIndexs[i]].EnableRewardTag();
                }
            }
        }

        #endregion

        public void BE_BATTLE_SELECT() {
            MainSceneRoute.OpenBattlePopup(stageType);
        }

        #region CHALLENGE_INFO

        [System.Serializable]
        internal sealed class ChallengeInfos {
            //===================================================[ CHALLENGE INFO ]===================================================
            [System.Serializable]
            class ChallengeInfo {
                //[SerializeField] TextMeshProUGUI textChallenge = null;
                //[SerializeField] Image imageDisable = null;
                [SerializeField] Image imageEnable  = null;

                public void TurnOffImage() {
                    imageEnable.gameObject.SetActive(false);
                }

                public void TurnOnImage() {
                    imageEnable.gameObject.SetActive(true);
                }
            }
            //========================================================================================================================

            //===============================================[ CHALLENGE INFO CONTROL ]===============================================
            [SerializeField] ChallengeInfo[] challengeInfos = null;

            public void DisableAllStar() {
                for (int i = 0; i < challengeInfos.Length; i++) {
                    challengeInfos[i].TurnOffImage();
                }
            }

            public void EnableStar(byte idx) {
                if (idx < 0 || idx > 2) { //enable index only 0, 1, 2
                    CatLog.ELog("Wrong index."); return;
                }
                challengeInfos[idx].TurnOnImage();
            }
            //========================================================================================================================
        }

        //==================================================[ SETTINGS PANEL CONTROL ]================================================
        [System.Serializable]
        internal sealed class SettingsInfo {
            [SerializeField] GameObject panelLock  = null;
            [SerializeField] Toggle toggleAutoShot = null;
            [SerializeField] Toggle toggleSpawnMutantMonster = null;
            StageSetting stageSetting = null;

            public void InitSettingPanel(bool isAllAchieve, StageSetting setting = null) {
                if(isAllAchieve == true) {
                    panelLock.gameObject.SetActive(false);

                    stageSetting = setting;

                    toggleAutoShot.isOn           = stageSetting.isOnAutoMode;
                    toggleSpawnMutantMonster.isOn = stageSetting.isOnEliteSpawn;
                }
                else {
                    panelLock.gameObject.SetActive(true);

                    //Release Toggles
                    toggleSpawnMutantMonster.isOn = false;
                    toggleAutoShot.isOn           = false;
                }
            }

            public void ToggleUpdateAutoMode() {
                //if (toggleAutoShot.isOn == true) {
                //    if(stageSetting == null) {
                //        CatLog.WLog("Invalid Input."); return;
                //    }
                //
                //    stageSetting.SetAutoMode(true);
                //    CatLog.Log("Toggle AutoShot isOn True.");
                //}
                //else {
                //    if(stageSetting == null) {
                //        CatLog.WLog("Invalid Input."); return;
                //    }
                //
                //    stageSetting.AutoModeSet
                //    CatLog.Log("Toggle AutoShot isOn False.");
                //}
                //CatLog.Log($"{toggleAutoShot.isOn}");

                if(stageSetting == null) {
                    throw new System.Exception("Stage Settings Data is Null.");
                }

                stageSetting.SetAutoMode(toggleAutoShot.isOn);

                //if (toggleAutoShot.isOn == true) CatLog.Log("Toggle AutoShot isOn True");
                //else                             CatLog.Log("Toggle AutoShot isOn False");
            }

            public void ToggleUpdateMutant() {
                if(stageSetting == null) {
                    throw new System.Exception("Stage Settings Data is Null.");
                }

                stageSetting.SetMutant(toggleAutoShot.isOn);
            }
        }
        //============================================================================================================================

        #endregion
    }
}

