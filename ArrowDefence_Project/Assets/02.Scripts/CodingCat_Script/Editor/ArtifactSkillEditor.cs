using UnityEngine;
using UnityEditor;
using ActionCat;

public class ArtifactSkillEditor : Editor {
    protected SerializedProperty conditionTypeProp = null;
    protected SerializedProperty maxStackProp = null;
    protected SerializedProperty maxCostProp = null;
    protected SerializedProperty increaseCostProp = null;
    protected SerializedProperty cooldownTimeProp = null;

    protected void InitConditionProps(SerializedObject serializedobject) {
        conditionTypeProp = serializedobject.FindProperty(nameof(AccessorySkillData.ConditionType));
        maxStackProp      = serializedobject.FindProperty(nameof(AccessorySkillData.MaxStack));
        maxCostProp       = serializedobject.FindProperty(nameof(AccessorySkillData.MaxCost));
        increaseCostProp  = serializedobject.FindProperty(nameof(AccessorySkillData.IncreaseCostCount));
        cooldownTimeProp  = serializedobject.FindProperty(nameof(AccessorySkillData.CoolDownTime));
    }

    protected void DrawConditionProps() {
        EditorGUILayout.PropertyField(conditionTypeProp);
        switch (conditionTypeProp.enumValueIndex) {
            case 1: //TYPE:: TRIGGER
                EditorGUILayout.PropertyField(maxStackProp,     new GUIContent("Max Stack"));
                EditorGUILayout.PropertyField(maxCostProp,      new GUIContent("Max Cost"));
                EditorGUILayout.PropertyField(increaseCostProp, new GUIContent("Increase Cost"));
                EditorGUILayout.PropertyField(cooldownTimeProp, new GUIContent("Cool Down Time"));
                break;
            case 2: //TYPE:: BUFF   
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(maxStackProp,     new GUIContent("Max Stack"));
                if (maxStackProp.intValue != 1) {
                    maxStackProp.intValue = 1;
                    CatLog.WLog("MaxStack Value Changed.");
                }
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.PropertyField(maxCostProp,      new GUIContent("Stacking Time"));
                if (increaseCostProp.floatValue > 0f) {
                    increaseCostProp.floatValue = 0f;
                    CatLog.WLog("Changed Increase Cost Count Value.");
                }
                EditorGUILayout.PropertyField(cooldownTimeProp, new GUIContent("Cool Down Time"));
                break;
            case 3: //TYPE:: DEBUFF   
                EditorGUILayout.PropertyField(maxStackProp, new GUIContent("Max Stack"));
                EditorGUILayout.PropertyField(maxCostProp,  new GUIContent("Stacking Time"));
                if (increaseCostProp.floatValue > 0f) {
                    increaseCostProp.floatValue = 0f;
                    CatLog.WLog("Changed Increase Cost Count Value.");
                }
                EditorGUILayout.PropertyField(cooldownTimeProp, new GUIContent("Cool Down Time"));
                break;
            default:  break;
        }
    }
}

#region EDITOR_ACSP

[CustomEditor(typeof(SkillDataAimSight))]
public class AimSightDataEditor : ArtifactSkillEditor {
    SerializedObject sobject;

    SerializedProperty idProp;
    SerializedProperty typeProp;
    SerializedProperty levelProp;
    SerializedProperty spriteProp;

    SerializedProperty nameTermsProp = null;
    SerializedProperty descTermsProp = null;

    SerializedProperty matProp;
    SerializedProperty lineWidthProp;

    SerializedProperty aimsightPrefProp;

    public void OnEnable() {
        sobject = new SerializedObject(target);

        idProp     = sobject.FindProperty("SkillId");
        typeProp   = sobject.FindProperty("EffectType");
        levelProp  = sobject.FindProperty("SkillLevel");
        spriteProp = sobject.FindProperty("SkillIconSprite");

        matProp       = sobject.FindProperty("LineRenderMat");
        lineWidthProp = sobject.FindProperty("LineWidth");

        nameTermsProp = sobject.FindProperty(nameof(SkillDataAimSight.NameTerms));
        descTermsProp = sobject.FindProperty(nameof(SkillDataAimSight.DescTerms));

        aimsightPrefProp = sobject.FindProperty(nameof(SkillDataAimSight.AimSightPref));
    }

    public override void OnInspectorGUI() {
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
        //EditorGUILayout.PropertyField(nameProp);
        //
        ////Description Field
        //GUILayout.Label("Description");
        //descProp.stringValue = EditorGUILayout.TextArea(descProp.stringValue, GUILayout.Height(50f));

        //NAME, DESCRIPTION TERMS FIELD 
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

        #region AIMSIGHT_SKILL_DATA
        GUILayout.Label("Aim Sight Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        //Line Material Field
        EditorGUILayout.PropertyField(matProp);

        //Line Width Field
        EditorGUILayout.PropertyField(lineWidthProp);

        EditorGUILayout.PropertyField(aimsightPrefProp, new GUIContent("AimSight Prefab"));

        GUILayout.EndVertical();
        #endregion

        EditorGUILayout.EndVertical();

        sobject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(SkillDataSlowTime))]
public class SlowTimeDataEditor : ArtifactSkillEditor {
    SerializedObject sobject;

    SerializedProperty idProp;
    SerializedProperty typeProp;
    SerializedProperty levelProp;
    SerializedProperty spriteProp;

    SerializedProperty nameTermsProp = null;
    SerializedProperty descTermsProp = null;

    SerializedProperty ratioProp;
    SerializedProperty durationProp;
    SerializedProperty cooldownProp;
    GUIStyle logoStyle = null;

