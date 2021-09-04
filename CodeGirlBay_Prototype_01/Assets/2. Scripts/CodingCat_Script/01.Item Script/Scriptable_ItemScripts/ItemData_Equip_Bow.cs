namespace CodingCat_Games
{
    using UnityEngine;
    using UnityEditor;
    using CodingCat_Scripts;

    [CreateAssetMenu(fileName = "Item_Bow_Asset", menuName = "Scriptable Object Asset/Item_Bow_Asset")]
    public class ItemData_Equip_Bow : ItemData_Equip
    {
        [Header("Bow Item Data")]
        public GameObject BowGameObject;
        public BOWSKILL_TYPE FirstSkill_Type;
        public BOWSKILL_TYPE SecondSkill_Type;
        public AD_BowSkill BowSkill_First;
        public AD_BowSkill BowSkill_Second;

        public ItemData_Equip_Bow() : base()
        {
            Equip_Type = EQUIP_ITEMTYPE.EQUIP_BOW;
        }

        private void OnEnable() => SetBowSkills();

        private void SetBowSkills()
        {
            switch (FirstSkill_Type)
            {
                case BOWSKILL_TYPE.SKILL_NULL:        BowSkill_First = null;                      break;
                case BOWSKILL_TYPE.SKILL_SPREAD_SHOT: BowSkill_First = new Skill_Multiple_Shot(); break;
                case BOWSKILL_TYPE.SKILL_RAPID_SHOT:  BowSkill_First = new Skill_Rapid_Shot();    break;
                case BOWSKILL_TYPE.SKILL_ARROW_RAIN:  BowSkill_First = new Skill_Arrow_Rain();    break;
                default: break;
            }

            switch (SecondSkill_Type)
            {
                case BOWSKILL_TYPE.SKILL_NULL:        BowSkill_Second = null;                      break;
                case BOWSKILL_TYPE.SKILL_SPREAD_SHOT: BowSkill_Second = new Skill_Multiple_Shot(); break;
                case BOWSKILL_TYPE.SKILL_RAPID_SHOT:  BowSkill_Second = new Skill_Rapid_Shot();    break;
                case BOWSKILL_TYPE.SKILL_ARROW_RAIN:  BowSkill_Second = new Skill_Arrow_Rain();    break;
                default: break;
            }

            if (BowSkill_First != null)  CatLog.Log($"{this.Item_Name} Enabled Bow Skill : {BowSkill_First.ToString()}");
            if (BowSkill_Second != null) CatLog.Log($"{this.Item_Name} Enabled Bow Skill : {BowSkill_Second.ToString()}");
        }
    }

#if UNITY_EDITOR

    //[CustomEditor(typeof(ItemData_Equip_Bow))]
    //public class BowItemDataScriptEditor : Editor
    //{
    //    public override void OnInspectorGUI()
    //    {
    //        //if (null == ItemAddress) return;
    //
    //        base.OnInspectorGUI();
    //
    //        ItemData_Equip_Bow itemAddress = (ItemData_Equip_Bow)target;
    //
    //        EditorGUILayout.Space();
    //        if (null != itemAddress.BowSkill_First)
    //            EditorGUILayout.LabelField("Enabled Bow Skill : ", itemAddress.BowSkill_First.ToString());
    //        else EditorGUILayout.LabelField("Enabled Bow Skill : ", "NULL");
    //        if (null != itemAddress.BowSkill_Second)
    //            EditorGUILayout.LabelField("Enabled Bow Skill : ", itemAddress.BowSkill_Second.ToString());
    //        else EditorGUILayout.LabelField("Enabled Bow Skill : ", "NULL");
    //        EditorGUILayout.BeginHorizontal();
    //        if(GUILayout.Button("Generate"))
    //        {
    //            switch (itemAddress.FirstSkill_Type)
    //            {
    //                case Enum_BowSkillType.SKILL_SPREAD_SHOT:
    //                    itemAddress.BowSkill_First = new Skill_Multiple_Shot();
    //                    break;
    //                case Enum_BowSkillType.SKILL_RAPID_SHOT:
    //                    itemAddress.BowSkill_First = new Skill_Rapid_Shot();
    //                    break;
    //                case Enum_BowSkillType.SKILL_ARROW_RAIN:
    //                    itemAddress.BowSkill_First = new Skill_Arrow_Rain();
    //                    break;
    //                default:
    //                    break;
    //            }
    //            
    //            EditorUtility.SetDirty(itemAddress);
    //        }
    //        EditorGUILayout.EndHorizontal();
    //
    //        //EditorGUILayout.Space();
    //        //SerializedProperty bowskill = serializedObject.FindProperty("AD_BowSkill");
    //        //EditorGUILayout.PropertyField(bowskill, true);
    //
    //        //bowItemData.BowSkillType = 
    //        //    (Enum_BowSkill)EditorGUILayout.EnumPopup("Bow Skill Type : ", bowItemData.BowSkillType);
    //        //Generate 버튼 만들어서 굴려보기 -> 전반적인 세팅 전부 다
    //        //Generate 버튼으로 ItemData의 Skill을 할당하고 있다
    //        //Editor 는 보여줄 뿐이다.. 여기서 스킬을 할당한다고 해도 저장되지 않는다
    //    }
    //}

#endif
}
