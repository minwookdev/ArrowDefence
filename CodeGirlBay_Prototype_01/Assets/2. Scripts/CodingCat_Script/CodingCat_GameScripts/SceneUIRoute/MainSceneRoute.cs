using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using CodingCat_Scripts;

public class MainSceneRoute : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup fadeCanvasGroup;
    private bool isFadePlaying;

    public float fadeTime = 2f;

    [Header("Main Scene UI Elements")]
    public GameObject optionPanel;
    public GameObject battleScenePanel;

    private void Start()
    {
        //StartFadeEffect(false);
    }

    #region Btn_Actions

    public void OptionBtnAction() => optionPanel.SetActive(optionPanel.activeSelf ? false : true);

    public void BtnActionBattleSceneLoad() => SceneManagement.Instance.LoadBattleScene();

    public void BtnActionBattle() => battleScenePanel.SetActive(battleScenePanel.activeSelf ? false : true);

    public void BtnLoadStartScene() => SceneManagement.Instance.LoadStartScene();

    #endregion

    #region Fade Effect Test Function

    public void FadeTestBtn() => StartFadeEffect(true);  

    private void StartFadeEffect(bool isFade)
    {
        if (isFadePlaying) return;

        if (isFade)
        {
            fadeCanvasGroup.gameObject.SetActive(true);
        }

        StartCoroutine(this.Fade(isFade));
    }

    private IEnumerator Fade(bool isFadeIn)
    {
        isFadePlaying = true;

        float timer = 0f;

        while (timer <= 1f)
        {
            yield return null;
            timer += Time.deltaTime / fadeTime;

            fadeCanvasGroup.alpha = Mathf.Lerp(isFadeIn ? 0 : 1, isFadeIn ? 1 : 0, timer);
        }

        if(!isFadeIn)
        {
            fadeCanvasGroup.gameObject.SetActive(false);
        }

        isFadePlaying = false;
    }

    #endregion
}
