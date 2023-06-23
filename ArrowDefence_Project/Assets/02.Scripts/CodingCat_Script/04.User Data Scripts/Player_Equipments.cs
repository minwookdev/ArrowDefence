namespace ActionCat {
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
                if (EquippedSpArr == null) {
                    return false;
                }
                return true;
            }
        }
        public bool IsEquippedBow {
            get {
                if (this.EquippedBow != null) {
                    return true;
                }
                return false;
            }
        }
        public bool IsEquippedArrMain {
            get {
                if (this.EquippedArrow_f != null) {
                    return true;
                }
                return false;
            }
        }
        public bool IsEquippedArrSub {
            get {
                if (this.EquippedArrow_s != null) {
                    return true;
                }
                return false;
            }
        }
        public Item_SpArr GetSpArrOrNull {
            get {
                if (EquippedSpArr == null) return null;
                else return EquippedSpArr;
            }
        }
        #endregion

        #region EQUIP_&_REALESE
        //########################################################## << 구버전 장칙 및 해제 함수 >> ##########################################################
        //public void Equip_BowItem(Item_Bow item)
        //{
        //    if (IsEquippedBow)
        //    {
        //        Release_BowItem();
        //        this.EquippedBow = new Item_Bow(item);
        //        CCPlayerData.inventory.RemoveItem(item);
        //    }
        //    else
        //    {
        //        this.EquippedBow = new Item_Bow(item);
        //        CCPlayerData.inventory.RemoveItem(item);
        //    }
        //
        //    //CCPlayerData.status.UpdateMainWeaponAbility(this);
        //    CatLog.Log($"{this.EquippedBow.GetTermsName} 아이템이 장착되었습니다 : 활");
        //}
        //
        //public void Release_BowItem()
        //{
        //    CCPlayerData.inventory.ToInven(this.EquippedBow);
        //    this.EquippedBow = null;
        //
        //    CatLog.Log("아이템이 해제되었습니다.");
        //}
        //
        //public void Equip_ArrowItem(Item_Arrow item)
        //{
        //    if (IsEquippedArrMain) Release_ArrowItem();
        //
        //    this.EquippedArrow_f = new Item_Arrow(item);
        //    CCPlayerData.inventory.RemoveItem(item);
        //    CatLog.Log($"{this.EquippedArrow_f.GetTermsName} 아이템이 장착되었습니다 : 화살");
        //}
        //
        //public void Release_ArrowItem() {
        //    CCPlayerData.inventory.ToInven(this.EquippedArrow_f);
        //    this.EquippedArrow_f = null;
        //
        //    CatLog.Log("아이템이 해제되었습니다.");
        //}
        //
        //public void Equip_SubArrow(Item_Arrow item)
        //{
        //    if (IsEquippedArrSub) Release_SubArrow();
        //
        //    this.EquippedArrow_s = new Item_Arrow(item);
        //    CCPlayerData.inventory.RemoveItem(item);
        //    CatLog.Log($"{this.EquippedArrow_s.GetTermsName} 아이템이 장착되었습니다.");
        //}
        //
        //public void Release_SubArrow()
        //{
        //    CCPlayerData.inventory.ToInven(this.EquippedArrow_s);
        //    this.EquippedArrow_s = null;
        //
        //    CatLog.Log("아이템이 해제되었습니다.");
        //}
        //
        //public void Equip_Accessory(Item_Accessory item, byte idx)
        //{
        //    switch (idx)
        //    {
        //        case 0: if (IsEquippedArtifact(idx)) Release_Accessory(idx);
        //                    this.EquippedAccessory_f = new Item_Accessory(item); break;
        //        case 1: if (IsEquippedArtifact(idx)) Release_Accessory(idx);
        //                    this.EquippedAccessory_s = new Item_Accessory(item); break;
        //        case 2: if (IsEquippedArtifact(idx)) Release_Accessory(idx);
        //                    this.EquippedAccessory_t = new Item_Accessory(item); break;
        //        default: CatLog.ELog($"PlayerEquipment : Wrong idx Accessory index number {idx}"); return;
        //    }
        //
        //    CatLog.Log($"{item.GetTermsName} Item Equipped, Type : Accessory");
        //    CCPlayerData.inventory.RemoveItem(item);
        //}
        //
        //public void Release_Accessory(byte idx)
        //{
        //    switch (idx)
        //    {
        //        case 0: CCPlayerData.inventory.ToInven(this.EquippedAccessory_f);
        //                this.EquippedAccessory_f = null; break;
        //        case 1: CCPlayerData.inventory.ToInven(this.EquippedAccessory_s);
        //                this.EquippedAccessory_s = null; break;
        //        case 2: CCPlayerData.inventory.ToInven(this.EquippedAccessory_t);
        //                this.EquippedAccessory_t = null; break;
        //        default: CatLog.ELog($"PlayerEquipment : Wrong idx in Accessory Realese Method {idx}"); break;
        //    }
        //
        //    CatLog.Log("Item Release Successfully");
        //}
        //
        //=================================================================================================================================================

        public void Equip_SpArrow(Item_SpArr item) {
            if (IsEquippedSpArr == true) {
                Release_SpArrow();
            }
            EquippedSpArr = item; // 실험적인 기능. 문제가 없다면 다른 아이템에도 동일 로직 적용
            CCPlayerData.inventory.RemoveItem(item);
            CatLog.Log(StringColor.GREEN, $"Equipment: Item Equipped {EquippedSpArr.GetTermsName}");
        }
        public void Release_SpArrow() {
            CCPlayerData.inventory.ToInven(EquippedSpArr);
            EquippedSpArr = null;
            CatLog.Log(StringColor.GREEN, "Equipment: Release Equipment Special Arrow.");
        }
        //=================================================================================================================================================
        public void EquipItem_Bow(Item_Bow equipment) {
            if (IsEquippedBow) {
                ReleaseItem_Bow();
            }

            this.EquippedBow = equipment;
            CCPlayerData.inventory.RemoveItem(equipment);
            CatLog.Log($"Item Equipped, Name: {equipment.GetNameByTerms}");
        }
        public void ReleaseItem_Bow() {
            CCPlayerData.inventory.EquipToInventory(this.EquippedBow);
            this.EquippedBow = null;
        }
        //=================================================================================================================================================
        public void EquipItem_MainArr(Item_Arrow equipment) {
            if (IsEquippedArrMain) {
                ReleaseItem_MainArr();
            }
            this.EquippedArrow_f = equipment;
            CCPlayerData.inventory.RemoveItem(equipment);
            CatLog.Log($"Item Equipped, Name: {equipment.GetNameByTerms}");
        }
        public void ReleaseItem_MainArr() {
            CCPlayerData.inventory.EquipToInventory(this.EquippedArrow_f);
            this.EquippedArrow_f = null;
        }
        //=================================================================================================================================================
        public void EquipItem_SubArr(Item_Arrow equipment) {
            if (IsEquippedArrSub) {
                ReleaseItem_SubArr();
            }
            this.EquippedArrow_s = equipment;
            CCPlayerData.inventory.RemoveItem(equipment);
            CatLog.Log($"Item Equipped, Name: {equipment.GetNameByTerms}");
        }
        public void ReleaseItem_SubArr() {
            CCPlayerData.inventory.EquipToInventory(this.EquippedArrow_s);
            this.EquippedArrow_s = null;
        }
        //=================================================================================================================================================
        public void EquipItem_Artifact(Item_Accessory equipment, byte index) {
            if (IsEquippedArtifact(index)) { //잘못된 Index Number들어오면 여기서 짤림
                ReleaseItem_Artifact(index);
            }
            switch (index) {
                case 0: this.EquippedAccessory_f = equipment; break;
                case 1: this.EquippedAccessory_s = equipment; break;
                case 2: this.EquippedAccessory_t = equipment; break;
                default: throw new System.NotImplementedException();
            }

            CCPlayerData.inventory.RemoveItem(equipment);
        }
        public void ReleaseItem_Artifact(byte index) {
            switch (index) {
                case 0: CCPlayerData.inventory.EquipToInventory(this.EquippedAccessory_f); this.EquippedAccessory_f = null; break;
                case 1: CCPlayerData.inventory.EquipToInventory(this.EquippedAccessory_s); this.EquippedAccessory_s = null; break;
                case 2: CCPlayerData.inventory.EquipToInventory(this.EquippedAccessory_t); this.EquippedAccessory_t = null; break;
                default: throw new System.NotImplementedException($"this Artifact Index Not Except, Input Index: {index}");
            }
        }
        //=================================================================================================================================================

        #endregion

        public Item_Bow GetBowItem() {
            return EquippedBow;
        }

        public Item_Arrow GetMainArrow() {
            return EquippedArrow_f;
        }

        public Item_Arrow GetSubArrow() {
            return EquippedArrow_s;
        }

        /// <summary>
        /// Null 체크 해야댐.
        /// </summary>
        /// <returns></returns>
        public Item_Accessory[] GetArtifacts() {
            Item_Accessory[] accessories = { this.EquippedAccessory_f,
                                             this.EquippedAccessory_s,
                                             this.EquippedAccessory_t };
            return accessories;
        }

        public Item_SpArr GetSpArrow() {
            if (this.EquippedSpArr == null) {
                throw new System.Exception("Equipped Special Arrow is Null.");
            }
            return this.EquippedSpArr;
        }

        /// <summary>
        /// 해당 Index Number에 Artifact가 장축중인지 여부를 반환
        /// </summary>
        /// <param name="idx">0 ~ 2</param>
        /// <returns></returns>
        public bool IsEquippedArtifact(byte idx) {
            switch (idx) {
                case 0: return (this.EquippedAccessory_f != null) ? true : false;
                case 1: return (this.EquippedAccessory_s != null) ? true : false;
                case 2: return (this.EquippedAccessory_t != null) ? true : false;
                default: throw new System.NotImplementedException("this Artifact Index Number Not Except.");
            }
        }
        #region INIT_EQUIPMENTS_IN_BATTLE

        /// <summary>
        /// 장비 아이템의 장착을 확인하고 Setup호출
        /// </summary>
        /// <param name="bowObjectInitPos"></param>
        /// <param name="bowObjectParentTr"></param>
        /// <param name="mainArrowPoolSpawnAmount"></param>
        /// <param name="subArrowPoolSpawnAmoun"></param>
        public void SetupEquipments(Transform bowPrefabInitTr, Transform bowPrefabParentTr, int mainArrowPoolSpawnAmount, int subArrowPoolSpawnAmoun, UI.SwapSlots slot) {
            // Setup Bow: Main
            AD_BowAbility ability = EquippedBow.Setup(bowPrefabInitTr.position, bowPrefabParentTr);
            if (ability == null) {
                CatLog.ELog("Equipped Bow Item Setup Failed.");
                return;
            }

            // Setup Arrows
            if (IsEquippedArrMain) {
                EquippedArrow_f.Setup(AD_Data.POOLTAG_MAINARROW, AD_Data.POOLTAG_MAINARROW_LESS, mainArrowPoolSpawnAmount, ability.GetAbility(0));
            }
            if (IsEquippedArrSub == true) {
                EquippedArrow_s.Setup(AD_Data.POOLTAG_SUBARROW, AD_Data.POOLTAG_SUBARROW_LESS, subArrowPoolSpawnAmoun, ability.GetAbility(1));
            }

            // Send Result
            ability.ArrowsSetupCompleted();
            
            // Setup Artifacts
            foreach (var artifact in GetArtifacts()) {
                if (artifact != null) {
                    artifact.Setup();
                }
            }

            // Setup SP Arrow
            if (IsEquippedSpArr) {
                EquippedSpArr.Setup(ability.GetAbility(2), 3);
                ability.SetCondition(EquippedSpArr.Condition, slot);
            }
        }

        /// <summary>
        /// Battle Scene에서 Release처리 되어야할 요소들 처리
        /// </summary>
        public void ReleaseEquipments() {
            if (IsEquippedArrMain) {
                EquippedArrow_f.Release();
            }
            if (IsEquippedArrSub) {
                EquippedArrow_s.Release();
            }
        }

        #endregion

        /// <summary>
        /// Clear Equipped Item Class
        /// </summary>
        public void Clear() {
            this.EquippedBow = null;
            this.EquippedArrow_f = null;
            this.EquippedArrow_s = null;
            this.EquippedAccessory_f = null;
            this.EquippedAccessory_s = null;
            this.EquippedAccessory_t = null;
        }

        public void UpdatePlayerAbility() {
            CCPlayerData.ability.UpdateAbility(this);
        }
    }
}
