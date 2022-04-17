using ActionCat;
using ES3Editor;
using UnityEditor;
using UnityEngine;

public static class ItemData_Editor
{
    public const string DefaultItemInfoText = "Default Item Info";
    public const string ItemTypeText        = "Item Type";
    public const string ItemGradeText       = "Item Grade";
    public const string ItemIdText          = "Item ID";
    public const string ItemNameText        = "Item Name";
    public const string ItemDescText        = "Item Description";
    public const string ItemAmountText      = "Item Amount";
    public const string ItemSpriteText      = "Item Sprite";

    public const string EquipmentItemText = "Equipment Item Info";
    public const string EquipmentTypeText = "Equipment Item Type";

    public const string BowItemInfoText  = "Bow Item Info";
    public const string BowObjectText    = "Bow Object";
    public const string BowSkillSlotText = "Bow Skill Slot";
    public const string BowSkillEnable   = "Enable Bow Skill";

    public const string ArrowItemInfoText   = "Arrow Item Info";
    public const string ArrowMainObjectText = "Arrow (Main) Object";
    public const string ArrowLessObjectText = "Arrow (Less) Object";

    public const string AccessoryItemInfoText = "Accessory Item Info";
    public const string AccessoryEffectText   = "Accessory Effect";
    public const string AccessorySpEffectText = "Special Effect Type";
}

[CustomEditor(typeof(ItemData_Mat))]
public class MatItemData_Editor : Editor {
    ItemData_Mat item;
    //New
    SerializedObject serialObject = null;
    SerializedProperty nameTermProp = null;
    SerializedProperty descTermProp = null;

    private void OnEnable() {
        item = (ItemData_Mat)target;
        serialObject = new SerializedObject(target);
        nameTermProp = serialObject.FindProperty(nameof(ItemData_Mat.NameTerms));
        descTermProp = serialObject.FindProperty(nameof(ItemData_Mat.DescTerms));
    }


    public override void OnInspectorGUI()
    {
        if (item == null) return;

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((ItemData_Mat)item), typeof(ItemData_Mat), false);
        EditorGUI.EndDisabledGroup();

        serializedObject.Update();

        GUILayout.Space(5);
        GUILayout.BeginVertical("HelpBox");
        //base.OnInspectorGUI();

        #region DEFAULT_ITEM_INFO

        GUILayout.Label(ItemData_Editor.DefaultItemInfoText, EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        EditorGUI.BeginDisabledGroup(true);
        item.Item_Type = (ITEMTYPE)EditorGUILayout.EnumPopup(ItemData_Editor.ItemTypeText, item.Item_Type);
        EditorGUI.EndDisabledGroup();

        item.Item_Grade = (ITEMGRADE)EditorGUILayout.EnumPopup(ItemData_Editor.ItemGradeText, item.Item_Grade);

        item.Item_Id = EditorGUILayout.TextField(ItemData_Editor.ItemIdText, item.Item_Id);

        //item.Item_Name = EditorGUILayout.TextField(ItemData_Editor.ItemNameText, item.Item_Name);
        //
        //EditorGUILayout.LabelField(ItemData_Editor.ItemDescText);
        //item.Item_Desc = EditorGUILayout.TextArea(item.Item_Desc, GUILayout.Height(50f));

        EditorGUILayout.PropertyField(nameTermProp);
        EditorGUILayout.PropertyField(descTermProp);

        item.DefaultAmount = EditorGUILayout.IntField("Default Amount", item.DefaultAmount);

        item.Item_Sprite = (Sprite)EditorGUILayout.ObjectField(ItemData_Editor.ItemSpriteText, item.Item_Sprite, 
                                                                typeof(Sprite), allowSceneObjects: true);

        GUILayout.EndVertical();

        #endregion

        #region GENERATE_BUTTON

        if (GUILayout.Button("GENERATE"))
        {
            EditorUtility.SetDirty(item);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            CatLog.Log($"<color=green>{item.NameByTerms} : Apply the Modified Value</color>");
            //changeChecker = false;
        }
        GUILayout.Space(5f);

        #endregion

        GUILayout.EndVertical();

        serialObject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(ItemData_Con))]
public class ConItemData_Editor : Editor {
    ItemData_Con item;
    //New
    SerializedObject serialObject   = null;
    SerializedProperty nameTermProp = null;
    SerializedProperty descTermProp = null;

    public void OnEnable() {
        item = (ItemData_Con)target;

        serialObject = new SerializedObject(target);
        nameTermProp = serialObject.FindProperty(nameof(ItemData_Con.NameTerms));
        descTermProp = serialObject.FindProperty(nameof(ItemData_Con.DescTerms));
    }

    public override void OnInspectorGUI()
    {
        if (item == null) return;

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((ItemData_Con)item), typeof(ItemData_Con), false);
        EditorGUI.EndDisabledGroup();

        serializedObject.Update();

        GUILayout.Space(5);
        GUILayout.BeginVertical("HelpBox");
        //base.OnInspectorGUI();

        #region DEFAULT_ITEM_INFO

        GUILayout.Label(ItemData_Editor.DefaultItemInfoText, EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        EditorGUI.BeginDisabledGroup(true);
        item.Item_Type = (ITEMTYPE)EditorGUILayout.EnumPopup(ItemData_Editor.ItemTypeText, item.Item_Type);
        EditorGUI.EndDisabledGroup();

        item.Item_Grade = (ITEMGRADE)EditorGUILayout.EnumPopup(ItemData_Editor.ItemGradeText, item.Item_Grade);

        item.Item_Id = EditorGUILayout.TextField(ItemData_Editor.ItemIdText, item.Item_Id);

        //item.Item_Name = EditorGUILayout.TextField(ItemData_Editor.ItemNameText, item.Item_Name);
        //
        //EditorGUILayout.LabelField(ItemData_Editor.ItemDescText);
        //item.Item_Desc = EditorGUILayout.TextArea(item.Item_Desc, GUILayout.Height(50f));

        EditorGUILayout.PropertyField(nameTermProp);
        EditorGUILayout.PropertyField(descTermProp);

        item.DefaultAmount = EditorGUILayout.IntField("Default Amount", item.DefaultAmount);

        item.Item_Sprite = (Sprite)EditorGUILayout.ObjectField(ItemData_Editor.ItemSpriteText, item.Item_Sprite,
                                                                typeof(Sprite), allowSceneObjects: true);

        GUILayout.EndVertical();

        #endregion

        #region GENERATE_BUTTON

        if (GUILayout.Button("GENERATE"))
        {
            EditorUtility.SetDirty(item);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            CatLog.Log($"<color=green>{item.NameByTerms} : Apply the Modified Value</color>");
            //changeChecker = false;
        }
        GUILayout.Space(5f);

        #endregion

        GUILayout.EndVertical();
        serialObject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(ItemData_Equip_Bow))]
