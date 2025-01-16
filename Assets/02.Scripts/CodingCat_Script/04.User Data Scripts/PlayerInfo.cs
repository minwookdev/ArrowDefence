namespace ActionCat.Data {
    using System.Collections.Generic;
    public class PlayerInfo {
        // < Crafting || Stage >
        Dictionary<string, StageInfo> stageInfo = new Dictionary<string, StageInfo>();
        List<CraftingInfo> craftingInfoList = new List<CraftingInfo>();

        // < Currency >
        int gold = 0;
        int stone = 0;

        const int MaxGoldAmount = 99999999;
        const int MaxStoneAmount = 999999;
        const int MinCurrency = 0;

        public CraftingInfo[] CraftingInfos {
            get {
                return craftingInfoList.ToArray();
            }
        }

        public int CraftSlotSize {
            get {
                if (craftingInfoList == null) {
                    return 0;
                }
                return craftingInfoList.Count;
            }
        }

        public int Gold {
            get {
                return gold;
            }
        }

        public int Stone {
            get {
                return stone;
            }
        }

        /// <summary>
        /// Stage Info Dictionary Update. [if not exist the value, create new element]
        /// </summary>
        /// <param name="stagekey"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool UpdateStageInfo(string stagekey, in BattleData data) {
            var isExistenceValue = stageInfo.TryGetValue(stagekey, out StageInfo info);
            if (isExistenceValue) {
                info.UpdateInfo(in data);
            }
            else {
                stageInfo.Add(stagekey, new StageInfo(in data));
            }
            return isExistenceValue;
        }

        public bool TryGetStageData(string key, out StageInfo data) {
            return stageInfo.TryGetValue(key, out data);
        }

        public void AddCraftSlot(byte count) {
            for (int i = 0; i < count; i++) {
                craftingInfoList.Add(new CraftingInfo());
            }
        }

        public void OpenSlot(byte index) {
            if (craftingInfoList[index].IsAvailable == true) {
                return;
            }

            craftingInfoList[index].Available();
        }

        public void OpenSlot(params byte[] index) {
            for (int i = 0; i < index.Length; i++) {
                if (craftingInfoList[index[i]].IsAvailable == true) {
                    return;
                }
                craftingInfoList[index[i]].Available();
            }
        }

        public void CraftingStart(int index, CraftingRecipe recipe) {
            if (craftingInfoList[index] == null) {
                throw new System.Exception("CRAFTING START FAILED: 슬롯 인덱스 NULL.");
            }

            if (craftingInfoList[index].InProgress) {
                throw new System.Exception("CRAFTING START FAILED: 이미 제작이 진행중인 슬롯.");
            }

            craftingInfoList[index].Start(recipe.CraftingTime, recipe.Result.Count, recipe.Result.Item);
        }

        public void UpdateCraftingInfo() {
            craftingInfoList.ForEach(slot => slot.Update());
        }

        #region CURRENCY

        public void IncGold(int amount) {
            gold = (gold + amount <= MaxGoldAmount) ? gold + amount : MaxGoldAmount;
        }

        public void IncStone(int amount) {
            stone = (stone + amount <= MaxStoneAmount) ? stone + amount : MaxStoneAmount;
        }

        public bool TryDecGold(int amount) {
            throw new System.NotImplementedException();
        }

        public bool TryDecStone(int amount) {
            throw new System.NotImplementedException();
        }

        #endregion

        #region CONSTRUCTOR
        public PlayerInfo() { }
        ~PlayerInfo() { }
        #endregion
    }

    public class StageInfo {
        // 최대 콤보 카운트
        public short MaxComboCount { get; private set; } = 0;
        // 총 처치 수
        public short KilledCount { get; private set; } = 0;
        // 총 클리어 횟수
        public byte ClearedCount { get; private set; } = 0;
        // 부활 사용 여부
        public bool IsUsedResurrect { get; private set; } = false;
        // 스테이지 클리어 여부
        public bool IsStageCleared { get; private set; } = false;
        // 자동 사격 사용가능 여부
        public bool IsUseableAuto { get; private set; } = false;   

        public void UpdateInfo(in BattleData data) {
            // 최대 콤보 수 갱신
            if (MaxComboCount < data.maxComboCount) {
                MaxComboCount = data.maxComboCount;
            }
            // 최대 처치 수 갱신
            if (KilledCount < data.totalKilledCount) {
                KilledCount = data.totalKilledCount;
            }
            // 마지막 플레이에서 부활기능 사용 여부 체크
            if (IsUsedResurrect == true) {
                IsUsedResurrect = data.isUsedResurrect;
            }
            // 마지막 플레이에서 스테이지가 클리어되었는지 체크
            IsStageCleared = data.isCleared;
            ClearedCount++; // 클리어 횟수 증가
        }



        public void EnableAutoUse() {
            IsUseableAuto = true;
        }

        public bool IsChallengeAchieve(System.Predicate<StageInfo> predicate, out bool isResult) {
            return isResult = predicate(this);
        }

        public StageInfo(in BattleData data) {
            MaxComboCount = data.maxComboCount;
            KilledCount = data.totalKilledCount;
            IsUsedResurrect = data.isUsedResurrect;
            IsStageCleared = data.isCleared;
            ClearedCount = 1;
        }

        public StageInfo() { }
    }

    public class CraftingInfo {
        public bool IsAvailable { get; private set; } = false;
        public bool IsSkipable { get; private set; } = false;
        public int Current { get; private set; } = 0;
        public int Max { get; private set; } = 0;
        public ItemData Result { get; private set; } = null;
        private int amount = 0;

        #region PROPERTY
        public bool IsComplete {
            get {
                if (Current >= Max) {
                    return true;
                }

                return false;
            }
        }

        public bool InProgress {
            get {
                if (Result == null) {
                    return false;
                }

                return true;
            }
        }

        public float Progress {
            get {
                return (float)Current / Max;
            }
        }
        #endregion


        public void Start(int craftingTime, int craftingAmount, ItemData resultItem) {
            if (resultItem == null) {
                throw new System.Exception("Result Item Is Null.");
            }

            Current = 0;
            Max = craftingTime;
            Result = resultItem;
            amount = craftingAmount;
            IsSkipable = true;
        }

        public void Update() {
            if (InProgress && Current < Max) {
                Current++;
            }
        }

        public void Clear() {
            Max = 0;
            Current = 0;
            amount = 0;
            Result = null;
            IsSkipable = false;
        }

        public void Available() {
            IsAvailable = true;
        }

        public void QuickComplete() {
            Current = Max;
        }

        public bool TryReceipt(out ItemData resultItemRef, out int resultAmount) {
            if (!IsComplete) {
                CatLog.ELog("Crafting is Not Complete !");
                resultItemRef = null;
                resultAmount = 0;
                return false;
            }

            CCPlayerData.inventory.AddItem(Result, amount);
            resultItemRef = Result;
            resultAmount = amount;

            Clear();
            return true;
        }
    }

}
