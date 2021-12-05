using TMPro;
using UnityEngine;
using ActionCat;

public class BattlePopup : MonoBehaviour {
    [Header("STAGE SELECT POPUP")]
    [SerializeField] TextMeshProUGUI textStageName;
    private string stageName;

    //Dest Stage Info
    int selectedStageNum;
    string stageTag = null;

    public void EnablePopup(int idx) {
        //Set Stage Info string & dest stage Number
        switch (idx) {
            case 0: selectedStageNum = idx; stageName = "DEV STAGE";        break;
            case 1: selectedStageNum = idx; stageName = "SECLUDED FOREST";  break;
            case 2: selectedStageNum = idx; stageName = "ENTRANCE DUNGEON"; break;
            default: ActionCat.CatLog.ELog("Invalid Number of Stage");      return;
        }

        //init-Stage Name string 
        textStageName.text = stageName;

        //Enable Popup
        gameObject.SetActive(true);
    }

    public void DisablePopup() {
        //Disable Popup Object
        gameObject.SetActive(false);

        //Clear Popup Data
        stageName             = "";
        textStageName.text = "";
        selectedStageNum      = 0;
    }

    #region BUTTON

    public void ButtonStart() {
        //init-load target scene-tag
        switch (selectedStageNum) {
            case 0: stageTag = AD_Data.SCENE_BATTLE_DEV;               break;
            case 1: stageTag = AD_Data.SCENE_BATTLE_FOREST_NORMAL;     break;
            case 2: stageTag = AD_Data.SCENE_BATTLE_DUNGEON_NORMAL;    break;
            default: ActionCat.CatLog.ELog("Invalid Number of Stage"); return;
        }

        //Load a Sceen with tag string
        MainSceneRoute.Fade(() => SceneLoader.Instance.LoadScene(stageTag));
    }

    #endregion
}
