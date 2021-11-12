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
        private float currentHealthPoint;
        private float currentManaPoint;
        //private float tempMonsterHealthPoint;
        //private bool isChangingColor = false;
        private bool isDeath         = false;
        private SpriteRenderer sprite;
        private Color startColor;
        Sequence hitSeq;

        //Battle Related Variables
        private float dropCorrection = 5f;
        private float clearGaugeIncreaseValue = 10f;

        private void Start()
        {
            sprite     = GetComponent<SpriteRenderer>();
            startColor = sprite.color;

            hitSeq = DOTween.Sequence().SetAutoKill(false)
                                       .OnStart(() => { })
                                       .Prepend(sprite.DOColor(HitColor, 0.01f))
                                       .Append(sprite.DOColor(startColor, 1f))
                                       .Pause();
        }

        private void Update()
        {
            //if(GameManager.Instance.GameState == GAMESTATE.STATE_ENDBATTLE)
            //{
            //    //Battle이 끝남에 따라 자체적으로 비활성화 처리로직 들어가도 된다 
            //}
        }

        public void DisableRequest(GameObject target) => CCPooler.ReturnToPool(target);

        public void OnStageClear() => DisableRequest(this.gameObject);

        public void OnHitObject(float damage)
        {
            //Active Monster Hit Event
            GameManager.Instance.MonsterHitEvent();

            //Decrease Monster's Health Point
            currentHealthPoint -= damage;

            //Hit Effect
            HitColorChange();

            if(currentHealthPoint <= 0)
            {
                isDeath = true;
                DisableRequest(this.gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D coll)
        {
            //if(coll.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_ARROW_LESS))
            //{
            //    GameManager.Instance.MonsterLessHitEvent();
            //    float damageCount = Random.Range(10f, 30f);
            //    OnHitObject(damageCount);
            //}

            if (coll.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_DISABLELINE))
            {
                BattleProgresser.OnDecreasePlayerHealthPoint(5f);
                DisableRequest(this.gameObject);
            }
        }

        private void OnCollisionEnter2D(Collision2D coll)
        {
            if (coll.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_ARROW))
            {   
                float damageCount = Random.Range(30f, 80f);
                OnHitObject(damageCount);
            }

            //if(coll.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_ARROW_LESS))
            //{
            //    GameManager.Instance.MonsterLessHitEvent();
            //    float damageCount = Random.Range(10f, 30f);
            //    OnHitObject(damageCount);
            //}
        }

        private void OnEnable()
        {
            this.currentHealthPoint = MaxMonsterHP;
            this.currentManaPoint   = MaxMonsterMP;

            isDeath = false;

            if (sprite != null)
                sprite.color = sprite.color;
        }

        private void OnDisable()
        {
            //if(isChangingColor)
            //{
            //    sprite.DOKill();
            //    sprite.color    = startColor;
            //    isChangingColor = false;
            //}

            //monster hit Sequence 진행중에 MainMenu로 돌아가거나 ApplicationQuit Event
            //발생하면 DoTween 에러를 방지
            sprite.DOKill();

            if(isDeath)
            {
                //연속된 빠른 화살에 계속 Hit될 경우 OnHit메서드에 넣을 경우 여러번 이벤트가 발생할 수 있어서
                //OnDisable 에서 따로 처리하고 있다.
                GameManager.Instance.MonsterDeathEvent();   //Active Test Event
                BattleProgresser.OnDropItemChance(dropCorrection);
                BattleProgresser.OnIncreaseClearGauge(clearGaugeIncreaseValue);
            }
        }

        private void HitColorChange()
        {
            //isChangingColor = true;
            //sprite.color    = HitColor;
            //
            //sprite.DOColor(startColor, 1f)
            //      .OnComplete(() => isChangingColor = false);

            hitSeq.Restart();
        }
    }
}
