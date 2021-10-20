using ActionCat;
using CodingCat_Scripts;
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
public class MatItemData_Editor : Editor
{
    ItemData_Mat item;

    private void OnEnable()
    {
        item = (ItemData_Mat)target;
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

        item.Item_Id = EditorGUILayout.IntField(ItemData_Editor.ItemIdText, item.Item_Id);

        item.Item_Name = EditorGUILayout.TextField(ItemData_Editor.ItemNameText, item.Item_Name);

        EditorGUILayout.LabelField(ItemData_Editor.ItemDescText);
        item.Item_Desc = EditorGUILayout.TextArea(item.Item_Desc, GUILayout.Height(50f));

        item.Item_Amount = EditorGUILayout.IntField(ItemData_Editor.ItemAmountText, item.Item_Amount);

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
            CatLog.Log($"<color=green>{item.Item_Name} : Apply the Modified Value</color>");
            //changeChecker = false;
        }
        GUILayout.Space(5f);

        #endregion

        GUILayout.EndVertical();

        //serializedObject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(ItemData_Con))]
public class ConItemData_Editor : Editor
{
    ItemData_Con item;

    public void OnEnable()
    {
        item = (ItemData_Con)target;
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

        item.Item_Id = EditorGUILayout.IntField(ItemData_Editor.ItemIdText, item.Item_Id);

        item.Item_Name = EditorGUILayout.TextField(ItemData_Editor.ItemNameText, item.Item_Name);

        EditorGUILayout.LabelField(ItemData_Editor.ItemDescText);
        item.Item_Desc = EditorGUILayout.TextArea(item.Item_Desc, GUILayout.Height(50f));

        item.Item_Amount = EditorGUILayout.IntField(ItemData_Editor.ItemAmountText, item.Item_Amount);

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
            CatLog.Log($"<color=green>{item.Item_Name} : Apply the Modified Value</color>");
            //changeChecker = false;
        }
        GUILayout.Space(5f);

        #endregion

        GUILayout.EndVertical();

        //serializedObject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(ItemData_Equip_Bow))]
public class BowItemData_Editor : Editor
{
    ItemData_Equip_Bow item;

    //bool changevalue;
    //string saveString = "";

    public void OnEnable()
    {
        item = (ItemData_Equip_Bow)target;
    }

    public override void OnInspectorGUI()
    {
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

        item.Item_Id = EditorGUILayout.IntField(ItemData_Editor.ItemIdText, item.Item_Id);

        item.Item_Name = EditorGUILayout.TextField(ItemData_Editor.ItemNameText, item.Item_Name);

        EditorGUILayout.LabelField(ItemData_Editor.ItemDescText);
        item.Item_Desc = EditorGUILayout.TextArea(item.Item_Desc, GUILayout.Height(50f));

        EditorGUI.BeginDisabledGroup(true);
        item.Item_Amount = EditorGUILayout.IntField(ItemData_Editor.ItemAmountText, item.Item_Amount);
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
            CatLog.Log($"<color=green>{item.Item_Name} : Apply the Modified Value</color>");
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
        

        serializedObject.ApplyModifiedProperties();
        
        //EditorApplication.update.Invoke();
    }
}

[CustomEditor(typeof(ItemData_Equip_Arrow))]
public class ArrowItemData_Editor : Editor
{
    ItemData_Equip_Arrow item;

    public void OnEnable()
    {
        item = (ItemData_Equip_Arrow)target;
    }

    public override void OnInspectorGUI()
    {
        if (item == null) return;

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((ItemData_Equip_Arrow)item), typeof(ItemData_Equip_Arrow), false);
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

        item.Item_Id = EditorGUILayout.IntField(ItemData_Editor.ItemIdText, item.Item_Id);

        item.Item_Name = EditorGUILayout.TextField(ItemData_Editor.ItemNameText, item.Item_Name);

