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

        private float maxBattleTime  = 0f;
        private string correctionSec  = "";
        private int currentSeconds = 0;

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
            currentSeconds = GameGlobal.GetSeconds(seconds);
            correctionSec  = (currentSeconds < 10) ? "0" + currentSeconds.ToString() : currentSeconds.ToString();
            return string.Format("{0}:{1}", GameGlobal.GetMinute(seconds), correctionSec);
        }
    }
}
