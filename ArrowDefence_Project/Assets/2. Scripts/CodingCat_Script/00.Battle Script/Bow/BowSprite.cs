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

        //Impact Effect Parameters
        string hitEffectBlendParams   = "_HitEffectBlend";   //Range(0f, 1f);
        string chromAberrAmountParams = "_ChromAberrAmount"; //Range(0f, 1f);
        string fishEyeAmountParams    = "_FishEyeUvAmount";  //Range(0f, .5f);
        string pinchAmountParams      = "_PinchUvAmount";    //Range(0f, .5f);

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

        public void ActiveImpact() {
            if(impactCo != null) {
                StopCoroutine(impactCo);
            }

            //Start Bow Shot Impact Coroutine
            impactCo = StartCoroutine(ShotImpactCo());
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

            //
            bowMaterial.SetFloat(hitEffectBlendParams, 0f);
            bowMaterial.SetFloat(chromAberrAmountParams, 0f);
            bowMaterial.SetFloat(fishEyeAmountParams, 0f);
            bowMaterial.SetFloat(pinchAmountParams, 0f);
        }
    }
}