public class BowItemData_Editor : Editor
{
    ItemData_Equip_Bow item;

    //Serialized Object
    SerializedObject serialObject;

    //Default Item Property
    SerializedProperty nameTermsProp = null;
    SerializedProperty descTermsProp = null;

    //Bow Ability Property
    SerializedProperty abilDamageProp;
    SerializedProperty abilCritDmgProp;
    SerializedProperty abilCritChanceProp;
    SerializedProperty abilChargedDamageProp;
    SerializedProperty armorPenetrationProp = null;
    SerializedProperty additionalArrowFireProp = null;
    SerializedProperty elementalActivation = null;

    //Ability Foldout
    bool isAbilityTapFoldout = false;
    AbilityDrawer abilityDrawer = null;
    GUIStyle logoStyle = null;

    public void OnEnable() {
        item = (ItemData_Equip_Bow)target;

        //Default Bow Properties
        serialObject = new SerializedObject(target);
        nameTermsProp = serialObject.FindProperty(nameof(ItemData_Equip_Bow.NameTerms));
        descTermsProp = serialObject.FindProperty(nameof(ItemData_Equip_Bow.DescTerms));

        //Boe Ability Properties
        abilDamageProp          = serialObject.FindProperty(nameof(ItemData_Equip_Bow.BaseDamage));
        abilCritChanceProp      = serialObject.FindProperty(nameof(ItemData_Equip_Bow.CriticalHitChance));
        abilCritDmgProp         = serialObject.FindProperty(nameof(ItemData_Equip_Bow.CriticalDamageMultiplier));
        abilChargedDamageProp   = serialObject.FindProperty(nameof(ItemData_Equip_Bow.FullChargedMultiplier));
        armorPenetrationProp    = serialObject.FindProperty(nameof(ItemData_Equip_Bow.ArmorPenetration));
        additionalArrowFireProp = serialObject.FindProperty(nameof(ItemData_Equip_Bow.AdditionalArrowFire));
        elementalActivation     = serialObject.FindProperty(nameof(ItemData_Equip_Bow.ElementalActivation));

        abilityDrawer = new AbilityDrawer();
        logoStyle = new GUIStyle();
        logoStyle.fontSize = 16;
        logoStyle.fontStyle = FontStyle.BoldAndItalic;
        logoStyle.normal.textColor = new Color(1f, 1f, 1f);
    }

    public override void OnInspectorGUI() {
        if (item == null) return;

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((ItemData_Equip_Bow)item), typeof(ItemData_Equip_Bow), false);
        EditorGUI.EndDisabledGroup();

        serializedObject.Update();
        //EditorGUI.BeginChangeCheck();

        GUILayout.Space(5);
        GUILayout.BeginVertical("HelpBox");
        //base.OnInspectorGUI();

        #region DEFAULT_ITEM_INFO

