namespace CodingCat_Scripts
{
    using UnityEngine.SceneManagement;

    public class SceneLoader : Singleton<SceneLoader>
    {
        private readonly string startScene   = AD_Data.Scene_Title;
        private readonly string mainScene    = AD_Data.Scene_Main;
        private readonly string battleScene  = AD_Data.Scene_Battle_Dev;
        private readonly string loadingScene = AD_Data.Scene_Loading;

        public string NextScene { get; private set; }

        /// <summary>
        /// Set the Scene to be Loaded and load the LoadingScene
        /// </summary>
        /// <param name="targetScene">Load target SceneName</param>
        public void LoadScene(string targetScene)
        {
            NextScene = targetScene;

            try
            {
                SceneManager.LoadScene(loadingScene);
            }
            catch
            {
                string failedMsg = string.Format("{0} :: {1}", "SceneManagement", "Failed Load Scene");
                CatLog.ELog(failedMsg);
                throw;
            }
        }
    }
}
