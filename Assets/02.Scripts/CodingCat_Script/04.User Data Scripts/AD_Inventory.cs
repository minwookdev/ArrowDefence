﻿namespace ActionCat
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class AD_Inventory {
        private List<AD_item> invenList = new List<AD_item>();
        private readonly byte maxItemCount = 255;

        public AD_Inventory()
        {
            CatLog.Log("Setup Inventory");
        }

        /// <summary>
        /// Add Item Data In Player Inventory
        /// </summary>
        /// <param name="item">New Item Data</param>
        /// <param name="quantity">Quantity Data does not apply to the acquisition of equipment items</param>
        public void AddItem(ItemData item, int quantity) {
            switch (item) {
                case ItemData_Con             newitem: Add_ConsumableItem(newitem, quantity); break;
                case ItemData_Mat             newitem: Add_MaterialItem(newitem, quantity);   break;
                case ItemData_Equip_Bow       newitem: Add_BowItem(newitem);                  break;
                case ItemDt_SpArr             newitem: Add_SpArrItem(newitem);                break;
                case ItemData_Equip_Arrow     newitem: Add_ArrowItem(newitem);                break;
                case ItemData_Equip_Accessory newitem: Add_AccessItem(newitem);               break;
                default: throw new NotImplementedException();
            }
        }

        #region ADD

        /// <summary>
        /// Item Stacking For Consumable Item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newItemData"></param>
        /// <param name="quantity"></param>
        private void StackOnConsumableItem<T>(T newItemData, int quantity) where T : ItemData_Con
        {
            var duplicateItems = invenList.FindAll(x => x.GetID == newItemData.Item_Id);

            //var duplicateItems = invenList.FindAll(x => x.GetID == newItemData.Item_Id &&
            //                                x.GetAmount < maxItemCount); //요로케 바꿔서 조건 좀 간단하게 할 수 있지않을까

            //인벤토리에 중복되는 아이템이 있는 경우 (Item ID로 비교)
            if (duplicateItems.Count > 0)
            {
                int tempQuantity = quantity;

                for (int i = 0; i < duplicateItems.Count; i++)
                {
                    if (duplicateItems[i].GetAmount < maxItemCount) //index i번 아이템에 더 집어넣을 수 있을때
                    {
                        int sumQuantity = tempQuantity + duplicateItems[i].GetAmount;

                        if(sumQuantity <= maxItemCount)     //i번 인덱스 아이템에 Add해도 최대수량을 넘지않는 경우 (바로 들어감)
                        {
                            CatLog.Log("기존 아이템에서 추가됨");
                            ((Item_Consumable)duplicateItems[i]).SetAmount(sumQuantity); break;
                        }
                        else if(sumQuantity > maxItemCount) //i번 인덱스 아이템에 Add하면 최대수량을 넘기는 경우
                        {
                            ((Item_Consumable)duplicateItems[i]).SetAmount(maxItemCount);
                            tempQuantity = sumQuantity - maxItemCount;

                            if (i == duplicateItems.Count - 1) invenList.Add(new Item_Consumable(newItemData, tempQuantity));
                            else continue;
                        }
                    }
                    else if (duplicateItems[i].GetAmount >= maxItemCount) //index i번 아이템에 더 집어넣을 수 없을때
                    {
                        if (i == duplicateItems.Count - 1) invenList.Add(new Item_Consumable(newItemData, tempQuantity));
                        else continue;
                    }
                }
            }
            else invenList.Add(new Item_Consumable(newItemData, quantity));
            //중복되는 아이템이 없는경우 바로 인벤토리에 ADD 해줌
        }

        /// <summary>
        /// Item Stacking For Material Item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newItemData"></param>
        /// <param name="quantity"></param>
        private void StackOnMaterialItem<T>(T newItemData, int quantity) where T : ItemData_Mat
        {
            var duplicateItems = invenList.FindAll(x => x.GetID == newItemData.Item_Id);

            //인벤토리에 중복되는 아이템이 있는 경우 (Item ID로 비교)
            if (duplicateItems.Count > 0)
            {
                int tempQuantity = quantity;

                for (int i = 0; i < duplicateItems.Count; i++)
                {
                    if (duplicateItems[i].GetAmount < maxItemCount) //index i번 아이템에 더 집어넣을 수 있을때
                    {
                        int sumQuantity = tempQuantity + duplicateItems[i].GetAmount;

                        if (sumQuantity <= maxItemCount)     //i번 인덱스 아이템에 Add해도 최대수량을 넘지않는 경우 (바로 들어감)
                        {
                            CatLog.Log("기존 아이템에서 추가됨");
                            ((Item_Material)duplicateItems[i]).SetAmount(sumQuantity); break;
                        }
                        else if (sumQuantity > maxItemCount) //i번 인덱스 아이템에 Add하면 최대수량을 넘기는 경우
                        {
                            ((Item_Material)duplicateItems[i]).SetAmount(maxItemCount);
                            tempQuantity = sumQuantity - maxItemCount;

                            if (i == duplicateItems.Count - 1) invenList.Add(new Item_Material(newItemData, tempQuantity));
                            else continue;
                        }
                    }
                    else if (duplicateItems[i].GetAmount >= maxItemCount) //index i번 아이템에 더 집어넣을 수 없을때
                    {
                        if (i == duplicateItems.Count - 1) invenList.Add(new Item_Material(newItemData, tempQuantity));
                        else continue;
                    }
                }
            }
            else invenList.Add(new Item_Material(newItemData, quantity));
            //중복되는 아이템이 없는경우 바로 인벤토리에 ADD 해줌
        }

        /// <summary>
        /// Consumable Item Add Inventory
        /// </summary>
        /// <param name="newItem">New Consumable Item Data</param>
        /// <param name="quantity">Item Quantity int Data</param>
        private void Add_ConsumableItem(ItemData_Con newItem, int quantity)
        {
            StackOnConsumableItem(newItem, quantity);
        }

        /// <summary>
        /// Material Item Add Inventory
        /// </summary>
        /// <param name="newItem">New Material Item Data</param>
        /// <param name="quantity">Item Quantity int Data</param>
        private void Add_MaterialItem(ItemData_Mat newItem, int quantity)
        {
            StackOnMaterialItem(newItem, quantity);
        }

        /// <summary>
        /// Bow Item Add Inventory
        /// </summary>
        /// <param name="newItem">BowItem Data</param>
        private void Add_BowItem(ItemData_Equip_Bow newItem)
        {
            invenList.Add(new Item_Bow(newItem));
        }

        /// <summary>
        /// Arrow Item Add Inventory
        /// </summary>
        /// <param name="newItem">Arrow item Data</param>
        private void Add_ArrowItem(ItemData_Equip_Arrow newItem) {
            invenList.Add(new Item_Arrow(newItem));
        }

        private void Add_SpArrItem(ItemDt_SpArr newItem) {
            invenList.Add(new Item_SpArr(newItem));
        }

        /// <summary>
        /// Accessory Item Add by Inventory
        /// </summary>
        /// <param name="newItem"></param>
        private void Add_AccessItem(ItemData_Equip_Accessory newItem)
        {
            invenList.Add(new Item_Accessory(newItem));
        }

        #region ONLY_USING_PLAYER_EQUIPMENT

        public void ToInven(Item_SpArr item) => invenList.Add(item);

        public void EquipToInventory(Item_Equipment item) {
            CatLog.Log($"Equipped Item to Inventory, Name: {item.GetNameByTerms}");
            invenList.Add(item);
        }

        #endregion

        #endregion

        #region DELETE

        /// <summary>
        /// Find the Item Reference and removes an Item from the Inventory
        /// </summary>
        /// <param name="target"></param>
        public void RemoveItem(AD_item target) {
            if (invenList.Contains(target)) {
                invenList.Remove(target);
                CatLog.Log($"인벤토리에서 해당 아이템 {target.GetTermsName}를(을) 제거하였습니다.");
            }
            else {
                CatLog.WLog("인벤토리 내부에 해당 아이템이 없습니다.");
            }
        }

        public bool RemoveItem(string itemid, int removeAmount) {
            var findItems = invenList.FindAll(item => item.GetID.Equals(itemid));
            int findItemsTotalAmount = 0;
            findItems.ForEach((item) => findItemsTotalAmount += item.GetAmount);
            if(findItems.Count <= 0 || removeAmount > findItemsTotalAmount) {
                return false;
            }

            var toRemove = new List<AD_item>();
            for (int i = findItems.Count - 1; i >= 0; i--) {
                if (removeAmount <= 0) 
                    break;

                var decreaseAmount = findItems[i].GetAmount - removeAmount;
                if (decreaseAmount <= 0) {
                    toRemove.Add(findItems[i]);
                }
                else {
                    if (findItems[i] is IStackable stackable) { //Mateiral, Consumable
                        stackable.SetAmount(decreaseAmount);
                    }
                    else { //Equipment
                        toRemove.Add(findItems[i]);
                        CatLog.ELog($"장비 아이템의 수량 지정 에러, 아이템 이름 : {findItems[i].GetTermsName}");
                    }
                }

                removeAmount -= findItems[i].GetAmount;
            }

            invenList.RemoveAll(toRemove.Contains);
            return true;
        }

        public void Clear() => invenList.Clear();

        #endregion

        #region FIND

        public bool IsExist(AD_item target) {
            return (invenList.Find(item => ReferenceEquals(item, target)) != null) ? true : false;
        }

        public bool TryGetAmount(string itemId, out int amount) {
            var findItems = invenList.FindAll((item) => item.GetID.Equals(itemId));
            amount = 0;
            if (findItems.Count > 0) {
                //target item 이 존재
                foreach (var item in findItems) {
                    amount += item.GetAmount;
                }
                return true;
            }
            else {
                //target item 이 존재하지 않음
                return false;
            }

        }

        /// <summary>
        /// ALL Item List Get
        /// </summary>
        /// <returns></returns>
        public List<AD_item> GetAllItemList() => invenList;

        /// <summary>
        /// Equipment Item (Bow) List Get
        /// </summary>
        /// <returns></returns>
        public List<AD_item> GetBowItemList()
        {
            //var equipList = invenList.FindAll(x => x.ItemType == Enum_Itemtype.ITEM_EQUIPMENT);
            var itemList = invenList.FindAll(x => x.GetType() == typeof(Item_Bow));
            //var itemList = invenList.FindAll(x => x is Item_Bow);
            //var itemList = invenList.FindAll(x => x.Equals(Item_Bow)); // -> 이건 잘못됨
            return itemList;
        }

        /// <summary>
        /// Equipment Item (Arrow) List Get
        /// </summary>
        /// <returns></returns>
        public List<AD_item> GetArrowItemList() {
            //var itemList = invenList.FindAll(x => x.GetType() == typeof(Item_Arrow));
            var arrItemList = invenList.FindAll(item => item.GetType() == typeof(Item_SpArr) || item.GetType() == typeof(Item_Arrow));
            return arrItemList;
        }

        /// <summary>
        /// Get Equipment Item (Accessory) List
        /// </summary>
        /// <returns></returns>
        public List<AD_item> GetAccessoryItemList()
        {
            var itemList = invenList.FindAll(x => x.GetType() == typeof(Item_Accessory));
            return itemList;
        }

        /// <summary>
        /// Get Material, Consumable Item List
        /// </summary>
        /// <returns></returns>
        public List<AD_item> GetItemList()
        {
            var itemList = invenList.FindAll(x => x.GetItemType == ITEMTYPE.ITEM_MATERIAL ||
                                                  x.GetItemType == ITEMTYPE.ITEM_CONSUMABLE);
            return itemList;
        }

        public AD_item[] GetAllBluePrints() {
            return invenList.FindAll(item => item.GetID.StartsWith("2")).ToArray();
        }

        public AD_item[] GetBluePrints(string[] matchKeys) {
            //설계도 아이템 축약
            var bluePrints = invenList.FindAll(item => item.GetID.StartsWith("2"));
            if (bluePrints.Count <= 0) {
                return bluePrints.ToArray();
            }

            var toRemove = new List<AD_item>(); //반환될 리스트에서 삭제될 요소
            for (int i = 0; i < bluePrints.Count; i++) {
                for (int j = 0; j < matchKeys.Length; j++) {
                    if (bluePrints[i].GetID.Equals(matchKeys[j]) == true) {
                        continue;
                    }

                    if (j == matchKeys.Length - 1) {
                        toRemove.Add(bluePrints[i]);
                    }
                }
            }

            bluePrints.RemoveAll(toRemove.Contains);
            return bluePrints.ToArray();
        }

        public AD_item[] GetBluePrints(BLUEPRINTTYPE type) {
            string targetId;
            switch (type) {
                case BLUEPRINTTYPE.MATERIAL:   targetId = "21"; break;
                case BLUEPRINTTYPE.CONSUMABLE: targetId = "22"; break;
                case BLUEPRINTTYPE.BOW:        targetId = "23"; break;
                case BLUEPRINTTYPE.ARROW:      targetId = "24"; break;
                case BLUEPRINTTYPE.ARTIFACT:   targetId = "25"; break;
                default: throw new NotImplementedException();
            }

            return invenList.FindAll(item => item.GetID.Substring(0, 2).Equals(targetId)).ToArray();
        }

        public AD_item[] GetUpgradeableItems(string[] matchKeys) {
            var equipments = invenList.FindAll(item => item.GetItemType == ITEMTYPE.ITEM_EQUIPMENT);
            var result = new List<AD_item>();
            for (int i = 0; i < equipments.Count; i++) {
                for (int j = 0; j < matchKeys.Length; j++) {
                    if(equipments[i].GetID.Equals(matchKeys[j])) {
                        result.Add(equipments[i]);
                        break;
                    }
                }
            }

            return result.ToArray();
        }

        #endregion

        #region CLONE

        public void CloneItem(Item_Bow newItem) => invenList.Add(new Item_Bow(newItem));

        public void CloneItem(Item_Arrow item) => invenList.Add(new Item_Arrow(item));

        public void CloneItem(Item_Accessory item) => invenList.Add(new Item_Accessory(item));

        #endregion
    }
}