        GUILayout.Label(ItemData_Editor.DefaultItemInfoText, EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        EditorGUI.BeginDisabledGroup(true);
        item.Item_Type = (ITEMTYPE)EditorGUILayout.EnumPopup(ItemData_Editor.ItemTypeText, item.Item_Type);
        EditorGUI.EndDisabledGroup();

        item.Item_Grade = (ITEMGRADE)EditorGUILayout.EnumPopup(ItemData_Editor.ItemGradeText, item.Item_Grade);

        item.Item_Id = EditorGUILayout.TextField(ItemData_Editor.ItemIdText, item.Item_Id);

        //item.Item_Name = EditorGUILayout.TextField(ItemData_Editor.ItemNameText, item.Item_Name);
        //
        //EditorGUILayout.LabelField(ItemData_Editor.ItemDescText);
        //item.Item_Desc = EditorGUILayout.TextArea(item.Item_Desc, GUILayout.Height(50f));

        EditorGUILayout.PropertyField(nameTermsProp);
        EditorGUILayout.PropertyField(descTermsProp);

        EditorGUI.BeginDisabledGroup(true);
        item.DefaultAmount = EditorGUILayout.IntField("Default Amount", item.DefaultAmount);
        EditorGUI.EndDisabledGroup();

        item.Item_Sprite = (Sprite)EditorGUILayout.ObjectField(ItemData_Editor.ItemSpriteText, item.Item_Sprite,
                                                                typeof(Sprite), allowSceneObjects: true);

        GUILayout.EndVertical();

        #endregion

        #region EQUIPMENT_ITEM_INFO

        GUILayout.Label(ItemData_Editor.EquipmentItemText, EditorStyles.boldLabel);

        GUILayout.BeginVertical("GroupBox");

        EditorGUI.BeginDisabledGroup(true);
        item.Equip_Type = (EQUIP_ITEMTYPE)EditorGUILayout.EnumPopup(ItemData_Editor.EquipmentTypeText, item.Equip_Type);
        EditorGUI.EndDisabledGroup();

        GUILayout.EndVertical();

        #endregion

        #region EQUIPMENT_ITEM_ABILITY
        GUILayout.Label("Item Ability", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");
        var tempIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;
        isAbilityTapFoldout = EditorGUILayout.Foldout(isAbilityTapFoldout, "Ability Sliders");
        EditorGUI.indentLevel = tempIndent;
        isAbilityTapFoldout = true;
        if (isAbilityTapFoldout) {
            GUILayout.Space(10f);
            GUILayout.Label("Inherence Abilities", logoStyle);
            abilityDrawer.Draw(AbilityChargedDamage.GetGradeCount(abilChargedDamageProp.floatValue));
            EditorGUILayout.PropertyField(abilChargedDamageProp);
            abilityDrawer.Draw(AdditionalFire.GetGradeCount(additionalArrowFireProp.intValue));
            EditorGUILayout.PropertyField(additionalArrowFireProp);
            abilityDrawer.Draw(ElementalActivation.GetGradeCount(elementalActivation.intValue));
            EditorGUILayout.PropertyField(elementalActivation);

            GUILayout.Space(10f);
            GUILayout.Label("Public Abilities", logoStyle);
            abilityDrawer.Draw(AbilityDamage.GetGradeCount(abilDamageProp.intValue));
            EditorGUILayout.PropertyField(abilDamageProp);
            abilityDrawer.Draw(AbilityCritChance.GetGradeCount(abilCritChanceProp.intValue));
            EditorGUILayout.PropertyField(abilCritChanceProp);
            abilityDrawer.Draw(AbilityCritDamage.GetGradeCount(abilCritDmgProp.floatValue));
            EditorGUILayout.PropertyField(abilCritDmgProp);
            abilityDrawer.Draw(PenetrationArmor.GetGradeCount(armorPenetrationProp.intValue));
            EditorGUILayout.PropertyField(armorPenetrationProp);
        }
        GUILayout.EndVertical();
        #endregion

        #region BOW_ITEM_INFO

        GUILayout.Label(ItemData_Editor.BowItemInfoText, EditorStyles.boldLabel);

        GUILayout.BeginVertical("GroupBox");

        GUILayout.Label("BOW OBJECT", EditorStyles.boldLabel);
        item.BowGameObject = (GameObject)EditorGUILayout.ObjectField(ItemData_Editor.BowObjectText, item.BowGameObject,
                                                                     typeof(GameObject), allowSceneObjects: false);

        //item.FirstSkill_Type  = (BOWSKILL_TYPE)EditorGUILayout.EnumPopup(ItemData_Editor.BowSkillSlotText, item.FirstSkill_Type);
        //item.SecondSkill_Type = (BOWSKILL_TYPE)EditorGUILayout.EnumPopup(ItemData_Editor.BowSkillSlotText, item.SecondSkill_Type);

        GUILayout.Space(10f); GUILayout.Label("SKILL-ASSET", EditorStyles.boldLabel);
        item.SkillAsset_f = (BowSkillData)EditorGUILayout.ObjectField("Skill-Asset First", item.SkillAsset_f, typeof(BowSkillData), allowSceneObjects: false);
        item.SkillAsset_s = (BowSkillData)EditorGUILayout.ObjectField("Skill-Asset Seconds", item.SkillAsset_s, typeof(BowSkillData), allowSceneObjects: false);

        EditorGUILayout.Space(5f);

        //if (item.BowSkill_First != null) EditorGUILayout.LabelField(ItemData_Editor.BowSkillEnable + " 0 : ", item.BowSkill_First.ToString());
        //else EditorGUILayout.LabelField(ItemData_Editor.BowSkillEnable + " 0 : ", "SKILL SLOT 0 NULL");
        //
        //if (item.BowSkill_Second != null) EditorGUILayout.LabelField(ItemData_Editor.BowSkillEnable + " 1 : ", item.BowSkill_Second.ToString());
        //else EditorGUILayout.LabelField(ItemData_Editor.BowSkillEnable + " 1 : ", "SKILL SLOT 1 NULL");

        GUILayout.EndVertical();

        #endregion

        //bool somethingChanged = (EditorGUI.EndChangeCheck()) ? false : true;
        //EditorGUI.BeginDisabledGroup(EditorGUI.EndChangeCheck() == false);
        //if (!somethingChanged) CatLog.Log("몬가 바뀜 !");
        //using (new EditorGUI.DisabledScope(!somethingChanged))
        //{
        //    if (GUILayout.Button("GENERATE"))
        //    {
        //        EditorUtility.SetDirty(item);
        //        AssetDatabase.SaveAssets();
        //        AssetDatabase.Refresh();
        //        //CatLog.Log($"{item.Item_Name} : Save Editor's Value !");
        //        somethingChanged = true;
        //    }
        //}
        //EditorGUI.EndDisabledGroup();

        //bool changeChecker = EditorGUI.EndChangeCheck();
        //if (changeChecker) CatLog.Log("몬가 값이 바뀜 !");

        #region GENERATE_BUTTON

        if (GUILayout.Button("GENERATE"))
        {
            EditorUtility.SetDirty(item);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            CatLog.Log($"<color=green>{item.NameByTerms} : Apply the Modified Value</color>");
            //changeChecker = false;

            //Repaint();
            item.OnEnable(); //AD_BowSkill 할당을 위한 OnEnable 호출
        }
        GUILayout.Space(5f);

        #endregion

        GUILayout.EndVertical();

        //if (changeChecker)
        //{
        //    saveString = "VALUE's CHANGE";
        //}
        //else saveString = "NOTHING";
        //
        //
        //Repaint(); 바로 Repaint가 업데이트 되지 않아서 saveString이 바뀌지 않는다

        //EditorGUILayout.LabelField(saveString);
        

        serialObject.ApplyModifiedProperties();
        
        //EditorApplication.update.Invoke();
    }
}

[CustomEditor(typeof(ItemData_Equip_Arrow))]
public class ArrowItemData_Editor : Editor {
    //Arrow Default Field
    ItemData_Equip_Arrow item;
    SerializedObject serialObject;
    SerializedProperty effectProp;
    SerializedProperty nameTermsProp = null;
    SerializedProperty descTermsProp = null;

    //Ability SerializedProperty
    SerializedProperty incArrowDamageProp;
    SerializedProperty speedProp;
    SerializedProperty projectileDamageProp = null;
    SerializedProperty spellDamageProp = null;
    //SerializedProperty elementalActivationProp = null;
    SerializedProperty damageIncProp = null;
    SerializedProperty criticalChanceProp = null;
    SerializedProperty criticalDamageProp = null;
    SerializedProperty armorPenetrationProp = null;

    //Is Ability Tap Foldout
    GUIStyle logoStyle = null;
    AbilityDrawer abilityDrawer = null;
    bool isAbilityTapFoldout = false;

