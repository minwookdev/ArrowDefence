using System.Collections.Generic;
using ActionCat;
using ActionCat.Data;
using UnityEditor;
using UnityEngine;

public class PlayerData_Editor : EditorWindow
{
    //SerializedObject so;
    public List<ItemInfo> invenInfo = new List<ItemInfo>();
    Vector2 scrollPos; //스크롤 바의 위치

    //Font Size GUI Style
    GUIStyle TitleStyle = new GUIStyle();

    //Bottom Info Message
    string Message = "Window에 마우스오버 또는 클릭하면 데이터가 즉시 갱신됩니다. \n" +
                     "**Player Data Class를 수정하지 않습니다.";

    public void SetItemInfo(List<AD_item> itemList)
    {
        this.invenInfo.Clear();
    
        if (itemList != null)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                invenInfo.Add(new ItemInfo(itemList[i]));
            }
        }
        else CatLog.Log("Item Info is null, return this Function");
    }

    //Add Menu item named "PlayerData_Editor" to the Window menu
    [MenuItem("ActionCat/Player Data Window")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PlayerData_Editor));
    }

    private void OnGUI()
    {
        SetItemInfo(CCPlayerData.inventory.GetAllItemList());

        TitleStyle.fontSize = 20;
        TitleStyle.fontStyle = FontStyle.BoldAndItalic;
        TitleStyle.normal.textColor = Color.white;
        GUILayout.Label("PlayerData Window (v0.5)", TitleStyle);

        GUILayout.BeginVertical("HelpBox");

        GUILayout.Label("ArrowDefenece PlayerData");

        #region Inventory_Info

        EditorGUILayout.LabelField("Inventory Data Field", EditorStyles.boldLabel);

        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty itemList = so.FindProperty("invenInfo");

        EditorGUILayout.LabelField("Total inventory Items : ", CCPlayerData.inventory.GetAllItemList().Count.ToString());

        scrollPos = GUI.BeginScrollView(new Rect(0, 60, 450, 600), scrollPos, new Rect(0, 60, 400, 1500));

        EditorGUILayout.PropertyField(itemList, true);

        GUI.EndScrollView();

        #endregion

        #region Equipment_Info

        EditorGUILayout.Space(10f);

        EditorGUILayout.LabelField("Equipment Data Field", EditorStyles.boldLabel);

        //Equipped Bow
        if (CCPlayerData.equipments.IsEquippedBow())
            EditorGUILayout.LabelField("Equipped Bow Item : ", CCPlayerData.equipments.GetBowItem().GetTermsName);
        else
            EditorGUILayout.LabelField("Euqippped Bow Item : ", "NULL");
        //Equipped Arrow (Main)
        if (CCPlayerData.equipments.IsEquippedArrowMain())
            EditorGUILayout.LabelField("Equipped Main Arrow Item : ", CCPlayerData.equipments.GetMainArrow().GetTermsName);
        else
            EditorGUILayout.LabelField("Equipped Main Arrow Item : ", "NULL");
        //Equipped Arrow (Sub)
        if (CCPlayerData.equipments.IsEquippedArrowSub())
            EditorGUILayout.LabelField("Equipped Sub Arrow Item : ", CCPlayerData.equipments.GetSubArrow().GetTermsName);
        else 
            EditorGUILayout.LabelField("Equipped Sub Arrow Item : ", "NULL");

        EditorGUILayout.Space(10f);

        //Get Array Accessories
        var accessories = CCPlayerData.equipments.GetAccessories();

        //Equipped Accessory (f)
        if (CCPlayerData.equipments.IsEquippedAccessory(0))
            EditorGUILayout.LabelField("Equipped Accessory [0] : ", accessories[0].GetTermsName);
        else
            EditorGUILayout.LabelField("Equipped Accessory [0] : ", "NULL");
        //Equipped Accessory (s)
        if (CCPlayerData.equipments.IsEquippedAccessory(1))
            EditorGUILayout.LabelField("Equipped Accessory [1] : ", accessories[1].GetTermsName);
        else
            EditorGUILayout.LabelField("Equipped Accessory [1] : ", "NULL");
        //Equipped Accessory (t)
        if (CCPlayerData.equipments.IsEquippedAccessory(2))
            EditorGUILayout.LabelField("Equipped Accessory [2] : ", accessories[2].GetTermsName);
        else
            EditorGUILayout.LabelField("Equipped Accessory [2] : ", "NULL");


        #endregion

        #region Data_Info

        //Another Player's Data in here

        #endregion

        GUILayout.Space(100);

        GUILayout.FlexibleSpace();  //화면 하단에 배치
        GUILayout.EndVertical();
        GUILayout.Label(Message, EditorStyles.boldLabel);

    }

    //요거 키면 Window 열고있을때 바로바로 업데이트됨 근데 에디터 많이 느려질 수도 있음
    //특정 조건에서만 켜던지 하자
    //private void OnInspectorUpdate()
    //{
    //    Repaint();
    //}
}

