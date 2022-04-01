using UnityEngine;
using UnityEditor;
using ActionCat;

public class SkillData_Editor
{

}

[CustomEditor(typeof(SkillDataSpreadShot))]
public class SpreadShot_DataEditor : Editor
{
    SerializedObject sobject;

    SerializedProperty idProp;
    SerializedProperty typeProp;
    SerializedProperty levelProp;
    SerializedProperty spriteProp;

    SerializedProperty arrowCountProp;
    SerializedProperty spreadAngleProp;

    SerializedProperty nameTermsProp = null;
    SerializedProperty descTermsProp = null;

    public void OnEnable() {
        sobject = new SerializedObject(target);

        idProp     = sobject.FindProperty("SkillId");
        typeProp   = sobject.FindProperty("SkillType");
        levelProp  = sobject.FindProperty("SkillLevel");
        spriteProp = sobject.FindProperty("SkillIconSprite");

        arrowCountProp  = sobject.FindProperty("ArrowShotCount");
        spreadAngleProp = sobject.FindProperty("SpreadAngle");

        nameTermsProp = sobject.FindProperty(nameof(SkillDataSpreadShot.NameTerms));
        descTermsProp = sobject.FindProperty(nameof(SkillDataSpreadShot.DescTerms));
    }

    public override void OnInspectorGUI() {
        //base.OnInspectorGUI();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((SkillDataSpreadShot)target),
                                                                       typeof(SkillDataSpreadShot), false);
        EditorGUI.EndDisabledGroup();

        sobject.Update();

        GUILayout.Space(5f);
        GUILayout.BeginVertical("HelpBox");

        #region BASIC_SKILL_DATA
        //Start Default Data Fields
        GUILayout.Label("Default Skill Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        //ID Field
        EditorGUILayout.PropertyField(idProp);

        ////Name Field
        //EditorGUILayout.PropertyField(nameProp, true);
        //
        ////Description Field
        //GUILayout.Label("Skill Description");
        //descProp.stringValue = EditorGUILayout.TextArea(descProp.stringValue, GUILayout.Height(50f));

        //Name, Description Terms Field
        EditorGUILayout.PropertyField(nameTermsProp);
        EditorGUILayout.PropertyField(descTermsProp);

        //Type Field [LOCK]
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(typeProp, true);
        EditorGUI.EndDisabledGroup();

        //Level Field
        EditorGUILayout.PropertyField(levelProp, true);

        //Icon Sprite Field
        EditorGUILayout.PropertyField(spriteProp, true);

        //End Field
        GUILayout.EndVertical();
        #endregion

        #region ANOTHER_SKILL_DATA
        GUILayout.Label("Spread Shot Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        //Arrow Shot Count Field
        EditorGUILayout.PropertyField(arrowCountProp);

        //Spread Angle Field
        EditorGUILayout.PropertyField(spreadAngleProp);

        GUILayout.EndVertical();
        #endregion

        GUILayout.EndVertical();

        //Save Property
        sobject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(SkillDataRapidShot))]
public class RapidShot_DataEditor : Editor
{
    SerializedObject sobject;

    SerializedProperty idProp;
    SerializedProperty typeProp;
    SerializedProperty levelProp;
    SerializedProperty spriteProp;

    SerializedProperty arrowCountProp;
    SerializedProperty intervalProp;
    SerializedProperty effectProp;

    SerializedProperty nameTermsProp = null;
    SerializedProperty descTermsProp = null;

    public void OnEnable() {
        sobject = new SerializedObject(target);

        idProp     = sobject.FindProperty("SkillId");
        typeProp   = sobject.FindProperty("SkillType");
        levelProp  = sobject.FindProperty("SkillLevel");
        spriteProp = sobject.FindProperty("SkillIconSprite");
        effectProp = sobject.FindProperty(nameof(SkillDataRapidShot.muzzleEffect));

        arrowCountProp  = sobject.FindProperty("ArrowShotCount");
        intervalProp    = sobject.FindProperty("ShotInterval");

        nameTermsProp = sobject.FindProperty(nameof(SkillDataSpreadShot.NameTerms));
        descTermsProp = sobject.FindProperty(nameof(SkillDataSpreadShot.DescTerms));
    }

    public override void OnInspectorGUI() {
        //base.OnInspectorGUI();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((SkillDataRapidShot)target),
                                                                       typeof(SkillDataRapidShot), false);
        EditorGUI.EndDisabledGroup();