    public void OnEnable() {
        item = (ItemData_Equip_Arrow)target;
        serialObject  = new SerializedObject(target);
        effectProp    = serialObject.FindProperty(nameof(ItemData_Equip_Arrow.effects));
        nameTermsProp = serialObject.FindProperty(nameof(ItemData_Equip_Arrow.NameTerms));
        descTermsProp = serialObject.FindProperty(nameof(ItemData_Equip_Arrow.DescTerms));

        //Ability Drawer
        abilityDrawer = new AbilityDrawer();
        logoStyle     = new GUIStyle();
        logoStyle.fontSize = 16;
        logoStyle.fontStyle = FontStyle.BoldAndItalic;
        logoStyle.normal.textColor = new Color(1f, 1f, 1f);

        //Arrow Ability Fields
        speedProp               = serialObject.FindProperty(nameof(ItemData_Equip_Arrow.AdditionalSpeed));
        incArrowDamageProp      = serialObject.FindProperty(nameof(ItemData_Equip_Arrow.ArrowDamageInc));
        projectileDamageProp    = serialObject.FindProperty(nameof(ItemData_Equip_Arrow.ProjectileDamageInc));
        spellDamageProp         = serialObject.FindProperty(nameof(ItemData_Equip_Arrow.SpellDamageInc));
        //elementalActivationProp = serialObject.FindProperty(nameof(ItemData_Equip_Arrow.ElementalActivation)); // <- Moveto Bow Item
        damageIncProp           = serialObject.FindProperty(nameof(ItemData_Equip_Arrow.BaseDamageInc));
        criticalChanceProp      = serialObject.FindProperty(nameof(ItemData_Equip_Arrow.CriticalChanceInc));
        criticalDamageProp      = serialObject.FindProperty(nameof(ItemData_Equip_Arrow.CriticalDamageMultiplierInc));
        armorPenetrationProp    = serialObject.FindProperty(nameof(ItemData_Equip_Arrow.ArmorPenetrationInc));
    }

    public override void OnInspectorGUI() {
        if (item == null) return;

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((ItemData_Equip_Arrow)item), typeof(ItemData_Equip_Arrow), false);
        EditorGUI.EndDisabledGroup();

        serialObject.Update();

        GUILayout.Space(5);
        GUILayout.BeginVertical("HelpBox");
        //base.OnInspectorGUI();

        #region DEFAULT_ITEM_INFO

        GUILayout.Label(ItemData_Editor.DefaultItemInfoText, EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        EditorGUI.BeginDisabledGroup(true);
        item.Item_Type = (ITEMTYPE)EditorGUILayout.EnumPopup(ItemData_Editor.ItemTypeText, item.Item_Type);
        EditorGUI.EndDisabledGroup();

        item.Item_Grade = (ITEMGRADE)EditorGUILayout.EnumPopup(ItemData_Editor.ItemGradeText, item.Item_Grade);

        item.Item_Id = EditorGUILayout.TextField(ItemData_Editor.ItemIdText, item.Item_Id);

        //item.Item_Name = EditorGUILayout.TextField(ItemData_Editor.ItemNameText, item.Item_Name);
        //
        //EditorGUILayout.LabelField(ItemData_Editor.ItemDescText);
        //item.Item_Desc = EditorGUILayout.TextArea(item.Item_Desc, GUILayout.Height(50f));

        EditorGUILayout.PropertyField(nameTermsProp);
        EditorGUILayout.PropertyField(descTermsProp);

        EditorGUI.BeginDisabledGroup(true);
        item.DefaultAmount = EditorGUILayout.IntField("Default Amount", item.DefaultAmount);
        EditorGUI.EndDisabledGroup();

        item.Item_Sprite = (Sprite)EditorGUILayout.ObjectField(ItemData_Editor.ItemSpriteText, item.Item_Sprite,
                                                                typeof(Sprite), allowSceneObjects: true);

        GUILayout.EndVertical();

        #endregion

        #region EQUIPMENT_ITEM_INFO

        GUILayout.Label(ItemData_Editor.EquipmentItemText, EditorStyles.boldLabel);

        GUILayout.BeginVertical("GroupBox");

        EditorGUI.BeginDisabledGroup(true);
        item.Equip_Type = (EQUIP_ITEMTYPE)EditorGUILayout.EnumPopup(ItemData_Editor.EquipmentTypeText, item.Equip_Type);
        EditorGUI.EndDisabledGroup();

        GUILayout.EndVertical();

        #endregion

        #region ABILITY_INFO

        GUILayout.Label("ABILITY INFO", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");
        var tempIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;
        isAbilityTapFoldout = EditorGUILayout.Foldout(isAbilityTapFoldout, "Ability Sliders");
        EditorGUI.indentLevel = tempIndent;
        isAbilityTapFoldout = true;
        if (isAbilityTapFoldout) {
            //Draw Inherence Ability Properties
            GUILayout.Space(10f);
            GUILayout.Label("Inherence Abilities", logoStyle);
            abilityDrawer.Draw(AbilitySpeed.GetGradeCount(speedProp.floatValue));
            EditorGUILayout.PropertyField(speedProp); 
            abilityDrawer.Draw(IncArrowDamageRate.GetGradeCount(incArrowDamageProp.floatValue));
            EditorGUILayout.PropertyField(incArrowDamageProp);
            abilityDrawer.Draw(IncProjectileDamage.GetGradeCount(projectileDamageProp.intValue));
            EditorGUILayout.PropertyField(projectileDamageProp);
            abilityDrawer.Draw(IncSpellDamage.GetGradeCount(spellDamageProp.intValue));
            EditorGUILayout.PropertyField(spellDamageProp);
            //abilityDrawer.Draw(ElementalActivation.GetGradeCount(elementalActivationProp.intValue));
            //EditorGUILayout.PropertyField(elementalActivationProp);
            //Draw Public Ability Properties
            GUILayout.Space(10f);
            GUILayout.Label("Public Abilities", logoStyle);
            abilityDrawer.Draw(AbilityDamage.GetGradeCount(damageIncProp.intValue));
            EditorGUILayout.PropertyField(damageIncProp);
            abilityDrawer.Draw(AbilityCritChance.GetGradeCount(criticalChanceProp.intValue));
            EditorGUILayout.PropertyField(criticalChanceProp);
            abilityDrawer.Draw(AbilityCritDamage.GetGradeCount(criticalDamageProp.floatValue));
            EditorGUILayout.PropertyField(criticalDamageProp);
            abilityDrawer.Draw(PenetrationArmor.GetGradeCount(armorPenetrationProp.intValue));
            EditorGUILayout.PropertyField(armorPenetrationProp);
        }
        GUILayout.EndVertical();

        #endregion

        #region ARROW_ITEM_INFO

        GUILayout.Label(ItemData_Editor.ArrowItemInfoText, EditorStyles.boldLabel);

        GUILayout.BeginVertical("GroupBox");
        GUILayout.Label("Prefab", EditorStyles.boldLabel);
        item.MainArrowObj = (GameObject)EditorGUILayout.ObjectField(ItemData_Editor.ArrowMainObjectText, item.MainArrowObj,
                                                                    typeof(GameObject), allowSceneObjects: false);

        item.LessArrowObj = (GameObject)EditorGUILayout.ObjectField(ItemData_Editor.ArrowLessObjectText, item.LessArrowObj,
                                                                    typeof(GameObject), allowSceneObjects: false);

        GUILayout.Space(5f);
        GUILayout.Label("Skill Slots", EditorStyles.boldLabel);
        item.ArrowSkillFst = (ArrowSkillData)EditorGUILayout.ObjectField("Skill Slot Fisrt", item.ArrowSkillFst,
                                                        typeof(ArrowSkillData), allowSceneObjects: false);

        item.ArrowSkillSec = (ArrowSkillData)EditorGUILayout.ObjectField("Skill Slot Seconds", item.ArrowSkillSec,
                                                        typeof(ArrowSkillData), allowSceneObjects: false);

        GUILayout.Space(5f);
        GUILayout.EndVertical();

        GUILayout.Label("Default Effect", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");
        //var effectGroupIndent = EditorGUI.indentLevel;
        //EditorGUI.indentLevel = 1;
        //EditorGUILayout.PropertyField(effectProp);
        //EditorGUI.indentLevel = effectGroupIndent;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Effect Array Length"); GUILayout.Space(70f);
        effectProp.arraySize = EditorGUILayout.IntField(effectProp.arraySize, new GUIStyle(GUI.skin.textField) { alignment = TextAnchor.MiddleCenter });
        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < effectProp.arraySize; i++) {
            EditorGUILayout.PropertyField(effectProp.GetArrayElementAtIndex(i), new GUIContent("Effect Element " + i));
        }
        GUILayout.EndVertical();

        #endregion

        #region GENERATE_BUTTON

        if (GUILayout.Button("GENERATE")) {
            EditorUtility.SetDirty(item);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            CatLog.Log($"<color=green>{item.NameByTerms} : Apply the Modified Value</color>");
            //changeChecker = false;
        }
        GUILayout.Space(5f);

        #endregion

        GUILayout.EndVertical();

        serialObject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(ItemData_Equip_Accessory))]
public class AccessItemData_Editor : Editor {
    ItemData_Equip_Accessory item;
    SerializedObject serialObject = null;
    SerializedProperty nameTermsProp = null;
    SerializedProperty descTermsProp = null;

