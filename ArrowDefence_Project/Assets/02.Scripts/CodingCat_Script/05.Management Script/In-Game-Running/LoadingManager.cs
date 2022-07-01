namespace ActionCat
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
        public Image SliderFillImage;
        public Color LoadingSliderColor;

        //private Text progressText = null;
        //private Text loadingText = null;

        private void Start()
        {
            //Set timescale Default Value
            //Current TimeScale variable affects loading Logic.
            if (Time.timeScale != 1f)
                ActionCat.GameManager.Instance.TimeToDefault();

            //Set Loading Slider Color
            SliderFillImage.color = LoadingSliderColor;

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

        IEnumerator LoadScene() {
            yield return null;

            AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
            op.allowSceneActivation = false;

            //float timer = 0.0f;

            while (!op.isDone) {
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
                if (progressSlider.value < 0.9f) {
                    progressSlider.value = Mathf.MoveTowards(progressSlider.value, 0.9f, Time.deltaTime);
                }
                else if (op.progress >= 0.9f) {
                    progressSlider.value = Mathf.MoveTowards(progressSlider.value, 1.0f, Time.deltaTime);
                }

                //Progress Persentage String
                loadingPer = progressSlider.value * 100;
                loadingPerStr = string.Format("{0} {1}", Mathf.FloorToInt(loadingPer).ToString(), "%");
                tmpProgress.text = loadingPerStr;

                //if Slider is the Max Value, Allow Active Target Scene 
                if (progressSlider.value >= 1f) {
                    op.allowSceneActivation = true;
                    yield break;
                }

                #endregion
            }
        }

        string sceneNameStr = "";

        IEnumerator LoadSceneAsyncWithProgress(LoadSceneMode loadSceneMode) {
            yield return null;

            // 씬 비동기 로드 객체 생성 
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneNameStr, loadSceneMode); 
            asyncOperation.allowSceneActivation = false; // 완전히 로드되지 전 까지 씬 로드 방지

            // Operation Progress 변수 획득이 필요한 경우
            float progressSliderValue = 0f;
            while (!asyncOperation.isDone) {
                progressSliderValue = asyncOperation.progress; // 비동기 로드 진행률 변수획득 (슬라이더 등으로 진행률 표시)
                if (asyncOperation.progress >= 0.9f) {         // 지정 로드율 넘어가면 씬 로드
                    progressSliderValue = 1f;
                    asyncOperation.allowSceneActivation = true;
                }
                yield return null;
            }
        }

        IEnumerator LoadSceneAsync(LoadSceneMode loadSceneMode) {
            yield return null;

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneNameStr, loadSceneMode);
            asyncOperation.allowSceneActivation = false;

            // Operation Progress 변수 획득이 필요하지 않은 경우. 지정 로드율 까지 대기
            yield return new WaitUntil(() => asyncOperation.progress >= 0.9f);

            asyncOperation.allowSceneActivation = true;
        }
    }
}
