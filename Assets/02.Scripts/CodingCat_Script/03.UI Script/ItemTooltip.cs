﻿namespace ActionCat.Games.UI {
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class ItemTooltip : MonoBehaviour {
        #region v1
        //public static ItemTooltip Instance {
        //    get {
        //        if(_instance == null) {
        //            var obj = FindObjectOfType<ItemTooltip>();
        //            if (obj != null) _instance = obj;
        //            else {
        //                var tooltipObj = Instantiate(Resources.Load<GameObject>("ArrowDefence_UI/Tooltip_v02_Pref")).AddComponent<ItemTooltip>();
        //                tooltipObj.name = "Tooltip_System";
        //                _instance = tooltipObj;
        //            }
        //        }
        //
        //        return _instance;
        //    }
        //}
        #endregion

        static ItemTooltip _instance;

        public static ItemTooltip Inst {
            get {
                if (_instance == null) { //Find or Create Tooltip Object.
                    var obj = FindObjectOfType<ItemTooltip>();
                    if (obj != null) _instance = obj;
                    else {
                        var newtooltip = Instantiate(Resources.Load<GameObject>("ArrowDefence_UI/Tooltip_v3_Pref")).GetComponent<ItemTooltip>();
                        newtooltip.gameObject.name = "[TOOLTIP_SYS]";
                        _instance = newtooltip;
                    }
                }

                return _instance;
            }
        }

        [Header("COMPONENT")]
        [SerializeField] RectTransform parentRect = null;
        [SerializeField] RectTransform tooltipRect = null;
        [SerializeField] CanvasGroup canvasGroup = null;
        [SerializeField] TextMeshProUGUI tmpName = null;
        [SerializeField] TextMeshProUGUI tmpDesc = null;

        [Header("ITEM SLOT PROPERTY")]
        [SerializeField] GameObject itemSlot = null;
        [SerializeField] RectTransform itemSlotRect = null;
        [SerializeField] Image itemSlotFrame = null;
        [SerializeField] Image itemSlotIcon = null;
        [SerializeField] TextMeshProUGUI itemAmountTmp = null;
        [SerializeField] GameObject[] rewardTags = null;

        [Header("MONSTER SLOT PROPERTY")]
        [SerializeField] GameObject monsterSlot = null;
        [SerializeField] Image monsterSlotFrame = null;

        [Header("OPTION")]
        [SerializeField] float tooltipSpacing = 5f;

        //Properties
        public bool IsOpenedTooltip { get => IsOpenedTooltip; private set { } }

        //Fields
        private bool isInitialize = false;
        WaitUntil coroutineWaitUntil = null;
        WaitForEndOfFrame coroutineWaitEndFrame = null;

        private void Awake() {
            var obj = FindObjectsOfType<ItemTooltip>();
            if (obj.Length != 1) {
                Destroy(gameObject);
                return;
            }

            CatLog.Log(StringColor.YELLOW, "Tooltip System Initialize.");
        }

        private void Start() {
            Init(out isInitialize);
        }

        void Init(out bool result) {
            //Check Tooltip Component
            if (tooltipRect == null || canvasGroup == null || tmpName == null || tmpDesc == null) {
                CatLog.ELog("TOOLTIP ERROR : Not Caching Component."); 
                result = false;
            }
            //Check temp ItemSlot Component
            if (itemSlot == null || itemSlotFrame == null || itemSlotIcon == null || itemAmountTmp == null) {
                CatLog.ELog("TOOLTIP ERROR : Check the Temp ItemSlot Component."); 
                result = false;
            }
            //Check temp MonsterSlot Component
            if(monsterSlot == null || monsterSlotFrame == null) {
                CatLog.ELog("TOOLTIP ERROR : Check the Temp MonsterSlot Component.");
                result = false;
            }

            //Canvas Alpha zero, prevent bounce
            canvasGroup.alpha = 0f;

            //Init Coroutine Variables
            coroutineWaitUntil    = new WaitUntil(() => isInitialize == true);
            coroutineWaitEndFrame = new WaitForEndOfFrame();

            result = true;
        }

        #region EXPOSE_HIDE_RELEASE

        /// <summary>
        /// Expose Monster Data Tooltip
        /// </summary>
        public void Expose() {
            StartCoroutine(EnableMonsterTooltip());
        }

        /// <summary>
        /// Expose Item Data Tooltip
        /// </summary>
        /// <param name="position"></param>
        /// <param name="targetCanvas"></param>
        /// <param name="data"></param>
        /// <param name="frame"></param>
        /// <param name="amount"></param>
        public void Expose(Vector2 position, RectTransform targetCanvas, ItemData data, Sprite frame, string amount, bool isReward = false) {
            //Activate GameObject [Stay Alpha 0]
            if (gameObject.activeSelf == false) {
                gameObject.SetActive(true);
            }
            //Enable Tooltip Coroutine Start
            StartCoroutine(EnableItemTooltip(position, targetCanvas, data, frame, amount, isReward));
        }

        /// <summary>
        /// Item Data Tooltip Expose Coroutine
        /// </summary>
        /// <param name="position"></param>
        /// <param name="targetCanvas"></param>
        /// <param name="data"></param>
        /// <param name="frame"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        IEnumerator EnableItemTooltip(Vector2 position, RectTransform targetCanvas, ItemData data, Sprite frame, string amount, bool isOnRewardTag) {
            yield return coroutineWaitUntil; //Wait ItemTooltip Initialized..

            //Change parent to target Canvas if not Changed
            if (parentRect.parent != targetCanvas) {
                parentRect.SetParent(targetCanvas, false);
                parentRect.RectResizer(Vector2.zero, Vector2.zero, Vector3.one);
            }

            //Init tempSlot values
            itemSlotFrame.sprite = frame;
            itemSlotIcon.sprite  = data.Item_Sprite;
            itemAmountTmp.text   = amount;

            //init tooltip values
            tmpName.text = data.NameByTerms;
            tmpDesc.text = data.DescByTerms;

            //Set temp ItemSlot Position.
            itemSlotRect.position = position;

            rewardTags.Foreach((tag) => {
                tag.SetActive(isOnRewardTag);
            });

            //<Wait until Tooltip Rect Size Changed> <this line is removeable>
            yield return coroutineWaitEndFrame;

            //Set Tooltip Position + Correction Y Position
            tooltipRect.position = position;
            Vector2 correction = tooltipRect.anchoredPosition;
            correction.y += tooltipRect.rect.height * 0.5f + tooltipSpacing;
            tooltipRect.anchoredPosition = correction;

            //[Ready to tooltip and tempSlot] [VISIBLE]
            canvasGroup.alpha = 1f;
        }

        /// <summary>
        /// Monster Data Tooltip Expose Tooltip
        /// </summary>
        /// <returns></returns>
        IEnumerator EnableMonsterTooltip() {
            yield return null;
        }

        /// <summary>
        /// If you have used it at least once in the scene, 
        /// you must call this method before the scene is changed to prevent it from being destroyed.
        /// </summary>
        public void ReleaseParent() {
            //disable Tooltip gameObject for Prevention Bounced
            if (gameObject.activeSelf == true) {
                gameObject.SetActive(false);
            }
            //<if the parent object exists, DontDestroyOnLoad cannot be executed.>
            if (transform.parent != null) { //Release Parent.
                transform.SetParent(null);
            }
            //Set DontDestroy, Prevention Create new Tooltip Object.
            DontDestroyOnLoad(gameObject);
        }

        #endregion

        #region BUTTON_EVENT

        public void Hide() {
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }

        #endregion

        #region v1

        //public void Expose(Vector2 pos, RectTransform targetCanvasRect, string itemName, string itemDesc,
        //           GameObject target, Camera targetCam) {
        //    if (_instance.gameObject.activeSelf == false)
        //        _instance.gameObject.SetActive(true);
        //    this.target = target;
        //    StartCoroutine(ShowPopupCo(pos, targetCanvasRect, targetCam, itemName, itemDesc));
        //}
        //
        //public void Hide(GameObject target) {
        //    if (target == this.target)
        //        canvasGroup.alpha = 0f;
        //}
        //
        //private IEnumerator ShowPopupCo(Vector2 pos, RectTransform parentCanvasRect, Camera targetCam,
        //                        string itemName, string itemDesc) {
        //    yield return new WaitUntil(() => this.isInitialize);
        //
        //    //init Parent & Scale
        //    if (parentCanvasRect != transform.parent)
        //    {
        //        transform.SetParent(parentCanvasRect);
        //        transform.localScale = Vector3.one;
        //        transform.position = parentCanvasRect.position;
        //    }
        //
        //    //Init Item Name, Desc Text
        //    itemNameTMP.text = itemName; itemDescTMP.text = itemDesc;
        //
        //    //Waiting One Frames Before the Calculate Position
        //    yield return null;
        //
        //    //Init Tooltip Position
        //    Vector2 initPos = (pos.x + tooltipRect.rect.width > parentCanvasRect.rect.xMax) ?
        //        new Vector2(pos.x - tooltipRect.rect.width * 0.5f, pos.y + tooltipRect.rect.height * 0.5f) :
        //        new Vector2(pos.x + tooltipRect.rect.width * 0.5f, pos.y + tooltipRect.rect.height * 0.5f);
        //
        //    //Calc Target Camera Width, Height
        //    //float camHeight = 2f * targetCam.orthographicSize;
        //    //float camWidth  = camHeight * targetCam.aspect;
        //    //
        //    //Vector2 initPos = (tooltipRect.anchoredPosition.x + tooltipRect.rect.width > parentCanvasRect.rect.width) ?
        //    //    new Vector2(pos.x - tooltipRect.rect.width * 0.5f, pos.y + tooltipRect.rect.height * 0.5f) :
        //    //    new Vector2(pos.x + tooltipRect.rect.width * 0.5f, pos.y + tooltipRect.rect.height * 0.5f);
        //    //
        //    //CatLog.Log($"X Position : {(pos.x + tooltipRect.rect.width).ToString()}" + '\n' +
        //    //           $"Parent rect Width : {parentCanvasRect.rect.width}" + '\n' +
        //    //           $"Cam Width : {camWidth.ToString()}");
        //
        //    //Vector2 initPos = new Vector2(pos.x + tooltipRect.rect.width * 0.5f, pos.y + tooltipRect.rect.height * 0.5f); ;
        //
        //    //anchoredPosition valueChanged : this Rect Transform anchor is Always LEFT-BOTTOM
        //    tooltipRect.anchoredPosition = initPos;
        //
        //    //Expose Tooltip [Set Alpha]
        //    canvasGroup.alpha = 1f;
        //}

        #endregion
    }
}
