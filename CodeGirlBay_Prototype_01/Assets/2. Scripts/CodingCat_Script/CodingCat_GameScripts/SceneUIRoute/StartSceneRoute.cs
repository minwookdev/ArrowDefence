using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CodingCat_Scripts;

/// <summary>
/// Script for UI Interaction of StartScene
/// It Serves as a Route to Help each Managers and UI Event connection
/// </summary>
public class StartSceneRoute : MonoBehaviour
{
    public float deltaT;
    public float undeltaT;

    public void StartBtnAction() => SceneManagement.Instance.LoadMainScene();

    public void ExitBtnAction() => Application.Quit();

    private void Start()
    {
        //Time.timeScale = 0.5f;

        //SingleTest.Instance.TestCall();

        //CatLog.NLog(string.Format("{0} : {1}", "Test 1", " Test 2"));

        //CatLog.Log("This", " is", " Test", " Log !");
        //CatLog.Log("This is Test Log !");
        //CatLog.Log(StringColor.GREEN, "This is Test Log !");
        //
        //CatLog.WLog("This is Test Log !");
        //CatLog.WLog("This", " is", " Test", " Log !");
        //CatLog.WLog(StringColor.BLACK, "This is Test Log !");
        //
        //CatLog.ELog("This is Test Log !");
        //CatLog.ELog("This", " is", " Test", " Log !");
        //CatLog.ELog(StringColor.RED, "This is Test Log !");

        //CatLog.ELog("is Stop Editor", true);
    }

    private void Update()
    {

    }

}
