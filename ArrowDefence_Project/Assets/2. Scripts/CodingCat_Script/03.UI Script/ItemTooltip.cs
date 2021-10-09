namespace ActionCat.Games.UI
{
    using System.Collections;
    using UnityEngine;
    using CodingCat_Scripts;

    public class ItemTooltip : MonoBehaviour
    {
        static ItemTooltip _instance;

        public static ItemTooltip Instance
        {
            get
            {
                if(_instance == null)
                {
                    var obj = FindObjectOfType<ItemTooltip>();
                    if (obj != null) _instance = obj;
                    else
                    {
                        var tooltipObj = Instantiate(Resources.Load<GameObject>("ArrowDefence_UI/Tooltip_Object")).AddComponent<ItemTooltip>();
                        tooltipObj.name = "UI_Tooltip_Object [Singleton]";
                        _instance = tooltipObj;
                    }
                }

                return _instance;
            }
        }

        private TMPro.TextMeshProUGUI itemNameTMP = null;
        private TMPro.TextMeshProUGUI itemDescTMP = null;
        private RectTransform tooltipRect;
        private CanvasGroup canvasGroup;
        private GameObject target;
        private bool isInitialize = false;

        //Properties
        public bool IsOpenedTooltip { get => IsOpenedTooltip; private set { } }

        private void Awake()
        {
            var obj = FindObjectsOfType<ItemTooltip>();
            if(obj.Length != 1)
            {
                Destroy(gameObject);
                return;
            }

            CatLog.Log(StringColor.YELLOW ,"Tooltip System Initializing");
        }

        private void Start() => InitTooltip();

        private void InitTooltip()
        {
            tooltipRect = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0f; //처음 Open할때 튀는 현상 방지

            itemNameTMP = transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
            itemDescTMP = transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();

            isInitialize = true;
        }

        private IEnumerator ShowPopupCo(Vector2 pos, RectTransform parentCanvasRect, Camera targetCam,
                                        string itemName, string itemDesc)
        {
            yield return new WaitUntil(() => this.isInitialize);

            //init Parent & Scale
            if (parentCanvasRect != transform.parent)
            {
                transform.SetParent(parentCanvasRect);
                transform.localScale = Vector3.one;
                transform.position = parentCanvasRect.position;
            }

            //Init Item Name, Desc Text
            itemNameTMP.text = itemName; itemDescTMP.text = itemDesc;

            //Waiting One Frames Before the Calculate Position
            yield return null;

            //Init Tooltip Position
            Vector2 initPos = (pos.x + tooltipRect.rect.width > parentCanvasRect.rect.xMax) ?
                new Vector2(pos.x - tooltipRect.rect.width * 0.5f, pos.y + tooltipRect.rect.height * 0.5f) :
                new Vector2(pos.x + tooltipRect.rect.width * 0.5f, pos.y + tooltipRect.rect.height * 0.5f);

            //Calc Target Camera Width, Height
            //float camHeight = 2f * targetCam.orthographicSize;
            //float camWidth  = camHeight * targetCam.aspect;
            //
            //Vector2 initPos = (tooltipRect.anchoredPosition.x + tooltipRect.rect.width > parentCanvasRect.rect.width) ?
            //    new Vector2(pos.x - tooltipRect.rect.width * 0.5f, pos.y + tooltipRect.rect.height * 0.5f) :
            //    new Vector2(pos.x + tooltipRect.rect.width * 0.5f, pos.y + tooltipRect.rect.height * 0.5f);
            //
            //CatLog.Log($"X Position : {(pos.x + tooltipRect.rect.width).ToString()}" + '\n' +
            //           $"Parent rect Width : {parentCanvasRect.rect.width}" + '\n' +
            //           $"Cam Width : {camWidth.ToString()}");

            //Vector2 initPos = new Vector2(pos.x + tooltipRect.rect.width * 0.5f, pos.y + tooltipRect.rect.height * 0.5f); ;

            //anchoredPosition valueChanged : this Rect Transform anchor is Always LEFT-BOTTOM
            tooltipRect.anchoredPosition = initPos;

            //Expose Tooltip [Set Alpha]
            canvasGroup.alpha = 1f;
        }

        #region EXPOSE_HIDE_RELEASE

        /// <summary>
        /// If you have used it at least once in the scene, 
        /// you must call this method before the scene is changed to prevent it from being destroyed.
        /// </summary>
        public void ReleaseParent()
        {
            if(gameObject.activeSelf)
            {
                target = null;
                transform.SetParent(null);
                DontDestroyOnLoad(this.gameObject);

                gameObject.SetActive(false);
            }
        }

        public void Expose(Vector2 pos, RectTransform targetCanvasRect, string itemName, string itemDesc,
                           GameObject target, Camera targetCam)
        {
            if (_instance.gameObject.activeSelf == false)
                _instance.gameObject.SetActive(true);
            this.target = target;
            StartCoroutine(ShowPopupCo(pos, targetCanvasRect, targetCam, itemName, itemDesc));
        }

        public void Hide(GameObject target)
        {
            if (target == this.target)
                canvasGroup.alpha = 0f;
        }

        #endregion
    }
}
