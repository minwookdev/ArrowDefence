namespace ActionCat
{
    using System;
    using UnityEngine;
    using ActionCat.Data;

    [Serializable]
    public class Player_Equipments {
        private Item_Bow EquippedBow;

        private Item_Arrow EquippedArrow_f;
        private Item_Arrow EquippedArrow_s;
        private Item_SpArr EquippedSpArr;

        private Item_Accessory EquippedAccessory_f;
        private Item_Accessory EquippedAccessory_s;
        private Item_Accessory EquippedAccessory_t;

        #region PROPERTY
        public bool IsEquippedSpArr {
            get {
                if (EquippedSpArr == null) return false;
                else                       return true;
            }
        }
        public Item_SpArr GetSpArrOrNull {
            get {
                if (EquippedSpArr == null) return null;
                else                       return EquippedSpArr;
            }
        }
        #endregion

        #region EQUIP_&_REALESE

        //마법활, 마법장신구등 장비종류 좀 늘어날텐데 이거 메서드 좀 적게 관리 가능할거같은데, 장비 종류 늘어날거 대비해서 늘리자
        //사실상 Equipment라는 하나의 인벤토리가 늘어난 셈으로 알면된다. -> 장신구는 배열로 깔아도 괜찮을것같다 아니면 장신구 관련 함수들은 배열로 하든지

        /// <summary>
        /// 인벤토리 내부의 Bow Item Data가져와서 새로 할당하고 Inventory내부에 있던 아이템 지워줌
        /// </summary>
        /// <param name="item"></param>
        public void Equip_BowItem(Item_Bow item)
        {
            if(IsEquippedBow())
            {
                Release_BowItem();
                this.EquippedBow = new Item_Bow(item);
                CCPlayerData.inventory.RemoveItem(item);
            }
            else
            {
                this.EquippedBow = new Item_Bow(item);
                CCPlayerData.inventory.RemoveItem(item);
            }

            //CCPlayerData.status.UpdateMainWeaponAbility(this);
            CatLog.Log($"{this.EquippedBow.GetName} 아이템이 장착되었습니다 : 활");
        }

        public void Release_BowItem()
        {
            CCPlayerData.inventory.ToInven(this.EquippedBow);
            this.EquippedBow = null;

            CatLog.Log("아이템이 해제되었습니다.");
        }

        public void Equip_ArrowItem(Item_Arrow item)
        {
            if (IsEquippedArrowMain()) Release_ArrowItem();

            this.EquippedArrow_f = new Item_Arrow(item);
            CCPlayerData.inventory.RemoveItem(item);
            CatLog.Log($"{this.EquippedArrow_f.GetName} 아이템이 장착되었습니다 : 화살");
        }

        public void Release_ArrowItem() {
            CCPlayerData.inventory.ToInven(this.EquippedArrow_f);
            this.EquippedArrow_f = null;

            CatLog.Log("아이템이 해제되었습니다.");
        }

        public void Equip_SubArrow(Item_Arrow item)
        {
            if (IsEquippedArrowSub()) Release_SubArrow();

            this.EquippedArrow_s = new Item_Arrow(item);
            CCPlayerData.inventory.RemoveItem(item);
            CatLog.Log($"{this.EquippedArrow_s.GetName} 아이템이 장착되었습니다.");
        }

        public void Release_SubArrow()
        {
            CCPlayerData.inventory.ToInven(this.EquippedArrow_s);
            this.EquippedArrow_s = null;

            CatLog.Log("아이템이 해제되었습니다.");
        }

        public void Equip_Accessory(Item_Accessory item, byte idx)
        {
            switch (idx)
            {
                case 0: if (IsEquippedAccessory(idx)) Release_Accessory(idx);
                            this.EquippedAccessory_f = new Item_Accessory(item); break;
                case 1: if (IsEquippedAccessory(idx)) Release_Accessory(idx);
                            this.EquippedAccessory_s = new Item_Accessory(item); break;
                case 2: if (IsEquippedAccessory(idx)) Release_Accessory(idx);
                            this.EquippedAccessory_t = new Item_Accessory(item); break;
                default: CatLog.ELog($"PlayerEquipment : Wrong idx Accessory index number {idx}"); return;
            }

            CatLog.Log($"{item.GetName} Item Equipped, Type : Accessory");
            CCPlayerData.inventory.RemoveItem(item);
        }

        public void Release_Accessory(byte idx)
        {
            switch (idx)
            {
                case 0: CCPlayerData.inventory.ToInven(this.EquippedAccessory_f);
                        this.EquippedAccessory_f = null; break;
                case 1: CCPlayerData.inventory.ToInven(this.EquippedAccessory_s);
                        this.EquippedAccessory_s = null; break;
                case 2: CCPlayerData.inventory.ToInven(this.EquippedAccessory_t);
                        this.EquippedAccessory_t = null; break;
                default: CatLog.ELog($"PlayerEquipment : Wrong idx in Accessory Realese Method {idx}"); break;
            }

            CatLog.Log("Item Release Successfully");
        }

        public void Equip_SpArrow(Item_SpArr item) {
            if(IsEquippedSpArr == true) {
                Release_SpArrow();
            }
            EquippedSpArr = item; // 실험적인 기능. 문제가 없다면 다른 아이템에도 동일 로직 적용
            CCPlayerData.inventory.RemoveItem(item);
            CatLog.Log(StringColor.GREEN, $"Equipment: Item Equipped {EquippedSpArr.GetName}");
        }

        public void Release_SpArrow() {
            CCPlayerData.inventory.ToInven(EquippedSpArr);
            EquippedSpArr = null;
            CatLog.Log(StringColor.GREEN, "Equipment: Release Equipment Special Arrow.");
        }

        #endregion

        public Item_Bow GetBowItem()
        {
            return EquippedBow;
        }

        public Item_Arrow GetMainArrow()
        {
            return EquippedArrow_f;
        }

        public Item_Arrow GetSubArrow()
        {
            return EquippedArrow_s;
        }

        public Item_Accessory[] GetAccessories()
        {
            Item_Accessory[] accessories = { this.EquippedAccessory_f,
                                             this.EquippedAccessory_s,
                                             this.EquippedAccessory_t };
            return accessories;
        }

        public Item_SpArr GetSpArrow() {
            if(this.EquippedSpArr == null) {
                throw new System.Exception("Equipped Special Arrow is Null.");
            }
            return this.EquippedSpArr;
        }

        public bool IsEquippedBow()
        {
            if (this.EquippedBow != null) return true;
            else                          return false;
        }

        public bool IsEquippedArrowMain()
        {
            if (this.EquippedArrow_f != null) return true;
            else                              return false;
        }

        public bool IsEquippedArrowSub()
        {
            if (this.EquippedArrow_s != null) return true;
            else                              return false;
        }

        public bool IsEquippedAccessory(byte idx)
        {
            bool isEquipped = false;

            switch (idx)
            {
                case 0: isEquipped = (EquippedAccessory_f != null) ? true : false; break;
                case 1: isEquipped = (EquippedAccessory_s != null) ? true : false; break;
                case 2: isEquipped = (EquippedAccessory_t != null) ? true : false; break;
                default: CatLog.ELog("Wrong index Number in IsEquippedAccessory(byte idx) Method return false"); break;
            }

            return isEquipped;
        }

        public bool[] IsEquippedAccessory()
        {
            bool[] boolArray = new bool[3];

            for (int i = 0; i < boolArray.Length; i++)
            {
                if      (i == 0) boolArray[i] = (EquippedAccessory_f != null) ? true : false;
                else if (i == 1) boolArray[i] = (EquippedAccessory_s != null) ? true : false;
                else if (i == 2) boolArray[i] = (EquippedAccessory_t != null) ? true : false;
            }

            return boolArray;
        }

        #region INIT_EQUIPMENTS_IN_BATTLE

        /// <summary>
        /// Battle Scene에서 Init처리되야할 요소들 처리
        /// </summary>
        /// <param name="bowObjectInitPos"></param>
        /// <param name="bowObjectParentTr"></param>
        /// <param name="mainArrowPoolQuantity"></param>
        /// <param name="subArrowPoolQuantity"></param>
        public void InitEquipments(Transform bowObjectInitPos, Transform bowObjectParentTr, int mainArrowPoolQuantity, int subArrowPoolQuantity, UI.SwapSlots slot) {
            AD_BowAbility ability = null;
            if(IsEquippedBow() == true) {
                ability = EquippedBow.Initialize(bowObjectInitPos, bowObjectParentTr);
            }
            if(IsEquippedArrowMain() == true) {
                EquippedArrow_f.Init(AD_Data.POOLTAG_MAINARROW, AD_Data.POOLTAG_MAINARROW_LESS, mainArrowPoolQuantity, ability.GetAbility(0));
            }
            if(IsEquippedArrowSub() == true) {
                EquippedArrow_s.Init(AD_Data.POOLTAG_SUBARROW, AD_Data.POOLTAG_SUBARROW_LESS, subArrowPoolQuantity, ability.GetAbility(1));
            }
            //Equipment Ready, (Wait For Spawn Arrow Prafabs)
            if (ability) {
                ability.IsInitializedEquipments(this, true);
            }
            
            foreach (var accessory in GetAccessories()) {
                if (accessory != null) accessory.Init();
            }

            if (IsEquippedSpArr) {
                EquippedSpArr.Init(ability.GetAbility(2), 3);
                ability.SetCondition(EquippedSpArr.Condition, slot);
            }
        }

        /// <summary>
        /// Battle Scene에서 Release처리 되어야할 요소들 처리
        /// </summary>
        public void ReleaseEquipments() {
            if (IsEquippedArrowMain()) EquippedArrow_f.Release();
            if (IsEquippedArrowSub())  EquippedArrow_s.Release();
        }

        #endregion

        /// <summary>
        /// Clear Equipped Item Class
        /// </summary>
        public void Clear() {
            this.EquippedBow         = null;
            this.EquippedArrow_f     = null;
            this.EquippedArrow_s     = null;
            this.EquippedAccessory_f = null;
            this.EquippedAccessory_s = null;
            this.EquippedAccessory_t = null;
        }

        public void UpdatePlayerAbility() {
            CCPlayerData.ability.UpdateAbility(this);
        }
    }
}
