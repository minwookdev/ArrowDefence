namespace ActionCat {
    using ActionCat.Interface;
    using System.Collections;
    using UnityEngine;

    public class ACEffector2D : MonoBehaviour, IPoolObject {
        [Header("EFFECT")]
        [SerializeField] EFFECTORTYPE effetorType = EFFECTORTYPE.NEWEFFECT;
        [SerializeField] Transform tr = null;
        [SerializeField] ParticleSystem particleSys = null;
        [SerializeField] ParticleSystemRenderer particleRenderer = null;

        //COROUTINE
        Coroutine playerCo  = null;
        WaitUntil waitUntil = null;

        private void Start() {
            particleRenderer.alignment        = ParticleSystemRenderSpace.Local;
            particleRenderer.sortingLayerName = GlobalSO.Inst.SORTINGLAYER_EFFECT;
            particleRenderer.sortingOrder     = 1;

            //Muzzle Effect Rotate Offset Apply
            //var mainMod = particleSys.main;
            //mainMod.startRotation = 90f;

            waitUntil = new WaitUntil(() => particleSys.isStopped == true);
        }

        public void DisableRequest() {
            CCPooler.ReturnToPool(gameObject);
        }

        public void Play(float eulerAnglesZ) {
            Vector3 rotation = tr.eulerAngles;
            rotation.z       = eulerAnglesZ;
            tr.eulerAngles   = rotation;

            gameObject.SetActive(true);

            if(playerCo != null) {
                StopCoroutine(playerCo);
            }
            playerCo = StartCoroutine(RunEffect());
        }

        IEnumerator RunEffect() {
            particleSys.Play();
            yield return waitUntil;
            DisableRequest();
        }
    }
}
