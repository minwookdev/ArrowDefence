namespace CodingCat_Games
{
    using CodingCat_Scripts;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    public class BattleProgresser : MonoBehaviour
    {
        [Header("BATTLE PROGRESS")]
        public float StartTime = 3f;
        public float EndTime = 2f;
        private float startWaitingTime;
        private float endWaitingTime;
        private bool isInitialized = false;

        [Header("STAGE CLEAR COUNT")]
        [Range(100, 1000)] 
        public float MaxClearCount = 100;
        private float currentClearCount = 0f;

        [Header("SLIDER'S")]
        public Slider ClearSlider;
        public Slider PlayerHealthSlider;
        public Slider EnemyBossHealthSlider;
        public float SliderSmoothTime = 1f;
        private float clearSliderDest;

        [Header("CURRENT BATTLE STATE")]
        [ReadOnly] public string BattleStr;

        private BattleSceneRoute battleSceneUI;

        public delegate void OnIncreaseValue(float value);
        public static OnIncreaseValue OnIncreaseClearGauge;

        private void Start()
        {
            battleSceneUI = GetComponent<BattleSceneRoute>();
            GameManager.Instance.SetGameState(GAMESTATE.STATE_BEFOREBATTLE);

            OnIncreaseClearGauge += IncreaseClearGauge;

            isInitialized = true;
        }

        private void Update()
        {
            switch (GameManager.Instance.GameState)
            {
                case GAMESTATE.STATE_BEFOREBATTLE : OnCheckBeforeBattle(); break;
                case GAMESTATE.STATE_INBATTLE     : OnCheckInBattle();     break;
                case GAMESTATE.STATE_BOSSBATTLE   : OnCheckBossBattle();   break;
                case GAMESTATE.STATE_ENDBATTLE    : OnCheckEndBattle();    break;
            }
        }

        private void OnDestroy()
        {
            OnIncreaseClearGauge -= IncreaseClearGauge;
        }

        #region GAME_GAUGE_LOGIC'S

        public void IncreaseClearGauge(float value)
        {
            currentClearCount += value;

            if(currentClearCount >= MaxClearCount)
            {
                currentClearCount = MaxClearCount;
            }

            if (ClearSlider != null || ClearSlider.value < 1f)
                StartCoroutine(MoveSlider());
        }

        private IEnumerator MoveSlider()
        {
            //var value = currentStageCount / MaxStageCount;
            //float startValue = StageSlider.value;
            float lerp = 0f;

            while (lerp < 1f)
            {
                clearSliderDest = currentClearCount / MaxClearCount; //처음에만 잡아줘버리면 계속 변동하는 수치를 잡지못함
                ClearSlider.value = Mathf.Lerp(ClearSlider.value, clearSliderDest, lerp);
                lerp += Time.deltaTime / SliderSmoothTime;

                yield return null;
            }

            ClearSlider.value = clearSliderDest;
            //adding Time.deltaTime will probably never add to a full Number, this is just rounding Slider value
            //so it's exactly what we want
        }

        #endregion

        #region ONCHECK_BATTLE_PROGRESS

        private void OnCheckBeforeBattle()
        {
            if (isInitialized)
            {
                startWaitingTime += Time.deltaTime;

                if (startWaitingTime >= StartTime)
                {
                    GameManager.Instance.SetGameState(GAMESTATE.STATE_INBATTLE);
                }
            }

            BattleStr = "BEFORE BATTLE";
        }

        private void OnCheckInBattle()
        {
            if(currentClearCount >= MaxClearCount)
            {
                GameManager.Instance.SetGameState(GAMESTATE.STATE_ENDBATTLE);
            }

            BattleStr = "IN BATTLE";
        }

        private void OnCheckBossBattle()
        {

        }

        private void OnCheckEndBattle()
        {
            if (endWaitingTime >= EndTime) battleSceneUI.OnResultPanel();
            else endWaitingTime += Time.deltaTime;

            BattleStr = "END BATTLE";
        }

        #endregion
    }
}
