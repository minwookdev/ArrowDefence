namespace ActionCat {
    using UnityEngine;
    using System.Collections;
    using ActionCat.Interface;

    public class ExplosionPref : ProjectilePref {
        [Header("EFFECT")]
        [SerializeField] ACEffector2D effector = null;

        [Header("SOUND")]
        [SerializeField][ReadOnly] Audio.ACSound audioSource = null;
        [SerializeField] AudioClip[] addExplosionClips = null;

        byte skillLevel = 0;
        float explosionHitRadius = 0f;
        float addExplosionRadius = 0f;
        float explosionInterval = 0.7f;
        short addExplosionDamage;
        string addExpTag = "";
        bool isReady = false;

        //Coroutine
        WaitForSeconds waitForInterval = null;
        WaitUntil waitEffectStop = null;
        WaitUntil waitUntilReady = null;

        private void Awake() {
            waitForInterval = new WaitForSeconds(explosionInterval);
            waitEffectStop = new WaitUntil(() => effector.IsPlaying() == false);
            waitUntilReady = new WaitUntil(() => isReady == true);
            CheckComponent();
        }

        private void Start() {
            //Get Audio Channel
            audioSource = SoundManager.Instance.TryGetChannel2Dic(CHANNELTYPE.PROJECTILE, out Audio.ACSound result) ? result : audioSource;
        }

        private void OnEnable() {
            StartCoroutine(ExplosionAsync());
        }

        public override void Shot(DamageStruct damage, short projectileDamage = 0) {
            finCalcDamage = projectileDamage;
            damageStruct = damage;
            damageStruct.SetDamage(finCalcDamage);

            isReady = true;
        }

        public override void ReturnToPoolRequest() {
            isReady = false;
            base.ReturnToPoolRequest();
        }

        protected override void CheckComponent() {
            if (tr == null) CatLog.ComponentMent();
            if (effector == null) CatLog.ComponentMent();
        }

        /// <summary>
        /// 스킬 발동 코루틴
        /// </summary>
        /// <returns></returns>
        IEnumerator ExplosionAsync() {
            // 변수들 초기화가 완료될 때 까지 대기
            yield return waitUntilReady;
            effector.Play();
            CineCam.Inst.ShakeCamera(8f, .5f);

            // OverlapCircleAll2D를 통한 범위 내 레이어 지정을 통한 몬스터 오브젝트만 검출
            if (GameGlobal.TryGetOverlapCircleAll2D(out Collider2D[] colliders, tr.position, explosionHitRadius, AD_Data.LAYER_MONSTER)) {
                foreach (var collider in colliders) {
                    // 검출된 몬스터 오브젝트들에게 데미지 처리
                    if (collider.TryGetComponent<IDamageable>(out IDamageable target)) {
                        var hitPoint = collider.ClosestPoint(tr.position);
                        var hitDirection = collider.transform.position - tr.position;
                        target.OnHitElemental(ref damageStruct, hitPoint, hitDirection);// 몬스터 오브젝트의 데미지 처리 함수 호출
                    }
                }
            }

            // 스킬 레벨 변수에 따른 추가 폭발 코루틴 종료 및 이펙트 종료 대기
            yield return StartCoroutine(AdditionalExplosionAsync(skillLevel));
            yield return waitEffectStop;

            // 오브젝트 풀 회수 요청 (비활성화)
            ReturnToPoolRequest();
        }

        /// <summary>
        /// 몬스터 오브젝트와 충돌 시 폭발 구현 함수
        /// </summary>
        private void ExplosionByCollision()
        {
            // OverlapCircleAll2D를 통한 범위 내 레이어 지정을 통한 몬스터 오브젝트만 검출
            if (GameGlobal.TryGetOverlapCircleAll2D(out Collider2D[] colliders, tr.position, explosionHitRadius, AD_Data.LAYER_MONSTER))
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].TryGetComponent(out IDamageable target))
                    {
                        // 검출된 몬스터 오브젝트의 데미지 처리 함수 호출 (충돌 지점 및 방향 전달)
                        var hitPoint = colliders[i].ClosestPoint(tr.position);
                        var hitDirection = colliders[i].transform.position - tr.position;
                        target.OnHitElemental(ref damageStruct, hitPoint, hitDirection); 
                    }
                }
            }

            // 폭발 이펙트 및 카메라 쉐이크
            effector.Play();
            CineCam.Inst.ShakeCamera(8f, .5f);

            // 오브젝트 풀 회수 요청 (비활성화)
            ReturnToPoolRequest();
        }

        public void SetValue(string smallExplosionTag, float expHitRange, float addExpRange, short addexpdamage, byte level) {
            skillLevel = level;
            addExpTag = smallExplosionTag;
            explosionHitRadius = expHitRange;
            addExplosionRadius = addExpRange;
            addExplosionDamage = addexpdamage;
        }

        IEnumerator AdditionalExplosionAsync(byte level) {
            switch (level) {
                case 1: yield break;
                case 2: break;
                case 3: break;
                default: CatLog.ELog("Over Range Explosion SkillLevel."); yield break;
            }

            yield return waitForInterval;

            float radius = (level <= 2) ? explosionHitRadius + addExplosionRadius : explosionHitRadius + (addExplosionRadius * 0.5f);
            //var targetColliderArray = GameGlobal.OverlapCircleAll2D(tr, radius, AD_Data.LAYER_MONSTER, target => Vector2.Distance(tr.position, target.transform.position) < explosionHitRadius);
            var targetColliderArray = GameGlobal.OverlapCircleAll2D(tr, radius, AD_Data.LAYER_MONSTER);
            for (int i = 0; i < targetColliderArray.Length; i++) {
                var addExPref = CCPooler.SpawnFromPool<ExplosionSmallPref>(addExpTag, targetColliderArray[i].transform.position, Quaternion.identity);
                if (addExPref) {
                    addExPref.Shot(damageStruct, addExplosionDamage);
                }
            }


            if (targetColliderArray.Length > 0) { //Play AddExplosion Sound, if the Target Exist.
                audioSource.PlayOneShot(addExplosionClips.RandIndex());
            }

            //Wait Next Explosion
            yield return StartCoroutine(AddExplosionPhaseEx(level));
        }

        IEnumerator AddExplosionPhaseEx(byte level) {
            switch (level) {
                case 3: break;
                default: yield break;
            }

            yield return waitForInterval;

            float radius = explosionHitRadius + addExplosionRadius;
            var targetColliderArray = GameGlobal.OverlapCircleAll2D(tr, radius, AD_Data.LAYER_MONSTER);
            for (int i = 0; i < targetColliderArray.Length; i++) {
                var addExPref = CCPooler.SpawnFromPool<ExplosionSmallPref>(addExpTag, targetColliderArray[i].transform.position, Quaternion.identity);
                if (addExPref) {
                    addExPref.Shot(damageStruct, addExplosionDamage);
                }
            }

            if (targetColliderArray.Length > 0) { //Play AddExplosion Sound, if the Target Exist.
                audioSource.PlayOneShot(addExplosionClips.RandIndex());
            }

            yield break;
        }
    }
}
