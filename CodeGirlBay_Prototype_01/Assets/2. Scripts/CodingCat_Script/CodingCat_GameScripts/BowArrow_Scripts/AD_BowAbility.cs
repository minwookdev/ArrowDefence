namespace CodingCat_Games
{
    using CodingCat_Scripts;
    using UnityEngine;

    public class AD_BowAbility : MonoBehaviour
    {
        private AD_BowController bowController;
        public AD_PlayerData playerData;

        private void Start()
        {
            bowController = gameObject.GetComponent<AD_BowController>();

            if (AD_PlayerData.PlayerEquipments.MainBow != null)
            {
                Item_Bow weapon = AD_PlayerData.PlayerEquipments.MainBow;

                for (int i = 0; i < weapon.GetBowSkills().Length; i++)
                {
                    if (weapon.GetBowSkills()[i] != null)
                        bowController.bowSkillSet += weapon.GetBowSkills()[i].BowSpecialSkill;
                    else CatLog.WLog($"{i} Skill Slot NULL");
                }
            }
            else CatLog.WLog("Equip Bow Item is Null");
        }
    }
}