    public void OnEnable() {
        item = (ItemData_Equip_Accessory)target;

        serialObject = new SerializedObject(target);
        nameTermsProp = serialObject.FindProperty(nameof(ItemData_Equip_Accessory.NameTerms));
        descTermsProp = serialObject.FindProperty(nameof(ItemData_Equip_Accessory.DescTerms));
    }

    public override void OnInspectorGUI()
    {
        if (item == null) return;

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((ItemData_Equip_Accessory)item), typeof(ItemData_Equip_Accessory), false);
        EditorGUI.EndDisabledGroup();

        //serializedObject.Update();

        GUILayout.Space(5);
        GUILayout.BeginVertical("HelpBox");
        //base.OnInspectorGUI();

        #region DEFAULT_ITEM_INFO

        GUILayout.Label(ItemData_Editor.DefaultItemInfoText, EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");

        EditorGUI.BeginDisabledGroup(true);
        item.Item_Type = (ITEMTYPE)EditorGUILayout.EnumPopup(ItemData_Editor.ItemTypeText, item.Item_Type);
        EditorGUI.EndDisabledGroup();

        item.Item_Grade = (ITEMGRADE)EditorGUILayout.EnumPopup(ItemData_Editor.ItemGradeText, item.Item_Grade);

        item.Item_Id = EditorGUILayout.TextField(ItemData_Editor.ItemIdText, item.Item_Id);

        //item.Item_Name = EditorGUILayout.TextField(ItemData_Editor.ItemNameText, item.Item_Name);
        //
        //EditorGUILayout.LabelField(ItemData_Editor.ItemDescText);
        //item.Item_Desc = EditorGUILayout.TextArea(item.Item_Desc, GUILayout.Height(50f));

        EditorGUILayout.PropertyField(nameTermsProp);
        EditorGUILayout.PropertyField(descTermsProp);

        EditorGUI.BeginDisabledGroup(true);
        item.DefaultAmount = EditorGUILayout.IntField("Default Amount", item.DefaultAmount);
        EditorGUI.EndDisabledGroup();

        item.Item_Sprite = (Sprite)EditorGUILayout.ObjectField(ItemData_Editor.ItemSpriteText, item.Item_Sprite,
                                                                typeof(Sprite), allowSceneObjects: true);

        GUILayout.EndVertical();

        #endregion

        #region EQUIPMENT_ITEM_INFO

        GUILayout.Label(ItemData_Editor.EquipmentItemText, EditorStyles.boldLabel);

        GUILayout.BeginVertical("GroupBox");

        EditorGUI.BeginDisabledGroup(true);
        item.Equip_Type = (EQUIP_ITEMTYPE)EditorGUILayout.EnumPopup(ItemData_Editor.EquipmentTypeText, item.Equip_Type);
        EditorGUI.EndDisabledGroup();

        GUILayout.EndVertical();

        #endregion

        #region ACCESSORY_ITEM_INFO

        GUILayout.Label(ItemData_Editor.AccessoryItemInfoText, EditorStyles.boldLabel);

        GUILayout.BeginVertical("GroupBox");

        //item.effect = EditorGUILayout.TextField(ItemData_Editor.AccessoryEffectText, item.effect);

        //item.Effect_AimSight = (MonoScript)EditorGUILayout.ObjectField("Effect Script", item.Effect_AimSight,
        //                                                    typeof(MonoScript), allowSceneObjects: false);

        //item.SPEffectType = (ACCESSORY_SPEFFECT_TYPE)EditorGUILayout.EnumPopup(ItemData_Editor.AccessorySpEffectText, item.SPEffectType);
        //
        //if (item.SpecialEffect != null) EditorGUILayout.LabelField("Loaded SP Effect : ", item.SpecialEffect.ToString());
        //else                            EditorGUILayout.LabelField("Loaded SP Effect : ", "NULL");

        //switch (item.SpecialEffect)
        //{
        //    case Acsp_AimSight effect:
        //        GUILayout.Label("AimSight Effect Property");
        //        effect.lineMat = (Material)EditorGUILayout.ObjectField("Line Renderer Material : ", effect.lineMat, typeof(Material), false);
        //        break;
        //    case Acsp_SlowTime effect: break;
        //    default: break;
        //}

        //GUILayout.Space(10f);
        //
        //GUILayout.Label("Aim Sight Effect Variables", EditorStyles.boldLabel);
        //
        //item.LineRenderMaterial = (Material)EditorGUILayout.ObjectField("Line Renderer Material : ", item.LineRenderMaterial, typeof(Material), false);

        GUILayout.Label("Special Effect Asset", EditorStyles.boldLabel);
        item.SPEffectAsset = (AccessorySkillData)EditorGUILayout.ObjectField("Skill Effect Asset", item.SPEffectAsset, typeof(AccessorySkillData), false);

        GUILayout.Space(10f); GUILayout.Label("Reinforcement Effect Asset", EditorStyles.boldLabel);
        //item.RFEffectAssets = 

        //foreach (var effect in item.RFEffectAssets)
        //{
        //    effect = (AccessoryRFSkillData)EditorGUILayout.ObjectField("RF Effect Asset", effect, typeof(AccessoryRFSkillData), allowSceneObjects: false);
        //}

        var serializedObject = new SerializedObject(target);
        var property = serializedObject.FindProperty("RFEffectAssets");
        serializedObject.Update();
        EditorGUILayout.PropertyField(property, true);
        serializedObject.ApplyModifiedProperties();

        GUILayout.EndVertical();

        #endregion

        #region GENERATE

        if (GUILayout.Button("GENERATE"))
        {
            EditorUtility.SetDirty(item);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            CatLog.Log($"<color=green>{item.NameByTerms} : Apply the Modified Value</color>");

            //item.SetAccessoryEffect();
            item.OnEnable(); //Accessory Special Effect & Reinforcement Effect 할당을 위해 OnEnable 호출
            //changeChecker = false;
        }

        GUILayout.Space(5f);

        #endregion

        GUILayout.EndVertical();
        serialObject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(ItemDt_SpArr))]
public class SpArrItemDataEditor : Editor {
    SerializedObject sobject = null;
    
