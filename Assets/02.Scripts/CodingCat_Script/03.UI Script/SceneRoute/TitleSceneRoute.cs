using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using ActionCat;
using ActionCat.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// Script for UI Interaction of StartScene
/// It Serves as a Route to Help each Managers and UI Event connection
/// </summary>
public class TitleSceneRoute : MonoBehaviour {
    [Header("REQUIER MODULE")]
    //[SerializeField] MonsterSpawner spawner = null;
    [SerializeField] TextWritter writter = null;
    [SerializeField] CreateFilePanel createFilePanel = null;
    [SerializeField] ErrorPanel errorPanel = null;
    [SerializeField] SettingsPanel settingsPanel = null;

    [Header("DepthOfField")]
    [SerializeField] PostProcessVolume titleVolume = null;
    [SerializeField] [RangeEx(0.1f, 3f, 0.1f)] float lerpDuration = 1.5f;
    float startFocalLength  = 0f;
    float targetFocalLength = 300f;

    [Header("INTRO")]
    [SerializeField] GameObject skipPanel = null;
    [SerializeField] [RangeEx(0.1f, 10f, 0.1f)] float maxWaitTime = 3f;

    [Header("TITLE ELEMENTE")]
    [SerializeField] RectTransform titleRect = null;
    [SerializeField] CanvasGroup startButtonCanvasGroup = null;
    [SerializeField] Image[] imagesButton = null;
    Vector3 titleTextScale = Vector3.zero;
    Color tempColor = Color.clear;

    [Header("CUTOUT")]
    [SerializeField] RectTransform cutoutRect = null;
    Vector2 cutoutSizeDelta = new Vector2(2000f, 2000f);

    [Header("CAMERA MOVEMENT")]
    [SerializeField] Transform camTargetTr = null;
    [SerializeField] [RangeEx(0.1f, 3f, 0.1f)] float randomDistX = 1f;
    [SerializeField] [RangeEx(0.1f, 3f, 0.1f)] float randomDistY = 1f;
    [SerializeField] [RangeEx(0.1f, 5f, .1f)]  float camMoveSpeed = 0.5f;
    [SerializeField] bool isCamMovementActive = true;
    Coroutine cameraMovementCo = null;

    [Header("SOUND")]
    [SerializeField] ACSound titleMusic = null;

    Sequence titleSeq = null;

    [Header("ASSET DOWNLOAD")]
    [SerializeField] RectTransform rectTrDownloadingParent = null;
    [SerializeField] Slider sliderDownload                 = null;
    [SerializeField] TMPro.TextMeshProUGUI tmpDownloadSize = null;
    [SerializeField] TMPro.TextMeshProUGUI tmpDownloadPack = null;
    AssetDeliveryManager assetDeliveryManager = new AssetDeliveryManager();
    WaitUntil waitSliderUpdate = null;
    bool isAllAssetDownloaded  = false;
    bool isSliderUpdate        = false;

    public void Awake() {
        GameManager.Instance.Initialize();
        AdsManager.Instance.InitRuntimeMgr();

        //Hide Title Text 
        titleTextScale = titleRect.localScale;
        titleRect.localScale = Vector3.zero;

        //Hide Start Button
        startButtonCanvasGroup.alpha = 0f;
        startButtonCanvasGroup.blocksRaycasts = false;

        //Hide All Title Buttons
        foreach (var image in imagesButton) {
            this.tempColor = image.color;
            this.tempColor.a = 0f;
            image.color = this.tempColor;
            image.raycastTarget = false;
        }

        //Fill Cutout Rect - Hide Screen
        cutoutRect.sizeDelta = Vector2.zero;
    }

