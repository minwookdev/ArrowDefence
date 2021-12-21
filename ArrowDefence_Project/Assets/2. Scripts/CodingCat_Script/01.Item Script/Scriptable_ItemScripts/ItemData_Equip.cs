using UnityEngine;

namespace ActionCat
{
    public class ItemData_Equip : ItemData
    {
        [Header("Equipment Item Data")]
        [Space(15)]
        [ReadOnly] public EQUIP_ITEMTYPE Equip_Type;
        public Ability[] abilityDatas = null;

        //Equipment Item Common Abilities
        [Range(0f, 500)]  public float BaseDamage = 0f;
        [Range(0, 30)]    public int CriticalHitChance = 0;
        [Range(1.5f, 3f)] public float CriticalMultiplier = 1.5f;
        
        protected ItemData_Equip() {
            Item_Type = ITEMTYPE.ITEM_EQUIPMENT;
            Item_Amount = 1;
        }
    }
}
