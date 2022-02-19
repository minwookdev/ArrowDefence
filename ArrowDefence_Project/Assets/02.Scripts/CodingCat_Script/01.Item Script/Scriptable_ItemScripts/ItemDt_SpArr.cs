namespace ActionCat {
    using UnityEngine;

    public class ItemDt_SpArr : ItemData_Equip_Arrow {
        public CHARGETYPE ChargeType = CHARGETYPE.NONE;
        public int MaxCost;
        [RangeEx(1, 3, 1)] public int MaxStackCount;
        public float CostIncrease;

        private void OnEnable() {

        }

        public ItemDt_SpArr() : base() {
            
        }
    }
}
