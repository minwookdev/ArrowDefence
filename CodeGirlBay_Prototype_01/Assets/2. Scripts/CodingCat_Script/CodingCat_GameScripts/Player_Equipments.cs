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

        public Item_Bow GetBowItem() => this.MainBow;

        public Item_Arrow GetMainArrow() => this.MainArrow;

        public Item_Arrow GetSubArrow() => this.SubArrow;

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

        public void CompareItem(Item_Bow compareTarget)
        {
            if (ReferenceEquals(this.MainBow, compareTarget)) CatLog.Log("같은 아이템 입니다.");
            else                                              CatLog.Log("다른 아이템 입니다.");
        }
    }
}
