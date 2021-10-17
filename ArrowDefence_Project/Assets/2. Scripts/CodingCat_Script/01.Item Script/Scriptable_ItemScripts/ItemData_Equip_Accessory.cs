namespace ActionCat
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    [CreateAssetMenu(fileName ="Item_Accessory_Asset", menuName = "Scriptable Object Asset/Item_Accessory_Asset")]
    public class ItemData_Equip_Accessory : ItemData_Equip
    {
        [Header("Accessory Item Data")]
        public string effect = "";
        //public MonoScript Effect_AimSight;

        [Header("EFFECT's")]
        public int MaxNumberOfEffect;
        public ACCESSORY_RFEFFECT_TYPE[] EffectsType;
        public AccessoryRFEffect[] Effects;

        [Header("SPECIAL EFFECT")]
        public ACCESSORY_SPECIALEFFECT_TYPE SPEffectType;
        public AccessorySPEffect SpecialEffect;

        [Header("AIM SIGHT")]
        public Material LineRenderMaterial;

        public ItemData_Equip_Accessory() : base()
        {
            this.Equip_Type = EQUIP_ITEMTYPE.EQUIP_ACCESSORY;
        }

        public void OnEnable()
        {
            InitSPeffect();
            InitRFeffect();
        }

        private void InitSPeffect()
        {
            switch (SPEffectType)
            {
                case ACCESSORY_SPECIALEFFECT_TYPE.SPEFFECT_NONE:     this.SpecialEffect = null;                                  break;
                case ACCESSORY_SPECIALEFFECT_TYPE.SPEFFECT_AIMSIGHT: this.SpecialEffect = new Acsp_AimSight(LineRenderMaterial); break;
                case ACCESSORY_SPECIALEFFECT_TYPE.SPEEFECT_SLOWTIME: this.SpecialEffect = new Acsp_SlowTime();                   break;
            }
        }

        public void InitRFeffect()
        {

        }
    }
}
