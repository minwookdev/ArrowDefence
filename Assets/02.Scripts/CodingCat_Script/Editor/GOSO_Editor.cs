using UnityEngine;
using UnityEditor;
using ActionCat;

[CustomEditor(typeof(GOSO))]
public class GOSO_Editor : Editor
{
    [MenuItem("ActionCat/Open Global ScriptableObject")]
    public static void OpenInspector() {
        Selection.activeObject = GOSO.Inst;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        //GUI상의 변경사항이 생겼을 때 Dirty Flag 세워서 저장 처리.
        //확실한 저장 처리를 위해 사용
        if(GUI.changed == true) {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }
    }
}