        sobject.Update();

        GUILayout.Space(5);
        GUILayout.BeginVertical("HelpBox");

        #region BASIC_SKILL_DATA
        //Start Default Data Fields
        GUILayout.Label("Default Skill Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        //ID Field
        EditorGUILayout.PropertyField(idProp);

        ////Name Field
        //EditorGUILayout.PropertyField(nameProp, true);
        //
        ////Description Field
        //GUILayout.Label("Skill Description");
        //descProp.stringValue = EditorGUILayout.TextArea(descProp.stringValue, GUILayout.Height(50f));

        //Name, Description Terms Field
        EditorGUILayout.PropertyField(nameTermsProp);
        EditorGUILayout.PropertyField(descTermsProp);

        //Type Field [LOCK]
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(typeProp, true);
        EditorGUI.EndDisabledGroup();

        //Level Field
        EditorGUILayout.PropertyField(levelProp, true);

        //Icon Sprite Field
        EditorGUILayout.PropertyField(spriteProp, true);

        //End Field
        GUILayout.EndVertical();
        #endregion

        #region ANOTHER_SKILL_DATA
        GUILayout.Label("Rapid Shot Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        //Arrow Count Field
        EditorGUILayout.PropertyField(arrowCountProp);

        //Spread Angle Field
        EditorGUILayout.PropertyField(intervalProp);

        //Muzzle Effect Field
        EditorGUILayout.PropertyField(effectProp);

        GUILayout.EndVertical();
        #endregion

        GUILayout.EndVertical();

        //Save Property
        sobject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(SkillDataArrowRain))]
public class RainArrow_DataEditor : Editor
{
    SerializedObject sobject;

    SerializedProperty idProp;
    SerializedProperty typeProp;
    SerializedProperty levelProp;
    SerializedProperty spriteProp;

    SerializedProperty arrowCountProp;
    SerializedProperty intervalProp;

    SerializedProperty nameTermsProp = null;
    SerializedProperty descTermsProp = null;

    public void OnEnable() {
        sobject = new SerializedObject(target);
                   
        idProp     = sobject.FindProperty("SkillId");
        typeProp   = sobject.FindProperty("SkillType");
        levelProp  = sobject.FindProperty("SkillLevel");
        spriteProp = sobject.FindProperty("SkillIconSprite");

        arrowCountProp = sobject.FindProperty("ArrowShotCount");
        intervalProp   = sobject.FindProperty("ShotInterval");

        nameTermsProp = sobject.FindProperty(nameof(SkillDataSpreadShot.NameTerms));
        descTermsProp = sobject.FindProperty(nameof(SkillDataSpreadShot.DescTerms));
    }

    public override void OnInspectorGUI() {
        //base.OnInspectorGUI();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((SkillDataArrowRain)target),
                                                                       typeof(SkillDataArrowRain), false);
        EditorGUI.EndDisabledGroup();

        sobject.Update();

        GUILayout.Space(5);
        GUILayout.BeginVertical("HelpBox");

        #region BASIC_SKILL_DATA
        //Start Default Data Fields
        GUILayout.Label("Default Skill Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        //ID Field
        EditorGUILayout.PropertyField(idProp);

        ////Name Field
        //EditorGUILayout.PropertyField(nameProp, true);
        //
        ////Description Field
        //GUILayout.Label("Skill Description");
        //descProp.stringValue = EditorGUILayout.TextArea(descProp.stringValue, GUILayout.Height(50f));

        //Name, Description Terms Field
        EditorGUILayout.PropertyField(nameTermsProp);
        EditorGUILayout.PropertyField(descTermsProp);

        //Type Field [LOCK]
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(typeProp, true);
        EditorGUI.EndDisabledGroup();

        //Level Field
        EditorGUILayout.PropertyField(levelProp, true);

        //Icon Sprite Field
        EditorGUILayout.PropertyField(spriteProp, true);

        //End Field
        GUILayout.EndVertical();
        #endregion

        #region ANOTHER_SKILL_DATA
        GUILayout.Label("Rain Arrow Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        //Arrow Count Field
        EditorGUILayout.PropertyField(arrowCountProp);

        //Interval Field
        EditorGUILayout.PropertyField(intervalProp, new GUIContent("Arrow Spawn Interval"));

        GUILayout.EndVertical();
        #endregion

        GUILayout.EndVertical();

        //Save Property
        sobject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(SkillData_Empty))]
public class Empty_DataEditor : Editor
{
    SerializedObject sobject;

