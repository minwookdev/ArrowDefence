using System.Collections.Generic;
using CodingCat_Games;
using CodingCat_Games.Data;
using CodingCat_Scripts;
using UnityEditor;
using UnityEngine;

public class PlayerData_Editor : EditorWindow
{
    //SerializedObject so;
    public List<ItemInfo> invenInfo = new List<ItemInfo>();
    Vector2 scrollPos; //스크롤 바의 위치

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
    [MenuItem("CodingCat/Data Window")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PlayerData_Editor));
    }

    private void OnGUI()
    {
        SetItemInfo(CCPlayerData.inventory.GetAllItemList());

        GUILayout.Label("PlayerData Window (v0.3)", EditorStyles.boldLabel);

        GUILayout.BeginVertical("HelpBox");

        GUILayout.Label("ArrowDefenece PlayerData");

        EditorGUILayout.LabelField("Total inventory Items : ", CCPlayerData.inventory.GetAllItemList().Count.ToString());

        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty itemList = so.FindProperty("invenInfo");

        scrollPos = GUI.BeginScrollView(new Rect(0, 60, 450, 600), scrollPos, new Rect(0, 60, 400, 1500));

        EditorGUILayout.PropertyField(itemList, true);

        GUI.EndScrollView();

        GUILayout.Space(100);

        GUILayout.FlexibleSpace();  //화면 하단에 배치
        GUILayout.EndVertical();
        GUILayout.Label("Editor Window의 데이터 갱신이 느릴 수 있습니다." + '\n' +
                        "Window에 마우스오버 또는 클릭하면 데이터가 갱신됩니다.");

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
        this.Name   = item.GetName;
        this.Id     = item.GetID.ToString();
        this.Desc   = item.GetDesc;
        this.Type   = item.GetItemType.ToString();
        this.Grade  = item.GetGrade.ToString();
        this.Amount = item.GetAmount.ToString();

        this.EquipType = empty;
        this.SkillString_0 = empty;
        this.SkillString_1 = empty;
    }

    public void InitItem_Con (Item_Consumable item)
    {
        this.Name   = item.GetName;
        this.Id     = item.GetID.ToString();
        this.Desc   = item.GetDesc;
        this.Type   = item.GetItemType.ToString();
        this.Grade  = item.GetGrade.ToString();
        this.Amount = item.GetAmount.ToString();

        this.EquipType = empty;
        this.SkillString_0 = empty;
        this.SkillString_1 = empty;
    }

    public void InitItem_Bow (Item_Bow item)
    {
        this.Name   = item.GetName;
        this.Id     = item.GetID.ToString();
        this.Desc   = item.GetDesc;
        this.Type   = item.GetItemType.ToString();
        this.Grade  = item.GetGrade.ToString();
        this.Amount = item.GetAmount.ToString();

        this.EquipType = item.GetEquipType().ToString();

        for (int i = 0; i < item.GetBowSkills().Length; i++)
        {
            if(item.GetBowSkills()[i] != null)
            {
                if (i == 0) SkillString_0 = item.GetBowSkills()[i].ToString();
                else if (i == 1)            item.GetBowSkills()[i].ToString();
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
        this.Name   = item.GetName;
        this.Id     = item.GetID.ToString();
        this.Desc   = item.GetDesc;
        this.Type   = item.GetItemType.ToString();
        this.Grade  = item.GetGrade.ToString();
        this.Amount = item.GetAmount.ToString();

        this.EquipType = item.GetEquipType().ToString();

        SkillString_0 = empty;
        SkillString_1 = empty;
    }
    
    public void InitItem_Access (Item_Accessory item)
    {
        this.Name   = item.GetName;
        this.Id     = item.GetID.ToString();
        this.Desc   = item.GetDesc;
        this.Type   = item.GetItemType.ToString();
        this.Grade  = item.GetGrade.ToString();
        this.Amount = item.GetAmount.ToString();

        this.EquipType = item.GetEquipType().ToString();

        SkillString_0 = empty;
        SkillString_1 = empty;
    }
}

