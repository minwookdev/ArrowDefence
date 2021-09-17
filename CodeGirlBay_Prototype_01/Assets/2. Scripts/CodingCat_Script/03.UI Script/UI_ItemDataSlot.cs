namespace CodingCat_Games
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class UI_ItemDataSlot : MonoBehaviour, IPointerClickHandler
    {
        [Header("ITEM DATA SLOT")]
        public TextMeshProUGUI ItemStackTmp;
        public Image ItemImg;
        public ItemData ItemDataAddress;
        public Image ItemFrame;
        public Sprite[] Frames;

        public void Setup(ItemData address, int visibleStack)
        {
            //address 여기서 필요한가..?
            ItemDataAddress = address;

            ItemImg.sprite = address.Item_Sprite;

            if (address.Item_Type != ITEMTYPE.ITEM_EQUIPMENT)
            {
                if (ItemStackTmp.gameObject.activeSelf == false)
                    ItemStackTmp.gameObject.SetActive(true);
                ItemStackTmp.text = visibleStack.ToString();
            }
            else ItemStackTmp.gameObject.SetActive(false);

            //Set Item Frame according to Item Grade
            ItemFrame.sprite = Frames[(int)address.Item_Grade];
        }

        public void Clear()
        {
            ItemDataAddress = null;
            ItemStackTmp.text = "";
            ItemFrame.sprite = Frames[0];

            this.gameObject.SetActive(false);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            //간단한 설명 툴팁띄우는 정도로 구현 -> 현재는 Battle Scene 정산 페이지 에서만 이 스크립트가 작동함
            return;
        }
    }
}
