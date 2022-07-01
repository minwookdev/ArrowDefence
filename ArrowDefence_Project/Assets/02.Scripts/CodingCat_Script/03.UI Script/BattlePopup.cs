using TMPro;
using UnityEngine;
using ActionCat;

public class BattlePopup : MonoBehaviour {
    [Header("STAGE SELECT POPUP")]
    [SerializeField] TextMeshProUGUI textStageName;
    private string stageName = null;
    private string stageTag  = null;

    [Header("SOUND EFFECT")]
    [SerializeField] ActionCat.Audio.ACSound soundEffect = null;

    public void EnablePopup(STAGETYPE type) {
        switch (type) {
            case STAGETYPE.STAGE_DEV:         stageName = I2.Loc.ScriptLocalization.StageName.Dev_Diff;       stageTag = AD_Data.SCENE_BATTLE_DEV;            break;
            case STAGETYPE.FOREST_SECLUDED_E: stageName = I2.Loc.ScriptLocalization.StageName.Forest_Easy;    stageTag = AD_Data.SCENE_BATTLE_FOREST_EASY;    break;
            case STAGETYPE.FOREST_SECLUDED_N: stageName = I2.Loc.ScriptLocalization.StageName.Forest_Normal;  stageTag = AD_Data.SCENE_BATTLE_FOREST_NORMAL;  break;
            case STAGETYPE.FOREST_SECLUDED_H: stageName = I2.Loc.ScriptLocalization.StageName.Forest_Hard;    stageTag = AD_Data.SCENE_BATTLE_FOREST_HARD;    break;
            case STAGETYPE.DUNGEON_E:         stageName = I2.Loc.ScriptLocalization.StageName.Dungeon_Easy;   stageTag = AD_Data.SCENE_BATTLE_DUNGEON_EASY;   break;
            case STAGETYPE.DUNGEON_N:         stageName = I2.Loc.ScriptLocalization.StageName.Dungeon_Normal; stageTag = AD_Data.SCENE_BATTLE_DUNGEON_NORMAL; break;
            case STAGETYPE.DUNGEON_H:         stageName = I2.Loc.ScriptLocalization.StageName.Dungeon_Hard;   stageTag = AD_Data.SCENE_BATTLE_DUNGEON_HARD;   break;
            default: throw new System.NotImplementedException();
        }

        textStageName.text = stageName;
        gameObject.SetActive(true);

        CatLog.Log($"I2.Loc.ScriptLocalization.StageName (example): {I2.Loc.ScriptLocalization.StageName.Dev_Diff}");
        CatLog.Log($"StageName String Value: {stageName}");

        soundEffect.PlayOneShot();
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
