using UnityEditor;
using UnityEngine;
using ActionCat;

public class ArrowSkillData_Editor : Editor
{
    protected SerializedObject sobject;

    protected SerializedProperty idProp;
    protected SerializedProperty nameProp;
    protected SerializedProperty descProp;
    protected SerializedProperty levelProp;
    protected SerializedProperty typeProp;
    protected SerializedProperty spriteProp;
    protected SerializedProperty activeTypeProp;

    protected void InitSerializedObject()
    {
        sobject = new SerializedObject(target);

        idProp         = sobject.FindProperty("SkillId");
        nameProp       = sobject.FindProperty("SkillName");
        descProp       = sobject.FindProperty("SkillDesc");
        levelProp      = sobject.FindProperty("SkillLevel");
        typeProp       = sobject.FindProperty("SkillType");
        spriteProp     = sobject.FindProperty("IconSprite");
        activeTypeProp = sobject.FindProperty("ActiveType");
    }

    public virtual void DrawMonoScript()
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((ArrowSkillData)target),
                                                                        typeof(ArrowSkillData), false);
        EditorGUI.EndDisabledGroup();
    }

    /// <summary>
    /// 함수 호출 후, 반드시 다음 라인에 GUILayout.EndVertical(); 코드 작성해야합니다.
    /// </summary>
    protected void DrawDefaultSkillData()
    {
        //DrawMonoScript();

        sobject.Update();

        GUILayout.Space(5f);
        GUILayout.BeginVertical("HelpBox");

        #region DEFAULT_ARROWSKILL_DATA
        GUILayout.Label("Default Arrow Skill info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        //Skill ID Field
        EditorGUILayout.PropertyField(idProp);

        //Skill Name Field
        EditorGUILayout.PropertyField(nameProp);

        //Skill Desc Field
        GUILayout.Label("Description");
        descProp.stringValue = EditorGUILayout.TextArea(descProp.stringValue, GUILayout.Height(50f));

        //Type Field [LOCK]
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(typeProp); EditorGUI.EndDisabledGroup();

        //Active Type Field [LOCK]
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(activeTypeProp); EditorGUI.EndDisabledGroup();

        //Level Field
        EditorGUILayout.PropertyField(levelProp);

        //Icon Sprite Field
        EditorGUILayout.PropertyField(spriteProp);

        GUILayout.EndVertical();
        #endregion
    }
}

[CustomEditor(typeof(DataRebound))]
public class ReboundArrowDataEditor : ArrowSkillData_Editor
{
    SerializedProperty scanRangeProp;
    SerializedProperty maxChainCountProp;

    public void OnEnable()
    {
        base.InitSerializedObject();

        //Rebound Arrow Property
        scanRangeProp     = sobject.FindProperty("ScanRadius");
        maxChainCountProp = sobject.FindProperty("MaxChainCount");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        //Draw Disable MonoScript
        DrawMonoScript();

        //Draw Default Skill Properties
        DrawDefaultSkillData();

        #region REBOUNDARROW_DATA
        GUILayout.Label("Rebound Arrow Info", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("GroupBox");

        //Scan Range Property
        EditorGUILayout.PropertyField(scanRangeProp);

        //Chain Count Property
        EditorGUILayout.PropertyField(maxChainCountProp);

        EditorGUILayout.EndVertical();
        #endregion

        //End HelpBox
        EditorGUILayout.EndVertical();

        //Save Property
        sobject.ApplyModifiedProperties();
    }

    public override void DrawMonoScript()
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((DataRebound)target),
                                                                        typeof(DataRebound), false); 
        EditorGUI.EndDisabledGroup();
    }
}

[CustomEditor(typeof(DataHoming))]
public class HomingArrowDataEditor : ArrowSkillData_Editor
{
    SerializedProperty intervalProp;
    SerializedProperty radiusProp;
    SerializedProperty speedProp;
    SerializedProperty rotateSpeedProp;

