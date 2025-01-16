namespace ActionCat {
    using UnityEngine;

    [CreateAssetMenu(fileName = "Item_Material_Asset", menuName = "Scriptable Object Asset/Item_Mat_Asset")]
    public class ItemData_Mat : ItemData {
        ItemData_Mat() {
            Item_Type = ITEMTYPE.ITEM_MATERIAL;
        }
    }
}
