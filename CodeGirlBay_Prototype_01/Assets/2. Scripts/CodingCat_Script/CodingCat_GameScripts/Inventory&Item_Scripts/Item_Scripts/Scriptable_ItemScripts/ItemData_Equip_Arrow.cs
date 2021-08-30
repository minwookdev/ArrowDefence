namespace CodingCat_Games
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Item_Arrow_Asset", menuName = "Scriptable Object Asset/Item_Arrow_Asset")]
    public class ItemData_Equip_Arrow : ItemData_Equip
    {
        [Header("Arrow Item Data")]
        [Space(15)]
        public GameObject MainArrowObj;
        public GameObject LessArrowObj;

        public ItemData_Equip_Arrow() : base()
        {
            this.Equip_Type = EQUIP_ITEMTYPE.EQUIP_ARROW;
        }
    }
}
