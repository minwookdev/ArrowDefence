namespace ActionCat
{
    using System.Collections;
    using UnityEngine;
    using DG.Tweening;
    

    public class AD_MonsterStatus : MonoBehaviour, IPoolObject, IDamageable
    {
        [Header("MONSTER STATUS DATA")]
        public float MaxMonsterHP;
        public float MaxMonsterMP;
        public float HitColorDuration = 1f;

        [Header("MONSTER HIT")]
        public Color HitColor;

        //Monster Status Value
        private float monsterHp;
        private float monsterMp;
        private bool isChangingColor = false;
        private SpriteRenderer sprite;
        private Color startColor;

        //Battle Related Variables
        private float dropCorrection = 5f;
        private float clearGaugeIncreaseValue = 10f;

        private void Start()
        {
            sprite = GetComponent<SpriteRenderer>();
            startColor = sprite.color;
        }

        private void Update()
        {
            //if(GameManager.Instance.GameState == GAMESTATE.STATE_ENDBATTLE)
            //{
            //    //Battle이 끝남에 따라 자체적으로 비활성화 처리로직 들어가도 된다 
            //}
        }

        public void DisableObject_Req(GameObject target) => CCPooler.ReturnToPool(target);

        public void OnStageClear() => DisableObject_Req(this.gameObject);

        public void OnHitObject(float value)
        {
            //Decrease Monster's Health Point
            monsterHp -= value;

            //Hit Effect
            HitColorChange();

            if(monsterHp <= 0)
            {
                BattleProgresser.OnIncreaseClearGauge(clearGaugeIncreaseValue);
                BattleProgresser.OnDropItemChance(dropCorrection);
                GameManager.Instance.MonsterDeathEvent();   //Active Test Event
                DisableObject_Req(this.gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_DISABLELINE))
            {
                BattleProgresser.OnDecreasePlayerHealthPoint(5f);
                DisableObject_Req(this.gameObject);
            }
        }

        private void OnCollisionEnter2D(Collision2D coll)
        {
            if (coll.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_ARROW))
            {
                GameManager.Instance.MonsterHitEvent();
                float damageCount = Random.Range(50f, 110f + 1f);
                OnHitObject(damageCount);
            }

            if(coll.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_ARROW_LESS))
            {
                GameManager.Instance.MonsterLessHitEvent();
                float damageCount = Random.Range(10f, 30f);
                OnHitObject(damageCount);
            }
        }

        private void OnEnable()
        {
            this.monsterHp = MaxMonsterHP;
            this.monsterMp = MaxMonsterMP;
        }

        private void OnDisable()
        {
            if(isChangingColor)
            {
                sprite.DOKill();
                sprite.color    = startColor;
                isChangingColor = false;
            }
        }

        private void HitColorChange()
        {
            isChangingColor = true;
            sprite.color    = HitColor;

            sprite.DOColor(startColor, 1f)
                  .OnComplete(() => isChangingColor = false);
        }
    }
}
