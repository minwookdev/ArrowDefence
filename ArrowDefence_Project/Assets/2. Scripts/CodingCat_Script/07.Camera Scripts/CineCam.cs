namespace ActionCat {
    using UnityEngine;
    using Cinemachine;

    public class CineCam : MonoBehaviour
    {
        public static CineCam Inst { get; private set; }

        private CinemachineVirtualCamera cineVirtualCam;
        private float startingIntensity;
        private float shakeTimer;
        private float shakeTimerTotal;

        private void Awake() {
            Inst = this;
            cineVirtualCam = GetComponent<CinemachineVirtualCamera>();

            //2D Zoom-in & out Orthographic size 
            //cineVirtualCam.m_Lens.OrthographicSize = 10f;
        }

        public void ShakeCamera(float intensity, float shaketime) {
            var basicMultipleChannelPerlin = 
                cineVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            basicMultipleChannelPerlin.m_AmplitudeGain = intensity;

            startingIntensity = intensity;
            shakeTimerTotal   = shaketime;
            shakeTimer        = shaketime;
        }

        private void Update() {
            if(shakeTimer > 0) {
                shakeTimer -= Time.deltaTime;
                if(shakeTimer <= 0f) {
                    //Timer Over
                    var basicMultipleChannelPerlin = 
                        cineVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                    basicMultipleChannelPerlin.m_AmplitudeGain = 
                        Mathf.Lerp(startingIntensity, 0f, (1 - (shakeTimer / shakeTimerTotal))); 
                }
            }

            /// -> Coroutine으로 다시 작성 가능 ??
            /// 
        }

        private void OnDestroy() {
            Inst = null;
        }
    }
}
