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
    }

    private void OnDestroy() {
        if (GameManager.Instance != null) {
            GameManager.Instance.ReleaseAllEventsWithNoneState();
        }
    }

    void RestoreButtons() {
        startButtonCanvasGroup.DOFade(StNum.floatOne, 0.7f).SetLoops(-1, LoopType.Yoyo).OnStart(() => startButtonCanvasGroup.blocksRaycasts = true);
        foreach (var image in imagesButton) {
            image.raycastTarget = true;
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
                                         foreach (var image in imagesButton) {
                                             image.DOFade(StNum.floatOne, 0.3f).SetDelay(tweenDelay).OnStart(() => image.raycastTarget = true);
                                         }
                                     })
                                     .SetDelay(tweenDelay);
        //Play Button Fade
        startButtonCanvasGroup.DOFade(StNum.floatOne, 0.7f).SetLoops(-1, LoopType.Yoyo).SetDelay(tweenDelay).OnStart(() => startButtonCanvasGroup.blocksRaycasts = true);

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
}

