namespace ActionCat {
    using ActionCat.Interface;
    using System.Collections;
    using UnityEngine;

    //[RequireComponent(typeof(MonsterController))]
    public class MonsterStatus : MonoBehaviour, IPoolObject, IDamageable {
        [Header("COMPONENT")]
        [SerializeField] MonsterState monsterState;
        [SerializeField] SpriteRenderer sprite;

        [Header("STATUS DATA")] //Scriptable Object 처리.
        public float MaxMonsterHP = 100f;
        public float MaxMonsterMP = 50f;
        public float ItemDropCorrection = 5f;
        public float GaugeIncreaseValue = 10f;
        public float AttackDamage = 5f;
        public short Armorating = 0;
        public byte CriticalResist = 0;

        [Header("SIMPLE HIT COLOR")]
        public bool isActiveHitColor = false;
        public Color HitColor;

        //Status value
        float currentHealthPoint = 0f;
        float currentManaPoint   = 0f;
        float damageCount   = 0f;
        bool isDeath = false;

        //Simple Hit Color Change
        float fadeTime = 0.5f;
        Color startColor;
        Coroutine colorCo;
        public Vector3 constVector {
            get {
                return new Vector3(0f, 0f, 0f);
            }
        }

        private void Start() {
            InitComponent();
            startColor = sprite.color;
            damageCount = AttackDamage;
        }

        private void OnEnable() {
            currentHealthPoint = MaxMonsterHP;
            currentManaPoint   = MaxMonsterMP;

            //Disable by Arrow
            if (isDeath == true) {
                sprite.color = startColor;
                isDeath = false;
            }
        }

        private void OnDisable() {
            if(isActiveHitColor == true) {
                if(colorCo != null) {
                    StopCoroutine(colorCo);
                }
            }

            //Disable by Arrow <Now Checking OnHit Method>
            //if(isDeath == true) {
            //    BattleProgresser.OnMonsterDeath();
            //    BattleProgresser.OnDropItemChance(ItemDropCorrection);
            //    BattleProgresser.OnIncreaseClearGauge(GaugeIncreaseValue);
            //}
        }

        void OnTriggerEnter2D(Collider2D collider) {
            if(collider.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_DISABLELINE)) {
                BattleProgresser.OnDecPlayerHealth(10f);
                DisableRequest();
            }
        }

        void InitComponent() {
            if (monsterState == null) {
                monsterState = GetComponent<MonsterState>();
            }
            if (sprite == null) {
                sprite = GetComponent<SpriteRenderer>();
            }
        }

        #region MESSAGE

        public void OnStageClear() => DisableRequest();

        #endregion

        #region POOL-OBJECT

        public void DisableRequest() => CCPooler.ReturnToPool(gameObject);

        #endregion

        #region ON_DAMAGE

        public void OnHit(ref DamageStruct damage, Vector3 contactPoint, Vector3 direction) {
            //if the Monster is already Death, Failed Hit.
            if (currentHealthPoint <= 0 || isDeath == true) {
                return;
            }

            //Recieve Final Calculated Damage
            var recieveDamage = damage.GetFinalCalcDamageOut(Armorating, CriticalResist, out bool isCritical);
            currentHealthPoint -= recieveDamage;
            DamageFloater.Instance.OnFloatingWithScale(recieveDamage, contactPoint, direction, isCritical);

            //Play Monster Hit Animation
            monsterState.OnHit();

            //Active Event Hit Event.
            BattleProgresser.OnMonsterHit();

            //Monster is Death ?
            if(currentHealthPoint <= 0 && isDeath == false) {
                monsterState.StateChanger(STATETYPE.DEATH);

                //=================================[ MONSTER DEATH EVENT ]================================
                BattleProgresser.OnMonsterDeath();
                BattleProgresser.OnItemDrop(ItemDropCorrection);
                //BattleProgresser.OnIncreaseClearGauge(GaugeIncreaseValue);
                //========================================================================================

                isDeath = true;
            }
        }

