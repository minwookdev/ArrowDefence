namespace CodingCat_Games
{
    using System.Collections;
    using UnityEngine;
    using CodingCat_Scripts;

    public class ItemDataInfoPop : MonoBehaviour
    {
        static ItemDataInfoPop _instance;
        public static ItemDataInfoPop Instance
        {
            get
            {
                if(_instance == null)
                {
                    var obj = FindObjectOfType<ItemDataInfoPop>();
                    if (obj != null) _instance = obj;
                    else
                    {
                        var tooltipSingleton = Instantiate(Resources.Load<GameObject>("ArrowDefence_UI/Tooltip_Object")).AddComponent<ItemDataInfoPop>();
                        tooltipSingleton.name = "UI_Tooltip_Object [Singleton]";
                        _instance = tooltipSingleton;
                    }

                }

                return _instance;
            }    
        }


        private TMPro.TextMeshProUGUI itemNameTMP = null;
        private TMPro.TextMeshProUGUI itemDescTMP = null;
        private RectTransform tooltipRect;
        private CanvasGroup canvasGroup;
        private bool isInitialize = false;

        private void Awake()
        {
            var obj = FindObjectsOfType<ItemDataInfoPop>();
            if(obj.Length != 1)
            {
                Destroy(gameObject);
                return;
            }

            CatLog.Log(StringColor.YELLOW, "Tooltip System is Initializing");
            //DontDestroyOnLoad(gameObject); //Scene 변경될 때, Release Parent 메서드를 받아서 처리될 수 있도록해줌
        }

        private void Start() => InitiallizeTooltip();

        public void Expose(Vector2 pos, Canvas targetCanvas, string itemName, string itemDesc)
        {
            if (_instance.gameObject.activeSelf == false)
                _instance.gameObject.SetActive(true);

            //4. ItemDataSlot UI Script에서 ToolTip띄우는 로직 최적화 해주기

            StartCoroutine(ShowPopupCo(pos, targetCanvas, itemName, itemDesc));
        }

        private IEnumerator ShowPopupCo(Vector2 pos, Canvas parentCanvas, string itemName, string itemDesc)
        {
            yield return new WaitUntil(() => this.isInitialize);

            //init Parent & Scale
            if (parentCanvas != transform.parent)
            {
                transform.SetParent(parentCanvas.transform);
                transform.localScale = Vector3.one;
                transform.position   = parentCanvas.transform.position;
            }

            #region OLD
            //float initPosX = pos.x - tooltipRect.rect.width * 0.5f;
            //float initPosY = pos.y + tooltipRect.rect.height * 0.5f;
            //Vector2 initVec = new Vector2(initPosX, initPosY);
            //tooltipRect.localPosition = new Vector2(pos.x,  pos.y);
            //tooltipRect.anchoredPosition = new Vector2(pos.x, pos.y);
            //tooltipRect.anchoredPosition = new Vector2(pos.x, pos.y);

            //float screenCheckValue = pos.x * tooltipRect.rect.width;
            #endregion

            //Init Item Name, Desc Text
            itemNameTMP.text = itemName; itemDescTMP.text = itemDesc;

            //Init Tooltip Position
            Vector2 InitPos = (pos.x + tooltipRect.rect.width > Screen.width) ?
                new Vector2(pos.x - tooltipRect.rect.width * 0.5f, pos.y + tooltipRect.rect.height * 0.5f) : 
                new Vector2(pos.x + tooltipRect.rect.width * 0.5f, pos.y + tooltipRect.rect.height * 0.5f);

            //anchoredPosition valueChanged : this Rect Transform anchor is Always LEFT-BOTTOM
            tooltipRect.anchoredPosition = InitPos;

            //Expose Tooltip [Set Alpha]
            canvasGroup.alpha = 1f;
        }

        /// <summary>
        /// Hide Tooltip [Set Alpha]
        /// </summary>
        public void Hide() => canvasGroup.alpha = 0f;

        private void InitiallizeTooltip()
        {
            tooltipRect = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;

            itemNameTMP = transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
            itemDescTMP = transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();

            isInitialize = true;
        }

        /// <summary>
        /// Prevents Destroy by executing the Parent Release method before the scene is changed.
        /// </summary>
        public void ReleaseParent()
        {
            if (_instance.gameObject.activeSelf == true)
                _instance.gameObject.SetActive(false);

            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
    }
}
