namespace CodingCat_Games.Testing
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using DG.Tweening;
    using CodingCat_Scripts;
    using CodingCat_Games.Data;
    using ES3Types;

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

        public void Button_SaveData() => CCPlayerData.Saveinventory();

        public void Button_LoadData() => CCPlayerData.LoadInventory();

        public void Button_Additems()
        {
            if(playerData == null)
            {
                CatLog.WLog("Player Data Scriptable Object is NULL");
                return;
            }

            var itemList = playerData.GetItemData();

            if (itemList.Count <= 0)
            {
                CatLog.Log("Player Data Scriptable has no Items"); 
                return;
            }

            foreach (var item in itemList)
            {
                CCPlayerData.inventory.AddItem(item);
            }

            CatLog.Log($"{itemList.Count} 개의 아이템이 전달되었습니다.");
        }

        public void Button_SetUserData(int num) => CCPlayerData.SetUserInt(num);

        #endregion

        //뒤집힌 그래픽에 대한 EventTrigger를 정상적으로 실행하지 않는 현상에 대한 기록할 것.
    }
}