    SerializedProperty ItemTypeProp;
    SerializedProperty ItemGradeProp;
    SerializedProperty ItemIdProp;
    SerializedProperty ItemAmountProp;
    SerializedProperty ItemSpriteProp;

    SerializedProperty nameTermsProp = null;
    SerializedProperty descTermsProp = null;

    SerializedProperty EquipTypeProp;
    SerializedProperty EquipAbilityProp;
    bool isAbilityTapFoldout = false;
    Texture2D enableStarTexture  = null;
    Texture2D disableStarTexture = null;

    SerializedProperty PrefProp;
    SerializedProperty SkillInfoFst;
    SerializedProperty SkillInfoSec;
    SerializedProperty SkillInfoTrd;

    SerializedProperty conditionTypeProp;
    SerializedProperty conditionMaxCostProp;
    SerializedProperty conditionMaxStackProp;
    SerializedProperty conditionCostIncProp;

    AbilityDrawer drawer = null;
    SerializedProperty additionalSpeedProp     = null;
    SerializedProperty arrowDamageProp         = null;
    SerializedProperty projectileDamageProp    = null;
    SerializedProperty spellDamageProp         = null;
    //SerializedProperty elementalActivationProp = null;
    SerializedProperty damageIncProp           = null;
    SerializedProperty criticalChanceProp      = null;
    SerializedProperty criticalDamageProp      = null;
    SerializedProperty armorPenetrationProp    = null;

    private void OnEnable() {
        sobject        = new SerializedObject(target);

        ItemTypeProp   = sobject.FindProperty(nameof(ItemDt_SpArr.Item_Type));
        ItemGradeProp  = sobject.FindProperty(nameof(ItemDt_SpArr.Item_Grade));
        ItemIdProp     = sobject.FindProperty(nameof(ItemDt_SpArr.Item_Id));
        ItemAmountProp = sobject.FindProperty(nameof(ItemDt_SpArr.DefaultAmount));
        ItemSpriteProp = sobject.FindProperty(nameof(ItemDt_SpArr.Item_Sprite));
        nameTermsProp  = sobject.FindProperty(nameof(ItemDt_SpArr.NameTerms));
        descTermsProp  = sobject.FindProperty(nameof(ItemDt_SpArr.DescTerms));

        EquipTypeProp    = sobject.FindProperty(nameof(ItemDt_SpArr.Equip_Type));
        EquipAbilityProp = sobject.FindProperty(nameof(ItemDt_SpArr.abilityDatas));
        enableStarTexture  = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/08.Sprites/ui_element/Scene_Main/Sprite_Icon/icon_star_grade_l.png");
        disableStarTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/08.Sprites/ui_element/Scene_Main/Sprite_Icon/icon_star_grade_l_d.png");
        if(enableStarTexture == null || disableStarTexture == null) {
            CatLog.WLog("Ability Star Texture is Null, Check the Texture Path");
        }

        PrefProp     = sobject.FindProperty(nameof(ItemDt_SpArr.MainArrowObj));
        SkillInfoFst = sobject.FindProperty(nameof(ItemDt_SpArr.ArrowSkillFst));
        SkillInfoSec = sobject.FindProperty(nameof(ItemDt_SpArr.ArrowSkillSec));
        SkillInfoTrd = sobject.FindProperty(nameof(ItemDt_SpArr.ArrowSkillTrd));

        conditionTypeProp     = sobject.FindProperty(nameof(ItemDt_SpArr.ChargeType));
        conditionMaxCostProp  = sobject.FindProperty(nameof(ItemDt_SpArr.MaxCost));
        conditionMaxStackProp = sobject.FindProperty(nameof(ItemDt_SpArr.MaxStackCount));
        conditionCostIncProp  = sobject.FindProperty(nameof(ItemDt_SpArr.CostIncrease));

        drawer = new AbilityDrawer();
        additionalSpeedProp     = sobject.FindProperty(nameof(ItemDt_SpArr.AdditionalSpeed));
        arrowDamageProp         = sobject.FindProperty(nameof(ItemDt_SpArr.ArrowDamageInc));
        projectileDamageProp    = sobject.FindProperty(nameof(ItemDt_SpArr.ProjectileDamageInc));
        spellDamageProp         = sobject.FindProperty(nameof(ItemDt_SpArr.SpellDamageInc));
        //elementalActivationProp = sobject.FindProperty(nameof(ItemDt_SpArr.ElementalActivation));
        damageIncProp           = sobject.FindProperty(nameof(ItemDt_SpArr.BaseDamageInc));
        criticalChanceProp      = sobject.FindProperty(nameof(ItemDt_SpArr.CriticalChanceInc));
        criticalDamageProp      = sobject.FindProperty(nameof(ItemDt_SpArr.CriticalDamageMultiplierInc));
        armorPenetrationProp    = sobject.FindProperty(nameof(ItemDt_SpArr.ArmorPenetrationInc));
    }

