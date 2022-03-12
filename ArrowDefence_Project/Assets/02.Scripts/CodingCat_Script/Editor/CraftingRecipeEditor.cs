using UnityEditor;
using UnityEngine;
using ActionCat;

[CustomEditor(typeof(CraftingRecipeSO))]
public class CraftingRecipeEditor : Editor {
    protected SerializedObject sobject;
    protected SerializedProperty recipeDictionaryProp;
    protected SerializedProperty recipeArrayProp;
    protected SerializedProperty versionProp;
    CraftingRecipeSO craftingSO;

    public void OnEnable() {
        sobject = new SerializedObject(target);
        recipeDictionaryProp = sobject.FindProperty(nameof(CraftingRecipeSO.RecipeDictionary));
        recipeArrayProp = sobject.FindProperty(nameof(CraftingRecipeSO.RecipeArray));
        versionProp = sobject.FindProperty(nameof(CraftingRecipeSO.version));

        craftingSO = target as CraftingRecipeSO;
    }

    public override void OnInspectorGUI() {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((CraftingRecipeSO)target), typeof(CraftingRecipeSO), false);
        EditorGUI.EndDisabledGroup();

        sobject.Update();
        GUILayout.Space(5f);
        EditorGUILayout.BeginVertical("HelpBox");

        #region DRAW_TOP
        GUILayout.BeginVertical("GroupBox");
        EditorGUILayout.PropertyField(versionProp);
        string initializedCraftingRecipe = (craftingSO.RecipeDictionary != null && craftingSO.RecipeDictionary.Count > 0) ? craftingSO.RecipeDictionary.Count.ToString() : "0";
        GUILayout.BeginHorizontal();
        GUILayout.Label($"Initialized Crafting Recipe: ");
        GUILayout.Label(initializedCraftingRecipe);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        #endregion

        #region DRAW_CRAFTING_ARRAY
        EditorGUILayout.BeginHorizontal("GroupBox");
        GUILayout.Label("Crafting Recipe Size: ");
        recipeArrayProp.arraySize = EditorGUILayout.IntField(recipeArrayProp.arraySize);
        EditorGUILayout.EndHorizontal();

        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;

        for (int i = 0; i < recipeArrayProp.arraySize; i++) {
            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.PropertyField(recipeArrayProp.GetArrayElementAtIndex(i), new GUIContent("RECIPE INDEX " + i));
            GUILayout.EndVertical();
        }
        #endregion

        EditorGUILayout.EndVertical();
        sobject.ApplyModifiedProperties();
    }
}

[CustomPropertyDrawer(typeof(CraftingRecipe))]
public class CraftingRecipeArrayDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        int originalIndentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;
        if(property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label)) {
            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(CraftingRecipe.BluePrint)));
            GUILayout.EndVertical();

            GUILayout.BeginVertical("GroupBox");
            SerializedProperty materialArray = property.FindPropertyRelative(nameof(CraftingRecipe.Mats));
            GUILayout.BeginHorizontal();
            GUILayout.Space(15f);
            GUILayout.Label("Materials");
            GUILayout.Space(20f);
            materialArray.arraySize = EditorGUILayout.IntField(materialArray.arraySize);
            Rect expandedPosition = position;
            expandedPosition.x += 80f;
            expandedPosition.y += 85f;
            materialArray.isExpanded = EditorGUI.Foldout(expandedPosition, materialArray.isExpanded, new GUIContent(""));
            //if(materialArray.isExpanded = EditorGUI.Foldout())
            GUILayout.EndHorizontal();
            if (materialArray.isExpanded) {
                for (int i = 0; i < materialArray.arraySize; i++) {
                    EditorGUILayout.PropertyField(materialArray.GetArrayElementAtIndex(i), GUILayout.Height(0f));
                }
            }
            GUILayout.EndVertical();
            
            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(CraftingRecipe.ResultItem)));
            GUILayout.EndVertical();
        }
        EditorGUI.indentLevel = originalIndentLevel;
    }
}

[CustomPropertyDrawer(typeof(CraftingRecipe.CraftingMaterial))]
public class MatsArrayDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        GUILayout.BeginHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(15f);
        GUILayout.Label("Mateial");
        EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(CraftingRecipe.CraftingMaterial.Mateiral)), new GUIContent(""));
        GUILayout.EndHorizontal();

        GUILayout.Space(15f);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Required");
        EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(CraftingRecipe.CraftingMaterial.Required)), new GUIContent(""));
        GUILayout.EndHorizontal();

        GUILayout.EndHorizontal();
    }
}
