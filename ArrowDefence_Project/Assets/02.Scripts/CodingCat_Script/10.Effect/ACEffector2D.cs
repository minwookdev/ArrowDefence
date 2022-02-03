namespace ActionCat {
    using ActionCat.Interface;
    using System.Collections;
    using UnityEngine;

    public class ACEffector2D : MonoBehaviour, IPoolObject {
        [Header("COMPONENT")]
        [SerializeField] Transform tr = null;
        [SerializeField] ParticleSystem particleSys = null;
        [SerializeField] ParticleSystemRenderer particleRenderer = null;

        [Header("EFFECT")]
        [SerializeField] [ReadOnly] 
        EFFECTORTYPE effectorType = EFFECTORTYPE.NONE;
        [SerializeField] string sortingLayerName = "";
        [SerializeField] int sortOrder = 0;

        //COROUTINE
        Coroutine playerCo  = null;
        WaitUntil waitUntil = null;

        private void Start() {
            particleRenderer.alignment        = ParticleSystemRenderSpace.Local;
            particleRenderer.sortingLayerName = sortingLayerName;
            particleRenderer.sortingOrder     = sortOrder;

            waitUntil = new WaitUntil(() => particleSys.isStopped == true);
        }

        public void DisableRequest() {
            CCPooler.ReturnToPool(gameObject);
        }

        public void PlayOnce(float degree) {
            if(effectorType == EFFECTORTYPE.NONE) {
                effectorType = EFFECTORTYPE.NEWEFFECT;
            }

            Vector3 rotation = tr.eulerAngles;
            rotation.z = degree;
            tr.eulerAngles = rotation;

            if(playerCo != null) {
                StopCoroutine(playerCo);
            } playerCo = StartCoroutine(RunEffect());
        }

        public void PlayOnce(bool isStartRandRotation = false) {
            if(effectorType == EFFECTORTYPE.NONE) {
                effectorType = EFFECTORTYPE.NEWEFFECT;
            }

            if(isStartRandRotation == true) {
                Vector3 eulerAngles = tr.eulerAngles;
                eulerAngles.z = GameGlobal.RandomAngleDeg();
                tr.eulerAngles = eulerAngles;
            }

            if(playerCo != null) {
                StopCoroutine(playerCo);
            } playerCo = StartCoroutine(RunEffect());
        }

        public void Play(bool isStartRandRotation = false) {
            if(effectorType == EFFECTORTYPE.NONE) {
                effectorType = EFFECTORTYPE.RESTARTER;
            }

            if(isStartRandRotation == true) {
                Vector3 eulerAngles = tr.eulerAngles;
                eulerAngles.z = GameGlobal.RandomAngleDeg();
                tr.eulerAngles = eulerAngles;
            }

            particleSys.Play();
        }

        IEnumerator RunEffect() {
            particleSys.Play();
            yield return waitUntil;
            DisableRequest();
        }
    }
}