    private void Start() {
        //Start Monster Spawn
        GameManager.Instance.ChangeGameState(GAMESTATE.STATE_INBATTLE);

        //Open Screen
        cutoutRect.DOSizeDelta(cutoutSizeDelta, 1f).From(Vector2.zero).SetDelay(.5f);
        Invoke(nameof(BE_SKIP), maxWaitTime); //Invoke Method

        //에러패널 부모 가져와서 쓰고있는데, 아무 UI 패널의 부모가져와서 박아주면된다
        Notify.Inst.Init(errorPanel.transform.root.GetComponent<RectTransform>());

        //Play TitleScene Background Music
        titleMusic.PlaySoundWithFadeOut(1f, true);

        //일단 체크해두는데, 에셋 다운로드보다 앱-업데이트 체크가 먼저 이루어져야함 ---
        //여기다 인-앱 업데이트 구현해두셈 

#if   UNITY_EDITOR
        isAllAssetDownloaded = true;
#elif UNITY_ANDROID
        // 모든 에셋팩이 다운로드 상태인지 체크
        isAllAssetDownloaded = assetDeliveryManager.IsDownloadedAllAssetPacks();
        if (!isAllAssetDownloaded) {
            StartCoroutine(AssetPackDownloadCoroutine());

            // 슬라이더 코루틴 켜줌
            waitSliderUpdate = new WaitUntil(() => isSliderUpdate);
            StartCoroutine(DownloadSliderUpdate());
        }
        // 릴리즈 필요없어서 일단 주석해둠. (이 스크립트에서 인스턴스 생성하기 때문에 씬 종료되면 알아서 지워짐)
        //else { // 이미 모든팩이 설치되어있음 - 딜리버리 매니저 필요없음
        //    assetDeliveryManager = null;
        //}

        bool isDetectedOldAssetPack = assetDeliveryManager.IsDetectedOldAssetPack(out string[] detectedOldPackNames);
        if (isDetectedOldAssetPack) {   // 사용되지 않는 에셋-팩이 다운로드 된 경우. 일단 캐치만 하고있음
            foreach (var assetName in detectedOldPackNames) {
                CatLog.Log($"Detected Old Asset Names: {assetName}");
            }
        }
#endif
    }

    private void OnDestroy() {
        if (GameManager.Instance != null) {
            GameManager.Instance.ReleaseAllEventsWithNoneState();
        }
    }

    void RestoreButtons() {
        startButtonCanvasGroup.DOFade(StNum.floatOne, 0.7f).SetLoops(-1, LoopType.Yoyo).OnStart(() => startButtonCanvasGroup.blocksRaycasts = true);
        foreach (var image in imagesButton) {
            image.DOFade(StNum.floatOne, .2f).From(0f).OnComplete(() => image.raycastTarget = true);
        }
    }

    void BreakButtons() {
        startButtonCanvasGroup.DOKill();
        startButtonCanvasGroup.alpha = 0f;
        startButtonCanvasGroup.blocksRaycasts = false;
        foreach (var image in imagesButton) {
            image.raycastTarget = false;
        }
    }

#region Action_Btn

    public void BE_START() {
        BreakButtons();
        StartCoroutine(LoadUserJson());
    }

    public void BE_SKIP() {
        CatLog.Log("Called Skip Function !");
        CancelInvoke(nameof(BE_SKIP));
        skipPanel.SetActive(false);
        //isOpenMenu = true;

        //Play Sequence
        float tweenDelay = Mathf.Round(0.7f / lerpDuration);
        titleSeq = DOTween.Sequence().Append(titleRect.DOScale(titleTextScale, StNum.floatOne).From(Vector3.zero).SetEase(Ease.OutExpo))
                                     .Append(titleRect.DOShakeScale(0.7f).SetEase(Ease.OutQuad))
                                     .OnStart(() => {
                                         // 모든 에셋팩이 다운로드 되어있다는 것을 확인하고 시작해줌
                                         if (isAllAssetDownloaded) {
                                             foreach (var image in imagesButton) {
                                                 image.DOFade(StNum.floatOne, 0.3f).SetDelay(tweenDelay).OnStart(() => image.raycastTarget = true);
                                             }
                                         }
                                     })
                                     .SetDelay(tweenDelay);
        //Play Button Fade
        if (isAllAssetDownloaded) { //모든 에셋팩이 다운로드되어있을때만 버튼 살려줌
            startButtonCanvasGroup.DOFade(StNum.floatOne, 0.7f).SetLoops(-1, LoopType.Yoyo).SetDelay(tweenDelay).OnStart(() => startButtonCanvasGroup.blocksRaycasts = true);
        }

        StartCoroutine(EnableBlurCo());

        //Camera Movement >> Blur처리가 완료되고 나서 실행됨.
        cameraMovementCo = (isCamMovementActive) ? StartCoroutine(CameraMovement(lerpDuration)) : null;
    }

