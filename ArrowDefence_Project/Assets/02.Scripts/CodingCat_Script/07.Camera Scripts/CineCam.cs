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
        [SerializeField] Transform testTr    = null;
        private float maxCamMoveDistance = 1f;  // 0f ~ 1f
        private Vector2 initCamPosition = Vector2.zero;
        private Vector2 tempCamPosition = Vector2.zero;

        [Header("CAMERA ZOOM")]
        [SerializeField] bool isRealTimeScaleZoom = false;
        [SerializeField] float zoomInOrthographicSize = 9.5f;
        float initOrthographicSize = 0f;
        float zoomInLerpDuration   = 1.5f;
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
            initCamPosition = camTarget.position;                                 //기준 포지션 잡아둠
            tempCamPosition = initCamPosition;

            //초기 OrthographicSize 캐싱해둠
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
        public void CamMovement(Vector3 direction) {
            //testTr.eulerAngles = eulerAngles;    //활의 오일러 각 받아옴

            //타겟 위치계산
            //Vector2 targetPosition = (Vector2.Distance(testTr.position, initCamPosition) <= 3f) ? testTr.position + (eulerAngles * maxCamMoveDistance) : testTr.position;
            //testTr.position = targetPosition;

            //Vector2 targetPos = testTr.position + (eulerAngles.normalized * 2f);
            //testTr.position = targetPos;

            //Ex1.
            //Vector2 targetPos  = testTr.position + (direction.normalized * maxCamMoveDistance);
            //maxCamMoveDistance = (Vector2.Distance(initCamPosition, targetPos) <= 2f) ? 1f : 0f;
            //testTr.position    = targetPos;
            //Vector2 destPos   = (Vector2.Distance(initCamPosition, targetPos) <= 2f) ? targetPos : (Vector2)testTr.position;
            //testTr.position = targetPos;

            //tempCamPosition = camTarget.position;
            //tempCamPosition = (Vector2.Distance(tempCamPosition, initCamPosition) <= 1f) ? tempCamPosition.normalized * maxCamMoveDistance : tempCamPosition;
            //tempCamPosition = (Vector2.Distance(tempCamPosition, initCamPosition) <= 1f) ? tempCamPosition.normalized * maxCamMoveDistance : tempCamPosition;
            //testTr.position = tempCamPosition;

            //Vector3 targetPos = Vector3.SmoothDamp(testTr.position, )

            //Direction Vector의 길이값을 가져와서 거리계산 다시
            //일단 방향은 잡혔음
            //그리고 지금 유지시간 체크한다음에 Orthographic 계산해줌
            //distanceOfDirection = (distanceOfDirection > 1f) ? distanceOfDirection - 1f : distanceOfDirection;
            float distanceOfDirection = (direction.magnitude <= 2f) ? direction.magnitude : 2f;     //distance에 들어오는 총량을 2f로 제한
            float distance = distanceOfDirection * 0.5f;                                            //distance변수는 항상 0~1사이의 값을 갖도록 함 (만약에 카메라를 좀 더 멀리가게 하고싶으면 저 0.5f를 계산해서 수정하면 된다)
            Vector2 destPosition = initCamPosition + (Vector2)(-direction.normalized * distance);
            testTr.position = destPosition;

            //이제 Target Transform이 따라붙어보기
        }

        /// <summary>
        /// Bow Controller에서 사용하기 위한 목적으로 설계
        /// </summary>
        /// <param name="direction"></param>
        public void CamMove2Direction(Vector3 direction) {

        }

        /// <summary>
        /// 카메라를 원래 위치로 복귀
        /// </summary>
        public void CameraMove2InitPos() {

        }

        public void ZoomIn2Co() {
            if (!isZoomIn) {
                if (zoomCoroutine != null) {
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

#endregion

        private void OnDestroy() {
            Inst = null;
        }
    }
}