[System.Serializable]
public class ItemInfo
{
    public string Name;
    public string Id;
    public string Desc;
    public string Type;
    public string Grade;
    public string Amount;

    public string EquipType;

    public string SkillString_0;
    public string SkillString_1;

    private string empty = "x";

    public ItemInfo(AD_item item)
    {
        switch (item)
        {
            case Item_Material matitem:     InitItem_Mat(matitem);       break;
            case Item_Consumable conitem:   InitItem_Con(conitem);       break;
            case Item_Bow bowitem:          InitItem_Bow(bowitem);       break;
            case Item_Arrow arrowitem:      InitItem_Arrow(arrowitem);   break;
            case Item_Accessory accessitem: InitItem_Access(accessitem); break;
            default: break;
        }
    }

    public void InitItem_Mat (Item_Material item) 
    {
        this.Name   = item.GetTermsName;
        this.Id     = item.GetID.ToString();
        this.Desc   = item.GetTermsDesc;
        this.Type   = item.GetItemType.ToString();
        this.Grade  = item.GetGrade.ToString();
        this.Amount = item.GetAmount.ToString();

        this.EquipType = empty;
        this.SkillString_0 = empty;
        this.SkillString_1 = empty;
    }

    public void InitItem_Con (Item_Consumable item)
    {
        this.Name   = item.GetTermsName;
        this.Id     = item.GetID.ToString();
        this.Desc   = item.GetTermsDesc;
        this.Type   = item.GetItemType.ToString();
        this.Grade  = item.GetGrade.ToString();
        this.Amount = item.GetAmount.ToString();

        this.EquipType = empty;
        this.SkillString_0 = empty;
        this.SkillString_1 = empty;
    }

    public void InitItem_Bow (Item_Bow item)
    {
        this.Name   = item.GetTermsName;
        this.Id     = item.GetID.ToString();
        this.Desc   = item.GetTermsDesc;
        this.Type   = item.GetItemType.ToString();
        this.Grade  = item.GetGrade.ToString();
        this.Amount = item.GetAmount.ToString();

        this.EquipType = item.GetEquipType().ToString();

        for (int i = 0; i < item.GetSkillsOrNull().Length; i++)
        {
            if(item.GetSkillsOrNull()[i] != null)
            {
                if (i == 0) SkillString_0 = item.GetSkillsOrNull()[i].ToString();
                else if (i == 1)            item.GetSkillsOrNull()[i].ToString();
            }
            else
            {
                if (i == 0)      SkillString_0 = empty;
                else if (i == 1) SkillString_1 = empty;
            }
        } 
    }

    public void InitItem_Arrow (Item_Arrow item)
    {
        this.Name   = item.GetTermsName;
        this.Id     = item.GetID.ToString();
        this.Desc   = item.GetTermsDesc;
        this.Type   = item.GetItemType.ToString();
        this.Grade  = item.GetGrade.ToString();
        this.Amount = item.GetAmount.ToString();

        this.EquipType = item.GetEquipType().ToString();

        SkillString_0 = empty;
        SkillString_1 = empty;
    }
    
    public void InitItem_Access (Item_Accessory item)
    {
        this.Name   = item.GetTermsName;
        this.Id     = item.GetID.ToString();
        this.Desc   = item.GetTermsDesc;
        this.Type   = item.GetItemType.ToString();
        this.Grade  = item.GetGrade.ToString();
        this.Amount = item.GetAmount.ToString();

        this.EquipType = item.GetEquipType().ToString();

        SkillString_0 = empty;
        SkillString_1 = empty;
    }
}

