using System.Collections.Generic;
using UnityEngine;
using ActionCat;

public class TITable : ScriptableObject
{
    [Header("TEMP ITEM LIST")]
    [SerializeField] 
    private List<ItemData> tempItems = new List<ItemData>();

    public List<ItemData> GetItemData() => tempItems;
}

#if UNITY_EDITOR
public static class CreateTempPlayerData
{
    [UnityEditor.MenuItem("ActionCat/Scriptable Object/Temp PlayerData Asset")]
    public static void CreateTemp()
    {
        string assetPath = "Assets/05. Scriptable_Object/Temp_PlayerData.asset";
        var asset = ScriptableObject.CreateInstance<TITable>();
        UnityEditor.AssetDatabase.CreateAsset(asset, assetPath);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();

        UnityEditor.EditorUtility.FocusProjectWindow();
        UnityEditor.Selection.activeObject = asset;
    }
}
#endif
