namespace ActionCat {
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

            //Disable by Arrow
            if(isDeath == true) {
                GameManager.Instance.CallMonsterDeathEvent();
                BattleProgresser.OnDropItemChance(ItemDropCorrection);
                BattleProgresser.OnIncreaseClearGauge(GaugeIncreaseValue);
            }
        }

        void OnTriggerEnter2D(Collider2D collider) {
            if(collider.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_DISABLELINE)) {
                BattleProgresser.OnDecreasePlayerHealthPoint(10f);
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

        #region ON_DAMAGE

        public void DisableRequest(GameObject target) => CCPooler.ReturnToPool(target);

        public void DisableRequest() => CCPooler.ReturnToPool(gameObject);

        public void OnHitObject(float damage) {
            //Active Monster Hit Event
            GameManager.Instance.CallMonsterHitEvent();

            //Play Hit Animation
            monsterState.OnHit();

            //Decrease Monster's Health Point
            currentHealthPoint -= damage;

            //is Active Simple Hit Effect?
            if(isActiveHitColor == true) {
                OnHitColorChange();
            }

            //On Monster Death
            if(currentHealthPoint <= 0 && isDeath == false) {
                monsterState.StateChanger(STATETYPE.DEATH);
                isDeath = true;
            }
        }

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
            BattleProgresser.OnDecreasePlayerHealthPoint(damageCount);
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
