namespace CodingCat_Games.Testing
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using DG.Tweening;
    using CodingCat_Scripts;

    public class DevTestPanel : MonoBehaviour
    {
        [Header("Only DevMode UI")]
        public Image OpenButton;
        public Image DevPanel;

        [Header("Player Data")]
        public AD_PlayerData playerData;

        private float endPosX;
        private float startPosX;
        private bool isOpen = false;
        private WaitForSeconds awaitTime = new WaitForSeconds(.5f);

        private void OnEnable()
        {
            //startPosX = TestPanel.localPosition.x;
            //endPosX = startPosX + TestPanel.rect.width;
            //CatLog.Log($"Panel Pos X : {TestPanel.localPosition.x}");
            //CatLog.Log($"Rect Width : {TestPanel.rect.width}");
            //CatLog.Log($"endPosX : {endPosX.ToString()}");

            RectTransform panelRect = DevPanel.GetComponent<RectTransform>();
            startPosX = panelRect.localPosition.x;
            endPosX = startPosX + panelRect.rect.width;
        }

        IEnumerator SetPlayerItems()
        {
            yield return awaitTime;
            playerData.SetTestItems();
            CatLog.Log("ADD Item Button Active");
        }

        #region BUTTON_METHOD

        public void Button_PanelOpen()
        {
            if (isOpen == false)
            {
                DevPanel.transform.DOLocalMoveX(endPosX, 1f, false)
                    .OnStart(() => {
                    OpenButton.raycastTarget = false;
                    DevPanel.raycastTarget   = false;
                }).OnComplete(() => {
                    OpenButton.raycastTarget = true;
                    DevPanel.raycastTarget   = true;
                    isOpen = true;
                });
            }
            else
            {
                DevPanel.transform.DOLocalMoveX(startPosX, 1f, false)
                    .OnStart(() => {
                    OpenButton.raycastTarget = false;
                    DevPanel.raycastTarget   = false;
                }).OnComplete(() => {
                    OpenButton.raycastTarget = true;
                    DevPanel.raycastTarget   = true;
                    isOpen = false;
                });
            }
        }

        public void Button_AddItem() => StartCoroutine(SetPlayerItems());

        #endregion

        //뒤집힌 그래픽에 대한 EventTrigger를 정상적으로 실행하지 않는 현상에 대한 기록할 것.
    }
}
