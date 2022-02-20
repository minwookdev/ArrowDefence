namespace ActionCat {
    using UnityEngine;

    public class ItemDt_SpArr : ItemData_Equip_Arrow {
        public CHARGETYPE ChargeType = CHARGETYPE.NONE;
        public int MaxCost;
        [RangeEx(1, 3, 1)] public int MaxStackCount;
        public float CostIncrease;

        private void OnEnable() {
            CheckCondition();
        }

        public ItemDt_SpArr() : base() {
            
        }

        void CheckCondition() {
            if (ChargeType == CHARGETYPE.NONE) CatLog.WLog($"Special Arrow ({Item_Name}), Not Setting Charge Type.");
            if (MaxStackCount <= 0)            CatLog.WLog($"Special Arrow ({Item_Name}), Not Setting MaxStackCount.");
        }
    }
}
