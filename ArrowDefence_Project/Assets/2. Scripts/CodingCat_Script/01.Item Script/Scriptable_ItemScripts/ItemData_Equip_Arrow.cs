namespace ActionCat
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Item_Arrow_Asset", menuName = "Scriptable Object Asset/Item_Arrow_Asset")]
    public class ItemData_Equip_Arrow : ItemData_Equip
    {
        [Header("Arrow Item Data")]
        public GameObject MainArrowObj;
        public GameObject LessArrowObj;

        public ArrowSkillData ArrowSkillFst;
        public ArrowSkillData ArrowSkillSec;

        public ItemData_Equip_Arrow() : base()
        {
            this.Equip_Type = EQUIP_ITEMTYPE.EQUIP_ARROW;
        }

        public void OnEnable()
        {
            //Check Arrow Skill Active Type
            if(ArrowSkillFst != null) {
                if (ArrowSkillSec == null) return;
                if (ArrowSkillFst.ActiveType == ArrowSkillSec.ActiveType) {
                    CatLog.WLog($"발동 타입이 중복된 Arrow Skill이 감지되었습니다. from. {Item_Name}");
                }
            }
        }
    }
}
