namespace ActionCat {
    using UnityEngine.SceneManagement;

    public class SceneLoader : Singleton<SceneLoader> {
        private readonly string loadingScene = AD_Data.SCENE_LOADING;
        public delegate void OnSceneChange();
        public static event OnSceneChange SceneChangeCallback;

        public string NextScene { get; private set; }

        /// <summary>
        /// Set the Scene to be Loaded and load the LoadingScene
        /// </summary>
        /// <param name="targetScene">Load target SceneName</param>
        public void LoadScene(string targetScene) {
            NextScene = targetScene;
            SceneChangeCallback?.Invoke();

            try {
                SceneManager.LoadScene(loadingScene);
            }
            catch {
                string failedMsg = string.Format("{0} :: {1}", "SceneManagement", "Failed Load Scene");
                CatLog.ELog(failedMsg);
                throw;
            }
        }

        public void ReloadScene() {
            NextScene = SceneManager.GetSceneAt(0).name;
            SceneChangeCallback();
            SceneManager.LoadScene(loadingScene);
        }

        public void LoadScene(string sceneName, float waitRealTime = 0f) {
            NextScene = sceneName;
            SceneChangeCallback?.Invoke();
            StartCoroutine(WaitLoadScene(waitRealTime));
        }

        public void ReloadScene(float waitRealTime = 0f) {
            NextScene = SceneManager.GetSceneAt(0).name;
            SceneChangeCallback?.Invoke();
            //if(waitRealTime <= 0f) {
            //    SceneManager.LoadScene(loadingScene);
            //}
            //else {
            //    StartCoroutine(WaitLoadScene(waitRealTime));
            //}

            StartCoroutine(WaitLoadScene(waitRealTime));
        }

        public string GetCurrentSceneName() {
            return SceneManager.GetActiveScene().name;
        }

        System.Collections.IEnumerator WaitLoadScene(float waitRealTime) {
            yield return new UnityEngine.WaitForSecondsRealtime(waitRealTime);
            SceneManager.LoadScene(loadingScene);
        }

        public System.Collections.IEnumerator AdditiveLoadUIScene() {
            float loadStartTime = UnityEngine.Time.time;
            yield return SceneManager.LoadSceneAsync("ArrowDefence_Battle_UI", LoadSceneMode.Additive);
            Scene targetScene = SceneManager.GetSceneByName("ArrowDefence_Battle_UI");
            SceneManager.SetActiveScene(targetScene);
            CatLog.Log($"UIScene LoadingTime : {UnityEngine.Time.time - loadStartTime}sec");
        }
    }
}
