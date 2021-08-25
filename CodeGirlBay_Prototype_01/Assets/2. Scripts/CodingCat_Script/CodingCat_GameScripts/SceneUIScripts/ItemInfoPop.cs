namespace CodingCat_Games
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using CodingCat_Scripts;

    public enum Popup_Type
    { 
        None             = 0,
        Popup_NormalItem = 1,
        Popup_BowItem    = 2,
        Popup_ArrowItem  = 3,
        Popup_Accessory  = 4
    }


    public class ItemInfoPop : MonoBehaviour
    {
        [Serializable]
        public class ItemPop_Normal
        {
            //Consumeable & Material Item
            public GameObject Popup_Object;
            public Image Image_Item;
            public Image Image_Frame;
            public TextMeshProUGUI Text_ItemType;
            public TextMeshProUGUI Text_ItemName;
            public TextMeshProUGUI Text_ItemDesc;
            public TextMeshProUGUI Text_ItemCount;

            private AD_item itemAddress; //어떤 아이템의 주소가 들어가있는지 Custom INspector를 통해 확인할 수 있도록?

            public void EnablePopup(Item_Consumable item, Sprite frame)
            {
                Text_ItemType.text  = "Consumable";
                Text_ItemName.text  = item.Item_Name;
                Text_ItemDesc.text  = item.Item_Desc;
                Text_ItemCount.text = item.Item_Amount.ToString();
                Image_Item.sprite   = item.Item_Sprite;
                Image_Frame.sprite  = frame;
                itemAddress         = item;

                Popup_Object.SetActive(true);
            }

            public void EnablePopup(Item_Material item, Sprite frame)
            {
                Text_ItemType.text  = "Consumable";
                Text_ItemName.text  = item.Item_Name;
                Text_ItemDesc.text  = item.Item_Desc;
                Text_ItemCount.text = item.Item_Amount.ToString();
                Image_Item.sprite   = item.Item_Sprite;
                Image_Frame.sprite  = frame;
                itemAddress         = item;

                Popup_Object.SetActive(true);
            }

            public void DisablePop()
            {
                Text_ItemType.text  = "";
                Text_ItemName.text  = "";
                Text_ItemDesc.text  = "";
                Text_ItemCount.text = "";
                itemAddress         = null;

                Popup_Object.SetActive(false);
            }
        }

        [Serializable]
        public class ItemPop_Equip_Bow
        {
            public GameObject PopObject;
            public Image Image_Item;
            public Image Image_Frame;
            public TextMeshProUGUI Text_ItemType;
            public TextMeshProUGUI Text_ItemName;
            public TextMeshProUGUI Text_ItemDesc;
            public Button Button_Equip;
            public Button Button_Release;

            public GameObject[] Object_SkillSlots;

            private Item_Bow itemAddress;

            public void EnablePopup(Item_Bow item, Sprite sprite)
            {
                Text_ItemType.text = "Equipment";
                Text_ItemName.text = item.Item_Name;
                Image_Item.sprite  = item.Item_Sprite;
                Image_Frame.sprite = sprite;
                itemAddress        = item;

                //if(item.IsBowSkill())
                //{
                //    Object_SkillSlots[0].SetActive(true);
                //    Object_SkillSlots[1].SetActive(false);
                //}
                //else
                //{
                //    foreach (var slot in Object_SkillSlots)
                //    {
                //        slot.SetActive(false);
                //    }
                //    //일단은 둘 다 꺼주고 있는데 BowSkill은 최대 2개까지 구상중이므로
                //    //Skill 이 복수인 상황도 생각해서 다시 작성
                //}

                for(int i =0;i<item.GetBowSkills().Length;i++)
                {
                    if (item.GetBowSkills()[i] != null) Object_SkillSlots[i].SetActive(true);
                    else                                Object_SkillSlots[i].SetActive(false);
                }

                //플레이어의 현재 장비아이템이 선택한 장비아이템인지 비교해서 버튼 띄워줌
                if (AD_PlayerData.PlayerEquipments.MainBow != itemAddress) SwitchButtons(false);
                else                                                       SwitchButtons(true);

                PopObject.SetActive(true);
            }

            public void DisablePopup()
            {
                Text_ItemType.text = "";
                Text_ItemName.text = "";
                itemAddress        = null;

                foreach (var item in Object_SkillSlots)
                {
                    if (item.activeSelf) item.SetActive(false);
                }

                PopObject.SetActive(false);
            }

            public Item_Bow GetItemAddress()
            {
                if(itemAddress != null)
                {
                    CatLog.Log("Bow Item Address 전달되었습니다.");
                    return itemAddress;
                }
                else
                {
                    CatLog.WLog("Bow Item Address 존재하지 않습니다.");
                    return null;
                }
            }

            //Temp Method
            public bool GetItemAddress(ref Item_Bow bowItem)
            {
                if(itemAddress != null)
                {
                    bowItem = itemAddress;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Button_EquipAction()
            {
                AD_PlayerData.PlayerEquipments.Equip_BowItem(itemAddress);
                SwitchButtons(true);

                //여기서 UpdateEquipUI Call
                UI_Equipments.Update_EquipUI();
            }

            public void Button_ReleaseAction()
            {
                AD_PlayerData.PlayerEquipments.Release_BowItem();
                SwitchButtons(false);

                UI_Equipments.Update_EquipUI();
            }

            //장착버튼 교체
            private void SwitchButtons(bool isEquip)
            {
                if(isEquip) //장착중인 아이템이면 해제버튼을 활성화
                {
                    Button_Release.gameObject.SetActive(true);
                    Button_Equip.gameObject.SetActive(false);
                }
                else        //장착중인 아이템이 아니면 장착버튼을 활성화
                {
                    Button_Equip.gameObject.SetActive(true);
                    Button_Release.gameObject.SetActive(false);
                }
            }
        }

        public Sprite[] Frames;

        [Space(10)]
        public ItemPop_Normal    ItemPop;      //Material, Consumable Item Popup
        public ItemPop_Equip_Bow ItemPop_Bow;  //Equipment Bow Item Popup
        private Popup_Type       popType = Popup_Type.None; //현재 열려있는 팝업

        /*
            index[0] Material Item, Consumable Item
            index[1] Equipment Item (Non-Skill)
            index[2] Equipment Item (One-SKill)
            index[3] Equipment Item (Two-Skill)
            index[4] Accessory Item ()
        */

        public void Open_Popup_ConItem(Item_Consumable item)
        {
            ItemPop.EnablePopup(item, Frames[(int)item.Item_Grade]);
            popType = Popup_Type.Popup_NormalItem;
        }

        public void Open_Popup_MatItem(Item_Material item)
        {
            ItemPop.EnablePopup(item, Frames[(int)item.Item_Grade]);
            popType = Popup_Type.Popup_NormalItem;
        }

        public void Open_Popup_EquipItem(Item_Equipment item)
        {
            switch (item)
            {
                case Item_Bow bowItem: ItemPop_Bow.EnablePopup(bowItem, Frames[(int)item.Item_Grade]); popType = Popup_Type.Popup_BowItem;  break;
                case Item_Arrow arrowItem: break;
                default: break;
            }
        }

        public void Open_Popup_Accessory()
        {

        }

        #region BUTTON_METHOD

        public void Button_EquipBowItem() => ItemPop_Bow.Button_EquipAction();

        public void Button_ReleaseBowItem() => ItemPop_Bow.Button_ReleaseAction();

        public void Close_Popup(int popnum)
        {
            //itemAddress = null;

            switch (popnum)
            {
                case 0: ItemPop.DisablePop();       break;   //Con, Mat
                case 1:                             break;   //Equip Non-Skill
                case 2:                             break;   //Equip One-Skill
                case 3: ItemPop_Bow.DisablePopup(); break;   //New Bow Item
                default: break;
            }

            popType = Popup_Type.None;
            this.gameObject.SetActive(false);
        }

        public void Close_Popup()
        {
            switch (popType)
            {
                case Popup_Type.None:             CatLog.Log("현재 열려있는 팝업 타입 Enum 없음."); break;
                case Popup_Type.Popup_NormalItem: ItemPop.DisablePop(); break;
                case Popup_Type.Popup_BowItem:    ItemPop_Bow.DisablePopup();  break;
                case Popup_Type.Popup_ArrowItem: break;
                case Popup_Type.Popup_Accessory: break;
                default: break;
            }

            popType = Popup_Type.None;
            this.gameObject.SetActive(false);
        }

        #endregion
    }
}
