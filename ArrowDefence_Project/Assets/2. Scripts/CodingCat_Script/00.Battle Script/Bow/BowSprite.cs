namespace ActionCat {
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    public class BowSprite : MonoBehaviour {
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

        [Header("FADE")]
        [Range(0f, 3f)] public float FadeBurnTime = 1f;

        //Impact Effect Parameters
        string hitEffectBlendParams   = "_HitEffectBlend";   //Range(0f, 1f);
        string chromAberrAmountParams = "_ChromAberrAmount"; //Range(0f, 1f);
        string fishEyeAmountParams    = "_FishEyeUvAmount";  //Range(0f, .5f);
        string pinchAmountParams      = "_PinchUvAmount";    //Range(0f, .5f);

        //Fade Effect parameters
        string FadeAmountParams = "_FadeAmount"; //Range(-0.1f, 1f);

        //Impact Coroutine
        Coroutine impactCo = null;

        public Sprite GetUISprite() {
            if(SpriteBow != null) {
                return SpriteBow;
            }
            else {
                return null;
            }
        }

        private void OnDisable() {
            RestoreMaterial();
        }

        public void ActiveImpact() {
            if(impactCo != null) {
                StopCoroutine(impactCo);
            }

            //Start Bow Shot Impact Coroutine
            impactCo = StartCoroutine(ShotImpactCo());
        }

        public void ActiveBurn(bool isRewind) {
            StartCoroutine(FadeCo(isRewind));
        }

        IEnumerator ShotImpactCo() {
            float progress = 0f;
            float speed = 1 / totalFadeTime;

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
        }

        IEnumerator FadeCo(bool isRewind) {
            float progress = 0f;
            float speed = 1 / FadeBurnTime;

            while (progress < 1) {
                progress += Time.unscaledDeltaTime * speed;
                if (isRewind == false)
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
        }

        void RestoreMaterial() {
            //Restore Impact Params
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

            //Resotore Fade Params
            if(bowMaterial.GetFloat(FadeAmountParams) != 0f) {
                bowMaterial.SetFloat(FadeAmountParams, 0f);
            }
        }
    }
}
