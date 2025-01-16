namespace ActionCat.UI {
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using ActionCat;

    public class RequirementSlot : MonoBehaviour {
        [Header("COMPONENT")]
        [SerializeField] Image imageIcon  = null;
        [SerializeField] Image imageFrame = null;
        [SerializeField] TextMeshProUGUI textAmountCondition = null;
        [SerializeField] TextMeshProUGUI textNeedAmount      = null;
        [SerializeField] TextMeshProUGUI textType            = null;

        [Header("FRAMES")]
        [SerializeField] Sprite[] frames = null;

        string stringColorRed   = "<color=red>";
        string stringColorGreen = "<color=green>";
        string stringColorEnd   = "</color>";
        bool isRequirementCondition = false;
        //string stringColorLime = "<color=lime>";
        
        public bool IsRequirementCondition {
            get {
                if(gameObject.activeSelf && isRequirementCondition) {
                    return true;
                }
                return false;
            }
        }

        public void DisableSlot() {
            gameObject.SetActive(false);
        }

        public void EnableSlot(UpgradeRecipe.Material material) {
            var entity = material.Mat;
            if(entity == null) {
                throw new System.Exception();
            }

            //find item player's inventory
            GameManager.Instance.TryGetItemAmount(entity.Item_Id, out int amount);

            imageFrame.sprite        = frames[(int)entity.Item_Grade];
            imageIcon.sprite         = entity.Item_Sprite;
            textType.text            = entity.GetItemTypeStr(true);
            textAmountCondition.text = GetAmountCondition(material.Required, amount, out string needamountstring);
            textNeedAmount.text      = needamountstring;

            isRequirementCondition = (amount >= material.Required);
            gameObject.SetActive(true);
        }

        string GetAmountCondition(int needAmount, int possessions, out string needAmountString) {
            string stringColor = (needAmount > possessions) ? stringColorRed : stringColorGreen;
            needAmountString   = ((needAmount - possessions) < 0) ? "0" : (needAmount - possessions).ToString(); 
            return string.Format("[ {2}{0}{3} / {1} ]", possessions, needAmount, stringColor, stringColorEnd);
        }

    }
}
