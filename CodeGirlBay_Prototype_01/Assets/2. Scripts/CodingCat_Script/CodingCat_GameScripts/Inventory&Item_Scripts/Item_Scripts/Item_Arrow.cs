namespace CodingCat_Games
{
    using UnityEngine;

    public class Item_Arrow : Item_Equipment
    {
        private GameObject mainArrowObject;
        private GameObject lessArrowObject;

        //private Arrow_Skill arrowSkill;
        //화살 전용 특수효과를 어떻게 담을지

        public Item_Arrow(int id, string name, Sprite sprite, ITEMGRADE grade, GameObject main, GameObject less) : base()
        {
            this.EquipType   = EQUIP_ITEMTYPE.EQUIP_ARROW;
            this.Item_Id     = id;
            this.Item_Name   = name;
            this.Item_Sprite = sprite;
            this.Item_Grade  = grade;

            this.mainArrowObject = main;
            this.lessArrowObject = less;
        }

        /// <summary>
        /// Constructor With no Parameters. (Used Saving Function. Don't Delete this) 
        /// </summary>
        public Item_Arrow() : base() { }

        public override object GetItem() => this;
    }
}