    public void BE_REMOVE_JSON() {
        GameManager.Instance.TryDeleteSaveJson(out string message);
        Notify.Inst.Message(message);
        ActionCat.Data.CCPlayerData.Clear();
    }

    public void BE_OPEN_SETTINGS() => settingsPanel.OpenPanel();

#endregion

    System.Collections.IEnumerator EnableBlurCo() {
        if (titleVolume == null) {
            CatLog.ELog("Global Volume is Not Exists.");
            yield break;
        }

        titleVolume.profile.TryGetSettings<DepthOfField>(out DepthOfField dof);
        if (dof == null) {
            CatLog.ELog("this PostProcessing Profile is Not Exists DepthOfField Settings.");
            yield break;
        }

        //dof.active = true;
        dof.enabled.value = true;
        startFocalLength = dof.focalLength;
        float timeElapsed = 0f;
        while (timeElapsed < lerpDuration) {
            dof.focalLength.value = Mathf.Lerp(startFocalLength, targetFocalLength, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        dof.focalLength.value = targetFocalLength;
    }

    System.Collections.IEnumerator LoadUserJson() {
        writter.Switch(true);
        bool isSuccessLoad = GameManager.Instance.LoadUserJsonFile(out string log, out bool isRecommendCreateNewJson);
        if (!isSuccessLoad) { //불러오기 실패
            if (!isRecommendCreateNewJson) { //새로운 세이브파일 생성을 권장하지 않을 경우. 에러패널 띄우고 코루틴 종료
                errorPanel.EnablePanel(log);
                yield break;
            }

            createFilePanel.EnablePanel();  //유저선택 대기
            yield return new WaitUntil(() => createFilePanel.IsRequestCreateJson.HasValue);

            if (createFilePanel.IsRequestCreateJson.Value == false) {
                writter.Switch(false);
                createFilePanel.ClearRequestValue();
                RestoreButtons();
                yield break;
            }

            GameManager.Instance.InitPlayerData();      //Clear Exists PlayerData.
            GameManager.Instance.SupplyInitItem();      //Supply Initial Items and Slot.
            GameManager.Instance.SaveUserJsonFile();    //New UserSave Json Create.
            if (GameManager.Instance.LoadUserJsonFile(out string retryLog, out bool retryRecommend) == false) {
                //Retry Load 
                errorPanel.EnablePanel(retryLog);
                yield break;
            }

            createFilePanel.ClearRequestValue();
        }

        //불러오기 성공 ! - 뻥대기
        yield return new WaitForSeconds(RandomEx.RangeFloat(3f, 4f));
        writter.Switch(false);
        if (isCamMovementActive) {  //Disable CameraMovement Coroutine.
            StopCoroutine(cameraMovementCo);
        }
        titleMusic.StopSoundWithFadeIn(1f, true);
        //GameManager.Instance.ChangeGameState(GAMESTATE.STATE_NONE); //Break MonsterSpawn <-- OnDestroy에서 해주고있음

        // writter 종료하고, 씬 넘기기 처리
        yield return cutoutRect.DOSizeDelta(Vector2.zero, StNum.floatOne).SetDelay(1.5f).WaitForCompletion();
        SceneLoader.Instance.LoadScene(AD_Data.SCENE_MAIN);
    }

    System.Collections.IEnumerator CameraMovement(float delayTime = 0f) {
        yield return new WaitForSeconds(delayTime);
        //
        Vector2 basicPosition = camTargetTr.position;
        float camMoveTime = 2f;
        //float checkDist = 0.1f;

        while (true) {
            var randomPos = new Vector2(RandomEx.RangeFloat(basicPosition.x - randomDistX, basicPosition.x + randomDistX), RandomEx.RangeFloat(basicPosition.y - randomDistY, basicPosition.y + randomDistY));
            float timeElapsed = camMoveTime;

            //CatLog.Log($"Random Position X: {randomPos.x}, Y: {randomPos.y}");

            while (timeElapsed > 0f) { //Distance Check는 혹시몰라서 넣어놓은 것.
                camTargetTr.position = Vector2.MoveTowards(camTargetTr.position, randomPos, camMoveSpeed);                      //항시 동일한 속도
                //camTargetTr.position = Vector2.MoveTowards(camTargetTr.position, randomPos, camMoveSpeed * Time.deltaTime);   //느리게~빨라짐
                timeElapsed -= Time.deltaTime;
                yield return null;
            }
        }
    }

    private void RestoreProfile() {
        //테스트 결과 Scene을 다시로드하면 코드상으로 수정한 Profile의 내용이 초기화 되는것을 확인.
        //Restore Profile은 필요하지 않다.
    }

#region PLAY-ASSET-DOWNLOAD
    
    System.Collections.IEnumerator AssetPackDownloadCoroutine() {
        float startTime = Time.time; // 에셋 팩 업데이트 작업시작 시간 저장

        // 시작하기 전 지워야 할 에셋 팩이 존재하는지 확인. <추후에 지워야 할 에셋 팩만 존재하는 경우 로직 분리>
        var isDetectedOldAssetPack = assetDeliveryManager.IsDetectedOldAssetPack(out string[] detectedOldAssetPackNames);
        if (isDetectedOldAssetPack) { // 딜리트 타겟 에셋팩 존재
            yield return assetDeliveryManager.RemoveOldAssetPacks(detectedOldAssetPackNames);
        }

        // 에셋 팩 다운로드 시작
        isSliderUpdate = true;       // 슬라이더 업데이트 시작
        if (!assetDeliveryManager.IsSoundPackDownloaded) {
            // 변수 정리
            tmpDownloadSize.text = "[00.00 KB]";
            tmpDownloadPack.text = "SOUND ASSET";

            // 에셋 팩 총 다운로드 용량 구함
            yield return StartCoroutine(assetDeliveryManager.GetTotalDownloadSizeAsync(CustomAssetPack.SoundAssetPackName));
            tmpDownloadSize.text = string.Format("[{0}]", assetDeliveryManager.GetAssetPackDownloadSize());  //용량 구하는 코루틴 끝났으면 텍스트 문자열 값 전달

            // 다운로드 시작.
            yield return StartCoroutine(assetDeliveryManager.LoadSoundAssetPackAsync());
        }

        if (!assetDeliveryManager.IsFontsPackDownloaded) {
            // 변수 정리
            tmpDownloadSize.text = "[00.00 KB]";
            tmpDownloadPack.text = "FONT ASSET";

            // 에셋 팩 총 다운로드 용량 구함
            yield return StartCoroutine(assetDeliveryManager.GetTotalDownloadSizeAsync(CustomAssetPack.FontsAssetPackName));
            tmpDownloadSize.text = string.Format("[{0}]", assetDeliveryManager.GetAssetPackDownloadSize());  //용량 구하는 코루틴 끝났으면 텍스트 문자열 값 전달

            // 다운로드 시작
            yield return StartCoroutine(assetDeliveryManager.LoadFontsAssetPackAsync());
        }

        // 슬라이더 업데이트 중지
        isSliderUpdate = false;

        // 완료대기시간 (개발자 확인용)
        float completeTime = Time.time;
        if (completeTime < startTime + maxWaitTime + 2f) {                  // 다운로드가 빨리 끝난 경우
            float waitTime = (startTime + maxWaitTime + 2f) - completeTime; // 타이틀이 정상적으로 열리는 시간 + 2 Sec 가 될때까지 대기
            while (waitTime > 0) {
                waitTime -= Time.deltaTime;
                yield return null;
            }
        }

        // 모든 에셋-팩이 정상적으로 다운로드되었는지 확인
        bool allAssetDownloaded = assetDeliveryManager.IsDownloadedAllAssetPacks();
        if (!allAssetDownloaded) {
            errorPanel.EnablePanel("Failed to Download AssetPack", "AssetPack Download Error");
            yield break;
        }

        // 다운로드 완료 처리해주고 버튼 살려줌
        isAllAssetDownloaded = true;
        RestoreButtons();   
    }

    System.Collections.IEnumerator DownloadSliderUpdate() {
        // 에셋 다운로드 슬라이더 그룹 활성화
        sliderDownload.value = 0f;
        rectTrDownloadingParent.gameObject.SetActive(true);

        while (!isAllAssetDownloaded) { // 에셋을 다운로드가 완료되지않았을때만 슬라이더 업데이트
            sliderDownload.value = assetDeliveryManager.CurrentDownloaded;
            yield return waitSliderUpdate; // 다운로드 중인 에셋이 바뀔때 잠시 대기 
        }

        rectTrDownloadingParent.gameObject.SetActive(false);
    }

#endregion
}

