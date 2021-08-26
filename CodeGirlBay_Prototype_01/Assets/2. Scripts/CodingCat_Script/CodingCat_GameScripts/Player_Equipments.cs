﻿namespace CodingCat_Games
{
    using CodingCat_Scripts;
    using System;

    [Serializable]
    public class Player_Equipments
    {
        public Item_Bow MainBow { get; private set; }
        private Item_Arrow MainArrow;
        private Item_Arrow SubArrow;

        /// <summary>
        /// 인벤토리 내부에 존재하는 Bow Item의 주소값을 들고있기
        /// </summary>
        /// <param name="item"></param>
        public void Equip_BowItem(Item_Bow item)
        {
            if(IsEquipBow())
            {
                Release_BowItem();
                this.MainBow = item;
            }
            else this.MainBow = item;

            CatLog.Log($"{this.MainBow.Item_Name} 아이템이 장착되었습니다.");
        }

        public void Release_BowItem()
        {
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
    }
}