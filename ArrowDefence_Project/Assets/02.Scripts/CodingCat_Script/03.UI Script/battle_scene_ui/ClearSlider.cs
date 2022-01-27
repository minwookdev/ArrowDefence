namespace ActionCat.UI {
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public sealed class ClearSlider : MonoBehaviour {
        [Header("COMPONENT")]
        [SerializeField] Slider clearSlider        = null;
        [SerializeField] Image imageMaxSliderPoint = null;
        [SerializeField] TextMeshProUGUI textTime  = null;

        [Header("SPRITE")]
        [SerializeField] Sprite spriteBossType = null;
        [SerializeField] Sprite spriteEndType  = null;

        private float maxBattleTime = 0f;

        public void InitSlider(bool isBoss, float maxTime) {
            imageMaxSliderPoint.sprite = (isBoss) ? spriteBossType : spriteEndType;
            maxBattleTime = maxTime;
            textTime.text = GetTimeStr(maxTime);
        }

        public void UpdateSlider(float currentTime) {
            clearSlider.value = (maxBattleTime - currentTime) / maxBattleTime;
            textTime.text = GetTimeStr(currentTime);
        }

        string GetTimeStr(float seconds) {
            //Type 1
            //return string.Format("{0:F0}:{1:F0}", ((seconds / 60) % 60), seconds % 60);  

            //Type 2
            //return string.Format("{0}:{1}", ((int)(seconds / 60) % 60), ((int)(seconds % 60)));

            //Type 3 [Correction Seconds]
            string correctionSeconds = (GameGlobal.GetSeconds(seconds) == 0) ? "00" : GameGlobal.GetSeconds(seconds).ToString();
            return string.Format("{0}:{1}", GameGlobal.GetMinute(seconds), correctionSeconds);
        }
    }
}
