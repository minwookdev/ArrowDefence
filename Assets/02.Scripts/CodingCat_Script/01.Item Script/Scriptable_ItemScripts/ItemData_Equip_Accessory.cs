namespace ActionCat
{
    using UnityEngine;

    [CreateAssetMenu(fileName ="Item_Accessory_Asset", menuName = "Scriptable Object Asset/Item_Accessory_Asset")]
    public class ItemData_Equip_Accessory : ItemData_Equip {
        [Header("RF EFFECT")]
        public int MaxNumberOfEffect;
        public AccessoryRFSkillData[] RFEffectAssets;

        [Header("SPECIAL EFFECT")]
        public AccessorySkillData SPEffectAsset;

        public AccessorySPEffect SPEffect {
            get {
                if (SPEffectAsset != null)
                    return SPEffectAsset.Skill();
                else
                    return null;
            }
        }

        public ItemData_Equip_Accessory() : base() {
            this.Equip_Type = EQUIP_ITEMTYPE.ARTIFACT;
        }
    }
}
