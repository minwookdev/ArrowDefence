using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattlePopup : MonoBehaviour
{
    public TextMeshProUGUI CurrentStageInfo;
    [HideInInspector] public string SelectStage;

    private void OnEnable()
    {
        if(SelectStage != null)
        {
            CurrentStageInfo.text = SelectStage;
        }
        else
        {
            CurrentStageInfo.text = "Select Stage : NULL";
        }
    }
}
