﻿namespace CodingCat_Scripts
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;
    using TMPro;

    public class LoadingManager : MonoBehaviour
    {
        private static string nextScene;
        private float loadingPer;
        private string loadingPerStr;

        [Header("Loading UI")]
        public Slider progressSlider = null;
        public TextMeshProUGUI tmpProgress = null;

        //private Text progressText = null;
        //private Text loadingText = null;

        private void Start()
        {
            //Set timescale Default Value when Start
            //BattleScene의 pauseAction이 TimeScale의 변수에 영향을 미치는것을 복원
            //GameManager.Instance.DefaultTimeScale(); -> Move To BattleScene Return Btn !

            if (SceneLoader.Instance.NextScene != null)
            {
                nextScene = SceneLoader.Instance.NextScene;
                StartCoroutine(this.LoadScene());
            }
            else
            {
                CatLog.ELog("Load Target Scene Info is NULL, Stop Loading Process");
            }
        }

        IEnumerator LoadScene()
        {
            yield return null;

            AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
            op.allowSceneActivation = false;

            //float timer = 0.0f;

            while (!op.isDone)
            {
                yield return null;

                #region NOT_FAKE_TIME

                //timer += Time.deltaTime;
                //
                //if(op.progress < 0.9f)
                //{
                //    progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                //
                //    if(progressBar.fillAmount >= op.progress)
                //    {
                //        timer = 0f;
                //    }
                //}
                //else
                //{
                //    progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                //
                //    if(progressBar.fillAmount == 1.0f)
                //    {
                //        op.allowSceneActivation = true;
                //        yield break;
                //    }
                //}

                #endregion

                #region FAKE_TIME

                //Slider Type Loading :: Fake Time
                if (progressSlider.value < 0.9f)
                {
                    progressSlider.value = Mathf.MoveTowards(progressSlider.value, 0.9f, Time.deltaTime);
                }
                else if (op.progress >= 0.9f)
                {
                    progressSlider.value = Mathf.MoveTowards(progressSlider.value, 1.0f, Time.deltaTime);
                }

                //Progress Persentage String
                loadingPer = progressSlider.value * 100;
                loadingPerStr = string.Format("{0} {1}", Mathf.FloorToInt(loadingPer).ToString(), "%");
                tmpProgress.text = loadingPerStr;

                //if Slider is the Max Value, Allow Active Target Scene 
                if (progressSlider.value >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }

                #endregion
            }
        }
    }
}