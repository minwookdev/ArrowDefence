namespace ActionCat {
    using UnityEngine.SceneManagement;

    public class SceneLoader : Singleton<SceneLoader> {
        private readonly string loadingScene = AD_Data.SCENE_LOADING;

        public string NextScene { get; private set; }

        /// <summary>
        /// Set the Scene to be Loaded and load the LoadingScene
        /// </summary>
        /// <param name="targetScene">Load target SceneName</param>
        public void LoadScene(string targetScene) {
            NextScene = targetScene;

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
            NextScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(loadingScene);
        }

        public string GetCurrentSceneName() {
            return SceneManager.GetActiveScene().name;
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
