namespace ActionCat
{
    using UnityEngine;

    [CreateAssetMenu(fileName ="Item_Accessory_Asset", menuName = "Scriptable Object Asset/Item_Accessory_Asset")]
    public class ItemData_Equip_Accessory : ItemData_Equip
    {
        //public MonoScript Effect_AimSight;

        [Header("RF EFFECT")]
        public int MaxNumberOfEffect;
        public AccessoryRFSkillData[] RFEffectAssets;

        [Header("SPECIAL EFFECT")]
        public AccessorySkillData SPEffectAsset;

        public AccessorySPEffect SPEffect
        {
            get
            {
                if (SPEffectAsset != null)
                    return SPEffectAsset.Skill();
                else
                    return null;
            }
        }


        public ItemData_Equip_Accessory() : base() {
            this.Equip_Type = EQUIP_ITEMTYPE.EQUIP_ACCESSORY;
        }

        public void OnEnable() {
            //InitSPeffect();
            //InitRFeffect();
        }

        //private void InitSPeffect()
        //{
        //    switch (SPEffectType)
        //    {
        //        case ACCESSORY_SPEFFECT_TYPE.SPEFFECT_NONE:     this.SpecialEffect = null;                                  break;
        //        case ACCESSORY_SPEFFECT_TYPE.SPEFFECT_AIMSIGHT: this.SpecialEffect = new Acsp_AimSight(LineRenderMaterial, 0.050f); break;
        //        case ACCESSORY_SPEFFECT_TYPE.SPEEFECT_SLOWTIME: this.SpecialEffect = new Acsp_SlowTime();                   break;
        //    }
        //}
        //
        //public void InitRFeffect()
        //{
        //
        //}
    }
}