        EditorGUILayout.LabelField(ItemData_Editor.ItemDescText);
        item.Item_Desc = EditorGUILayout.TextArea(item.Item_Desc, GUILayout.Height(50f));

        EditorGUI.BeginDisabledGroup(true);
        item.Item_Amount = EditorGUILayout.IntField(ItemData_Editor.ItemAmountText, item.Item_Amount);
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

        #region ARROW_ITEM_INFO

        GUILayout.Label(ItemData_Editor.ArrowItemInfoText, EditorStyles.boldLabel);

        GUILayout.BeginVertical("GroupBox");

        item.MainArrowObj = (GameObject)EditorGUILayout.ObjectField(ItemData_Editor.ArrowMainObjectText, item.MainArrowObj,
                                                                    typeof(GameObject), allowSceneObjects: false);

        item.LessArrowObj = (GameObject)EditorGUILayout.ObjectField(ItemData_Editor.ArrowLessObjectText, item.LessArrowObj,
                                                                    typeof(GameObject), allowSceneObjects: false);

        GUILayout.EndVertical();

        #endregion

        #region GENERATE_BUTTON

        if (GUILayout.Button("GENERATE"))
        {
            EditorUtility.SetDirty(item);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            CatLog.Log($"<color=green>{item.Item_Name} : Apply the Modified Value</color>");
            //changeChecker = false;
        }
        GUILayout.Space(5f);

        #endregion

        GUILayout.EndVertical();

        //serializedObject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(ItemData_Equip_Accessory))]
public class AccessItemData_Editor : Editor
{
    ItemData_Equip_Accessory item;

    public void OnEnable()
    {
        item = (ItemData_Equip_Accessory)target;
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

        item.Item_Id = EditorGUILayout.IntField(ItemData_Editor.ItemIdText, item.Item_Id);

        item.Item_Name = EditorGUILayout.TextField(ItemData_Editor.ItemNameText, item.Item_Name);

        EditorGUILayout.LabelField(ItemData_Editor.ItemDescText);
        item.Item_Desc = EditorGUILayout.TextArea(item.Item_Desc, GUILayout.Height(50f));

        EditorGUI.BeginDisabledGroup(true);
        item.Item_Amount = EditorGUILayout.IntField(ItemData_Editor.ItemAmountText, item.Item_Amount);
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
            CatLog.Log($"<color=green>{item.Item_Name} : Apply the Modified Value</color>");

            //item.SetAccessoryEffect();
            item.OnEnable(); //Accessory Special Effect & Reinforcement Effect 할당을 위해 OnEnable 호출
            //changeChecker = false;
        }

        GUILayout.Space(5f);

        #endregion

        GUILayout.EndVertical();

        //serializedObject.ApplyModifiedProperties();
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
        string createAssetPath = "Assets/05. Scriptable_Object/ItemAssets/2.Material/Material_Item.asset";
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
        string createAssetPath = "Assets/05. Scriptable_Object/ItemAssets/1.Consume/Consumable_Item.asset";
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
        string createAssetPath = "Assets/05. Scriptable_Object/ItemAssets/0.Equipment/Bow/Bow_Item.asset";
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
        string createAssetPath = "Assets/05. Scriptable_Object/ItemAssets/0.Equipment/Arrow/Arrow_Item.asset";
        var asset = ScriptableObject.CreateInstance<ItemData_Equip_Arrow>();
        AssetDatabase.CreateAsset(asset, createAssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [UnityEditor.MenuItem("ActionCat/Scriptable Object/Item Data Asset/Accessory Item Asset")]
    public static void CreateAccessoryItemAsset()
    {
        string createAssetPath = "Assets/05. Scriptable_Object/ItemAssets/0.Equipment/Aceessory/Accessory_Item.asset";
        var asset = ScriptableObject.CreateInstance<ItemData_Equip_Accessory>();
        AssetDatabase.CreateAsset(asset, createAssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}

