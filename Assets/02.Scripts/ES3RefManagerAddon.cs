namespace ActionCat {
    using UnityEngine;
    using System.Collections.Generic;

    public class ES3RefManagerAddon {
        bool IsFindRefManager(out ES3ReferenceMgr result) {
            var sceneExistRefManager = Object.FindObjectOfType<ES3ReferenceMgr>();
            if (sceneExistRefManager) {
                result = sceneExistRefManager;
                return true;
            }
            else {
                result = null;
                return false;
            }
        }

        public void AddRefs2Manager(ItemDropList droplist) {
            if (IsFindRefManager(out ES3ReferenceMgr refManager) == false) {
                CatLog.WLog("Reference Not Found this Scene. Add ES3Reference Manager this Scene First.");
                return;
            }

            if (droplist == null) {
                CatLog.WLog("Scene Items List is Null.");
                return;
            }

            var unityRefList = new System.Collections.Generic.List<Object>();
            foreach (var item in droplist.GetDropTable) {
                switch (item.ItemAsset) {
                    case ItemData_Mat material:             GetMaterialItemRef(material, unityRefList);          break;
                    case ItemData_Con consumable:           GetConsumeItemRef(consumable, unityRefList);         break;
                    case ItemData_Equip_Bow bow:            GetBowItemRefs(bow, unityRefList);                   break;
                    case ItemData_Equip_Accessory artifact: GetArtifactItemRefs(artifact, unityRefList);         break;
                    case ItemDt_SpArr specialArrow:         GetSpecialArrowItemRefs(specialArrow, unityRefList); break;
                    case ItemData_Equip_Arrow arrow:        GetArrowItemRefs(arrow, unityRefList);               break;
                    default: break;
                }
            }

            for (int i = 0; i < unityRefList.Count; i++) {
                refManager.Add(unityRefList[i]);
            }
        }

        void GetMaterialItemRef(ItemData_Mat material, List<Object> list) {
            list.Add(material.Item_Sprite);
        }

        void GetConsumeItemRef(ItemData_Con consumable, List<Object> list) {
            list.Add(consumable.Item_Sprite);
        }

        void GetBowItemRefs(ItemData_Equip_Bow equipment, List<Object> list) {
            list.Add(equipment.Item_Sprite);

            list.Add(equipment.BowGameObject);

            if (equipment.SkillAsset_f) {
                list.Add(equipment.SkillAsset_f.SkillIconSprite);
                list.AddRange(equipment.SkillAsset_f.SoundEffects);
            }

            if (equipment.SkillAsset_s) {
                list.Add(equipment.SkillAsset_s.SkillIconSprite);
                list.AddRange(equipment.SkillAsset_s.SoundEffects);
            }
        }

        void GetArrowItemRefs(ItemData_Equip_Arrow equipment, List<Object> list) {
            list.Add(equipment.Item_Sprite);

            list.Add(equipment.MainArrowObj);
            list.Add(equipment.LessArrowObj);
            list.AddRange(equipment.effects);

            if (equipment.ArrowSkillFst) {
                list.Add(equipment.ArrowSkillFst.IconSprite);
                list.AddRange(equipment.ArrowSkillFst.effects);
                list.AddRange(equipment.ArrowSkillFst.Sounds);

                switch (equipment.ArrowSkillFst.SkillType) {
                    case ARROWSKILL.SKILL_REBOUND:  break; // - NONE
                    case ARROWSKILL.SKILL_HOMING:   break; // - NONE
                    case ARROWSKILL.SKILL_SPLIT:    break; // - NONE
                    case ARROWSKILL.SKILL_PIERCING: break; // - NONE
                    case ARROWSKILL.SPLIT_DAGGER:
                        var splitdagger = equipment.ArrowSkillFst as DataSplitDagger;
                        if (splitdagger) {
                            list.Add(splitdagger.daggerPref);
                        }
                        else {
                            CatLog.ELog("ArrowSkill Class and Type Not Matched !");
                        }
                        break; 
                    case ARROWSKILL.ELEMENTAL_FIRE:
                        var elementalFire = equipment.ArrowSkillFst as DataEltalFire;
                        if (elementalFire) {
                            list.Add(elementalFire.firePref);
                        }
                        else {
                            CatLog.ELog("ArrowSkill Class and Type Not Matched !");
                        }
                        break;
                    case ARROWSKILL.NONE:           break; // - NONE
                    case ARROWSKILL.EXPLOSION:      throw new System.Exception();
                    case ARROWSKILL.WINDPIERCING:   throw new System.Exception();
                    case ARROWSKILL.BUFF:           throw new System.Exception();
                    default:                        throw new System.NotImplementedException();
                }
            }

            if (equipment.ArrowSkillSec) {
                list.Add(equipment.ArrowSkillSec.IconSprite);
                list.AddRange(equipment.ArrowSkillSec.effects);
                list.AddRange(equipment.ArrowSkillSec.Sounds);

                switch (equipment.ArrowSkillSec.SkillType) {
                    case ARROWSKILL.SKILL_REBOUND:  break; // - NONE
                    case ARROWSKILL.SKILL_HOMING:   break; // - NONE
                    case ARROWSKILL.SKILL_SPLIT:    break; // - NONE
                    case ARROWSKILL.SKILL_PIERCING: break; // - NONE
                    case ARROWSKILL.SPLIT_DAGGER:
                        var splitdagger = equipment.ArrowSkillSec as DataSplitDagger;
                        if (splitdagger) {
                            list.Add(splitdagger.daggerPref);
                        }
                        else {
                            CatLog.ELog("ArrowSkill Class and Type Not Matched !");
                        }
                        break;
                    case ARROWSKILL.ELEMENTAL_FIRE:
                        var elementalFire = equipment.ArrowSkillSec as DataEltalFire;
                        if (elementalFire) {
                            list.Add(elementalFire.firePref);
                        }
                        else {
                            CatLog.ELog("ArrowSkill Class and Type Not Matched !");
                        }
                        break;
                    case ARROWSKILL.NONE:         break; // - NONE
                    case ARROWSKILL.EXPLOSION:    throw new System.Exception();
                    case ARROWSKILL.WINDPIERCING: throw new System.Exception();
                    case ARROWSKILL.BUFF:         throw new System.Exception();
                    default:                      throw new System.NotImplementedException();
                }
            }
        }

        void GetArtifactItemRefs(ItemData_Equip_Accessory artifact, List<Object> list) {
            list.Add(artifact.Item_Sprite);
            if (artifact.SPEffectAsset) {
                list.Add(artifact.SPEffectAsset.SkillIconSprite);

                switch (artifact.SPEffectAsset.EffectType) {
                    case ACSP_TYPE.SPEFFECT_AIMSIGHT:
                        var aimsight = artifact.SPEffectAsset as SkillDataAimSight;
                        if (aimsight) {
                            list.Add(aimsight.AimSightPref);
                        }
                        else {
                            CatLog.ELog("Artifact Skill and Class Not Matched !");
                        }
                        break;
                    case ACSP_TYPE.SPEEFECT_SLOWTIME: break;
                    case ACSP_TYPE.CURE:              break;
                    case ACSP_TYPE.CURSE_SLOW:        break;
                    case ACSP_TYPE.SPEFFECT_NONE:     break;
                    default: throw new System.NotImplementedException();
                }
            }
        }

        void GetSpecialArrowItemRefs(ItemDt_SpArr specialArrow, List<Object> list) {
            list.Add(specialArrow.Item_Sprite);

            list.Add(specialArrow.MainArrowObj);
            list.Add(specialArrow.LessArrowObj);
            list.AddRange(specialArrow.effects);

            if (specialArrow.ArrowSkillFst) {
                list.Add(specialArrow.ArrowSkillFst.IconSprite);
                list.AddRange(specialArrow.ArrowSkillFst.effects);
                list.AddRange(specialArrow.ArrowSkillFst.Sounds);
                switch (specialArrow.ArrowSkillFst.SkillType) {
                    case ARROWSKILL.NONE:         break;
                    case ARROWSKILL.EXPLOSION:
                        var explosion = specialArrow.ArrowSkillFst as Dt_Explosion;
                        if (explosion) {
                            list.Add(explosion.ExplosionPref);
                            list.Add(explosion.SmallExPref);
                        }
                        else {
                            CatLog.ELog("ArrowSkill Class and Type Not Matched !");
                        }
                        break;
                    case ARROWSKILL.WINDPIERCING: break;
                    case ARROWSKILL.BUFF:         break;
                    default: throw new System.NotImplementedException();
                }
            }

        }
    }
}
