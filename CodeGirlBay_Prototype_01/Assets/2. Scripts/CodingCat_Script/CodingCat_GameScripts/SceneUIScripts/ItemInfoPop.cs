namespace CodingCat_Games
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using CodingCat_Scripts;
    using CodingCat_Games.Data;

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
                var itemInfo = item.GetItem();

                Text_ItemType.text = "Consumable";
                Text_ItemName.text = item.GetName;
                Text_ItemDesc.text = item.GetDesc;
                Text_ItemCount.text = item.GetAmount.ToString();
                Image_Item.sprite = item.GetSprite;
                Image_Frame.sprite = frame;
                itemAddress = item;

                Popup_Object.SetActive(true);
            }

            public void EnablePopup(Item_Material item, Sprite frame)
            {
                Text_ItemType.text = "Consumable";
                Text_ItemName.text = item.GetName;
                Text_ItemDesc.text = item.GetDesc;
                Text_ItemCount.text = item.GetAmount.ToString();
                Image_Item.sprite = item.GetSprite;
                Image_Frame.sprite = frame;
                itemAddress = item;

                Popup_Object.SetActive(true);
            }

            public void DisablePop()
            {
                Text_ItemType.text = "";
                Text_ItemName.text = "";
                Text_ItemDesc.text = "";
                Text_ItemCount.text = "";
                itemAddress = null;

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
                Text_ItemName.text = item.GetName;
                Image_Item.sprite = item.GetSprite;
                Image_Frame.sprite = sprite;
                itemAddress = item;

                for (int i = 0; i < item.GetBowSkills().Length; i++)
                {
                    if (item.GetBowSkills()[i] != null) Object_SkillSlots[i].SetActive(true);
                    else Object_SkillSlots[i].SetActive(false);
                }

                //if (CCPlayerData.equipments.GetBowItem() != itemAddress) SwitchButtons(false);
                //else                                                     SwitchButtons(true);

                //플레이어의 현재 장비아이템이 선택한 장비아이템인지 비교해서 버튼 띄워줌
                if (ReferenceEquals(CCPlayerData.equipments.GetBowItem(), itemAddress)) SwitchButtons(true);
                else                                                                    SwitchButtons(false);

                PopObject.SetActive(true);
            }

            public void DisablePopup()
            {
                Text_ItemType.text = "";
                Text_ItemName.text = "";
                itemAddress = null;

                foreach (var item in Object_SkillSlots)
                {
                    if (item.activeSelf) item.SetActive(false);
                }

                PopObject.SetActive(false);
            }

            public Item_Bow GetItemAddress()
            {
                if (itemAddress != null)
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

            public void Button_EquipAction()
            {
                CCPlayerData.equipments.Equip_BowItem(itemAddress);
            }

            public void Button_ReleaseAction()
            {
                CCPlayerData.equipments.Release_BowItem();
            }

            //장착버튼 교체
            private void SwitchButtons(bool isEquip)
            {
                if (isEquip) //장착중인 아이템이면 해제버튼을 활성화
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

        [Serializable]
        public class ItemPop_Equip_Arrow
        {
            public GameObject PopObject;
            public Image Image_Item;
            public Image Image_Frame;
            public TextMeshProUGUI Text_ItemName;
            public TextMeshProUGUI Text_ItemDesc;
            public Button Button_Equip;
            public Button Button_Release;

            public GameObject[] Object_Skill_Slot;
            private Item_Arrow itemAddress;

            public void EnablePopup(Item_Arrow address, Sprite frameSprite)
            {
                itemAddress = address;

                Text_ItemName.text = address.GetName;
                Image_Item.sprite  = address.GetSprite;
                Image_Frame.sprite = frameSprite;

                //Skil Slot Enable Logic
                foreach (var item in Object_Skill_Slot)
                {
                    item.SetActive(false);
                }

                //Enable / Disable Equip Button Logic 현재 들고있는 Item Reference랑 비교해서 Equip / Release Button의 Enable 결정
                bool isEquipped = (ReferenceEquals(CCPlayerData.equipments.GetMainArrow(), itemAddress) || 
                                   ReferenceEquals(CCPlayerData.equipments.GetSubArrow(),  itemAddress)) ? true : false;

                SwitchButton(isEquipped);

                PopObject.SetActive(true);
            }

            public void DisablePopup()
            {
                Text_ItemName.text = "";
                itemAddress = null;

                foreach (var item in Object_Skill_Slot)
                {
                    if (item.activeSelf == true)
                        item.SetActive(false);
                }

                PopObject.SetActive(false);
            }

            public void Button_EquipAction()
            {
                //Open Equip Slot Choose Panel
                UI_Equipments.Instance.OpenChoosePanel(SlotChoosePop.SLOTPANELTYPE.SLOT_ARROW, itemAddress);
            }

            public void Button_ReleaseAction()
            {
                //Release 버튼이 떳다는건 일단 Main, Sub Arrow 둘중에 하나 끼고있는거니까 둘간에 비교해서 Release 해주면 된다
                if (ReferenceEquals(CCPlayerData.equipments.GetMainArrow(), itemAddress))
                    CCPlayerData.equipments.Release_ArrowItem();
                else CCPlayerData.equipments.Release_SubArrow();
            }

            private void SwitchButton(bool isEquip)
            {
                if (isEquip)
                {
                    Button_Equip.gameObject.SetActive(false);
                    Button_Release.gameObject.SetActive(true);
                }
                else
                {
                    Button_Equip.gameObject.SetActive(true);
                    Button_Release.gameObject.SetActive(false);
                }
            }
        }

        [Serializable]
        public class ItemPop_Equip_Accessory
        {
            public GameObject PopObject;
            public Image Image_Item;
            public Image Image_Frame;
            public TextMeshProUGUI Text_ItemName;
            public TextMeshProUGUI Text_ItemDesc;
            public Button Button_Equip;
            public Button Button_Release;

            public GameObject[] Object_Skill_Slot;

            private Item_Accessory itemAddress;
            private byte accessoryIdx = 0;

            public void EnablePopup(Item_Accessory address, Sprite frameSprite)
            {
                itemAddress = address;

                Text_ItemName.text = address.GetName;
                Image_Item.sprite = address.GetSprite;
                Image_Frame.sprite = frameSprite;

                //Skill Slot Enable Logic
                foreach (var item in Object_Skill_Slot)
                {
                    item.SetActive(false);
                }

                //Enable / Disable Equip Button Logic
                foreach (var item in CCPlayerData.equipments.GetAccessories())
                {
                    if (ReferenceEquals(item, itemAddress))
                    {
                        SwitchButton(true);
                        break;
                    }
                    else SwitchButton(false);

                    accessoryIdx++;
                }

                PopObject.SetActive(true);
            }

            public void DisablePopup()
            {
                Text_ItemName.text = "";
                itemAddress = null;
                accessoryIdx = 0;

                foreach (var item in Object_Skill_Slot)
                {
                    if (item.activeSelf == true)
                        item.SetActive(false);
                }

                PopObject.SetActive(false);
            }

            public void Button_EquipAction()
            {
                //Open Equipment Slot Choose Panel
                UI_Equipments.Instance.OpenChoosePanel(SlotChoosePop.SLOTPANELTYPE.SLOT_ACCESSORY, itemAddress);

                //CCPlayerData.equipments.Equip_AccessoryItem(itemAddress);
            }

            public void Button_ReleaseAction()
            {
                //새로운 Release 메서드를 사용하도록 수정
                //릴리즈 버튼이 떳다는건 지금 악세칸에 있다는거임.
                //CCPlayerData.equipments.Release_AccessoryItem();

                CCPlayerData.equipments.Release_Accessory(accessoryIdx);
            }

            private void SwitchButton(bool isEquip)
            {
                if(isEquip)
                {
                    Button_Equip.gameObject.SetActive(false);
                    Button_Release.gameObject.SetActive(true);
                }
                else
                {
                    Button_Equip.gameObject.SetActive(true);
                    Button_Release.gameObject.SetActive(false);
                }
            }
        }

        [Header("Item Grade Frames")]
        public Sprite[] Frames;


        [Header("Item Popup Type")]
        [Space(10)]
        public ItemPop_Normal    ItemPop;               //Material, Consumable Item Popup
        public ItemPop_Equip_Bow ItemPop_Bow;           //Equipment Bow Item Popup
        public ItemPop_Equip_Arrow ItemPop_Arrow;       //Equipment Arrow Item Popup
        public ItemPop_Equip_Accessory ItemPop_Access;  //Equipment Accessory Item Popup
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
            ItemPop.EnablePopup(item, Frames[(int)item.GetGrade]);
            popType = Popup_Type.Popup_NormalItem;
        }

        public void Open_Popup_MatItem(Item_Material item)
        {
            ItemPop.EnablePopup(item, Frames[(int)item.GetGrade]);
            popType = Popup_Type.Popup_NormalItem;
        }

        public void Open_Popup_EquipItem(Item_Equipment item)
        {
            switch (item)
            {
                case Item_Bow bowItem:          ItemPop_Bow.EnablePopup(bowItem, Frames[(int)item.GetGrade]); popType = Popup_Type.Popup_BowItem;  break;
                case Item_Arrow arrowItem:      ItemPop_Arrow.EnablePopup(arrowItem, Frames[(int)item.GetGrade]); popType = Popup_Type.Popup_ArrowItem;  break;
                case Item_Accessory accessItem: ItemPop_Access.EnablePopup(accessItem, Frames[(int)item.GetGrade]); popType = Popup_Type.Popup_Accessory; break;
                default: break;
            }
        }

        #region BUTTON_METHOD

        public void Button_EquipBowItem()
        {
            ItemPop_Bow.Button_EquipAction();
            UI_Equipments.Instance.UpdateEquipUI();
            UI_Inventory.InvenUpdate();
            
            Close_Popup(3);
        }

        public void Button_ReleaseBowItem()
        {
            ItemPop_Bow.Button_ReleaseAction();
            UI_Equipments.Instance.UpdateEquipUI();
            UI_Inventory.InvenUpdate();

            Close_Popup(3);
        }

        public void Button_EquipArrowItem()
        {
            ItemPop_Arrow.Button_EquipAction();
            UI_Equipments.Instance.UpdateEquipUI();
            UI_Inventory.InvenUpdate();


            Close_Popup(1);
        }

        public void Button_ReleaseArrowItem()
        {
            ItemPop_Arrow.Button_ReleaseAction();
            UI_Equipments.Instance.UpdateEquipUI();
            UI_Inventory.InvenUpdate();

            Close_Popup(1);
        }
        
        public void Button_EquipAccessItem()
        {
            ItemPop_Access.Button_EquipAction();
            UI_Equipments.Instance.UpdateEquipUI();
            UI_Inventory.InvenUpdate();

            Close_Popup(2);
        }

        public void Button_ReleaseAccessItem()
        {
            ItemPop_Access.Button_ReleaseAction();
            UI_Equipments.Instance.UpdateEquipUI();
            UI_Inventory.InvenUpdate();

            Close_Popup(2);
        }

        public void Close_Popup(int popnum)
        {
            //itemAddress = null;

            switch (popnum)
            {
                case 0: ItemPop.DisablePop();           break;   //Con, Mat
                case 1: ItemPop_Arrow.DisablePopup();   break;   //Equip Non-Skill
                case 2: ItemPop_Access.DisablePopup();  break;   //Equip One-Skill
                case 3: ItemPop_Bow.DisablePopup();     break;   //New Bow Item
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
                case Popup_Type.Popup_NormalItem: ItemPop.DisablePop();                           break;
                case Popup_Type.Popup_BowItem:    ItemPop_Bow.DisablePopup();                     break;
                case Popup_Type.Popup_ArrowItem:  ItemPop_Arrow.DisablePopup();                   break;
                case Popup_Type.Popup_Accessory:  ItemPop_Access.DisablePopup();                  break;
                default:                                                                          break;
            }

            popType = Popup_Type.None;
            this.gameObject.SetActive(false);
        }

        #endregion
    }
}
