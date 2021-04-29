using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using CodingCat_Scripts;

namespace CodingCat_Scripts
{
    public class LoadingSceneManager : MonoBehaviour
    {
        private static string nextScene;
        private float loadingPer;
        private string loadingPerStr;
        //private float fakeLoadingTime = 3.0f;

        //[SerializeField]
        //private Image progressBar = null;
        [SerializeField]
        private Text progressText = null;

        public Slider progressSlider = null;
        public Text loadingText = null;

        private void Start()
        {
            //Set timescale Default Value when Start
            //BattleScene의 pauseAction이 TimeScale의 변수에 영향을 미치는것을 복원
            //GameManager.Instance.DefaultTimeScale(); -> Move To BattleScene Return Btn !

            if (SceneManagement.Instance.NextScene != null)
            {
                nextScene = SceneManagement.Instance.NextScene;
                StartCoroutine(this.LoadScene());
            }
            else
            {
                Debug.LogError("NextScene Info is Null, Can't Load Next Scene ! Check the SceneManagement");
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

                //if(op.progress < 0.9f)
                //{
                //    progressBar.fillAmount = op.progress;
                //}
                //else
                //{
                //    timer += Time.unscaledDeltaTime / fakeLoadingTime;
                //    progressBar.fillAmount = Mathf.Lerp(0.9f, 1.0f, timer);
                //    
                //    if(progressBar.fillAmount >= 1f)
                //    {
                //        op.allowSceneActivation = true;
                //        yield break;
                //    }
                //
                //    //Time.unscaledDeltaTime 은 TimeScale의 영향을 받지않는 deltaTime
                //}

                //Slider Type Loading :: Fake Time
                if (progressSlider.value < 0.9f)
                {
                    progressSlider.value = Mathf.MoveTowards(progressSlider.value, 0.9f, Time.deltaTime);
                }
                else if (op.progress >= 0.9f)
                {
                    progressSlider.value = Mathf.MoveTowards(progressSlider.value, 1.0f, Time.deltaTime);
                }

                //if(progressSlider.value >= 1f)
                //{
                //    loadingText.text = "Press SpaceBar !";
                //}

                //if(Input.GetKeyDown(KeyCode.Space) && progressSlider.value >= 1f && op.progress >= 0.9f)
                //{
                //      NextSceneCommand !
                //}

                if (progressSlider.value >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }

                //Get Integer, Progress Persentage 
                loadingPer = progressSlider.value * 100;
                loadingPerStr = string.Format("{0} {1}", Mathf.FloorToInt(loadingPer).ToString(), "%");
                progressText.text = loadingPerStr;

                #endregion
            }
        }
    }
}
