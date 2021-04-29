using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    // SerializeFiled
    public string SceneName;

    public void PushButton()
    {
        SceneManager.LoadScene(SceneName);
    }
}
