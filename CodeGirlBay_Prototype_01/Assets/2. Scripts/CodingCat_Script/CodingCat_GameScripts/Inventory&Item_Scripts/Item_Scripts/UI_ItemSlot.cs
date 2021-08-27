namespace CodingCat_Games
{
    using CodingCat_Scripts;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class UI_ItemSlot : MonoBehaviour, IPointerClickHandler
    {
        public TextMeshProUGUI ItemStackTmp;
        public Image ItemImg;
        public AD_item ItemAddress;
        public Image ItemFrame;
        public Sprite[] Frames;

        public void SetItemSprite(Sprite sprite) => ItemImg.sprite = sprite;

        //Setup 함수 받아서 Slot 변수들 설정과 해당 Item 주소값 들고있게함
        public void Setup(AD_item address)
        {
            ItemAddress = address;

            //Set Item Sprite
            ItemImg.sprite = address.GetSprite;
            ItemImg.preserveAspect = true;  //PreserveAspect는 프리팹에서 고정해놓는게 좋지않을까 한번 정해놓은거 풀리지는 않는지 체크

            if (address.GetItemType != ITEMTYPE.ITEM_EQUIPMENT)
            {
                if (ItemStackTmp.gameObject.activeSelf == false)
                    ItemStackTmp.gameObject.SetActive(true);
                ItemStackTmp.text = address.GetAmount.ToString();
            }
            else ItemStackTmp.gameObject.SetActive(false);

            //Setting Item Frame For Item Grade
            this.ItemFrame.sprite = Frames[(int)address.GetGrade];
        }

        public void Clear()
        {
            //Data 정리하고 자체 Disable 처리
            ItemAddress = null;
            ItemStackTmp.text = "";
            ItemFrame.sprite = Frames[0];

            gameObject.SetActive(false);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData data)
        {
            //throw new System.NotImplementedException();

            if (ItemAddress.GetItemType == ITEMTYPE.ITEM_EQUIPMENT)
            {
                #region Item_FeedBack
                //switch (ItemAddress)
                //{
                //    case Item_Bow itemBow:
                //        CatLog.Log($"Item Name : {itemBow.ItemName} \n" +
                //                   $"Item Amount : {itemBow.Amount.ToString()} \n" +
                //                   $"Item Type : {itemBow.EquipType.ToString()} \n" + 
                //                   $"Item Skill : {itemBow.GetBowSkill().ToString()}"); break;
                //    case Item_Arrow itemArrow:
                //        CatLog.Log($"Item Name   : {itemArrow.ItemName}  \n  " +
                //                   $"Item Amount : {itemArrow.Amount} \n  " +
                //                   $"Equip Type  : {itemArrow.EquipType.ToString()} \n  " +
                //                   $"ITem Type   : {itemArrow.ItemType.ToString()}"); break;
                //    default: break;
                //} //피드백 필요하면 열어서 사용하기
                //Item_Bow itemBow = (Item_Bow)ItemAddress; 이거랑 똑같은 말인가?
                #endregion

                MainSceneRoute.OpenItemInfo(ItemAddress);
            }
            else
            {
                //CatLog.Log($"Item Name   : {ItemAddress.GetName}  \n  " +
                //           $"Item Amount : {ItemAddress.GetAmount} \n  " +
                //           $"ITem Type   : {ItemAddress.GetItemType.ToString()} ");

                MainSceneRoute.OpenItemInfo(ItemAddress);
            }
        }

        #region SLOT_SETTING_METHOD

        #endregion
    }
}
