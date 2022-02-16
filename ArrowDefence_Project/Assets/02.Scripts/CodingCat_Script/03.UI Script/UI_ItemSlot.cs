namespace ActionCat
{
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
        public void Setup(AD_item address) {
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

        void IPointerClickHandler.OnPointerClick(PointerEventData data) {
            MainSceneRoute.OpenItemInfo(ItemAddress);
            #region FEEDBACKINLOG

            //if(ItemAddress.GetItemType != ITEMTYPE.ITEM_EQUIPMENT)
            //{
            //    string itemInfoStr = "Item Info \n" +
            //        $"Name : {ItemAddress.GetName}" +
            //        $"Desc : {}";
            //}
            //else
            //{
            //    if(ItemAddress is Item_Bow)
            //    {
            //        Item_Bow item = (Item_Bow)ItemAddress;
            //    }
            //    else if (ItemAddress is Item_Arrow)
            //    {
            //        Item_Arrow item = (Item_Arrow)ItemAddress;
            //    }
            //    else if(ItemAddress is Item_Accessory)
            //    {
            //        Item_Accessory item = (Item_Accessory)ItemAddress;
            //    }
            //}

            #endregion
        }

        #region SLOT_SETTING_METHOD

        #endregion
    }
}
