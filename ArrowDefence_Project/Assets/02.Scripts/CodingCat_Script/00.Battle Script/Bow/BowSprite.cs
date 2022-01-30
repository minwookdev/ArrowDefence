namespace ActionCat {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class BowSprite : MonoBehaviour {
        #region CUSTOM-EDITOR
#if UNITY_EDITOR
        [CustomEditor(typeof(BowSprite))]
        public class BowSpriteEditor : Editor {
            SerializedObject serialObject;
            BowSprite bowSprite;

            private void OnEnable() {
                serialObject = new SerializedObject(target);
                bowSprite    = target as BowSprite;
            }

            public override void OnInspectorGUI() {
                base.OnInspectorGUI();
                serialObject.Update();
                GUILayout.Space(10f);
                if(GUILayout.Button("SET DEFAULT PARTICLE")) {
                    string path0 = "Assets/09.Effects/Muzzle/ef_1_red.prefab";
                    string path1 = "Assets/09.Effects/Muzzle/ef_2_red.prefab";
                    string path2 = "Assets/09.Effects/Muzzle/ef_3_red.prefab";
                    var particleAsset0 = AssetDatabase.LoadAssetAtPath<ACEffector2D>(path0);
                    var particleAsset1 = AssetDatabase.LoadAssetAtPath<ACEffector2D>(path1);
                    var particleAsset2 = AssetDatabase.LoadAssetAtPath<ACEffector2D>(path2);

                    List<ACEffector2D> particles = new List<ACEffector2D>();
                    particles.Add(particleAsset0);
                    particles.Add(particleAsset1);
                    particles.Add(particleAsset2);

                    bowSprite.muzzleEffect = particles.ToArray();
                    EditorUtility.SetDirty(bowSprite);

                    CatLog.Log(StringColor.GREEN, "SET DEFAULT PARTICLES in BOWSPRITE");
                }
            }

            /// DESCRIPTION
            ///private 멤버 변수를 불러와야 했기 때문에 내부 클래스로 작성함.
        }
#endif
#endregion

        [Header("BOW SPRITE")]
        [SerializeField] Image ImageBattleBow;
        [SerializeField] Sprite SpriteBow;

        [Header("MATERIAL")]
        [SerializeField] Material bowMaterial;

        [Header("IMPACT")]
        [Range(0f, 1f)]  public float StartHitEffectBlend   = 0f;
        [Range(0f, 1f)]  public float StartChromAberrAmount = 0f;
        [Range(0f, .5f)] public float StartFishEyeAmount    = 0f;
        [Range(0f, .5f)] public float StartPinchAmount      = 0f;
        [Range(0f, 1f)]  public float totalFadeTime = .5f;

        [Header("CHARGED")]
        [SerializeField] [RangeEx(0.1f, 1f, 0.1f, "Hit Effect Blend")]
        float startChargedHitEffectBlend = 0.8f;
        [SerializeField] [RangeEx(0.1f, 3f, 0.1f)]
        float chargeEffectTime = 1f;

        [Header("FADE")]
        [Range(0f, 3f)] public float FadeBurnTime = 1f;

        [Header("EFFECT TYPE")]
        [SerializeField] [ReadOnly]
        BOWEFFECTYPE effectType = BOWEFFECTYPE.NONE;

        [Header("PARTICLE")]
        [SerializeField] ACEffector2D[] muzzleEffect = null;

        //Impact Effect Parameters
        string hitEffectBlendParams   = "_HitEffectBlend";   //Range(0f, 1f);
        string chromAberrAmountParams = "_ChromAberrAmount"; //Range(0f, 1f);
        string fishEyeAmountParams    = "_FishEyeUvAmount";  //Range(0f, .5f);
        string pinchAmountParams      = "_PinchUvAmount";    //Range(0f, .5f);

        //Fade Effect parameters
        string FadeAmountParams = "_FadeAmount"; //Range(-0.1f, 1f);

        //Impact Coroutine
        Coroutine effectCoroutine = null;

        public Sprite GetUISprite() {
            if(SpriteBow != null) {
                return SpriteBow;
            }
            else {
                return null;
            }
        }

        private void Start() {
            for (int i = 0; i < muzzleEffect.Length; i++) {
                string pooltag = GlobalSO.Inst.POOLTAG_MUZZLE + i.ToString();
                CCPooler.AddPoolList(pooltag, 2, muzzleEffect[i].gameObject, false);
            }
        }

        private void OnDisable() {
            RestoreMaterial();
        }

        public void EffectImpact() {
            if(effectCoroutine != null) {
                StopCoroutine(effectCoroutine);
            }

            //Start Bow Shot Impact Coroutine
            effectCoroutine = StartCoroutine(EffectImpactCo());
        }

        public void Effect(BOWEFFECTYPE type, bool isRewind = false) {
            switch (effectType) { // Stop Coroutine, Clear Current Type Effect
                case BOWEFFECTYPE.IMPACT:  StopImpact(); break;
                case BOWEFFECTYPE.CHARGED: StopCharge(); break;
                case BOWEFFECTYPE.FADE:    StopFade();  return; // Not Change
            }

            switch (type) {
                case BOWEFFECTYPE.NONE:    RestoreMaterial(); /*Restore Bow Sprite Material*/  break;
                case BOWEFFECTYPE.IMPACT:  effectCoroutine = StartCoroutine(EffectImpactCo());   break;
                case BOWEFFECTYPE.CHARGED: effectCoroutine = StartCoroutine(EffectChargeCo());       break;
                case BOWEFFECTYPE.FADE:    effectCoroutine = StartCoroutine(EffectFadeCo(isRewind)); break;
            }
        }

        public void ActiveBurn(bool isRewind) {
            StartCoroutine(EffectFadeCo(isRewind));
        }

#region EFFECT_COROUTINE

        IEnumerator EffectImpactCo() {
            float progress = 0f;
            float speed = 1 / totalFadeTime;
            effectType = BOWEFFECTYPE.IMPACT;

            while (progress < 1) {
                progress += Time.unscaledDeltaTime * speed;
                bowMaterial.SetFloat(hitEffectBlendParams, Mathf.Lerp(StartHitEffectBlend, 0f, progress));
                bowMaterial.SetFloat(chromAberrAmountParams, Mathf.Lerp(StartChromAberrAmount, 0f, progress));
                bowMaterial.SetFloat(fishEyeAmountParams, Mathf.Lerp(StartFishEyeAmount, 0f, progress));
                bowMaterial.SetFloat(pinchAmountParams, Mathf.Lerp(StartPinchAmount, 0f, progress));

                yield return null;
            }

            //값 보정.
            bowMaterial.SetFloat(hitEffectBlendParams, 0f);
            bowMaterial.SetFloat(chromAberrAmountParams, 0f);
            bowMaterial.SetFloat(fishEyeAmountParams, 0f);
            bowMaterial.SetFloat(pinchAmountParams, 0f);

            //Change Effect Type
            effectType = BOWEFFECTYPE.NONE;
        }

        IEnumerator EffectFadeCo(bool isRewind) {
            float progress = 0f;
            float speed = 1 / FadeBurnTime;
            effectType = BOWEFFECTYPE.FADE;

            while (progress < 1) {
                progress += Time.unscaledDeltaTime * speed;
                if (isRewind == true)
                    bowMaterial.SetFloat(FadeAmountParams, Mathf.Lerp(0f, 1f, progress));
                else
                    bowMaterial.SetFloat(FadeAmountParams, Mathf.Lerp(1f, 0f, progress));
                
                yield return null;
            }

            //값 보정.
            if (isRewind == false)
                bowMaterial.SetFloat(FadeAmountParams, 1f);
            else
                bowMaterial.SetFloat(FadeAmountParams, 0f);

            //Change Effect Type
            effectType = BOWEFFECTYPE.NONE;
        }

        IEnumerator EffectChargeCo() {
            float progress = 0f;
            float speed = 1 / chargeEffectTime;
            effectType = BOWEFFECTYPE.CHARGED;

            while (progress < 1) {
                progress += Time.unscaledDeltaTime * speed;
                bowMaterial.SetFloat(hitEffectBlendParams, Mathf.Lerp(startChargedHitEffectBlend, 0f, progress));

                yield return null;
            }

            //Restore Material, Lerp Safety
            bowMaterial.SetFloat(hitEffectBlendParams, 0f);

            //Change Effect Type
            effectType = BOWEFFECTYPE.NONE;
        }


#endregion

#region EFFECT_STOP

        void StopImpact() {
            if(effectCoroutine != null) {
                StopCoroutine(effectCoroutine);
            }

            bowMaterial.SetFloat(hitEffectBlendParams, 0f);
            bowMaterial.SetFloat(chromAberrAmountParams, 0f);
            bowMaterial.SetFloat(fishEyeAmountParams, 0f);
            bowMaterial.SetFloat(pinchAmountParams, 0f);
        }


        void StopCharge() {
            if(effectCoroutine != null) {
                StopCoroutine(effectCoroutine);
            }

            bowMaterial.SetFloat(hitEffectBlendParams, 0f);
        }

        void StopFade() {
            CatLog.WLog("Fade Effect cannot be stopped while it is active.");
        }

#endregion

        void RestoreMaterial() {
            //Restore <Imapct> <Charged> Params
            if (bowMaterial.GetFloat(hitEffectBlendParams) != 0f){
                bowMaterial.SetFloat(hitEffectBlendParams, 0f);
            }

            if (bowMaterial.GetFloat(chromAberrAmountParams) != 0f){
                bowMaterial.SetFloat(chromAberrAmountParams, 0f);
            }

            if (bowMaterial.GetFloat(fishEyeAmountParams) != 0f) {
                bowMaterial.SetFloat(fishEyeAmountParams, 0f);
            }

            if (bowMaterial.GetFloat(pinchAmountParams) != 0f) {
                bowMaterial.SetFloat(pinchAmountParams, 0f);
            }

            //Resotore <Fade> Params
            if(bowMaterial.GetFloat(FadeAmountParams) != 0f) {
                bowMaterial.SetFloat(FadeAmountParams, 0f);
            }
        }

        #region EFFECT_MUZZLE

        public void ActiveMuzzleFlash(Vector3 position, float eulerAnglesZ) {
            CCPooler.SpawnFromPool<ACEffector2D>(GetRandomMuzzleTag(), position, Quaternion.identity).Play(eulerAnglesZ);
        }

        string GetRandomMuzzleTag() {
            return string.Format("{0}{1}", GlobalSO.Inst.POOLTAG_MUZZLE, Random.Range(0, muzzleEffect.Length));
        }

        #endregion
    }
}
