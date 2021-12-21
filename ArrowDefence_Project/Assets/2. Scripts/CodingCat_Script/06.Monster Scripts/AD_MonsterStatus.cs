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
        float fadeTime = 0.5f;

        //Monster Status Value
        private float currentHealthPoint;
        private float currentManaPoint;
        private bool isDeath = false;

        //private float tempMonsterHealthPoint;
        //private bool isChangingColor = false;

        SpriteRenderer sprite;
        Sequence hitSeq;

        //Battle Related Variables
        private float dropCorrection = 5f;
        private float clearGaugeIncreaseValue = 10f;

        //Monster Hit Color
        Coroutine hitColorChangeCo = null;
        Color startColor;

        private void Start() {
            sprite     = GetComponent<SpriteRenderer>();
            startColor = sprite.color;

            hitSeq = DOTween.Sequence().SetAutoKill(false)
                                       .OnStart(() => { })
                                       .Prepend(sprite.DOColor(HitColor, 0.01f))
                                       .Append(sprite.DOColor(startColor, 1f))
                                       .Pause();
        }

        private void Update() {
            //if(GameManager.Instance.GameState == GAMESTATE.STATE_ENDBATTLE)
            //{
            //    //Battle이 끝남에 따라 자체적으로 비활성화 처리로직 들어가도 된다 
            //}
        }

        public void DisableRequest(GameObject target) => CCPooler.ReturnToPool(target);

        public void DisableRequest() => CCPooler.ReturnToPool(gameObject);

        public void OnStageClear() => DisableRequest();

        public void OnHitObject(ref DamageStruct damage) {

        }

        public void OnHitObject(float damage)
        {
            //Active Monster Hit Event
            GameManager.Instance.CallMonsterHitEvent();

            //Decrease Monster's Health Point
            currentHealthPoint -= damage;

            //OnHit Effect (Change Color)
            OnHitColorChange();

            if(currentHealthPoint <= 0) {
                isDeath = true;
                DisableRequest();
            }
        }

        private void OnTriggerEnter2D(Collider2D coll) {
            if (coll.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_DISABLELINE)) {
                BattleProgresser.OnDecreasePlayerHealthPoint(5f);
                DisableRequest();
            }
        }

        private void OnEnable() {
            this.currentHealthPoint = MaxMonsterHP;
            this.currentManaPoint   = MaxMonsterMP;

            isDeath = false;

            //Recovery Color
            if(sprite != null) {
                //Sprite Null Check
                sprite.color = startColor;
            }
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
            //if(isColorChanging == true) {
            //    StopCoroutine(HitColorCo());
            //    isColorChanging = false;
            //}

            //Stop Hit Color Change Coroutine
            if(hitColorChangeCo != null) {
                StopCoroutine(hitColorChangeCo);
            }

            //Disabled by being hit by arrows or other objects
            if (isDeath) {
                GameManager.Instance.CallMonsterDeathEvent();
                BattleProgresser.OnDropItemChance(dropCorrection);
                BattleProgresser.OnIncreaseClearGauge(clearGaugeIncreaseValue);
            }
        }

        private void HitColorChange() {
            hitSeq.Restart();
        }

        void OnHitColorChange() {
            if(hitColorChangeCo != null) { //이미 실행중인 Coroutine이 있다면 정지 처리 후 실행
                StopCoroutine(hitColorChangeCo);
            }

            hitColorChangeCo = StartCoroutine(HitColorCo());
        }

        IEnumerator HitColorCo() {
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
        }
    }
}
