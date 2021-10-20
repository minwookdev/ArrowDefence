using UnityEngine;
using UnityEditor;
using ActionCat;

public class ACSkillData_Editor
{

}

[CustomEditor(typeof(SkillDataAimSight))]
public class AimSightDataEditor : Editor
{
    SerializedObject sobject;

    SerializedProperty idProp;
    SerializedProperty nameProp;
    SerializedProperty descProp;
    SerializedProperty typeProp;
    SerializedProperty levelProp;

    SerializedProperty matProp;
    SerializedProperty lineWidthProp;

    public void OnEnable()
    {
        sobject = new SerializedObject(target);

        idProp    = sobject.FindProperty("SkillId");
        nameProp  = sobject.FindProperty("SkillName");
        descProp  = sobject.FindProperty("SkillDesc");
        typeProp  = sobject.FindProperty("EffectType");
        levelProp = sobject.FindProperty("SkillLevel");

        matProp       = sobject.FindProperty("LineRenderMat");
        lineWidthProp = sobject.FindProperty("LineWidth");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((SkillDataAimSight)target),
                                                                       typeof(SkillDataAimSight), false);
        EditorGUI.EndDisabledGroup();

        sobject.Update();

        EditorGUILayout.Space(5f);
        EditorGUILayout.BeginVertical("HelpBox");

        #region DEFAULT_SKILL_DATA
        //Start Default Skill Data Field
        GUILayout.Label("Default Skill Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        //ID Field
        EditorGUILayout.PropertyField(idProp);

        //Name Field
        EditorGUILayout.PropertyField(nameProp);

        //Description Field
        GUILayout.Label("Description");
        descProp.stringValue = EditorGUILayout.TextArea(descProp.stringValue, GUILayout.Height(50f));

        //Type Field [LOCK]
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(typeProp, true);
        EditorGUI.EndDisabledGroup();

        //Level Field
        EditorGUILayout.PropertyField(levelProp, true);

        //End Field
        GUILayout.EndVertical();
        #endregion

        #region AIMSIGHT_SKILL_DATA
        GUILayout.Label("Aim Sight Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        //Line Material Field
        EditorGUILayout.PropertyField(matProp);

        //Line Width Field
        EditorGUILayout.PropertyField(lineWidthProp);

        GUILayout.EndVertical();
        #endregion

        EditorGUILayout.EndVertical();

        sobject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(SkillDataSlowTime))]
public class SlowTimeDataEditor : Editor
{
    SerializedObject sobject;

    SerializedProperty idProp;
    SerializedProperty nameProp;
    SerializedProperty descProp;
    SerializedProperty typeProp;
    SerializedProperty levelProp;

    public void OnEnable()
    {
        sobject = new SerializedObject(target);

        idProp    = sobject.FindProperty("SkillId");
        nameProp  = sobject.FindProperty("SkillName");
        descProp  = sobject.FindProperty("SkillDesc");
        typeProp  = sobject.FindProperty("EffectType");
        levelProp = sobject.FindProperty("SkillLevel");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((SkillDataSlowTime)target),
                                                                       typeof(SkillDataSlowTime), false);
        EditorGUI.EndDisabledGroup();

        sobject.Update();

        GUILayout.Space(5f);
        GUILayout.BeginVertical("HelpBox");

        #region DEFAULT_SKILL_DATA
        //Start Default Skill Data Field
        GUILayout.Label("Default Skill Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        //ID Field
        EditorGUILayout.PropertyField(idProp);

        //Name Field
        EditorGUILayout.PropertyField(nameProp);

        //Description Field
        GUILayout.Label("Description");
        descProp.stringValue = EditorGUILayout.TextArea(descProp.stringValue, GUILayout.Height(50f));

        //Type Field [LOCK]
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(typeProp, true);
        EditorGUI.EndDisabledGroup();

        //Level Field
        EditorGUILayout.PropertyField(levelProp, true);

        //End Field
        GUILayout.EndVertical();
        #endregion

        #region SLOWTIME_SKILLDATA

        #endregion

        GUILayout.EndVertical();

        sobject.ApplyModifiedProperties();
    }
}

/// <summary>
/// Add Skill Data Scriptable Object Asset Create Button to Action Cat Menu
/// </summary>
public static class CreateAcspDataAsset
{
    [MenuItem("ActionCat/Scriptable Object/Accessory Skill Asset/Aim Sight")]
    public static void CreateAimSightAsset()
    {

    }

    [MenuItem("ActionCat/Scriptable Object/Accessory Skill Asset/Slow Time")]
    public static void CreateSlowTimeAsset()
    {

    }
}

