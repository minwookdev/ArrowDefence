using CodingCat_Games;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

public static class ItemData_Editor
{
    public const string DefaultItemInfoText = "  Default Item Info";
    public const string ItemTypeText        = "Item Type";
    public const string ItemGradeText       = "Item Grade";
    public const string ItemIdText          = "Item ID";
    public const string ItemNameText        = "Item Name";
    public const string ItemDescText        = "Item Description";
    public const string ItemAmountText      = "Item Amount";
    public const string ItemSpriteText      = "Item Sprite";

    public const string EquipmentItemText = "Equipment Item Info";
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

        //serializedObject.Update();

        GUILayout.Space(10);
        GUILayout.BeginVertical("HelpBox");
        //base.OnInspectorGUI();

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

        //serializedObject.Update();

        GUILayout.Space(10);
        GUILayout.BeginVertical("HelpBox");
        //base.OnInspectorGUI();

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

        GUILayout.EndVertical();

        //serializedObject.ApplyModifiedProperties();
    }
}

#endif
