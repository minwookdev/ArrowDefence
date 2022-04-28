namespace ActionCat {
    using ActionCat.Interface;
    using UnityEngine;

    public class ArrowSkillSet {
        ARROWSKILL_ACTIVETYPE activeType = ARROWSKILL_ACTIVETYPE.NONE;
        AttackActiveTypeAS hitSkill = null;
        AirType airSkill            = null;
        ProjectileType addProjSkill = null;
        string arrowPoolTag;

        //temp
        Transform tempTr;

        #region CONSTRUCTOR

        /// <summary>
        /// Constructor I. Arrow Item Class에서 Origin Arrow Skill Sets Class 생성.
        /// </summary>
        /// <param name="skillInfoFst"></param>
        /// <param name="skillInfoSec"></param>
        public ArrowSkillSet(ASInfo skillInfoFst, ASInfo skillInfoSec, string tag, PlayerAbilitySlot ability) {
            arrowPoolTag = tag;
            //First Arrow SkillData Init
            if(skillInfoFst != null) {
                switch (skillInfoFst.ActiveType) {
                    case ARROWSKILL_ACTIVETYPE.ATTACK:  InitHit(skillInfoFst.SkillData);                 break;
                    case ARROWSKILL_ACTIVETYPE.AIR:     InitAir(skillInfoFst.SkillData);                 break;
                    case ARROWSKILL_ACTIVETYPE.ADDPROJ: InitProjectile(skillInfoFst.SkillData, ability); break;
                    default: throw new System.NotImplementedException();
                }
            }
            //Seconds Arrow SkillData Init
            if(skillInfoSec != null) {
                switch (skillInfoSec.ActiveType) {
                    case ARROWSKILL_ACTIVETYPE.ATTACK:  InitHit(skillInfoSec.SkillData);                 break;
                    case ARROWSKILL_ACTIVETYPE.AIR:     InitAir(skillInfoSec.SkillData);                 break;
                    case ARROWSKILL_ACTIVETYPE.ADDPROJ: InitProjectile(skillInfoSec.SkillData, ability); break;
                    default: throw new System.NotImplementedException();
                }
            }

            //Start SkillSets Activate Type Init
            activeType = InitArrowSkillActiveType(0);
            CatLog.Log($"Arrow SkillSets Active Type : {activeType.ToString()}");
        }
        
        /// <summary>
        /// Special Type Arrow Constructor
        /// </summary>
        /// <param name="array"></param>
        /// <param name="tag"></param>
        /// <param name="ability"></param>
        public ArrowSkillSet(ASInfo[] array, string tag, PlayerAbilitySlot ability) {
            arrowPoolTag = tag;

            for (int i = 0; i < array.Length; i++) {
                switch (array[i].ActiveType) {
                    case ARROWSKILL_ACTIVETYPE.ATTACK:  InitHit(array[i].SkillData); break;
                    case ARROWSKILL_ACTIVETYPE.AIR:     InitAir(array[i].SkillData); break;
                    case ARROWSKILL_ACTIVETYPE.ADDPROJ: InitProjectile(array[i].SkillData, ability); break;
                    case ARROWSKILL_ACTIVETYPE.BUFF:  break;
                    case ARROWSKILL_ACTIVETYPE.EMPTY: break;
                    default: throw new System.NotImplementedException();
                }
            }

            //Find Special Active Type
            for (int i = 0; i < array.Length; i++) {
                switch (array[i].SkillType) {
                    case ARROWSKILL.EXPLOSION:    activeType = ARROWSKILL_ACTIVETYPE.SP_EXPLOSION;    break;
                    case ARROWSKILL.WINDPIERCING: activeType = ARROWSKILL_ACTIVETYPE.SP_WINDPIERCING; break;
                }

                if(activeType != ARROWSKILL_ACTIVETYPE.NONE) {
                    break;
                }
            }

            if(activeType == ARROWSKILL_ACTIVETYPE.NONE) {
                throw new System.Exception("Not Found Special Arrow Active Type.");
            }
            CatLog.Log($"Initialized ActiveType: {activeType.ToString()}");
        }

        /// <summary>
        /// Constructor II. Copy origin ArrowSkillSets Class.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="num"></param>
        public ArrowSkillSet(ArrowSkillSet origin) {
            // Clone-Active Type
            activeType   = origin.activeType;
            arrowPoolTag = origin.arrowPoolTag;

            // Clone-Hit Type Skill
            if(origin.hitSkill != null) {
                switch (origin.hitSkill) {
                    case ReboundArrow rebound:   hitSkill = new ReboundArrow(rebound, arrowPoolTag); break;
                    case PiercingArrow piercing: hitSkill = new PiercingArrow(piercing);             break;
                    default: throw new System.NotImplementedException("this type is Not Implemented.");
                }
            }

            // Clone-Air Type Skill
            if(origin.airSkill != null) {
                switch (origin.airSkill) {
                    case HomingArrow homing: airSkill = new HomingArrow(homing); break;
                    default: throw new System.NotImplementedException("this type is Not Implemented.");
                }
            }

            // Clone-Additional Projectiles Typ Skil
            if (origin.addProjSkill != null) {
                switch (origin.addProjSkill) {
                    case SplitArrow projectile:    addProjSkill = new SplitArrow(projectile);    break;
                    case SplitDagger projectile:   addProjSkill = new SplitDagger(projectile);   break;
                    case ElementalFire projectile: addProjSkill = new ElementalFire(projectile); break;
                    case Explosion projectile:     addProjSkill = new Explosion(projectile);     break;
                    default: throw new System.NotImplementedException("this type is Not Implemented.");
                }
            }
        }

        #endregion

        #region INIT

        void InitHit(ArrowSkill skillData) {
            if(hitSkill != null) {
                CatLog.ELog($"Error : 중복된 ActiveType의 ArrowSkill {skillData}이(가) 할당되었습니다. [ATTACK]"); 
                return;
            }

            if(skillData is AttackActiveTypeAS hitTypeSkill) {
                hitSkill = hitTypeSkill;
            }
            else {
                CatLog.WLog($"잘못된 Item의 ActiveType: {skillData}는(은) HitType이 아닙니다. (ActiveType을 확인)");
            }
        }

        void InitAir(ArrowSkill skillData) {
            if (airSkill != null) {
                throw new System.Exception($"중복된 Air ActivationType의 ArrSkill ({skillData}) 할당되었습니다.");
            }

            if (skillData is AirType airActiveTypeSkill) {
                airSkill = airActiveTypeSkill;
            }
            else {
                CatLog.WLog($"잘못된 Item의 ActiveType: {skillData}는(은) AirType이 아닙니다. (ActiveType을 확인)");
            }
        }

        void InitProjectile(ArrowSkill skillData, PlayerAbilitySlot ability) {
            if(addProjSkill != null) {
                throw new System.Exception($"중복된 ProjectileType의 ArrSkill ({skillData}) 할당되었습니다.");
            }

            if(skillData is ProjectileType projectileTypeSkill) {
                ///addProjSkill = projectileTypeSkill;
                ///if(addProjSkill.TryGetPrefab(out ProjectilePref projectile)) {
                ///    string projectilePoolTag = string.Format("{0}{1}{2}", arrowPoolTag, AD_Data.POOLTAG_PROJECTILE, projectileTypeSkill.GetUniqueTag());
                ///    addProjSkill.SetPoolTag(projectilePoolTag);
                ///    addProjSkill.SetAbility(ability);
                ///    CCPooler.AddPoolList(projectilePoolTag, projectileTypeSkill.DefaultSpawnSize(), projectile.gameObject, isTracking: false);
                ///}
                addProjSkill = projectileTypeSkill;
                addProjSkill.SetAbility(ability);
                var prefabDictionary = addProjSkill.GetProjectileDic();
                foreach (var keyValuePair in prefabDictionary) {
                    string tag = string.Format("{0}{1}{2}", arrowPoolTag, AD_Data.POOLTAG_PROJECTILE, keyValuePair.Key);
                    addProjSkill.AddPoolTag(tag);
                    CCPooler.AddPoolList(tag, addProjSkill.DefaultSpawnSize(), keyValuePair.Value, false);
                }
            }
            else {
                CatLog.WLog($"잘못된 Item의 ActiveType: {skillData}는(은) ProjectileType이 아닙니다. (ActiveType을 확인)");
            }
        }

        /// <summary>
        /// Init-SkillActive-Type
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        ARROWSKILL_ACTIVETYPE InitArrowSkillActiveType(int num)
        {
            switch (num)
            {
                #region CYCLE 1. ATTACK_TYPE_CHECKING
                case 0:
                    if (hitSkill != null)
                        return InitArrowSkillActiveType(1);
                    else
                        return InitArrowSkillActiveType(2);
                #endregion
                #region CYCLE 2. AIR_TYPE_CHECKING
                case 1:
                    if (airSkill != null)
                        return InitArrowSkillActiveType(3);
                    else
                        return InitArrowSkillActiveType(4);
                case 2:
                    if (airSkill != null)
                        return InitArrowSkillActiveType(5);
                    else
                        return InitArrowSkillActiveType(6);
                #endregion
                #region CYCLE 3. ADDPROJ_TYPE_CHECKING
                case 3:
                    if (addProjSkill != null)
                        return InitArrowSkillActiveType(7);
                    else
                        return InitArrowSkillActiveType(8);
                case 4:
                    if (addProjSkill != null)
                        return InitArrowSkillActiveType(9);
                    else
                        return InitArrowSkillActiveType(10);
                case 5:
                    if (addProjSkill != null)
                        return InitArrowSkillActiveType(11);
                    else
                        return InitArrowSkillActiveType(12);
                case 6:
                    if (addProjSkill != null)
                        return InitArrowSkillActiveType(13);
                    else
                        return InitArrowSkillActiveType(14);
                #endregion
                #region CYCLE 4. FINALLY
                case 7: return ARROWSKILL_ACTIVETYPE.FULL;
                case 8: return ARROWSKILL_ACTIVETYPE.ATTACK_AIR;
                case 9: return ARROWSKILL_ACTIVETYPE.ATTACK_ADDPROJ;
                case 10: return ARROWSKILL_ACTIVETYPE.ATTACK;
                case 11: return ARROWSKILL_ACTIVETYPE.AIR_ADDPROJ;
                case 12: return ARROWSKILL_ACTIVETYPE.AIR;
                case 13: return ARROWSKILL_ACTIVETYPE.ADDPROJ;
                case 14: return ARROWSKILL_ACTIVETYPE.EMPTY;
                default: return ARROWSKILL_ACTIVETYPE.EMPTY;
                #endregion
            }
        }

        /// <summary>
        /// Init-Arrow Skill Sets
        /// </summary>
        /// <param name="arrowTr"></param>
        /// <param name="rigidBody"></param>
        /// <param name="arrow"></param>
        public void Init(Transform arrowTr, Rigidbody2D rigidBody, IArrowObject arrow)
        {
            if (addProjSkill != null)
                addProjSkill.Init(arrowTr, rigidBody, arrow);

            if (hitSkill != null)
                hitSkill.Init(arrowTr, rigidBody, arrow);

            if (airSkill != null)
                airSkill.Init(arrowTr, rigidBody, arrow);
        }

        #endregion

        #region CYCLE

        public bool OnHit(Collider2D collider, ref DamageStruct damage, Vector3 contactPos, Vector2 direction) {
            switch (activeType) {
                case ARROWSKILL_ACTIVETYPE.FULL:           return HitFull(collider, ref damage, contactPos, direction); 
                case ARROWSKILL_ACTIVETYPE.ATTACK_AIR:     return HitAtkAir(collider, ref damage, contactPos, direction);        
                case ARROWSKILL_ACTIVETYPE.ATTACK_ADDPROJ: return HitAtkProj(collider, ref damage, contactPos, direction);
                case ARROWSKILL_ACTIVETYPE.ATTACK:         return HitAtk(collider, ref damage, contactPos, direction);
                case ARROWSKILL_ACTIVETYPE.AIR_ADDPROJ:    return HitProj(collider, ref damage, contactPos, direction);
                case ARROWSKILL_ACTIVETYPE.AIR:            return HitDefault(collider, ref damage, contactPos, direction);
                case ARROWSKILL_ACTIVETYPE.ADDPROJ:        return HitProj(collider, ref damage, contactPos, direction);
                case ARROWSKILL_ACTIVETYPE.SP_EXPLOSION:   return IsActiveExplosion(ref damage, contactPos);
                default:                                   return false;
            }
        }

        /// <summary>
        /// Only Use Air Skill Update
        /// </summary>
        public void OnUpdate() {
            switch (activeType) {
                case ARROWSKILL_ACTIVETYPE.FULL:        UpdateOnAir(); break;
                case ARROWSKILL_ACTIVETYPE.ATTACK_AIR:  UpdateOnAir(); break;
                case ARROWSKILL_ACTIVETYPE.AIR_ADDPROJ: UpdateOnAir(); break;
                case ARROWSKILL_ACTIVETYPE.AIR:         UpdateOnAir(); break;
                default:                                               break;
            }
        }

        /// <summary>
        /// Only Use Air Skill Update 
        /// </summary>
        public void OnFixedUpdate() {
            switch (activeType) {
                case ARROWSKILL_ACTIVETYPE.FULL:        FixedUpdateOnAir(); break;
                case ARROWSKILL_ACTIVETYPE.ATTACK_AIR:  FixedUpdateOnAir(); break;
                case ARROWSKILL_ACTIVETYPE.AIR_ADDPROJ: FixedUpdateOnAir(); break;
                case ARROWSKILL_ACTIVETYPE.AIR:         FixedUpdateOnAir(); break;
                default:                                                    break;
            }
        }

        public void OnExit(Collider2D target) {
            switch (activeType) {
                case ARROWSKILL_ACTIVETYPE.FULL:           hitSkill.OnExit(target); break;
                case ARROWSKILL_ACTIVETYPE.ATTACK_AIR:     hitSkill.OnExit(target); break;
                case ARROWSKILL_ACTIVETYPE.ATTACK_ADDPROJ: hitSkill.OnExit(target); break;
                case ARROWSKILL_ACTIVETYPE.ATTACK:         hitSkill.OnExit(target); break;
                default:                                                            break;
            }
        }

        #endregion

        /// <summary>
        /// Call When Disable Arrow. if the Init SkillSets
        /// </summary>
        public void Clear() {
            switch (activeType) {
                case ARROWSKILL_ACTIVETYPE.FULL:           ClearHit(); ClearAir(); ClearAddProj(); break;
                case ARROWSKILL_ACTIVETYPE.ATTACK_AIR:     ClearHit(); ClearAir();                 break;
                case ARROWSKILL_ACTIVETYPE.ATTACK_ADDPROJ: ClearHit(); ClearAddProj();             break;
                case ARROWSKILL_ACTIVETYPE.ATTACK:         ClearHit();                             break;
                case ARROWSKILL_ACTIVETYPE.AIR_ADDPROJ:    ClearAir(); ClearAddProj();             break;
                case ARROWSKILL_ACTIVETYPE.AIR:            ClearAir();                             break;
                case ARROWSKILL_ACTIVETYPE.ADDPROJ:        ClearAddProj();                         break;
            }
        }

        #region ON-HIT-CALLBACK

        bool HitFull(Collider2D collider, ref DamageStruct damage, Vector3 contactpoint, Vector2 direction) {
            addProjSkill.OnHit(contactpoint, ref damage);
            bool isDisable = hitSkill.OnHit(collider, out tempTr, ref damage, contactpoint, direction);
            if (isDisable == false)
                airSkill.OnHitCallback(tempTr);
            return isDisable;
        }

        bool HitAtkProj(Collider2D collider, ref DamageStruct damage, Vector3 contactpoint, Vector2 direction) {
            addProjSkill.OnHit(contactpoint, ref damage);
            return hitSkill.OnHit(collider, ref damage, contactpoint, direction);
        }

        bool HitAtk(Collider2D collider, ref DamageStruct damage, Vector3 contactpoint, Vector2 direction) {
            return hitSkill.OnHit(collider, ref damage, contactpoint, direction);
        }

        bool HitProj(Collider2D coll, ref DamageStruct damage, Vector3 contactpoint, Vector2 direction) {
            addProjSkill.OnHit(contactpoint, ref damage);
            return HitDefault(coll, ref damage, contactpoint, direction);
        }

        bool HitAtkAir(Collider2D collider, ref DamageStruct damage, Vector3 contactpoint, Vector2 direction)
        {
            bool isDisable = hitSkill.OnHit(collider, out tempTr, ref damage, contactpoint, direction);
            if (isDisable == false) //Disable되는 상황이 아닐 경우만 Transform 보내줌
                airSkill.OnHitCallback(tempTr);
            return isDisable;

            ///tempTr을 Hitskill의 OnHit함수로 매개변수로써 보내고, 
            ///OnHit함수에서 적절한 타겟을 찾게되어 tempTr에 null이 아닌 값이 할당되면
            ///OnAir에서 업데이트로 목표를 찾는 로직을 실행하지 않고 바로 
            ///targetTransform을 사용하여 효과를 실행시킬 수 있게된다.
        }

        #endregion

        #region AIR-UPDATE

        void UpdateOnAir() => airSkill.OnUpdate();

        void FixedUpdateOnAir() => airSkill.OnFixedUpdate();

        #endregion

        #region CLEAR

        void ClearHit() => hitSkill.Clear();

        void ClearAir() => airSkill.Clear();

        void ClearAddProj() => addProjSkill.Clear();

        #endregion

        #region DEFAULT

        /// <summary>
        /// Only Use Air Type Skill.
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="damage"></param>
        /// <param name="contactPos"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        bool HitDefault(Collider2D collider, ref DamageStruct damage, Vector3 contactPos, Vector2 direction) {
            return collider.GetComponent<IDamageable>().TryOnHit(ref damage, contactPos, direction);
        }

        #endregion

        #region SPECIAL

        bool IsActiveExplosion(ref DamageStruct damage, Vector3 point) {
            addProjSkill.OnHit(point, ref damage);
            return true;
        }

        #endregion
    }
}
