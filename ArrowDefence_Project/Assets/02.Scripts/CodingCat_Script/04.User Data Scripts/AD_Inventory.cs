namespace ActionCat
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

        public void ToInven(Item_Bow newItem) => invenList.Add(new Item_Bow(newItem));

        public void ToInven(Item_Arrow item) => invenList.Add(new Item_Arrow(item));

        public void ToInven(Item_Accessory item) => invenList.Add(new Item_Accessory(item));

        public void ToInven(Item_SpArr item) => invenList.Add(item);

        #endregion

        #endregion

        #region DELETE

        /// <summary>
        /// Find the Item Reference and removes an Item from the Inventory
        /// </summary>
        /// <param name="target"></param>
        public void DelItem(AD_item target) {
            if (invenList.Contains(target)) {
                invenList.Remove(target);
                CatLog.Log($"인벤토리에서 해당 아이템 {target.GetName}를(을) 제거하였습니다.");
            }
            else {
                CatLog.WLog("인벤토리 내부에 해당 아이템이 없습니다.");
            }
        }

        public void Clear() => invenList.Clear();

        #endregion

        #region FIND

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

        #endregion
    }
}
