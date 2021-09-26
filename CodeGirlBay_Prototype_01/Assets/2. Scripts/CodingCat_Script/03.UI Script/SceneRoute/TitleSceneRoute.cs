using CodingCat_Scripts;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for UI Interaction of StartScene
/// It Serves as a Route to Help each Managers and UI Event connection
/// </summary>
public class TitleSceneRoute : MonoBehaviour
{
    [Header("Fade")]
    public Image ImgFade = null;
    public float FadeTime = 2.0f;

    #region Action_Btn

    public void BtnLoadMain()
    {
        ImgFade.DOFade(1, FadeTime).OnStart(() => ImgFade.raycastTarget = false).
                OnComplete(() => { SceneLoader.Instance.LoadScene(AD_Data.SCENE_MAIN); });
    }

    #endregion
}

