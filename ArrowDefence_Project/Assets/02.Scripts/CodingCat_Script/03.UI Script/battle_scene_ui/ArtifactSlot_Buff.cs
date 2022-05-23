namespace ActionCat {
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using TMPro;

    public class ArtifactSlot_Buff : AccessorySkillSlot {
        [Header("BUFF TYPE SLOT")]
        [SerializeField] Image coolDownMaskImage = null;
        [SerializeField] TextMeshProUGUI coolDownTmp = null;

        [Header("BUFF")]
        [SerializeField] [ReadOnly] float maxCoolDown = 0f;
        [SerializeField] [ReadOnly] float currentCoolDownTime = 0f;
        [SerializeField] [ReadOnly] float effectDuration = 0f;

        /// <summary>
        /// GameState가 PauseMode에 들어갔을 때, 버프가 종료되는 것을 방지. Duration동안 대기. (임시방편)
        /// </summary>
        WaitUntil waitPauseEnd = null;

        public bool InBattleState {
            get {
                return (GameManager.Instance.GameState == GAMESTATE.STATE_INBATTLE || GameManager.Instance.GameState == GAMESTATE.STATE_BOSSBATTLE);
            }
        }

        public ArtifactSlot_Buff Init(AccessorySPEffect effect, System.Action notifyAction) {
            artifactEffect    = effect;
            notifyPlayAction  = notifyAction;

            eventTrigger     = GetComponent<EventTrigger>();
            skillIcon.sprite = (artifactEffect.IconSprite != null) ? artifactEffect.IconSprite : skillIcon.sprite;

            maxCoolDown         = artifactEffect.Condition.CoolDown;
            currentCoolDownTime = (artifactEffect.IsStartingPrepared) ? 0f : maxCoolDown;

            coolDownMaskImage.fillAmount = 1f;
            coolDownTmp.text = "";

            //게임종료 및 게임오버 시 효과 발동 중지처리
            GameManager.Instance.AddListnerEndBattle(ForceQuitEffect);
            GameManager.Instance.AddListnerGameOver(ForceQuitEffect);

            //Assignment new Wait Pause Ended
            waitPauseEnd = new WaitUntil(() => GameManager.Instance.GameState != GAMESTATE.STATE_PAUSE);
            return this;
        }

        /// <summary>
        /// 유물효과 발동 중 강제종료 처리 콜백
        /// </summary>
        private void ForceQuitEffect() {
            if (isEffectActivation) {
                StopCoroutine(effectActivationCoroutine);
                artifactEffect.OnStop();
                CatLog.Log("Artifact 효과 강제종료 처리.");

                //효과발동 비활성화
                isEffectActivation = false;
                isPreparedActive = false;
                coolDownTmp.text = "";
                coolDownMaskImage.fillAmount = 1f;
            }
        }

        System.Collections.IEnumerator ActiveEffectCoroutine() {
            isEffectActivation = true;
            //effectDuration = artifactEffect.OnActive();
            //effectDuration = artifactEffect.GetDuration(out int activatingCount);
            var duration = artifactEffect.GetDuration(out int activatingCount);
            //effectDuration = duration;
            currentCoolDownTime = maxCoolDown;

            while (activatingCount > 0) {
                artifactEffect.OnActive();
                effectDuration = duration;
                while (effectDuration > 0f) {
                    yield return waitPauseEnd;  //yield return null 해줄필요 없음. 그 역활도 같이 함
                    //yield return null;
                    effectDuration -= Time.unscaledDeltaTime;
                }
                activatingCount--;
            }

            //Wait For Duration
            //while (effectDuration > 0f) {
            //    yield return null;
            //    effectDuration -= Time.unscaledDeltaTime;
            //}

            CatLog.Log("Artifact Effect 종료, 초기화 진행.");

            artifactEffect.OnStop();
            isPreparedActive   = false;
            isEffectActivation = false;
        }

        private void Update() {
            //Only Update In-Battle State
            if (!InBattleState) {
                return;
            }

            //Update CoolDown Timer
            if (isPreparedActive == false) {
                currentCoolDownTime -= Time.unscaledDeltaTime;
                if (currentCoolDownTime <= 0f) {
                    isPreparedActive = true;
                    notifyPlayAction();
                }
            }

            //Update Slot UI Element
            coolDownTmp.text = (currentCoolDownTime > 0f && currentCoolDownTime < maxCoolDown) ? Mathf.CeilToInt(currentCoolDownTime).ToString() : "";
            coolDownMaskImage.fillAmount = currentCoolDownTime / maxCoolDown;
        }

        public void BE_ACTIVE() {
            //발동 가능여부 판단해서 효과발동 처리
            if (isPreparedActive && isEffectActivation == false) {
                effectActivationCoroutine = StartCoroutine(ActiveEffectCoroutine());
            }
            else {
                Notify.Inst.Message("Artifact Not Prepared !");
            }
        }
    }
}
