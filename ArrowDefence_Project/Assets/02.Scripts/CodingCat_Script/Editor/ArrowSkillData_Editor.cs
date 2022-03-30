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
    protected SerializedProperty effectProp;
    protected SerializedProperty nameTermsProp = null;
    protected SerializedProperty descTermsProp = null;

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
        effectProp     = sobject.FindProperty(nameof(ArrowSkillData.effects));
        nameTermsProp  = sobject.FindProperty(nameof(ArrowSkillData.NameTerms));
        descTermsProp  = sobject.FindProperty(nameof(ArrowSkillData.DescTerms));
    }

    public virtual void DrawMonoScript() {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((ArrowSkillData)target),
                                                                        typeof(ArrowSkillData), false);
        EditorGUI.EndDisabledGroup();
    }

    /// <summary>
    /// 함수 호출 후, 반드시 다음 라인에 GUILayout.EndVertical(); 코드 작성해야합니다.
    /// </summary>
    protected void DrawDefaultSkillData() {
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

        //SKILL NAME, DESCRIPTION TERMS
        EditorGUILayout.PropertyField(nameTermsProp);
        EditorGUILayout.PropertyField(descTermsProp);

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

    GUIStyle guiStyle;

    public void OnEnable() {
        base.InitSerializedObject();

        //Rebound Arrow Property
        scanRangeProp     = sobject.FindProperty("ScanRadius");
        maxChainCountProp = sobject.FindProperty("MaxChainCount");

        guiStyle = new GUIStyle();
        guiStyle.alignment = TextAnchor.MiddleCenter;
    }

    public override void OnInspectorGUI() {
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

        #region EFFECT_DATA
        GUILayout.Label("Hit Skill Effect", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("GroupBox");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Effect Array Length");
        effectProp.arraySize = EditorGUILayout.IntField(effectProp.arraySize, new GUIStyle(GUI.skin.box) {alignment = TextAnchor.MiddleCenter });
        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < effectProp.arraySize; i++) {
            EditorGUILayout.PropertyField(effectProp.GetArrayElementAtIndex(i), new GUIContent("Effect Element " + i));
        }
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

[CustomEditor(typeof(DataSplitDagger))]
public class SpliDaggerEditor : ArrowSkillData_Editor {
    SerializedProperty projectileCountProp;
    SerializedProperty daggerPrefProp;
    SerializedProperty damageProp;

    private void OnEnable() {
        base.InitSerializedObject();
        damageProp          = sobject.FindProperty(nameof(DataSplitDagger.ProjectileDamage));
        daggerPrefProp      = sobject.FindProperty(nameof(DataSplitDagger.daggerPref));
        projectileCountProp = sobject.FindProperty(nameof(DataSplitDagger.projectileCount));
    }

    public override void OnInspectorGUI() {
        DrawMonoScript();
        DrawDefaultSkillData();
        #region PROJECTILE
        GUILayout.Label("Projectile Common", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("GroupBox");
        EditorGUILayout.PropertyField(damageProp);
        EditorGUILayout.EndVertical();
        #endregion
        #region SPLITDAGGER
        GUILayout.Label("Split Dagger", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("GroupBox");
        EditorGUILayout.PropertyField(projectileCountProp);
        EditorGUILayout.PropertyField(daggerPrefProp);
        EditorGUILayout.EndVertical();
        #endregion
        GUILayout.EndVertical();
        sobject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(DataEltalFire))]
public class ElementalFireEditor : ArrowSkillData_Editor {
    SerializedProperty probabilityProp;
    SerializedProperty firePrefProp;
    SerializedProperty damageProp;

    private void OnEnable() {
        base.InitSerializedObject();
        damageProp      = sobject.FindProperty(nameof(DataEltalFire.ProjectileDamage));
        probabilityProp = sobject.FindProperty(nameof(DataEltalFire.ActivationProbability));
        firePrefProp    = sobject.FindProperty(nameof(DataEltalFire.firePref));
    }

    public override void OnInspectorGUI() {
        DrawMonoScript();
        DrawDefaultSkillData();
        #region PROJECTILE
        GUILayout.Label("Projectile Common", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("GroupBox");
        EditorGUILayout.PropertyField(damageProp);
        EditorGUILayout.EndVertical();
        #endregion
        #region ELEMENTAL_FIRE
        GUILayout.Label("Elemental Arrow - Fire", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("GroupBox");
        EditorGUILayout.PropertyField(probabilityProp);
        EditorGUILayout.PropertyField(firePrefProp);
        EditorGUILayout.EndVertical();
        #endregion
        GUILayout.EndVertical();
        sobject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(Dt_Explosion))]
public class ExplosionEditor : ArrowSkillData_Editor {
    SerializedProperty explosionPref;
    SerializedProperty smallExPref;
    SerializedProperty explosionRange;
    SerializedProperty addExplosionRange;
    SerializedProperty explosionDamageProp;
    SerializedProperty addExplosionDamageProp;

    private void OnEnable() {
        base.InitSerializedObject();
        explosionPref     = sobject.FindProperty(nameof(Dt_Explosion.ExplosionPref));
        smallExPref       = sobject.FindProperty(nameof(Dt_Explosion.SmallExPref));
        explosionRange    = sobject.FindProperty(nameof(Dt_Explosion.ExplosionRange));
        addExplosionRange = sobject.FindProperty(nameof(Dt_Explosion.AddExplosionRange));
        explosionDamageProp    = sobject.FindProperty(nameof(Dt_Explosion.ExplosionDamage));
        addExplosionDamageProp = sobject.FindProperty(nameof(Dt_Explosion.AddExDamage));
    }

    public override void OnInspectorGUI() {
        DrawMonoScript();
        DrawDefaultSkillData();
        #region PROJECTILE
        GUILayout.Label("Projectile Common");
        EditorGUILayout.BeginVertical("GroupBox");
        EditorGUILayout.PropertyField(explosionDamageProp, new GUIContent("Base Damage"));
        EditorGUILayout.PropertyField(addExplosionDamageProp, new GUIContent("Base Damage - 2"));
        EditorGUILayout.EndVertical();
        #endregion
        #region EXPLOSION
        GUILayout.Label("Explosion");
        EditorGUILayout.BeginVertical("GroupBox");
        EditorGUILayout.PropertyField(explosionPref);
        EditorGUILayout.PropertyField(smallExPref);
        EditorGUILayout.PropertyField(explosionRange);
        EditorGUILayout.PropertyField(addExplosionRange);
        var currentIndentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;
        EditorGUILayout.PropertyField(effectProp, new GUIContent("ShockWave Effect"));
        EditorGUI.indentLevel = currentIndentLevel;
        EditorGUILayout.EndVertical();
        #endregion
        GUILayout.EndVertical();
        sobject.ApplyModifiedProperties();
    }
}

public class CreateArrowSkillDataAsset {  
    [MenuItem("ActionCat/Scriptable Object/Arrow Skill Asset/Rebound Arrow")]
    public static void CreateReboundArrowAsset()
    {
        string assetCreatePath = "Assets/05.SO/SO.Skill/ArrowSkillAsset/ReboundArrow.asset";
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
        string assetCreatePath = "Assets/05.SO/SO.Skill/ArrowSkillAsset/HomingArrow.asset";
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
        string assetCreatePath = "Assets/05.SO/SO.Skill/ArrowSkillAsset/PiercingArrow.asset";
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
        string assetCreatePath = "Assets/05.SO/SO.Skill/ArrowSkillAsset/SplitArrow.asset";
        var asset = ScriptableObject.CreateInstance<DataSplit>();
        AssetDatabase.CreateAsset(asset, assetCreatePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [MenuItem("ActionCat/Scriptable Object/Arrow Skill Asset/Split Dagger")]
    public static void CreateSplitDaggerAsset() {
        string assetCreatePath = "Assets/05.SO/SO.Skill/ArrowSkillAsset/SplitDagger_Default.asset";
        var asset = ScriptableObject.CreateInstance<DataSplitDagger>();
        AssetDatabase.CreateAsset(asset, assetCreatePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [MenuItem("ActionCat/Scriptable Object/Arrow Skill Asset/Elemental Arrow - Fire")]
    public static void CreateElementalFire() {
        string assetCreatePath = "Assets/05.SO/SO.Skill/ArrowSkillAsset/ElemetalArrow-Fire_Default.asset";
        var asset = ScriptableObject.CreateInstance<DataEltalFire>();
        AssetDatabase.CreateAsset(asset, assetCreatePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [MenuItem("ActionCat/Scriptable Object/Arrow Skill Asset/Special/Explosion")]
    public static void CreateExplosionAsset() {
        string assetCreatePath = "Assets/05.SO/SO.Skill/ArrowSkillAsset/Special_Explosion_default.asset";
        var asset = ScriptableObject.CreateInstance<Dt_Explosion>();
        AssetDatabase.CreateAsset(asset, assetCreatePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}




