namespace ActionCat {
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using ActionCat.Audio;

    public class UI_ItemSlot : MonoBehaviour, IPointerClickHandler {
        [Header("COMPONENT")]
        public TextMeshProUGUI ItemStackTmp;
        public Image ItemImg;
        public AD_item ItemAddress;
        public Image ItemFrame;
        public Sprite[] Frames;

        [Header("OPTION")]
        [SerializeField] bool ignoreClick = false;

        [Header("SOUND")]
        [SerializeField] bool isSoundCompatiblity = false;
        [SerializeField] CHANNELTYPE channelKey   = CHANNELTYPE.NONE;
        [SerializeField] ACSound channel          = null;

        public void Start() {
            //Get Channel
            channel = (isSoundCompatiblity && SoundManager.Instance.TryGetChannel2Dic(channelKey, out ACSound result)) ? result : channel;
            if (isSoundCompatiblity && channel == null) {
                CatLog.ELog("Channel Not Found !");
            }
        }

        public void SetItemSprite(Sprite sprite) => ItemImg.sprite = sprite;

        //Setup 함수 받아서 Slot 변수들 설정과 해당 Item 주소값 들고있게함
        public void EnableSlot(AD_item address) {
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

        public void EnableSlot(ItemData item, int notationNumber = 0) {
            ItemImg.sprite = item.Item_Sprite;
            ItemImg.preserveAspect = true;
            ItemFrame.sprite = Frames[(int)item.Item_Grade];

            if(notationNumber <= 0) {
                ItemStackTmp.gameObject.SetActive(false);
            }
            else {
                ItemStackTmp.text = notationNumber.ToString();
                ItemStackTmp.gameObject.SetActive(true);
            }

            gameObject.SetActive(true);
        }

        public void DisableSlot() {
            //Data 정리하고 자체 Disable 처리
            ItemAddress = null;
            ItemStackTmp.text = "";
            ItemFrame.sprite = Frames[0];

            gameObject.SetActive(false);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData data) {
            if (ignoreClick) {
                return;
            }

            if (isSoundCompatiblity) {
                channel.PlayOneShot();
            }
            MainSceneRoute.OpenInfo_InventoryItem(ItemAddress);
        }
    }
}
