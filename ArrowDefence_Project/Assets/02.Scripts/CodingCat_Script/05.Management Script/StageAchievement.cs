namespace ActionCat.Data {
    using System;
    using System.Collections.Generic;
    using UnityEngine.Rendering;

    public class StageAchievement {
        #region STAGEINFO
        public byte[] GetStarCount(STAGETYPE type, StageInfo info, out bool isAllAchievementCleared) {
            if (info == null) throw new Exception("Stage Info is Null.");

            switch (type) {
                case STAGETYPE.NONE: throw new Exception("StageType is NONE.");
                case STAGETYPE.FOREST_SECLUDED_E: return GetBytesForestEasy(info, out isAllAchievementCleared);
                case STAGETYPE.FOREST_SECLUDED_N: return GetBytesForestNormal(info, out isAllAchievementCleared);
                case STAGETYPE.FOREST_SECLUDED_H: return GetBytesForestHard(info, out isAllAchievementCleared);
                case STAGETYPE.DUNGEON_E: throw new System.NotImplementedException();
                case STAGETYPE.DUNGEON_N: throw new System.NotImplementedException();
                case STAGETYPE.DUNGEON_H: throw new System.NotImplementedException();
                case STAGETYPE.STAGE_DEV: return GetBytesDevStage(info, out isAllAchievementCleared);
                default: throw new NotImplementedException("this StageType is Not Implemented.");
            }
        }

        byte[] GetBytesDevStage(StageInfo info, out bool isAllCleared) {
            var byteList = new List<byte>();
            //=========================================<< ACHIEVEMENT FIRST >>=========================================
            if (info.IsChallengeAchieve(data => data.IsStageCleared == true, out bool achieveFirst)) {
                byteList.Add(0);
            }
            //========================================<< ACHIEVEMENT SECONDS >>========================================
            if (info.IsChallengeAchieve(data => data.IsUsedResurrect == false, out bool achieveSeconds)) {
                byteList.Add(1);
            }
            //=========================================<< ACHIEVEMENT THIRD >>=========================================
            if (info.IsChallengeAchieve(data => data.KilledCount >= 30, out bool achieveThird)) {
                byteList.Add(2);
            }
            //======================================<< ALL ACHIEVEMENT CLEARED >>======================================
            if (achieveFirst && achieveSeconds && achieveThird) {
                isAllCleared = true;
            }
            else {
                isAllCleared = false;
            }
            //===========================================<< RESULT RETURN >>===========================================
            return byteList.ToArray();
            //=========================================================================================================
        }

        //=============================================================================================================
        //========================================== << SECLUDED FOREST >> ============================================
        List<bool> tempResultBoolList = new List<bool>();
        /// <summary>
        /// 도전과제 달성 여부 배열과 모든 도전과제의 달성여부를 out 키워드로 반환
        /// </summary>
        /// <param name="stageInfo"></param>
        /// <param name="isAllAchievementCleared"></param>
        /// <returns></returns>
        bool[] GetStageAchievement(StageInfo stageInfo, out bool isAllAchievementCleared) {
            // 결과 temp list clear
            tempResultBoolList.Clear();

            // 도전과제 달성 조건과 그 조건의 달성 여부를 out으로 반환
            if (stageInfo.IsChallengeAchieve(info => info.IsStageCleared == true, out bool isClearedFirst)) {
                // 조건1. 스테이지가 클리어 되었는지 체크
                tempResultBoolList.Add(isClearedFirst);
            }
            if (stageInfo.IsChallengeAchieve(info => info.KilledCount >= 20, out bool isClearedSeconds)) {
                // 조건2. 한 스테이지에서 20마리 이상 처치되었는지 체크
                tempResultBoolList.Add(isClearedSeconds);
            }
            if (stageInfo.IsChallengeAchieve(info => info.IsUsedResurrect == false, out bool isClearedThirds)) {
                // 조건3. 부활 기능을 사용하지 않았는지 체크
                tempResultBoolList.Add(isClearedThirds);
            }

            // 모든 도전과제를 달성했는지 out 반환
            isAllAchievementCleared = tempResultBoolList.TrueForAll(result => result);
            return tempResultBoolList.ToArray(); // 도전과제 달성 여부 배열 반환
        }

        byte[] GetBytesForestEasy(StageInfo info, out bool isAllAchieveCleared) {
            List<byte> byteList = new List<byte>();
            if (info.IsChallengeAchieve(data => data.IsStageCleared == true, out bool achievefirst)) {
                byteList.Add(0);
            }
            if (info.IsChallengeAchieve(data => data.KilledCount >= 20, out bool achieveseconds)) {
                byteList.Add(1);
            }
            if (info.IsChallengeAchieve(data => data.IsUsedResurrect == false, out bool achievethird)) {
                byteList.Add(2);
            }

            isAllAchieveCleared = (achievefirst && achieveseconds && achievethird);
            return byteList.ToArray();
        }

        byte[] GetBytesForestNormal(StageInfo info, out bool isAllAchieveCleared) {
            var byteList = new List<byte>();
            if (info.IsChallengeAchieve(data => data.KilledCount >= 40, out bool achievefirst)) {
                byteList.Add(0);
            }
            if (info.IsChallengeAchieve(data => data.MaxComboCount >= 10, out bool achieveseconds)) {
                byteList.Add(1);
            }
            if (info.IsChallengeAchieve(data => data.ClearedCount >= 3, out bool achievethird)) {
                byteList.Add(2);
            }

            isAllAchieveCleared = (achievefirst && achieveseconds && achievethird);
            return byteList.ToArray();
        }

        byte[] GetBytesForestHard(StageInfo info, out bool isAllAchieveCleared) {
            var byteList = new List<byte>();
            if (info.IsChallengeAchieve(data => data.KilledCount >= 50, out bool achievefirst)) {
                byteList.Add(0);
            }
            if (info.IsChallengeAchieve(data => data.IsUsedResurrect == false, out bool achieveseconds)) {
                byteList.Add(1);
            }
            if (info.IsChallengeAchieve(data => data.ClearedCount >= 3, out bool achievethird)) {
                byteList.Add(2);
            }

            isAllAchieveCleared = (achievefirst && achieveseconds && achievethird);
            return byteList.ToArray();
        }

        //=============================================================================================================
        //========================================= << DUNGEON ENTRANCE >> ============================================

        byte[] GetByteDungeon(StageInfo info, out bool isAllCleared) {
            isAllCleared = false;
            byte[] tempArray = new byte[3] { 0, 1, 2 };
            return tempArray;
        }

        //=============================================================================================================
        //=============================================================================================================
        #endregion

        #region BATTLEDATA

        public byte[] GetBattleResult(STAGETYPE type, in BattleData data, out string[] achieveStrings) {
            switch (type) {
                case STAGETYPE.NONE: throw new Exception("StageType is NONE.");
                case STAGETYPE.STAGE_DEV: return GetResultDev(in data, out achieveStrings);
                case STAGETYPE.STAGE_FOREST_SECLUDED: return GetResultForest(in data, out achieveStrings);
                case STAGETYPE.STAGE_DUNGEON_ENTRANCE: return GetResultDungeon(in data, out achieveStrings);
                default: throw new NotImplementedException("this StageType is Not Implemented.");
            }
        }

        byte[] GetResultDev(in BattleData data, out string[] achievementStrings) {
            achievementStrings = new string[3] { "Stage Cleared",
                                                 "Not Used Resurrect",
                                                 "Kills 30 or More" };
            var starCountList = new List<byte>();
            if (data.isCleared == true) starCountList.Add(0);
            if (data.isUsedResurrect == false) starCountList.Add(1);
            if (data.totalKilledCount >= 30) starCountList.Add(2);
            return starCountList.ToArray();
        }

        byte[] GetResultForest(in BattleData data, out string[] achievementStrings) {
            achievementStrings = new string[3];
            return new byte[3];
        }

        byte[] GetResultDungeon(in BattleData data, out string[] achievementStrings) {
            achievementStrings = new string[3];
            return new byte[3];
        }

        #endregion
    }
}
