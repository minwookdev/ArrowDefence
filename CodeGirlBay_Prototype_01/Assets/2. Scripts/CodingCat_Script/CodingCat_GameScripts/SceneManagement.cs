using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodingCat_Scripts
{
    public class SceneManagement : Singleton<SceneManagement>
    {
        private readonly string startScene = "ArrowDefence_Title";
        private readonly string mainScene = "ArrowDefence_Main";
        private readonly string battleScene = "ArrowDefence_Battle_01";
        private readonly string loadingScene = "ArrowDefence_Loading";

        public string StartSceneName = "StartScene";
        public string MainSceneName = "MainScene";
        public string BattleSceneName = "BattleTestScene";
        public string LoadingSceneName = "LoadingScene";

        private string nextScene;

        public string NextScene { get { return nextScene; } }

        private void Awake()
        {
            //DontDestroyOnLoad(this.gameObject);
        }

        public void LoadStartScene()
        {
            try
            {
                CallLoadingScene(startScene);

                string successMsg = string.Format("{0} :: {1}", "SceneManagement", "Load StartScene Successfully");
                Debug.Log(successMsg);
            }
            catch
            {
                string failedMsg = string.Format("{0} :: {1}", "SceneManagement", "Failed Load Scene");
                Debug.LogError(failedMsg);
                throw;
            }
        }

        public void LoadMainScene()
        {
            try
            {
                CallLoadingScene(mainScene);

                string successMsg = string.Format("{0} :: {1}", "SceneManagement", "Load MainScene Successfully");
                Debug.Log(successMsg);
            }
            catch
            {
                string failedMsg = string.Format("{0} :: {1}", "SceneManagement", "Failed Load Scene");
                Debug.LogError(failedMsg);
                throw;
            }
        }

        public void LoadBattleScene()
        {
            try
            {
                CallLoadingScene(battleScene);

                string successMsg = string.Format("{0} :: {1}", "SceneManagement", "Load BattleScene Successfully");
                Debug.Log(successMsg);
            }
            catch
            {
                string failedMsg = string.Format("{0} :: {1}", "SceneManagement", "Failed Load Scene");
                Debug.LogError(failedMsg);
                throw;
            }
        }

        /// <summary>
        /// Set the Scene to be Loaded and load the LoadingScene
        /// </summary>
        /// <param name="targetScene">Load target SceneName</param>
        public void CallLoadingScene(string targetScene)
        {
            this.nextScene = targetScene;

            try
            {
                SceneManager.LoadScene(loadingScene);
                string successMsg = string.Format("{0} :: {1}", "SceneManagement", "Loading Scene Call Successfully");
                Debug.Log(successMsg);
            }
            catch
            {
                string failedMsg = string.Format("{0} :: {1}", "SceneManagement", "Failed Load Scene");
                Debug.LogError(failedMsg);
                throw;
            }
        }
    }
}
