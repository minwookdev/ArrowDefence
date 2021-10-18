namespace ActionCat
{
    using UnityEditor;
    using UnityEngine;

    public class Item_Accessory : Item_Equipment
    {
        private int maxNumberOfEffect;
        private AccessoryRFEffect[] effects;
        private AccessorySPEffect specialEffect;

        public Item_Accessory(ItemData_Equip_Accessory item) : base()
        {
            //Set Equipment Item Type
            this.EquipType = EQUIP_ITEMTYPE.EQUIP_ACCESSORY;

            //Set Default Item Data
            this.Item_Id     = item.Item_Id;
            this.Item_Name   = item.Item_Name;
            this.Item_Desc   = item.Item_Desc;
            this.Item_Sprite = item.Item_Sprite;
            this.Item_Grade  = item.Item_Grade;

            //Set Accessory Item Data
            //specialEffect = item.SpecialEffect;
            specialEffect = item.SPEffect;
        }

        public Item_Accessory(Item_Accessory item) : base()
        {
            //Set Equipment Item Type
            this.EquipType = EQUIP_ITEMTYPE.EQUIP_ACCESSORY;

            //Set Default Item Data
            this.Item_Id     = item.Item_Id;
            this.Item_Name   = item.Item_Name;
            this.Item_Desc   = item.Item_Desc;
            this.Item_Sprite = item.Item_Sprite;
            this.Item_Grade  = item.Item_Grade;

            //Set Accessory Item Data 
            specialEffect = item.specialEffect;
        }

        /// <summary>
        /// Constructor With no Parameters. (Used Saving Function. Don't Delete this) 
        /// </summary>
        public Item_Accessory() : base() { }

        public void Setup()
        {
            //여기다가 Effect 실행하는 코드 써보면된다 !
            //Bow GameObject를 변수로 받던지 하면될거같다
            //아니면 GameObject.Find 사용하거나 등등

            //var bowGameObject = GameObject.FindWithTag(AD_Data.OBJECT_TAG_BOW);
            //if(bowGameObject)
            //{
            //    System.Type effectScript = AcEffectScript.GetClass();
            //    bowGameObject.AddComponent(effectScript);
            //}

            if (specialEffect != null) specialEffect.Setup();

            //사실 그냥 이렇게 하면되긴하네 GameObject 형에 직접적으로 컴포넌트를 붙이는 거니깐
            //bowGameObject.AddComponent<AcEffect_AimSight>();
        }
    }
}