    public void OnEnable()
    {
        base.InitSerializedObject();

        intervalProp    = sobject.FindProperty("TargetSearchInterval");
        radiusProp      = sobject.FindProperty("ScanRadius");
        speedProp       = sobject.FindProperty("HomingSpeed");
        rotateSpeedProp = sobject.FindProperty("HomingRotateSpeed");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        //Draw Disable MonoScript
        DrawMonoScript();

        //Draw Default Skill Properties
        DrawDefaultSkillData();

        #region HOMINGARROW_DATA

        //Target Finder Interval
        GUILayout.Label("Homing Arrow Info", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("GroupBox");
        EditorGUILayout.PropertyField(intervalProp);

        EditorGUILayout.PropertyField(radiusProp);

        EditorGUILayout.PropertyField(speedProp);

        EditorGUILayout.PropertyField(rotateSpeedProp);
        EditorGUILayout.EndVertical();
        #endregion

        //End HelpBox
        EditorGUILayout.EndVertical();

        //Save Property
        sobject.ApplyModifiedProperties();
    }

    public override void DrawMonoScript()
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((DataHoming)target),
                                                                        typeof(DataHoming), false);
        EditorGUI.EndDisabledGroup();
    }
}

[CustomEditor(typeof(DataPiercing))]
public class PiercingArrowDataEditor : ArrowSkillData_Editor
{
    SerializedProperty maxChainCountProp;

    public void OnEnable()
    {
        base.InitSerializedObject();

        maxChainCountProp = sobject.FindProperty("MaxChainCount");
    }

    public override void OnInspectorGUI()
    { 
        //base.OnInspectorGUI
        //Draw Disable MonoScript
        DrawMonoScript();

        DrawDefaultSkillData();

        #region PIERCING_ARROW_DATA
        GUILayout.Label("Piercing Arrow Info", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("GroupBox");

        //Max Chain Count Property
        EditorGUILayout.PropertyField(maxChainCountProp);

        EditorGUILayout.EndVertical();
        #endregion

        GUILayout.EndVertical();

        sobject.ApplyModifiedProperties();
    }
}

public class CreateArrowSkillDataAsset
{ 
    [MenuItem("ActionCat/Scriptable Object/Arrow Skill Asset/Rebound Arrow")]
    public static void CreateReboundArrowAsset()
    {
        string assetCreatePath = "Assets/05. Scriptable_Object/SkillAsset/ArrowSkillAsset/ReboundArrow.asset";
        var asset = ScriptableObject.CreateInstance<DataRebound>();
        AssetDatabase.CreateAsset(asset, assetCreatePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [MenuItem("ActionCat/Scriptable Object/Arrow Skill Asset/Homing Arrow")]
    public static void CreateHomingArrowAsset()
    {
        string assetCreatePath = "Assets/05. Scriptable_Object/SkillAsset/ArrowSkillAsset/HomingArrow.asset";
        var asset = ScriptableObject.CreateInstance<DataHoming>();
        AssetDatabase.CreateAsset(asset, assetCreatePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
    
    [MenuItem("ActionCat/Scriptable Object/Arrow Skill Asset/Piercing Arrow")]
    public static void CreatePiercingArrowAsset()
    {
        string assetCreatePath = "Assets/05. Scriptable_Object/SkillAsset/ArrowSkillAsset/PiercingArrow.asset";
        var asset = ScriptableObject.CreateInstance<DataPiercing>();
        AssetDatabase.CreateAsset(asset, assetCreatePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [MenuItem("ActionCat/Scriptable Object/Arrow Skill Asset/Split Arrow")]
    public static void CreateSplitArrowAsset()
    {
        string assetCreatePath = "Assets/05. Scriptable_Object/SkillAsset/ArrowSkillAsset/SplitArrow.asset";
        var asset = ScriptableObject.CreateInstance<DataSplit>();
        AssetDatabase.CreateAsset(asset, assetCreatePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}


