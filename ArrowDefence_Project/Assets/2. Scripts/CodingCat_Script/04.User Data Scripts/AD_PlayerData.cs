using System.Collections.Generic;
using UnityEngine;
using ActionCat;
using CodingCat_Scripts;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Object Asset/PlayerData")]
public class AD_PlayerData : ScriptableObject
{
    [Header("TEMP ITEM LIST")]
    [SerializeField]
    private List<ItemData> tempItems = new List<ItemData>();

    public List<ItemData> GetItemData() => tempItems;
}
