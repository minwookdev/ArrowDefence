using System.Collections.Generic;
using UnityEngine;
using CodingCat_Games;
using CodingCat_Scripts;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Object Asset/PlayerData")]
public class AD_PlayerData : ScriptableObject
{
    public int PlayerHP = 0;

    [Space(20)]
    [SerializeField]
    public static Player_Equipments PlayerEquipments = new Player_Equipments();

    public ItemData_Equip_Bow TempEquip_Bow;
    public ItemData_Equip_Arrow TempEquip_Arrow_Main;
    public ItemData_Equip_Arrow TempEquip_Arrow_Less;

    [Space(20)]
    [SerializeField]
    public AD_Inventory inventory = new AD_Inventory();

    [Space(20)]
    [SerializeField]
    private List<ItemData> tempItems = new List<ItemData>();

    //[Space(20)]
    //public Object UnityObject;

    private void OnDisable()
    {
        inventory.ClearInventory();
    }

    public void SetTestItems()
    {
        for (int i = 0; i < tempItems.Count; i++)
        {
            inventory.AddItem(tempItems[i]);
        }

        //if (TempEquip_Bow != null)
        //{
        //    PlayerEquipments.EquipBowItem(TempEquip_Bow);
        //}
    }

    public List<ItemData> GetItemData() => tempItems;
}