    SerializedProperty idProp;
    SerializedProperty typeProp;
    SerializedProperty levelProp;
    SerializedProperty spriteProp;

    SerializedProperty nameTermsProp = null;
    SerializedProperty descTermsProp = null;

    public void OnEnable()
    {
        sobject = new SerializedObject(target);

        idProp     = sobject.FindProperty("SkillId");
        typeProp   = sobject.FindProperty("SkillType");
        levelProp  = sobject.FindProperty("SkillLevel");
        spriteProp = sobject.FindProperty("SkillIconSprite");

        nameTermsProp = sobject.FindProperty(nameof(SkillDataSpreadShot.NameTerms));
        descTermsProp = sobject.FindProperty(nameof(SkillDataSpreadShot.DescTerms));
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((SkillData_Empty)target), 
                                                                       typeof(SkillData_Empty), false);
        EditorGUI.EndDisabledGroup();

        sobject.Update();

        GUILayout.Space(5);
        GUILayout.BeginVertical("HelpBox");

        #region BASIC_SKILL_DATA
        //Start Default Data Fields
        GUILayout.Label("Default Skill Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        //ID Field
        EditorGUILayout.PropertyField(idProp);

        ////Name Field
        //EditorGUILayout.PropertyField(nameProp, true);
        //
        ////Description Field
        //GUILayout.Label("Skill Description");
        //descProp.stringValue = EditorGUILayout.TextArea(descProp.stringValue, GUILayout.Height(50f));

        //Name, Description Terms Field
        EditorGUILayout.PropertyField(nameTermsProp);
        EditorGUILayout.PropertyField(descTermsProp);

        //Type Field [LOCK]
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(typeProp, true);
        EditorGUI.EndDisabledGroup();

        //Level Field
        EditorGUILayout.PropertyField(levelProp, true);

        //Icon Sprite Field
        EditorGUILayout.PropertyField(spriteProp, true);

        //End Field
        GUILayout.EndVertical();
        #endregion

        #region ANOTHER_SKILL_DATA

        #endregion

        GUILayout.EndVertical();

        //Save Property
        sobject.ApplyModifiedProperties();
    }
}

/// <summary>
/// Add Skill Data Scriptable Asset Create Button to ActionCat Menu
/// </summary>
public static class CreateSkillDataAsset
{
    [MenuItem("ActionCat/Scriptable Object/Bow Skill Asset/Spread Shot")]
    public static void CreateSpreadShotAsset()
    {
        string assetCreatePath = "Assets/05. Scriptable_Object/SkillAsset/BowSkillAsset/SpreadShot_(level).asset";
        var asset = ScriptableObject.CreateInstance<SkillDataSpreadShot>();
        AssetDatabase.CreateAsset(asset, assetCreatePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [MenuItem("ActionCat/Scriptable Object/Bow Skill Asset/Rapid Shot")]
    public static void CreateRapidShotAsset()
    {
        string assetCreatePath = "Assets/05. Scriptable_Object/SkillAsset/BowSkillAsset/RapidShot_(level).asset";
        var asset = ScriptableObject.CreateInstance<SkillDataRapidShot>();
        AssetDatabase.CreateAsset(asset, assetCreatePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [MenuItem("ActionCat/Scriptable Object/Bow Skill Asset/Rain Arrow")]
    public static void CreateRainArrowAsset()
    {
        string assetCreatePath = "Assets/05. Scriptable_Object/SkillAsset/BowSkillAsset/RainArrow_(level).asset";
        var asset = ScriptableObject.CreateInstance<SkillDataArrowRain>();
        AssetDatabase.CreateAsset(asset, assetCreatePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [MenuItem("ActionCat/Scriptable Object/Bow Skill Asset/Empty Slot")]
    public static void CreateEmptySlotAsset()
    {
        string assetCreatePath = "Assets/05. Scriptable_Object/SkillAsset/BowSkillAsset/EmptySlot_(level).asset";
        var asset = ScriptableObject.CreateInstance<SkillData_Empty>();
        AssetDatabase.CreateAsset(asset, assetCreatePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}



