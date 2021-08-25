namespace CodingCat_Games
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;
    using CodingCat_Scripts;

    [Serializable]
    public class AD_Inventory
    {
        public List<AD_item> invenList = new List<AD_item>();
        private readonly byte maxItemCount = 255;

        public AD_Inventory()
        {
            CatLog.Log("Setup Inventory");
        }

        public void AddItem(ItemData item)
        {
            switch (item.Item_Type)
            {
                case ITEMTYPE.ITEM_CONSUMABLE: Add_ConItem(item); break;
                case ITEMTYPE.ITEM_MATERIAL:   Add_MatItem(item); break;
                case ITEMTYPE.ITEM_EQUIPMENT:
                    switch (item)
                    {
                        case ItemData_Equip_Bow   item_Bow:   Add_BowItem(item_Bow);     break;
                        case ItemData_Equip_Arrow item_Arrow: Add_ArrowItem(item_Arrow); break;
                    }
                    break;
            }
            #region OLD_CODE

            //AD_item newItem     = new AD_item();
            //newItem.Id          = item.Item_Id;
            //newItem.ItemType    = item.Item_Type;
            //newItem.Title       = item.Item_Name;
            //newItem.Description = item.Item_Desc;
            //newItem.itemSprite  = item.Item_Sprite;
            //newItem.Amount      = item.Item_Amount;

            //invenList.Add(newItem);

            //패턴-일치 Switch 문
            //switch (item)
            //{
            //    case ItemData_Con item_con:
            //        CatLog.Log("Consume Item Added");
            //        break;
            //    case ItemData_Mat item_mat:
            //        CatLog.Log("Material Item Added");
            //        break;
            //    case ItemData_Equip item_equip:
            //        CatLog.Log("Equip Item Added");
            //        break;
            //    default:
            //        break;
            //}

            //switch (item.Item_Type)
            //{
            //    case Enum_Itemtype.ITEM_CONSUMABLE:
            //        Item_Consumable conItem = new Item_Consumable();
            //        conItem.Id.ToString...
            //        break;
            //    case Enum_Itemtype.ITEM_MATERIAL:
            //        break;
            //    case Enum_Itemtype.ITEM_EQUIPMENT:
            //        break;
            //    default:
            //        break;
            //}

            //if(newItem is )

            //equipItem.Amount         = item.Item_Amount;
            //ItemData_Equip equip = (ItemData_Equip)item; 다운캐스팅
            //equipItem.BowSkill = (ItemData_Equip)item.bowSKill;
            //equipItem.BowSkill = ;
            //item = new ItemData_Equip();

            #endregion
        }

        //Item Stack 중첩 구현 (최대 255개)
        private void Add_ConItem(ItemData newItem)
        {
            var duplicateItems = invenList.FindAll(x => x.Item_Id == newItem.Item_Id);

            //Duplicate Items Index 있는 경우 (중복되는 아이템이 있다)
            if(duplicateItems.Count > 0)
            {
                var itemAmount = newItem.Item_Amount;

                for (int i = 0; i < duplicateItems.Count; i++)
                {
                    if (duplicateItems[i].Item_Amount < maxItemCount)
                    {
                        var sumAmount = duplicateItems[i].Item_Amount + itemAmount;

                        if (sumAmount <= maxItemCount) //이번 인덱스에 들어가도 최대수량 안되는경우 바로 들어감
                        {
                            CatLog.Log("기존 아이템에서 추가됨");
                            //duplicateItems[i] = new Item_Consumable(newItem.Item_Id, sum, newItem.Item_Name,
                            //                                        newItem.Item_Desc, newItem.Item_Sprite); break;
                            
                            ((Item_Consumable)duplicateItems[i]).SetAmount(sumAmount); break;
                        }
                        else if (sumAmount > maxItemCount) //이번 인덱스에 들어가면 최대수량 넘음
                        {
                            ((Item_Consumable)duplicateItems[i]).SetAmount(maxItemCount); //합산 아이템에 최대수량
                            itemAmount = sumAmount - maxItemCount;                        //다음 index에 추가될 수량에서 뺴줌

                            if (i == duplicateItems.Count - 1) //Last Index 경우 바로 추가해줌
                            {
                                invenList.Add(new Item_Consumable(newItem.Item_Id, itemAmount,
                                                                  newItem.Item_Name,
                                                                  newItem.Item_Desc,
                                                                  newItem.Item_Sprite,
                                                                  newItem.Item_Grade));
                            }
                            else continue;
                        }
                    }
                    else if (duplicateItems[i].Item_Amount >= maxItemCount)
                    {
                        if (i == duplicateItems.Count - 1) //Last Index 경우 바로 추가해줌
                        {
                            invenList.Add(new Item_Consumable(newItem.Item_Id, itemAmount,
                                                              newItem.Item_Name,
                                                              newItem.Item_Desc,
                                                              newItem.Item_Sprite,
                                                              newItem.Item_Grade));
                        }
                        else continue;
                    }
                }
            }
            else //중복되는 아이템이 없다 -> 바로 추가해줌
            {
                invenList.Add(new Item_Consumable(newItem.Item_Id,
                                                  newItem.Item_Amount,
                                                  newItem.Item_Name,
                                                  newItem.Item_Desc,
                                                  newItem.Item_Sprite,
                                                  newItem.Item_Grade));
            }
        }

        //Item Stack 중첩 구현 (최대 255개)
        private void Add_MatItem(ItemData newItem)
        {
            var dupItems = invenList.FindAll(x => x.Item_Id == newItem.Item_Id);

            //Duplicate Items Index 있는 경우 (중복되는 아이템이 있다)
            if (dupItems.Count > 0)
            {
                var itemAmount = newItem.Item_Amount;

                for (int i = 0; i < dupItems.Count; i++)
                {
                    if (dupItems[i].Item_Amount < maxItemCount) //인덱스가 최대수량 아님
                    {
                        var sumAmount = dupItems[i].Item_Amount + itemAmount;

                        if (sumAmount <= maxItemCount) //이번 인덱스에 들어가도 최대수량 안되는경우 바로 들어감
                        {
                            ((Item_Material)dupItems[i]).SetAmount(sumAmount); break;
                        }
                        else if (sumAmount > maxItemCount) //이번 인덱스에 들어가면 최대수량 넘음
                        {
                            ((Item_Material)dupItems[i]).SetAmount(maxItemCount); //합산 아이템에 최대수량
                            itemAmount = sumAmount - maxItemCount;        //다음 index에 추가될 수량에서 뺴줌

                            if (i == dupItems.Count - 1) //Last Index 경우 바로 추가해줌
                            {
                                invenList.Add(new Item_Material(newItem.Item_Id, itemAmount,
                                                                newItem.Item_Name,
                                                                newItem.Item_Desc,
                                                                newItem.Item_Sprite,
                                                                newItem.Item_Grade));
                            }
                            else continue;
                        }
                    }
                    else if (dupItems[i].Item_Amount >= maxItemCount) //인덱스가 이미 최대수량
                    {
                        if (i == dupItems.Count - 1) //Last Index 경우 바로 추가해줌
                        {
                            invenList.Add(new Item_Material(newItem.Item_Id, itemAmount,
                                                            newItem.Item_Name,
                                                            newItem.Item_Desc,
                                                            newItem.Item_Sprite,
                                                            newItem.Item_Grade));
                        }
                        else continue;
                    }
                }
            }
            else //중복되는 아이템이 없다 -> 바로 추가해줌
            {
                invenList.Add(new Item_Material(newItem.Item_Id,
                                                newItem.Item_Amount,
                                                newItem.Item_Name,
                                                newItem.Item_Desc,
                                                newItem.Item_Sprite,
                                                newItem.Item_Grade));
            }

            //Con Item, Mat Item Byte형 데이터로 바꿔주기
        }

        /// <summary>
        /// Bow Item Add Inventory
        /// </summary>
        /// <param name="newItem">BowItem Data</param>
        private void Add_BowItem(ItemData_Equip_Bow newItem)
        {
            invenList.Add(new Item_Bow(newItem.Item_Id,
                                       newItem.Item_Name,
                                       newItem.Item_Desc,
                                       newItem.Item_Sprite,
                                       newItem.Item_Grade,
                                       newItem.BowGameObject,
                                       newItem.BowSkill_First,
                                       newItem.BowSkill_Second));
        }

        /// <summary>
        /// Arrow Item Add Inventory
        /// </summary>
        /// <param name="newItem">Arrow item Data</param>
        private void Add_ArrowItem(ItemData_Equip_Arrow newItem)
        {
            invenList.Add(new Item_Arrow(newItem.Item_Id,
                                         newItem.Item_Name,
                                         newItem.Item_Sprite,
                                         newItem.Item_Grade,
                                         newItem.MainArrowObj,
                                         newItem.LessArrowObj));
        }

        private void Add_AccessItem()
        {

        }

        public void ClearInventory() => invenList.Clear();

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

            CatLog.Log($"itemList Count : {itemList.Count}");

            return itemList;
        }

        /// <summary>
        /// Equipment Item (Arrow) List Get
        /// </summary>
        /// <returns></returns>
        public List<AD_item> GetArrowItemList()
        {
            var itemList = invenList.FindAll(x => x.GetType() == typeof(Item_Arrow));

            return itemList;
        }

        /// <summary>
        /// Get Equipment Item (Accessory) List
        /// </summary>
        /// <returns></returns>
        //public List<AD_item> GetAccessoryItemList()
        //{
        //    return invenList.FindAll(x => x.GetType() == typeof(Item_Bow));
        //}

        /// <summary>
        /// Get Material, Consumable Item List
        /// </summary>
        /// <returns></returns>
        public List<AD_item> GetItemList()
        {
            var itemList = invenList.FindAll(x => x.Item_Type == ITEMTYPE.ITEM_MATERIAL ||
                                                  x.Item_Type == ITEMTYPE.ITEM_CONSUMABLE);

            CatLog.Log($"itemList Count : {itemList.Count}");

            return itemList;
        }
    }
}
