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
        [SerializeField] ScrollRect dropListScrollRect = null;
        [SerializeField] RectTransform dropListParent = null;
        [SerializeField] ItemDropList dropTable       = null;
        [SerializeField] UI_ItemDataSlot slotPref     = null;
        List<UI_ItemDataSlot> dropListSlots = null;

        [Header("CHALLENGES")]
        [SerializeField] ChallengeInfos challengeInfos = null;

        [Header("SETTINGS")]
        [SerializeField] SettingsInfo settingsInfo = null;
        [SerializeField] [ReadOnly] bool isAchieveAll = false;

        [Header("DEBUG")]
        [SerializeField] RectTransform lockPanelRectTr = null;
        [SerializeField] bool isDebug = false;
        [SerializeField] bool isLockOnBuild = false;

        void Start() {
            // Update Challenge Panel
            UpdateChallengeList(ref isAchieveAll, out bool isClearedStage);
            UpdateMonsterList();
            UpdateDropTableList(isClearedStage);
            UpdateSettingsPanel(isAchieveAll);
        }

        void OnEnable() {
#if UNITY_ANDROID
            if (isLockOnBuild) {
                lockPanelRectTr.gameObject.SetActive(true);
            }
#endif
        }

        void UpdateSettingsPanel(bool isOpenPanel) {
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

        void UpdateDropTableList(bool isClearedStage) {
            dropListSlots = new List<UI_ItemDataSlot>();
            for (int i = 0; i < dropListParent.childCount; i++) {
                if (dropListParent.GetChild(i).TryGetComponent<UI_ItemDataSlot>(out UI_ItemDataSlot slot)) {
                    slot.SetScrollRect(dropListScrollRect);
                    dropListSlots.Add(slot);
                }
                else {
                    CatLog.ELog($"ERROR: ChildCount Number {i} is Not Have ItemDataSlot Component.");
                }
            }

            //부족한 슬롯 수 계산 및 새로 할당
            sbyte slotNeedCount = (sbyte)(dropTable.GetTotalTableSize(isClearedStage) - dropListSlots.Count);
            if (slotNeedCount > 0) {
                for (sbyte i = 0; i < slotNeedCount; i++) {
                    var newslot = GameObject.Instantiate(slotPref, dropListParent);
                    newslot.SetScrollRect(dropListScrollRect);
                    dropListSlots.Add(newslot);
                }
            }

            byte enableSlotCount = 0;
            var DropsTable  = dropTable.GetDropTable;
            var rewardTable = dropTable.GetRewardTable;
            //초회 클리어 보상 슬롯 활성화, 클리어 기록이 없는 스테이지 한정
            if (!isClearedStage) { 
                foreach (var reward in rewardTable) {
                    dropListSlots[enableSlotCount].EnableDropListSlot(reward.ItemAsset, true);
                    enableSlotCount++;
                }
            }
            //드랍 테이블의 아이템으로 활성화
            foreach (var drops in DropsTable) {
                dropListSlots[enableSlotCount].EnableDropListSlot(drops.ItemAsset);
                enableSlotCount++;
            }
            //잉여슬롯 비활성화
            if (enableSlotCount < dropListSlots.Count) {
                for (int i = enableSlotCount; i < dropListSlots.Count; i++) {
                    dropListSlots[i].DisableSlot();
                }
            }
        }

        void UpdateMonsterList() {
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

        void UpdateChallengeList(ref bool isAchieveAll, out bool isClearedStage) {
            challengeInfos.DisableAllStar();
            isClearedStage = GameManager.Instance.TryGetStageData(GameGlobal.GetStageKey(stageType), out Data.StageInfo info);
            if (isClearedStage) { //StageData 못받아오면 Cleared 기록이 없다는 것임
                var byteArray = new Data.StageAchievement().GetStarCount(stageType, info, out isAchieveAll);
                for (int i = 0; i < byteArray.Length; i++) {
                    challengeInfos.EnableStar(byteArray[i]);
                }

                //Unlock Auto Trigger
                if (isAchieveAll == true && info.IsUseableAuto == false) {
                    info.EnableAutoUse();
                }
            }
        }

        #endregion

        public void BE_BATTLE_SELECT() {
            MainSceneRoute.Inst.OpenBattlePopup(stageType);
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
                if(stageSetting == null) {
                    throw new System.Exception("Stage Settings Data is Null.");
                }
                stageSetting.SetAutoMode(toggleAutoShot.isOn);
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

