using UnityEngine;
using UnityEditor;
using ActionCat;

public class SkillData_Editor
{

}

public class SpreadShot_DataEditor
{

}

public class RapidShot_DataEditor
{

}

public class RainArrow_DataEditor
{

}

[CustomEditor(typeof(SkillData_Empty))]
public class Empty_DataEditor : Editor
{
    SerializedObject sobject;
    SerializedProperty nameProperty;
    SerializedProperty descProperty;
    SerializedProperty typeProperty;
    SerializedProperty levelProperty;

    public void OnEnable()
    {
        sobject = new SerializedObject(target);
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((SkillData_Empty)target), typeof(SkillData_Empty), false);
        EditorGUI.EndDisabledGroup();

        sobject.Update();

        nameProperty  = sobject.FindProperty("SkillName");
        descProperty  = sobject.FindProperty("SkillDesc");
        typeProperty  = sobject.FindProperty("SkillType");
        levelProperty = sobject.FindProperty("SkillLevel");
        //levelProperty = sobject.FindProperty(nameof(SkillData_Empty.SkillName));
        

        GUILayout.Space(5);
        GUILayout.BeginVertical("HelpBox");

        #region BASIC_SKILL_DATA
        GUILayout.Space(5f); GUILayout.Label("  Basic Skill Data", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        EditorGUILayout.PropertyField(nameProperty, true);
        //EditorGUILayout.PropertyField(descProperty, true);
        GUILayout.Label("Skill Description");
        descProperty.stringValue = EditorGUILayout.TextArea(descProperty.stringValue, GUILayout.Height(50f));
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(typeProperty, true);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.PropertyField(levelProperty, true);

        GUILayout.EndVertical();
        #endregion

        GUILayout.EndVertical();

        sobject.ApplyModifiedProperties();
    }
}