    public void OnEnable() {
        sobject = new SerializedObject(target);

        //Default Property
        idProp     = sobject.FindProperty("SkillId");
        typeProp   = sobject.FindProperty("EffectType");
        levelProp  = sobject.FindProperty("SkillLevel");
        spriteProp = sobject.FindProperty("SkillIconSprite");

        //Slow Time Property
        ratioProp = sobject.FindProperty("TimeSlowRatio");
        durationProp = sobject.FindProperty("Duration");
        cooldownProp = sobject.FindProperty("CoolDown");

        nameTermsProp = sobject.FindProperty(nameof(SkillDataSlowTime.NameTerms));
        descTermsProp = sobject.FindProperty(nameof(SkillDataSlowTime.DescTerms));

        InitConditionProps(sobject);
        logoStyle = new GUIStyle();
        logoStyle.fontSize = 16;
        logoStyle.fontStyle = FontStyle.BoldAndItalic;
        logoStyle.normal.textColor = new Color(1f, 1f, 1f);
    }

    public override void OnInspectorGUI() {
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
        //EditorGUILayout.PropertyField(nameProp);
        //
        ////Description Field
        //GUILayout.Label("Description");
        //descProp.stringValue = EditorGUILayout.TextArea(descProp.stringValue, GUILayout.Height(50f));

        //NAME, DESCRIPTION TERMS PROP
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

        #region SLOWTIME_SKILLDATA

        GUILayout.Label("Slow Time Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        EditorGUILayout.PropertyField(ratioProp);

        EditorGUILayout.PropertyField(durationProp);

        EditorGUILayout.PropertyField(cooldownProp);

        GUILayout.EndVertical();

        #endregion

        #region CONDITION
        GUILayout.BeginHorizontal();
        GUILayout.Space(10f);
        GUILayout.Label("Condition", logoStyle);
        GUILayout.EndHorizontal();
        GUILayout.BeginVertical("GroupBox");
        DrawConditionProps();
        GUILayout.EndVertical();

        #endregion

        GUILayout.EndVertical();

        sobject.ApplyModifiedProperties();
    }
}

#endregion

#region EDITOR_RFEF

[CustomEditor(typeof(RFDataDamageUp))]
public class RFEditor_DamageUp : Editor
{
    SerializedObject sobject;

    SerializedProperty idProp;
    SerializedProperty typeProp;

    SerializedProperty valueProp;

    SerializedProperty termsNameProp = null;
    SerializedProperty termsDescProp = null;

    public void OnEnable() {
        sobject = new SerializedObject(target);

        idProp   = sobject.FindProperty("SkillId");
        typeProp = sobject.FindProperty("EffectType");

        valueProp = sobject.FindProperty("DamageIncreaseValue");

        termsNameProp = sobject.FindProperty(nameof(RFDataDamageUp.NameTerms));
        termsDescProp = sobject.FindProperty(nameof(RFDataDamageUp.DescTerms));
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((RFDataDamageUp)target),
                                                                       typeof(RFDataDamageUp), false);
        EditorGUI.EndDisabledGroup();

        sobject.Update();

        GUILayout.BeginVertical("HelpBox");

        #region DEFAULT_FIELD
        //Start Default Skill Data Field
        GUILayout.Label("Default Skill Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        //ID Field
        EditorGUILayout.PropertyField(idProp);

        //Name Field
        //EditorGUILayout.PropertyField(nameProp);

        //Desc Field
        //GUILayout.Label("Description");
        //descProp.stringValue = EditorGUILayout.TextArea(descProp.stringValue, GUILayout.Height(50f));
        //EditorGUILayout.PropertyField(descProp);

        EditorGUILayout.PropertyField(termsNameProp);
        EditorGUILayout.PropertyField(termsDescProp);

        //Effecy Type Field
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(typeProp); EditorGUI.EndDisabledGroup();

        GUILayout.EndVertical();

        #endregion

        #region INHERENCE_FIELD

        GUILayout.Label("RF Effect Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        //Increase Damage Field
        EditorGUILayout.PropertyField(valueProp);

        GUILayout.EndVertical();

        #endregion

        GUILayout.EndVertical();

        sobject.ApplyModifiedProperties();
    }
}

#endregion

/// <summary>
/// Add Skill Data Scriptable Object Asset Create Button to Action Cat Menu
/// </summary>
public static class CreateAcspDataAsset
{
    [MenuItem("ActionCat/Scriptable Object/Accessory Skill Asset/Aim Sight")]
    public static void CreateAimSightAsset()
    {
        string assetCreatePath = 
            "Assets/05. Scriptable_Object/SkillAsset/AccessorySkillAsset/AimSight_(skillLevel).asset";
        var asset = ScriptableObject.CreateInstance<SkillDataAimSight>();
        AssetDatabase.CreateAsset(asset, assetCreatePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [MenuItem("ActionCat/Scriptable Object/Accessory Skill Asset/Slow Time")]
    public static void CreateSlowTimeAsset()
    {
        string assetCreatePath =
             "Assets/05. Scriptable_Object/SkillAsset/AccessorySkillAsset/SlowTime_(skillLevel).asset";
        var asset = ScriptableObject.CreateInstance<SkillDataSlowTime>();
        AssetDatabase.CreateAsset(asset, assetCreatePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [MenuItem("ActionCat/Scriptable Object/RFEF/DamageUP")]
    public static void CreateRFEFDamageUp()
    {
        string assetPath = "Assets/05. Scriptable_Object/SkillAsset/AccessorySkillAsset/RFEF/DamageUP.asset";
        var asset = ScriptableObject.CreateInstance<RFDataDamageUp>();
        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}

