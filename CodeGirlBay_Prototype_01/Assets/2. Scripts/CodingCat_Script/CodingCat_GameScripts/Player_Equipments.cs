namespace CodingCat_Games
{
    using CodingCat_Games.Data;
    using CodingCat_Scripts;
    using System;

    [Serializable]
    public class Player_Equipments
    {
        private Item_Bow MainBow;
        private Item_Arrow MainArrow;
        private Item_Arrow SubArrow;

        private Item_Accessory AccessoryItem;

        #region EQUIP_REALESE

        //마법활, 마법장신구등 장비종류 좀 늘어날텐데 이거 메서드 좀 적게 관리 가능할거같은데, 장비 종류 늘어날거 대비해서 늘리자
        //사실살 Equipment라는 하나의 인벤토리가 늘어난 셈으로 알면된다. -> 장신구는 배열로 깔아도 괜찮을것같다 아니면 장신구 관련 함수들은 배열로 하든지

        /// <summary>
        /// 인벤토리 내부의 Bow Item Data가져와서 새로 할당하고 Inventory내부에 있던 아이템 지워줌
        /// </summary>
        /// <param name="item"></param>
        public void Equip_BowItem(Item_Bow item)
        {
            if(IsEquipBow())
            {
                Release_BowItem();
                this.MainBow = new Item_Bow(item);
                CCPlayerData.inventory.DelItem(item);
            }
            else
            {
                this.MainBow = new Item_Bow(item);
                CCPlayerData.inventory.DelItem(item);
            }

            CatLog.Log($"{this.MainBow.GetName} 아이템이 장착되었습니다.");
        }

        public void Release_BowItem()
        {
            CCPlayerData.inventory.Add_BowItem(this.MainBow);
            this.MainBow = null;

            CatLog.Log("아이템이 해제되었습니다.");
        }

        public void Equip_ArrowItem(Item_Arrow item)
        {
            if (IsEquipMainArrow()) Release_ArrowItem();

            this.MainArrow = new Item_Arrow(item);
            CCPlayerData.inventory.DelItem(item);
            CatLog.Log($"{this.MainArrow.GetName} 아이템이 장착되었습니다.");
        }

        public void Release_ArrowItem()
        {
            CCPlayerData.inventory.Add_ArrowItem(this.MainArrow);
            this.MainArrow = null;

            CatLog.Log("아이템이 해제되었습니다.");
        }

        public void Equip_SubArrow(Item_Arrow item)
        {
            if (IsEquipSubArrow()) Release_SubArrow();

            this.SubArrow = new Item_Arrow(item);
            CCPlayerData.inventory.DelItem(item);
            CatLog.Log($"{this.SubArrow.GetName} 아이템이 장착되었습니다.");
        }

        public void Release_SubArrow()
        {
            CCPlayerData.inventory.Add_ArrowItem(this.SubArrow);
            this.SubArrow = null;

            CatLog.Log("아이템이 해제되었습니다.");
        }

        public void Equip_AccessoryItem(Item_Accessory item)
        {
            if (IsEquipAccessory()) Release_AccessoryItem();

            this.AccessoryItem = new Item_Accessory(item);
            CCPlayerData.inventory.DelItem(item);
            CatLog.Log($"{AccessoryItem.GetName} 아이템이 장착되었습니다.");
        }

        public void Release_AccessoryItem()
        {
            CCPlayerData.inventory.Add_AccessoryItem(this.AccessoryItem);
            this.AccessoryItem = null;

            CatLog.Log("아이템이 해제되었습니다.");
        }

        #endregion

        public Item_Bow GetBowItem() => this.MainBow;

        public Item_Arrow GetMainArrow() => this.MainArrow;

        public Item_Arrow GetSubArrow() => this.SubArrow;

        public Item_Accessory GetAccessory() => this.AccessoryItem;

        public bool IsEquipBow()
        {
            if (this.MainBow != null) return true;
            else                      return false;
        }

        public bool IsEquipMainArrow()
        {
            if (this.MainArrow != null) return true;
            else                        return false;
        }

        public bool IsEquipSubArrow()
        {
            if (this.SubArrow != null) return true;
            else                       return false;
        }

        public bool IsEquipAccessory()
        {
            if (this.AccessoryItem != null) return true;
            else                            return false;  
        }

        public void CompareItem(Item_Bow compareTarget)
        {
            if (ReferenceEquals(this.MainBow, compareTarget)) CatLog.Log("같은 아이템 입니다.");
            else                                              CatLog.Log("다른 아이템 입니다.");
        }
    }
}
