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
                    var tooltipObject = Instantiate(Resources.Load<GameObject>("ArrowDefence_UI/Tooltip_Object"));
                    _instance = tooltipObject.AddComponent<ItemDataInfoPop>();
                    tooltipObject.name = "UI_Tooltip_Object [Singleton]";
                }

                return _instance;
            }    
        }

        //Item Data Tooltip String [Get Item Asset Data]
        private string itemDescStr = null;
        private string itemNameStr = null;

        private RectTransform tooltipRect;
        private CanvasGroup canvasGroup;
        private bool isInitialize = false;

        private void Awake() => CatLog.Log("Tooltip System is Initializing");

        private void Start() => InitiallizeTooltip();

        public void Expose(Vector2 pos, Transform parentTr)
        {
            if (_instance.gameObject.activeSelf == false)
                _instance.gameObject.SetActive(true);

            //4. ItemDataSlot UI Script에서 ToolTip띄우는 로직 최적화 해주기

            StartCoroutine(ShowPopupCo(pos, parentTr));
        }

        private IEnumerator ShowPopupCo(Vector2 pos, Transform parentCanvasTr)
        {
            yield return new WaitUntil(() => this.isInitialize);

            //init Parent & Scale
            if (parentCanvasTr != transform.parent)
            {
                transform.SetParent(parentCanvasTr);
                transform.localScale = Vector3.one;
                transform.position   = parentCanvasTr.position;
            }

            #region DELETE
            //float initPosX = pos.x - tooltipRect.rect.width * 0.5f;
            //float initPosY = pos.y + tooltipRect.rect.height * 0.5f;
            //Vector2 initVec = new Vector2(initPosX, initPosY);
            //tooltipRect.localPosition = new Vector2(pos.x,  pos.y);
            //tooltipRect.anchoredPosition = new Vector2(pos.x, pos.y);
            //tooltipRect.anchoredPosition = new Vector2(pos.x, pos.y);

            //float screenCheckValue = pos.x * tooltipRect.rect.width;
            #endregion

            //Init Tooltip Position
            Vector2 InitPos = (pos.x + tooltipRect.rect.width > Screen.width) ?
                new Vector2(pos.x - tooltipRect.rect.width * 0.5f, pos.y + tooltipRect.rect.height * 0.5f) : 
                new Vector2(pos.x + tooltipRect.rect.width * 0.5f, pos.y + tooltipRect.rect.height * 0.5f);

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
