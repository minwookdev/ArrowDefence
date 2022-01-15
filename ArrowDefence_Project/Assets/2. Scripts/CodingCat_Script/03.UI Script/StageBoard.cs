namespace ActionCat.UI.StageBoard {
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class StageBoard : MonoBehaviour {
        [Header("STAGE")]
        [SerializeField] STAGETYPE stageType = STAGETYPE.NONE;

        [Header("CHALLENGES")]
        [SerializeField] ChallengeInfos challengeInfos = null;

        [Header("SETTINGS")]
        [SerializeField] SettingsInfo settingsInfo = null;
        [SerializeField] bool isAchieveAll = false;

        void Start() {
            // Update Challenge Panel
            UpdateChallengeInfo(ref isAchieveAll);

            // Update Settings Panel
            UpdateSettingsInfo(isAchieveAll);
        }

        void UpdateChallengeInfo(ref bool isAchieveAll) {
            switch (stageType) {
                case STAGETYPE.STAGE_DEV:              SetChallengeStageDev(ref isAchieveAll); break;
                case STAGETYPE.STAGE_FOREST_SECLUDED:  SetChallengeStageFst(); break;
                case STAGETYPE.STAGE_DUNGEON_ENTRANCE: SetChallengeStageSec(); break;
                default: throw new System.NotImplementedException("this Stage Type is NotImplemented.");
            }
        }

        void UpdateSettingsInfo(bool isOpen) {
            settingsInfo.InitSettingPanel(isOpen);
        }

        #region CHALLNEGES

        void SetChallengeStageFst() { 
            //set challenge info
        }

        void SetChallengeStageSec() {
            //set challenge info

        }

        void SetChallengeStageDev(ref bool isAchieveAll) {
            challengeInfos.DisableAllStar();
            //Get Players Stage Progress Data
            if(GameManager.Instance.TryGetStageData(GameGlobal.GetStageKey(stageType), out Data.StageInfo data)) {
                ////Data Get Success <1. Stage Cleared> <2. Not Used Resurrect> <3. Killed Monster Over 30+>
                if(data.IsChallengeAchieve(info => info.IsStageCleared == true, out bool isAchieveFirst)) {
                    challengeInfos.EnableStar(0);
                }

                if(data.IsChallengeAchieve(info => info.IsUsedResurrect == false, out bool isAchieveSeconds)) {
                    challengeInfos.EnableStar(1);
                }

                if(data.IsChallengeAchieve(info => info.KilledCount >= 30, out bool isAchieveThird)) {
                    challengeInfos.EnableStar(2);
                }

                //All Challenge is Achieve?
                if(isAchieveFirst && isAchieveSeconds && isAchieveThird) {
                    isAchieveAll = true;
                }
            }
        }

        #endregion

        #region CHALLENGE_INFO

        [System.Serializable]
        internal class ChallengeInfos {
            //===================================================[ CHALLENGE INFO ]===================================================
            [System.Serializable]
            class ChallengeInfo {
                [SerializeField] TextMeshProUGUI textChallenge = null;
                [SerializeField] Image imageEnable  = null;
                [SerializeField] Image imageDisable = null;

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
        internal class SettingsInfo {
            [SerializeField] GameObject panelLock  = null;
            [SerializeField] Toggle toggleAutoShot = null;
            [SerializeField] Toggle toggleSpawnMutantMonster = null;

            public void InitSettingPanel(bool isAllAchieve) {
                if(isAllAchieve == true) {
                    panelLock.gameObject.SetActive(false);
                }
                else {
                    panelLock.gameObject.SetActive(true);

                    //Release Toggles
                    toggleSpawnMutantMonster.isOn = false;
                    toggleAutoShot.isOn           = false;
                }
            }
        }
        //============================================================================================================================

        #endregion
    }
}

