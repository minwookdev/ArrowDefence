using CodingCat_Games;
using CodingCat_Scripts;
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

        item.BowGameObject = (GameObject)EditorGUILayout.ObjectField(ItemData_Editor.BowObjectText, item.BowGameObject,
                                                                     typeof(GameObject), allowSceneObjects: false);

        item.FirstSkill_Type  = (BOWSKILL_TYPE)EditorGUILayout.EnumPopup(ItemData_Editor.BowSkillSlotText, item.FirstSkill_Type);
        item.SecondSkill_Type = (BOWSKILL_TYPE)EditorGUILayout.EnumPopup(ItemData_Editor.BowSkillSlotText, item.SecondSkill_Type);

        EditorGUILayout.Space(5f);

        if (item.BowSkill_First != null) EditorGUILayout.LabelField(ItemData_Editor.BowSkillEnable + " 0 : ", item.BowSkill_First.ToString());
        else EditorGUILayout.LabelField(ItemData_Editor.BowSkillEnable + " 0 : ", "SKILL SLOT 0 NULL");

        if (item.BowSkill_Second != null) EditorGUILayout.LabelField(ItemData_Editor.BowSkillEnable + " 1 : ", item.BowSkill_Second.ToString());
        else EditorGUILayout.LabelField(ItemData_Editor.BowSkillEnable + " 1 : ", "SKILL SLOT 1 NULL");

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

        item.effect = EditorGUILayout.TextField(ItemData_Editor.AccessoryEffectText, item.effect);

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
