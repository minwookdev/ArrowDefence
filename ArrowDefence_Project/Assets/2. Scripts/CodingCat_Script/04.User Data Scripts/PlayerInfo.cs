namespace ActionCat.Data {
    using System.Collections.Generic;
    public class PlayerInfo {
        Dictionary<string, StageInfo> stageInfo = new Dictionary<string, StageInfo>();

        /// <summary>
        /// Update Stage Info Dictionary
        /// </summary>
        public void UpdateStageInfo(string key, in BattleData data) {
            //Try Update Stage Info Data
            if(stageInfo.TryGetValue(key, out StageInfo info)) {
                info.UpdateInfo(in data);
            }
            else { //if the failed, Add New Stage Info Dictionary
                stageInfo.Add(key, new StageInfo(in data));
            }
        }

        public bool TryGetStageData(string key, out StageInfo data) {
            return stageInfo.TryGetValue(key, out data);
        }

        #region CONSTRUCTOR [for ES3]
        public PlayerInfo() { }
        ~PlayerInfo() { }
        #endregion
    }

    public class StageInfo {
        public short MaxComboCount { get; private set; } = 0;
        public short KilledCount { get; private set; }   = 0;
        public byte ClearedCount { get; private set; }   = 0;
        public bool IsUsedResurrect { get; private set; } = false;
        public bool IsStageCleared { get; private set; }  = false;
        public bool IsUseableAuto { get; private set; }   = false;

        public void UpdateInfo(in BattleData data) {
            if(MaxComboCount < data.maxComboCount) {
                MaxComboCount = data.maxComboCount;
            }

            if(KilledCount < data.totalKilledCount) {
                KilledCount = data.totalKilledCount;
            }

            if(IsUsedResurrect == true) {
                IsUsedResurrect = data.isUsedResurrect;
            }
            
            IsStageCleared = data.isCleared;
            ClearedCount++;
        }

        public void EnableAutoUse() {
            IsUseableAuto = true;
        }

        public bool IsChallengeAchieve(System.Predicate<StageInfo> predicate, out bool isResult) {
            return isResult = predicate(this);
        }

        public StageInfo(in BattleData data) {
            MaxComboCount   = data.maxComboCount;
            KilledCount     = data.totalKilledCount;
            IsUsedResurrect = data.isUsedResurrect;
            IsStageCleared  = data.isCleared;
            ClearedCount    = 1;
        }

        public StageInfo() { }
    }


}
