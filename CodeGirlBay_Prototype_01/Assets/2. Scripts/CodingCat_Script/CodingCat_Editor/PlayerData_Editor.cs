using CodingCat_Games.Data;
using UnityEditor;
using UnityEngine;

public class PlayerData_Editor : EditorWindow
{
    //private int playerInt;

    //Add Menu item named "PlayerData_Editor" to the Window menu
    [MenuItem("CodingCat/Data Window")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PlayerData_Editor));
    }

    private void OnGUI()
    {
        GUILayout.Label("PlayerData Window", EditorStyles.boldLabel);

        GUILayout.BeginVertical("HelpBox");

        GUILayout.Label("ArrowDefenece PlayerData");

        EditorGUILayout.LabelField("Player int Value : ", CCPlayerData.user_int.ToString());

        GUILayout.Space(100);


        GUILayout.FlexibleSpace();  //화면 하단에 배치
        GUILayout.EndVertical();
        GUILayout.Label("Editor Window의 데이터 갱신이 느릴 수 있습니다." + '\n' +
                        "Window에 마우스오버 또는 클릭하면 데이터가 갱신됩니다.");

    }

    //private void Update()
    //{
    //    
    //}
}

