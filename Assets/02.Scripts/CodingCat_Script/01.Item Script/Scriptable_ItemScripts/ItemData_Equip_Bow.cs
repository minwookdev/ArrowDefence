﻿namespace ActionCat
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Item_Bow_Asset", menuName = "Scriptable Object Asset/Item_Bow_Asset")]
    public class ItemData_Equip_Bow : ItemData_Equip
    {
        [Header("Bow Item Data")]
        public GameObject BowGameObject;
        public BowSkillData SkillAsset_f;
        public BowSkillData SkillAsset_s;

        //Inherence Ability
        [RangeEx(0, 500, 50, "DAMAGE")]    
        public short BaseDamage = 0;
        [RangeEx(0, 30, 3, "CRIT CHANCE")] 
        public byte CriticalHitChance = 0;
        [RangeEx(0f, 1.5f, 0.15f, "CRIT DAMAGE")] 
        public float CriticalDamageMultiplier = 0f;
        [RangeEx(0f, 1.25f, 0.125f, "CHARGED DAMAGE")] 
        public float FullChargedMultiplier = 0f;
        [RangeEx(0, 150, 15, "AROMOR PENETRATION")]
        public short ArmorPenetration = 0;
        [RangeEx(0, 5, 1, "ADDITIONAL ARROW")]
        public byte AdditionalArrowFire = 0;
        [RangeEx(0, 30, 3, "ELEMENTAL ACTIVATION INC")]
        public short ElementalActivation = 0;

        //PROPERTY
        public AD_BowSkill SkillFst {
            get {
                if (SkillAsset_f != null)
                    return SkillAsset_f.Skill();
                else
                    return null;
            }
        }

        public AD_BowSkill SkillSec {
            get {
                if (SkillAsset_s != null)
                    return SkillAsset_s.Skill();
                else
                    return null;
            }
        }

        public ItemData_Equip_Bow() : base() {
            Equip_Type = EQUIP_ITEMTYPE.BOW;
        }

        public void OnEnable() {
            InitAbilities();
        }

        private void InitAbilities() {
            System.Collections.Generic.List<Ability> tempAbility = new System.Collections.Generic.List<Ability>();
            //1. Add Base Damage Ability.
            if (BaseDamage > 0f) { //
                if (BaseDamage > AbilityDamage.MaxValue) {
                    throw new System.Exception($"Item: {name}, Damage value is RangeOver !");
                }
                tempAbility.Add(new AbilityDamage(BaseDamage));
            }

            //2. Add Critical Hit Chance Ability.
            if (CriticalHitChance > 0) {
                if (CriticalHitChance > AbilityCritChance.MaxValue) {
                    throw new System.Exception($"Item: {name}, Critical Hit Chance Value is RangeOver !");
                }
                tempAbility.Add(new AbilityCritChance(System.Convert.ToByte(CriticalHitChance)));
            }

            //3. Add Critical Hit Damage Ability.
            if (CriticalDamageMultiplier > 0f) {
                if (CriticalDamageMultiplier > AbilityCritDamage.MaxValue) {
                    throw new System.Exception($"Item: {name}, Critical Damage Value is RangeOver !");
                }
                tempAbility.Add(new AbilityCritDamage(CriticalDamageMultiplier));
            }

            //4. Add Ability Charged Damage Multiplier.
            if (FullChargedMultiplier > 0f) {
                if (FullChargedMultiplier > AbilityChargedDamage.MaxValue) {
                    throw new System.Exception($"Item: {name}, Charged Damage Value is RangeOver !");
                }
                tempAbility.Add(new AbilityChargedDamage(FullChargedMultiplier));
            }

            //5. Add Armor Penetration
            if (ArmorPenetration > 0) {
                if (ArmorPenetration > PenetrationArmor.MaxValue) {
                    throw new System.Exception($"Item: {name}, ArmorPenetration Value is RangeOver !");
                }
                tempAbility.Add(new PenetrationArmor(ArmorPenetration));
            }

            //6. Add Additional Arrow Fire
            if (AdditionalArrowFire > 0) {
                if (AdditionalArrowFire > AdditionalFire.MaxValue) {
                    throw new System.Exception($"Item: {name}, Additional Arorw Fire Value is RangeOver !");
                }
                tempAbility.Add(new AdditionalFire(AdditionalArrowFire));
            }

            //7. Add Elemental Activation
            if (ElementalActivation > 0) {
                if (ElementalActivation > ActionCat.ElementalActivation.MaxValue) {
                    throw new System.Exception($"Item: {name}, Elemetnal Activation Value is RangeOver !");
                }
                tempAbility.Add(new ActionCat.ElementalActivation(ElementalActivation));
            }

            //Final. if the tempAbility Length is Bigger than 1, Init the Ability Array.
            if(tempAbility.Count > 4) {
                throw new System.Exception("Bow Ability Length Over, this is less than 5.");
            }
            abilityDatas = tempAbility.ToArray();
        }
    }

#if UNITY_EDITOR
    //뻘짓들 남겨둠
    //현재는 Editor 관련된 것들 전부 Editor 폴더로 빼두었다
    //[UnityEditor.CustomEditor(typeof(ItemData_Equip_Bow))]
    //public class ItemDataEditor : Editor
    //{
    //    ItemData_Equip_Bow item;
    //
    //    private void OnEnable()
    //    {
    //        item = (ItemData_Equip_Bow)target;
    //    }
    //
    //    public override void OnInspectorGUI()
    //    {
    //        base.OnInspectorGUI();
    //
    //        //serializedObject.Update();
    //
    //        //너가 에디터에서 값을변경 하든지 할거
    //        //
    //        //
    //
    //        //serializedObject.ApplyModifiedProperties();
    //
    //        EditorUtility.SetDirty(target);
    //    }
    //}

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