        public bool TryOnHit(ref DamageStruct damage, Vector3 point, Vector2 direction) {
            //Monster is already Death, Failed Hit.
            if(currentHealthPoint <= 0 || isDeath == true) {
                return false;
            }

            //Recieve Final Calculated Damage
            var recieveDamage = damage.GetFinalCalcDamageOut(Armorating, CriticalResist, out bool isCritical);
            currentHealthPoint -= recieveDamage;
            DamageFloater.Instance.OnFloatingWithScale(recieveDamage, point, direction, isCritical);

            //Play Monster Hit Animation
            monsterState.OnHit();

            //Call Event Monster Hit
            BattleProgresser.OnMonsterHit();

            //Monster is Death ?
            if(currentHealthPoint <= 0 && isDeath == false) {
                monsterState.StateChanger(STATETYPE.DEATH);

                //=================================[ MONSTER DEATH EVENT ]================================
                BattleProgresser.OnMonsterDeath();
                BattleProgresser.OnItemDrop(ItemDropCorrection);
                //BattleProgresser.OnIncreaseClearGauge(GaugeIncreaseValue);
                //========================================================================================

                isDeath = true;
            }

            return true;
        }

        public void OnHitElemental(ref DamageStruct damage, Vector3 point, Vector2 direction) {
            if(currentHealthPoint <= 0 || isDeath) {
                return;
            }

            //Recieve Final Calculated Damage
            var recieveDamage = damage.GetElementalDamage();
            currentHealthPoint -= recieveDamage;
            DamageFloater.Instance.OnFloatingWithScale(recieveDamage, point, direction, false);

            //Play Monster Hit Animation
            monsterState.OnHit();

            //Call Event Monster Hit
            BattleProgresser.OnMonsterHit();

            //Check Monster is Death (Once)
            if(currentHealthPoint <= 0 && isDeath == false) {
                monsterState.StateChanger(STATETYPE.DEATH);
                //=================================[ MONSTER DEATH EVENT ]================================
                BattleProgresser.OnMonsterDeath();
                BattleProgresser.OnItemDrop(ItemDropCorrection);
                //========================================================================================
                isDeath = true;
            }
        }

        //public void OnHitProjectile(ref DamageStruct damage, Vector3 contactPoint, Vector3 direction) {
        //    if(currentHealthPoint <= 0 || isDeath == true) {
        //        return;
        //    }
        //
        //    //Recieve Final Calculated Damage Count
        //    var recieveDamage = damage.GetProjectileDamage();
        //    currentHealthPoint -= recieveDamage;
        //    DamageFloater.Instance.OnFloatingWithScale(recieveDamage, contactPoint, direction, iscritical: false);
        //
        //    //Play Monster Hit Animation
        //    monsterState.OnHit();
        //
        //    //Active Hit Event
        //    BattleProgresser.OnMonsterHit();
        //
        //    //Monster is Death?
        //    if(currentHealthPoint <= 0 && isDeath == false) {
        //        monsterState.StateChanger(STATETYPE.DEATH);
        //
        //        //=================================[ MONSTER DEATH EVENT ]================================
        //        BattleProgresser.OnMonsterDeath();
        //        BattleProgresser.OnItemDrop(ItemDropCorrection);
        //        //========================================================================================
        //
        //        isDeath = true;
        //    }
        //}

        public bool IsAlive() => (isDeath == false || currentHealthPoint > 0) ? true : false;

        /// <summary>
        /// Animation Event 
        /// </summary>
        public void OnMonsterDeath() {
            DisableRequest();
        }

        /// <summary>
        /// Animation Event
        /// </summary>
        public void OnDamageWall() {
            BattleProgresser.OnDecPlayerHealth(damageCount);
        }

        /// <summary>
        /// Animation Event
        /// </summary>
        public void OnDropReward() {

        }

        #endregion

        #region SIMPLE_COLOR_CHNAGER

        void OnHitColorChange() {
            if (colorCo != null) {
                //실행중인 Color Coroutine이 있다면 중지처리.
                StopCoroutine(colorCo);
            }

            colorCo = StartCoroutine(HitColorCo());
        }

        IEnumerator HitColorCo() {
            float progress = 0f;
            float speed = 1 / fadeTime;

            while (progress < 1) {
                progress += Time.deltaTime * speed;
                sprite.color = Color.Lerp(HitColor, startColor, progress);
                yield return null;
            }

            if (sprite.color != startColor) {
                sprite.color = startColor;
            }
        }

        #endregion
    }
}
