namespace ActionCat {
    using UnityEngine;
    using Cinemachine;

    public class CineCam : MonoBehaviour {
        public static CineCam Inst { get; private set; }

        private CinemachineVirtualCamera cineVirtualCam;
        private float startingIntensity;
        private float shakeTimer;
        private float shakeTimerTotal;

        [Header("CAMERA MOVEMENT")]
        [SerializeField] Transform camTarget = null;
        [SerializeField] Transform followTargetTr = null;

        float distanceOfDirection = 0f;
        float distance            = 0f;
        float maxDistance         = 0.8f;
        float restoreLerpDuration = 0.2f;
        Vector2 initCamPos  = Vector2.zero;
        Vector2 destPos     = Vector2.zero;
        Vector2 camVelocity = Vector2.zero;
        Coroutine camRestoreCoroutine = null;

        [Header("CAMERA ZOOM")]
        [SerializeField] bool isRealTimeScaleZoom = false;
        [SerializeField] float zoomInOrthographicSize = 9.3f;
        float initOrthographicSize = 0f;
        float zoomInLerpDuration   = 1f;
        float zoomOutLerpDuration  = 0.2f;
        bool isZoomIn = false;
        Coroutine zoomCoroutine = null;

        //float?이걸로 실험해보기
        //코루틴에 대한 실험

        private void Awake() {
            Inst = this;
            cineVirtualCam = GetComponent<CinemachineVirtualCamera>();

            //2D Zoom-in & out Orthographic size 
            //cineVirtualCam.m_Lens.OrthographicSize = 10f;
            camTarget = (camTarget == null) ? cineVirtualCam.Follow : camTarget;  //CineVirtualCam에서 Follow타겟 잡아오고
            initCamPos = camTarget.position;                                      //기준 포지션 잡아둠

            //초기 OrthographicSize 캐싱
            initOrthographicSize = cineVirtualCam.m_Lens.OrthographicSize;
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

#region CAMERA_MOVEMENT

        /// <summary>
        /// Bow Controller 전용으로 사용하기 위함으로 설계
        /// </summary>
        /// <param name="direction"></param>
        public void CamMove2Direction(Vector3 direction) {
            if (camRestoreCoroutine != null) {  //카메라 복귀 코루틴 중지
                StopCoroutine(camRestoreCoroutine);
            }

            distanceOfDirection = (direction.magnitude < 2f) ? direction.magnitude : 2f;    //활을 당기고있을 때, 터치에 맞춰서 움직일 거리
            distance = distanceOfDirection * (maxDistance / 2f);
            destPos  = initCamPos + (Vector2)(-direction.normalized * distance);
            followTargetTr.position = destPos;
            camTarget.position = Vector2.SmoothDamp(camTarget.position, followTargetTr.position, ref camVelocity, 0.1f);
        }

        /// <summary>
        /// 카메라를 원래 위치로 복귀 (업데이트)
        /// </summary>
        public void CamPosRestore() {
            if (camRestoreCoroutine != null) {
                StopCoroutine(camRestoreCoroutine);
            }

            camTarget.position = Vector2.SmoothDamp(camTarget.position, initCamPos, ref camVelocity, 0.1f);
        }

        /// <summary>
        /// 카메라를 원래 위치로 복귀 (코루틴)
        /// </summary>
        public void CamPosRestore2Co() {
            camRestoreCoroutine = StartCoroutine(CameraPosRestore());
        }

        public void ZoomOut2Co() {
            if (!isZoomIn) {                 // Update에서 돌려져도 상관없도록 현재 상태정의 
                if (zoomCoroutine != null) { // 이미 Zoom관련 로직이 실행중이면 중지처리
                    StopCoroutine(zoomCoroutine);
                }
                zoomCoroutine = StartCoroutine(ZoomInCoroutine());
                isZoomIn = true;
            }
        }

        public void ZoomRestore2Co() {
            if (isZoomIn) {
                if (zoomCoroutine != null) {
                    StopCoroutine(zoomCoroutine);
                }
                zoomCoroutine = StartCoroutine(ZoomOutCoroutine());
                isZoomIn = false;
            }
        }

        System.Collections.IEnumerator ZoomInCoroutine() {
            float timeElapsed = 0f;
            float currentOrthographic = cineVirtualCam.m_Lens.OrthographicSize;
            while (timeElapsed < zoomInLerpDuration) {
                cineVirtualCam.m_Lens.OrthographicSize = Mathf.Lerp(currentOrthographic, zoomInOrthographicSize, timeElapsed / zoomInLerpDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            cineVirtualCam.m_Lens.OrthographicSize = zoomInOrthographicSize;
        }

        System.Collections.IEnumerator ZoomOutCoroutine() {
            float timeElapsed = 0f;
            float currentOrthographicSize = cineVirtualCam.m_Lens.OrthographicSize;
            while (timeElapsed < zoomOutLerpDuration) {
                cineVirtualCam.m_Lens.OrthographicSize = Mathf.Lerp(currentOrthographicSize, initOrthographicSize, timeElapsed / zoomOutLerpDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            cineVirtualCam.m_Lens.OrthographicSize = initOrthographicSize;
        }

        System.Collections.IEnumerator CameraPosRestore() {
            float timeElapsed = 0f;
            Vector2 currentCamPos = camTarget.position;
            while (timeElapsed < restoreLerpDuration) {
                camTarget.position = Vector2.Lerp(currentCamPos, initCamPos, timeElapsed / restoreLerpDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            camTarget.position = initCamPos;
        }

#endregion

        private void OnDestroy() {
            Inst = null;
        }
    }
}
