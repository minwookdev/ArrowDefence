using TMPro;
using UnityEngine;
using ActionCat;

public class BattlePopup : MonoBehaviour {
    [Header("STAGE SELECT POPUP")]
    [SerializeField] TextMeshProUGUI textStageName;
    private string stageName = null;
    private string stageTag  = null;

    public void EnablePopup(STAGETYPE type) {
        switch (type) {
            case STAGETYPE.STAGE_DEV:         stageName = "DEV TEST STAGE";       stageTag = AD_Data.SCENE_BATTLE_DEV;            break;
            case STAGETYPE.FOREST_SECLUDED_E: stageName = "SECLUDED FOREST I";    stageTag = AD_Data.SCENE_BATTLE_FOREST_EASY;    break;
            case STAGETYPE.FOREST_SECLUDED_N: stageName = "SECLUDED FOREST II";   stageTag = AD_Data.SCENE_BATTLE_FOREST_NORMAL;  break;
            case STAGETYPE.FOREST_SECLUDED_H: stageName = "SECLUDED FOREST III";  stageTag = AD_Data.SCENE_BATTLE_FOREST_HARD;    break;
            case STAGETYPE.DUNGEON_E:         stageName = "DUNGEON ENTRANCE I";   stageTag = AD_Data.SCENE_BATTLE_DUNGEON_EASY;   break;
            case STAGETYPE.DUNGEON_N:         stageName = "DUNGEON ENTRANCE II";  stageTag = AD_Data.SCENE_BATTLE_DUNGEON_NORMAL; break;
            case STAGETYPE.DUNGEON_H:         stageName = "DUNGEON ENTRANCE III"; stageTag = AD_Data.SCENE_BATTLE_DUNGEON_HARD;   break;
            default: throw new System.NotImplementedException();
        }

        textStageName.text = stageName;
        gameObject.SetActive(true);
    }

    public void DisablePopup() {
        //Disable Popup Object
        gameObject.SetActive(false);

        //Clear Popup Data
        stageName = null;
        stageTag  = null;
        textStageName.text = "";
    }

    #region BUTTON

    public void BE_START() {
        MainSceneRoute.FadeIn(() => ActionCat.Data.CCPlayerData.equipments.UpdatePlayerAbility(),
                              () => SceneLoader.Instance.LoadScene(stageTag));
    }

    #endregion
}
