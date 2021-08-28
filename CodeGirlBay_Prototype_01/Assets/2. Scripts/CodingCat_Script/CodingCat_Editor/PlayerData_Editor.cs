using System.Collections.Generic;
using CodingCat_Games;
using CodingCat_Games.Data;
using CodingCat_Scripts;
using UnityEditor;
using UnityEngine;

public class PlayerData_Editor : EditorWindow
{
    SerializedObject so;
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

    private void OnEnable()
    {
        //scriptable = new ScriptableObject(CCPlayerData.inventory);
        //inven = new SerializedObject(CCPlayerData.inventory);
        //itemList = CCPlayerData.inventory.GetItemList();
        //
        //ScriptableObject target = this;
        //so = new SerializedObject(target);
    }

    private void OnGUI()
    {
        SetItemInfo(CCPlayerData.inventory.GetAllItemList());

        GUILayout.Label("PlayerData Window", EditorStyles.boldLabel);

        GUILayout.BeginVertical("HelpBox");

        GUILayout.Label("ArrowDefenece PlayerData");

        //EditorGUILayout.LabelField("Player int Value : ", CCPlayerData.user_int.ToString());
        //
        //GUILayout.Space(10);

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
    public string Id;
    public string Name;
    public string Desc;
    public string Grade;
    public string Amount;
    public string Type;
    public string EquipType;
    public Sprite ItemSprite;

    public ItemInfo(AD_item item)
    {
        this.Id         = item.GetID.ToString();
        this.Name       = item.GetName;
        this.Desc       = item.GetDesc;
        this.Grade      = item.GetGrade.ToString();
        this.Amount     = item.GetAmount.ToString();
        this.Type       = item.GetItemType.ToString();
        this.ItemSprite = item.GetSprite;

        //switch (item)
        //{
        //    case Item_Material matitem:     break;
        //    case Item_Consumable conitem:   break;
        //    case Item_Bow bowitem:          break;
        //    case Item_Arrow arrowitem:      break;
        //    case Item_Accessory accessitem: break;
        //    default: break;
        //}
    }
}

