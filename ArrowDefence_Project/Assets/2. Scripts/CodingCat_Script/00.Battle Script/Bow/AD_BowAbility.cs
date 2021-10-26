namespace ActionCat
{
    using ActionCat.Data;
    using UnityEngine;

    public class AD_BowAbility : MonoBehaviour
    {
        private AD_BowController bowController;

        private void Start()
        {
            bowController = gameObject.GetComponent<AD_BowController>();

            if (CCPlayerData.equipments.GetBowItem() != null)
            {
                var bowSkills = CCPlayerData.equipments.GetBowItem().GetSkills();

                for (int i = 0; i < bowSkills.Length; i++)
                {
                    if(bowSkills[i] != null)
                    {
                        bowController.BowSkillSet += bowSkills[i].BowSpecialSkill;
                        CatLog.Log($"Skill Slot {i} Init, Skill Name : {bowSkills[i].ToString()}");
                    }
                }
            }
        }
    }
}
