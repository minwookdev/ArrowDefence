namespace CodingCat_Games
{
    using CodingCat_Games.Data;
    using CodingCat_Scripts;
    using UnityEngine;

    public class AD_BowAbility : MonoBehaviour
    {
        private AD_BowController bowController;

        private void Start()
        {
            bowController = gameObject.GetComponent<AD_BowController>();

            if (CCPlayerData.equipments.GetBowItem() != null)
            {
                Item_Bow weapon = CCPlayerData.equipments.GetBowItem();

                for (int i = 0; i < weapon.GetBowSkills().Length; i++)
                {
                    if (weapon.GetBowSkills()[i] != null)
                    {
                        bowController.bowSkillSet += weapon.GetBowSkills()[i].BowSpecialSkill;
                        CatLog.WLog($"Skill Slot {i} Init, Skill Name : {weapon.GetBowSkill().ToString()}");
                    }
                }
            }
        }
    }
}
