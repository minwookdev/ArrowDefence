using UnityEngine;
using UnityEditor;
using ActionCat;

[CustomEditor(typeof(ItemDropList))]
public class DropList_Editor : Editor
{
    SerializedObject sobject;

    SerializedProperty dropListProp;

    GUIStyle labelColorGreen = new GUIStyle();
    GUIStyle labelColorRed = new GUIStyle();

    public void OnEnable()
    {
        sobject = new SerializedObject(target);
        dropListProp = sobject.FindProperty("DropTableArray");

        labelColorGreen.normal.textColor = Color.green;
        labelColorRed.normal.textColor = Color.red;
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((ItemDropList)target),
                                                                       typeof(ItemDropList), false);
        EditorGUI.EndDisabledGroup();

        sobject.Update();

        GUILayout.BeginVertical("HelpBox");

        EditorGUILayout.Space(10f);

        //Drop Item Array Length
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Total Drop List Item Counts : ");
        dropListProp.arraySize = EditorGUILayout.IntField(dropListProp.arraySize);
        EditorGUILayout.EndHorizontal();

        //Item Drop Total Chance
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Drop Rate Total");
        EditorGUI.BeginDisabledGroup(true);
        float totalDropChance = 0;
        for (int i = 0; i < dropListProp.arraySize; i++)
        {
            totalDropChance += dropListProp.GetArrayElementAtIndex(i).FindPropertyRelative("DropChance").floatValue;
        }
        if (totalDropChance == 100) GUILayout.Label(totalDropChance.ToString() + " %    /    100 %", labelColorGreen);
        else                        GUILayout.Label(totalDropChance.ToString() + " %    /    100 %", labelColorRed);

        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        //EditorGUILayout.PropertyField(dropListProp);

        for (int i = 0; i < dropListProp.arraySize; i++)
        {
            EditorGUILayout.BeginVertical("GroupBox");
            EditorGUILayout.PropertyField(dropListProp.GetArrayElementAtIndex(i), new GUIContent("DROP ITEM" + i));
            EditorGUILayout.EndVertical();

            //dropListProp.GetArrayElementAtIndex(i).Find
        }

        GUILayout.EndVertical();

        sobject.ApplyModifiedProperties();
    }
}

[CustomPropertyDrawer(typeof(ItemDropList.DropTable))]
public class DropTableArrayDraw : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);

        //IndentLevel -> 공간 띄우기 [tab]효과
        int oldIndentLevel = EditorGUI.indentLevel;
        label = EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = position;

        Rect correctionPos = position;
        correctionPos.x = correctionPos.x + 5f;

        //Inspector ▼ 모양의 여닫기
        if(property.isExpanded == EditorGUI.Foldout(correctionPos, property.isExpanded, label))
        {
            //Array 내부 원소당 높이 합쳐서 16이상
            if(position.height > 16f)
            {
                //position.height = 16f;
                //EditorGUI.indentLevel += 1; //더 띄워줌
                //contentPosition = EditorGUI.IndentedRect(position);
                //contentPosition.y += 18f;
            }
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 1;
            EditorGUILayout.BeginVertical();
            //Array 내부 변수 하나씩 다 잡아줌. 따로 원하는 위치에 하고싶은 경우 EditorGUI.PropertyField() 사용
            //Rect는 position을 변형하여 사용해주면 된다
            EditorGUILayout.PropertyField(property.FindPropertyRelative("ItemAsset"), new GUIContent("Item Asset"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("DropChance"), new GUIContent("Item Drop Chance"));
            EditorGUILayout.BeginVertical("GroupBox");
            //EditorGUILayout.PropertyField(property.FindPropertyRelative("QuantityRange"), new GUIContent("Drop Quantity Range"));

            //Quantity Array Space
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Drop Quantity Range");
            property.FindPropertyRelative("QuantityRange").arraySize = EditorGUILayout.IntField(property.FindPropertyRelative("QuantityRange").arraySize);
            EditorGUILayout.EndHorizontal();

            //Limit Quantity Array Length, Less than 3
            if (property.FindPropertyRelative("QuantityRange").arraySize > 2 || property.FindPropertyRelative("QuantityRange").arraySize <= 0)
            {
                property.FindPropertyRelative("QuantityRange").arraySize = 2;
                CodingCat_Scripts.CatLog.WLog("The Length of a Quantity Range Array can only have a value of 1 or 2.");
            }

            int quantityArrayLength = property.FindPropertyRelative("QuantityRange").arraySize;

            //Quantity Array Element Space
            EditorGUILayout.BeginHorizontal();
            string labelString = (quantityArrayLength >= 2) ? "Between" : "Single";
            GUILayout.Label(labelString);
            for (int i = 0; i < quantityArrayLength; i++)
            {
                //EditorGUILayout.PropertyField(property.FindPropertyRelative("QuantityRange").GetArrayElementAtIndex(i).);
                property.FindPropertyRelative("QuantityRange").GetArrayElementAtIndex(i).intValue =
                    EditorGUILayout.IntField(property.FindPropertyRelative("QuantityRange").GetArrayElementAtIndex(i).intValue);
                if (i != quantityArrayLength - 1)
                    GUILayout.Label(" ~ ");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = indent;
            EditorGUILayout.EndVertical();
        }
    }
}
