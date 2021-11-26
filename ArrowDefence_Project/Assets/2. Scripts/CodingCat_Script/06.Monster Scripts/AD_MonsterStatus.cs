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
        bool isColorChanging = false;
        float fadeTime = 0.5f;

        //Monster Status Value
        private float currentHealthPoint;
        private float currentManaPoint;
        //private float tempMonsterHealthPoint;
        //private bool isChangingColor = false;
        private bool isDeath = false;
        private SpriteRenderer sprite;
        Color startColor;
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
            GameManager.Instance.CallMonsterHitEvent();

            //Decrease Monster's Health Point
            currentHealthPoint -= damage;

            //Hit Effect
            //HitColorChange(); //-> Origin
            OnHitColorChange();

            if(currentHealthPoint <= 0) {
                isDeath = true;
                DisableRequest(this.gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D coll) {
            if (coll.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_DISABLELINE)) {
                BattleProgresser.OnDecreasePlayerHealthPoint(5f);
                DisableRequest(this.gameObject);
            }
        }

        private void OnEnable() {
            this.currentHealthPoint = MaxMonsterHP;
            this.currentManaPoint   = MaxMonsterMP;

            isDeath = false;

            //Recovery Color
            //if(sprite.color != startColor) {
            //    sprite.color = startColor;
            //}
            //-> Component Get 하기전에 잡아버림
            if(sprite != null) {
                sprite.color = startColor;
            }

            //if (sprite != null)
            //    sprite.color = sprite.color;
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
            //sprite.DOKill();

            //Stop if Color Change Coroutine is in progress
            if(isColorChanging == true) {
                StopCoroutine(HitColorCo());
                isColorChanging = false;
            }

            //Disabled by being hit by arrows or other objects
            if (isDeath) {
                GameManager.Instance.CallMonsterDeathEvent();
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

        void OnHitColorChange() {
            if(isColorChanging == true) {
                StopCoroutine(HitColorCo());
            }

            StartCoroutine(HitColorCo());
        }

        IEnumerator HitColorCo() {
            isColorChanging = true;
            float progress  = 0f;
            float speed = 1 / fadeTime;

            while (progress < 1) {
                progress += Time.deltaTime * speed;
                sprite.color = Color.Lerp(HitColor, startColor, progress);
                yield return null;
            }

            //
            if(sprite.color != startColor) {
                sprite.color = startColor;
            }

            isColorChanging = false;
        }
    }
}
