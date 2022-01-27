namespace ActionCat.Data {
    using System;
    using System.Collections.Generic;

    public class StageAchievement {
        #region STAGEINFO
        public byte[] GetStarCount(STAGETYPE type, StageInfo info, out bool isAllAchievementCleared) {
            if (info == null) throw new Exception("Stage Info is Null.");

            switch (type) {
                case STAGETYPE.NONE:                   throw new Exception("StageType is NONE.");
                case STAGETYPE.STAGE_DEV:              return GetByteDev(info, out isAllAchievementCleared);
                case STAGETYPE.STAGE_FOREST_SECLUDED:  return GetByteForest(info, out isAllAchievementCleared);
                case STAGETYPE.STAGE_DUNGEON_ENTRANCE: return GetByteDungeon(info, out isAllAchievementCleared);
                default: throw new NotImplementedException("this StageType is Not Implemented.");
            }
        }

        byte[] GetByteDev(StageInfo info, out bool isAllCleared) {
            var byteList = new List<byte>();
            //=========================================<< ACHIEVEMENT FIRST >>=========================================
            if (info.IsChallengeAchieve(data => data.IsStageCleared == true, out bool achieveFirst)) {
                byteList.Add(0);
            }
            //========================================<< ACHIEVEMENT SECONDS >>========================================
            if(info.IsChallengeAchieve(data => data.IsUsedResurrect == false, out bool achieveSeconds)) {
                byteList.Add(1);
            }
            //=========================================<< ACHIEVEMENT THIRD >>=========================================
            if(info.IsChallengeAchieve(data => data.KilledCount >= 30, out bool achieveThird)) {
                byteList.Add(2);
            }
            //======================================<< ALL ACHIEVEMENT CLEARED >>======================================
            if(achieveFirst && achieveSeconds && achieveThird) {
                isAllCleared = true;
            }
            else {
                isAllCleared = false;
            }
            //===========================================<< RESULT RETURN >>===========================================
            return byteList.ToArray();
            //=========================================================================================================
        }

        byte[] GetByteForest(StageInfo info, out bool isAllCleared) {
            isAllCleared = false;
            byte[] tempArray = new byte[3] { 0, 1, 2 };
            return tempArray;
        }

        byte[] GetByteDungeon(StageInfo info, out bool isAllCleared) {
            isAllCleared = false;
            byte[] tempArray = new byte[3] { 0, 1, 2 };
            return tempArray;
        }

        #endregion

        #region BATTLEDATA

        public byte[] GetBattleResult(STAGETYPE type, in BattleData data, out string[] achieveStrings) {
            switch (type) {
                case STAGETYPE.NONE:                   throw new Exception("StageType is NONE.");
                case STAGETYPE.STAGE_DEV:              return GetResultDev(in data, out achieveStrings);
                case STAGETYPE.STAGE_FOREST_SECLUDED:  return GetResultForest(in data, out achieveStrings);
                case STAGETYPE.STAGE_DUNGEON_ENTRANCE: return GetResultDungeon(in data, out achieveStrings);
                default: throw new NotImplementedException("this StageType is Not Implemented.");
            }
        }

        byte[] GetResultDev(in BattleData data, out string[] achievementStrings) {
            achievementStrings = new string[3] { "Stage Cleared",
                                                 "Not Used Resurrect", 
                                                 "Kills 30 or More" };
            var starCountList = new List<byte>();
            if (data.isCleared == true)        starCountList.Add(0);
            if (data.isUsedResurrect == false) starCountList.Add(1);
            if (data.totalKilledCount >= 30)   starCountList.Add(2);
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
