namespace ActionCat.UI {
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class BluePrintInfoPopup : MonoBehaviour {
        [Header("SLOT")]
        [SerializeField] Image imageItemIcon = null;
        [SerializeField] Image imageFrame = null;
        [SerializeField] TextMeshProUGUI textItemAmount = null;
        [SerializeField] Sprite[] frames = null;

        [Header("DESCRIPTION")]
        [SerializeField] TextMeshProUGUI textDesc = null;

        [Header("REQUIREMENT SLOT")]
        [SerializeField] Crafting.CraftingRequirement[] matSlots = null;

        AD_item previewCache = null;

        public void SetPopup(AD_item item, CraftingRecipe recipe) {
            imageItemIcon.sprite = item.GetSprite;
            imageFrame.sprite    = frames[(int)item.GetGrade];
            textItemAmount.text  = item.GetAmount.ToString();
            textDesc.text        = item.GetDesc;

            var mats = recipe.Mats;
            byte enableSlotNumber = 0;
            for (int i = 0; i < mats.Length; i++) {
                GameManager.Instance.TryGetItemAmount(mats[i].Mateiral.Item_Id, out int amount);
                matSlots[i].SetSlot(mats[i].Mateiral, amount, mats[i].Required);
                enableSlotNumber++;
            }

            if(enableSlotNumber > 5) {
                CatLog.ELog("Material Slot Index Over !");
            }

            for (int i = enableSlotNumber; i < matSlots.Length; i++) {
                matSlots[i].Disable();
            }

            //그리고 여기서는 그냥 갯수체크만 진행하고, 실질적으로 제작가능하다 안된다 판단은 
            //CRAFT 버튼 눌렀을 때, CraftingFunction에서 쥐고있는 레시피 데이터로 진행시키면 되겠죠??
            //그리고 그걸로 SELECT SLOT으로 진행시킬때도 그렇게 진행시키면 되겠죠??

            //Set Preview ItemData Cache
            var result = recipe.Result;
            switch (result.Item.Item_Type) {
                case ITEMTYPE.ITEM_MATERIAL:
                    var material = result.Item as ItemData_Mat;
                    if (material) {
                        previewCache = new Item_Material(material, result.Count);
                    }
                    else {
                        throw new System.Exception("Not Matched ItemType. GetType: Material");
                    }
                    break;
                case ITEMTYPE.ITEM_CONSUMABLE:
                    var consumable = result.Item as ItemData_Con;
                    if (consumable) {
                        previewCache = new Item_Consumable(consumable, result.Count);
                    }
                    else {
                        throw new System.Exception("Not Matched ItemType. GetType: Consumable");
                    }
                    break;
                case ITEMTYPE.ITEM_EQUIPMENT:
                    switch (result.Item) {
                        case ItemData_Equip_Bow bowItemData:        previewCache = new Item_Bow(bowItemData);        break;
                        case ItemDt_SpArr sparrItemData:            previewCache = new Item_SpArr(sparrItemData);    break;
                        case ItemData_Equip_Arrow arrowItemData:    previewCache = new Item_Arrow(arrowItemData);    break;
                        case ItemData_Equip_Accessory artifactData: previewCache = new Item_Accessory(artifactData); break;
                        default: throw new System.NotImplementedException();
                    }
                    break;
                default: throw new System.NotImplementedException();
            }
        }

        public void BE_PREVIEW() {
            MainSceneRoute.OpenPreview(previewCache);
        }
    }
}