    public override void OnInspectorGUI() {
        sobject.Update();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((ItemDt_SpArr)target), typeof(ItemDt_SpArr), false);
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(5f);
        GUILayout.BeginVertical("HelpBox");

        #region Default_info
        GUILayout.Label("Default Item Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(ItemTypeProp);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.PropertyField(ItemGradeProp);
        EditorGUILayout.PropertyField(ItemIdProp);
        EditorGUILayout.PropertyField(nameTermsProp);
        EditorGUILayout.PropertyField(descTermsProp);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(ItemAmountProp);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.PropertyField(ItemSpriteProp, new GUIContent("Item Icon"));
        var itemIconSprite = AssetPreview.GetAssetPreview(ItemSpriteProp.objectReferenceValue);
        if (itemIconSprite != null) {
            //GUILayout.BeginArea(new Rect(100f, 100f, 100f, 100f)); 이 방법도 다른곳에서는 유용할 듯 !
            var previewLogoStyle = new GUIStyle();
            previewLogoStyle.fontSize = 18;
            previewLogoStyle.fontStyle = FontStyle.BoldAndItalic;
            previewLogoStyle.normal.textColor = new Color(1f, 1f, 1f);
            GUILayout.BeginHorizontal();
            GUILayout.Label("ICON SPRITE PREVIEW", previewLogoStyle);
            GUILayout.FlexibleSpace();
            GUILayout.Label(itemIconSprite, new GUIStyle(GUI.skin.box), GUILayout.Width(100f), GUILayout.Height(100f));
            GUILayout.EndHorizontal();
            //GUI.DrawTexture(new Rect(100f, 100f, 100f, 100f), itemIconSprite);
            //GUILayout.EndArea();
        }
        GUILayout.EndVertical();
        #endregion
        #region Equipment_Info
        GUILayout.Label("Equipment Item Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(EquipTypeProp);
        EditorGUI.EndDisabledGroup();
        var tempIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;
        isAbilityTapFoldout = EditorGUILayout.Foldout(isAbilityTapFoldout, "Ability Sliders", true);
        EditorGUI.indentLevel = tempIndent;
        isAbilityTapFoldout = true;
        if (isAbilityTapFoldout) {
            var logoStyles = new GUIStyle();
            logoStyles.fontSize = 16;
            logoStyles.fontStyle = FontStyle.BoldAndItalic;
            logoStyles.normal.textColor = new Color(1f, 1f, 1f);
            //Inherence Ability Fields
            GUILayout.Space(10f);
            GUILayout.Label("Inherence Abilities", logoStyles);
            drawer.Draw(AbilitySpeed.GetGradeCount(additionalSpeedProp.floatValue));
            EditorGUILayout.PropertyField(additionalSpeedProp);
            drawer.Draw(IncArrowDamageRate.GetGradeCount(arrowDamageProp.floatValue));
            EditorGUILayout.PropertyField(arrowDamageProp);
            drawer.Draw(IncProjectileDamage.GetGradeCount(projectileDamageProp.intValue));
            EditorGUILayout.PropertyField(projectileDamageProp);
            drawer.Draw(IncSpellDamage.GetGradeCount(spellDamageProp.intValue));
            EditorGUILayout.PropertyField(spellDamageProp);
            //drawer.Draw(ElementalActivation.GetGradeCount(elementalActivationProp.intValue));
            //EditorGUILayout.PropertyField(elementalActivationProp);

            //public ability fields
            GUILayout.Space(10f);
            GUILayout.Label("Public Abilities", logoStyles);
            drawer.Draw(AbilityDamage.GetGradeCount(damageIncProp.intValue));
            EditorGUILayout.PropertyField(damageIncProp);
            drawer.Draw(AbilityCritChance.GetGradeCount(criticalChanceProp.intValue));
            EditorGUILayout.PropertyField(criticalChanceProp);
            drawer.Draw(AbilityCritDamage.GetGradeCount(criticalDamageProp.floatValue));
            EditorGUILayout.PropertyField(criticalDamageProp);
            drawer.Draw(PenetrationArmor.GetGradeCount(armorPenetrationProp.intValue));
            EditorGUILayout.PropertyField(armorPenetrationProp);
        }
        GUILayout.EndVertical();
        #endregion
        #region Special_Arrow_Info
        GUILayout.Label("Special Arrow Info", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");
        EditorGUILayout.PropertyField(PrefProp);
        EditorGUILayout.PropertyField(SkillInfoFst, new GUIContent("Skill Info Fst"));
        EditorGUILayout.PropertyField(SkillInfoSec, new GUIContent("Skill Info Sec"));
        EditorGUILayout.PropertyField(SkillInfoTrd, new GUIContent("Skill Info Trd"));
        GUILayout.EndVertical();
        #endregion
        #region SPECIAL_CONDITION_INFO
        GUILayout.Label("Condition", EditorStyles.boldLabel);
        GUILayout.BeginVertical("GroupBox");
        EditorGUILayout.PropertyField(conditionTypeProp);
        EditorGUILayout.PropertyField(conditionMaxStackProp);
        EditorGUILayout.PropertyField(conditionMaxCostProp);
        EditorGUILayout.PropertyField(conditionCostIncProp);
        GUILayout.EndVertical();
        #endregion

        GUILayout.EndVertical();
        sobject.ApplyModifiedProperties();
    }

    void DrawStar(byte count) {
        byte maxStarCount = 5;
        GUILayout.BeginHorizontal();
        for (int i = 0; i < count; i++) {
            maxStarCount--;
            //Draw Enable Texture
            GUILayout.Label(enableStarTexture, GUILayout.Width(30f), GUILayout.Height(30f));
        }

        if (maxStarCount > 0) {
            for (int i = 0; i < maxStarCount; i++) {
                //Draw Disable Texture
                GUILayout.Label(disableStarTexture, GUILayout.Width(30f), GUILayout.Height(30f));
            }
        }
        GUILayout.EndHorizontal();
    }
}

internal sealed class AbilityDrawer {
    Texture2D enableStarTexture = null;
    Texture2D disableStarTexure = null;
    Texture2D halfStarTexture   = null;
    float textureSize  = 30f;
    bool isInitialized = false;

    internal AbilityDrawer() {
        enableStarTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/08.Sprites/ui_element/Scene_Main/Sprite_Icon/icon_star_grade_l.png");
        disableStarTexure = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/08.Sprites/ui_element/Scene_Main/Sprite_Icon/icon_star_grade_l_d.png");
        halfStarTexture   = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/08.Sprites/ui_element/Scene_Main/Sprite_Icon/icon_star_grade_l_half.png");
        if (enableStarTexture == null || disableStarTexure == null || halfStarTexture == null) {
            throw new System.Exception("Ability Grade Star Texture is Null. Check the Texture Path.");
        }
        isInitialized = true;
    }

    /// <summary>
    /// Draw Ability Grade Star
    /// </summary>
    /// <param name="gradeCount">Range 0~10</param>
    public void Draw(int gradeCount) {
        if(!isInitialized) {
            throw new System.Exception("Ability Preview is Not Initialzied.");
        }

        float calculatedGrade = (float)gradeCount / 2;
        bool isHalf = !((calculatedGrade % 1) == 0);
        byte totalStarCount = 5;
        GUILayout.BeginHorizontal();
        for (int i = 0; i < ((isHalf) ? calculatedGrade - 1 : calculatedGrade); i++) {
            GUILayout.Label(enableStarTexture, GUILayout.Width(textureSize), GUILayout.Height(textureSize));
            totalStarCount--;
        }
        if (isHalf) {
            GUILayout.Label(halfStarTexture, GUILayout.Width(textureSize), GUILayout.Height(textureSize));
            totalStarCount--;
        }
        for (int i = 0; i < totalStarCount; i++) {
            GUILayout.Label(disableStarTexure, GUILayout.Width(textureSize), GUILayout.Height(textureSize));
        }
        GUILayout.EndHorizontal();
    }
}

/// <summary>
/// Add Item Data Asset Create Button to CodingCat Menu
/// </summary>
public class CreateItemDataAsset
{
    [UnityEditor.MenuItem("ActionCat/Scriptable Object/Item Data Asset/Material Item Asset")]
    public static void CreateMaterialItemAsset()
    {
        string createAssetPath = "Assets/05.SO/SO.Item/2.Material/(code)-material-(name).asset";
        var asset = ScriptableObject.CreateInstance<ItemData_Mat>();
        AssetDatabase.CreateAsset(asset, createAssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [UnityEditor.MenuItem("ActionCat/Scriptable Object/Item Data Asset/Consumable Item Asset")]
    public static void CreateConsumableItemAsset()
    {
        string createAssetPath = "Assets/05.SO/SO.Item/1.Consume/(code)-consumable-(name).asset";
        var asset = ScriptableObject.CreateInstance<ItemData_Con>();
        AssetDatabase.CreateAsset(asset, createAssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [UnityEditor.MenuItem("ActionCat/Scriptable Object/Item Data Asset/Bow Item Asset")]
    public static void CreateBowItemAsset()
    {
        string createAssetPath = "Assets/05.SO/SO.Item/0.Equipment/Bow/(code)-bow-(name).asset";
        var asset = ScriptableObject.CreateInstance<ItemData_Equip_Bow>();
        AssetDatabase.CreateAsset(asset, createAssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [UnityEditor.MenuItem("ActionCat/Scriptable Object/Item Data Asset/Arrow Item Asset")]
    public static void CreateArrowItemAsset()
    {
        string createAssetPath = "Assets/05.SO/SO.Item/0.Equipment/Arrow/(code)-arrow-(name).asset";
        var asset = ScriptableObject.CreateInstance<ItemData_Equip_Arrow>();
        AssetDatabase.CreateAsset(asset, createAssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [UnityEditor.MenuItem("ActionCat/Scriptable Object/Item Data Asset/Accessory Item Asset")]
    public static void CreateAccessoryItemAsset() {
        string createAssetPath = "Assets/05.SO/SO.Item/0.Equipment/Aceessory/(code)-accessory-(name).asset";
        var asset = ScriptableObject.CreateInstance<ItemData_Equip_Accessory>();
        AssetDatabase.CreateAsset(asset, createAssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [MenuItem("ActionCat/Scriptable Object/Item Data Asset/Special Arrow Item Asset")]
    public static void CreateItemAssetSpArr() {
        string createAssetPath = "Assets/05.SO/SO.Item/0.Equipment/Arrow/Item_Special_Arrow.asset";
        var asset = ScriptableObject.CreateInstance<ItemDt_SpArr>();
        AssetDatabase.CreateAsset(asset, createAssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}

