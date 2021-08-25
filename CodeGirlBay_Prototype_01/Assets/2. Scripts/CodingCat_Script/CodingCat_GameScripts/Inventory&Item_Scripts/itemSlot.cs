//namespace CodingCat_Games
//{
//    using CodingCat_Scripts;
//    using TMPro;
//    using UnityEngine;
//    using UnityEngine.EventSystems;
//    using UnityEngine.UI;
//
//    public class itemSlot : MonoBehaviour, IPointerClickHandler
//    {
//        public TextMeshProUGUI ItemStackTmp;
//        public Image ItemImg;
//        public AD_item ItemAddress;
//
//        public void SetItemSprite(Sprite sprite) => ItemImg.sprite = sprite;
//
//        //Setup 함수 받아서 Slot 변수들 설정과 해당 Item 주소값 들고있게함
//        public void Setup(AD_item address)
//        {
//            ItemAddress = address;
//
//            //Set Item Sprite
//            ItemImg.sprite = address.itemSprite;
//            ItemImg.preserveAspect = true;
//
//            //Set Item StackCount :: Equipment Item Not Active Amount Text
//            if (address.Amount <= 1) ItemStackTmp.gameObject.SetActive(false);
//            else
//            {
//                if (false == ItemStackTmp.gameObject.activeSelf)
//                    ItemStackTmp.gameObject.SetActive(true);
//                ItemStackTmp.text = address.Amount.ToString();
//            }
//        }
//
//        void IPointerClickHandler.OnPointerClick(PointerEventData data)
//        {
//            //throw new System.NotImplementedException();
//
//
//            if(ItemAddress.ItemType == Enum_Itemtype.ITEM_EQUIPMENT)
//            {
//                Item_Equipment equipItem = (Item_Equipment)ItemAddress;
//                CatLog.Log($"Item Name   : {equipItem.Title}  \n  " +
//                           $"Item Amount : {equipItem.Amount} \n  " +
//                           $"ITem Type   : {equipItem.ItemType.ToString()} \n" +
//                           $"Skill       : {equipItem.BowSkill.ToString()}");
//
//            }
//            else
//            {
//                CatLog.Log($"Item Name   : {ItemAddress.Title}  \n  " +
//                           $"Item Amount : {ItemAddress.Amount} \n  " +
//                           $"ITem Type   : {ItemAddress.ItemType.ToString()} ");
//            }
//        }
//    }
//}
