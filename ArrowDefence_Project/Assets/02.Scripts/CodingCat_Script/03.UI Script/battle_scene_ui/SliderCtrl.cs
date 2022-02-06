namespace ActionCat.UI {
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;

    public class SliderCtrl : MonoBehaviour {
        [Header("COMPONENT")]
        Slider mainSlider = null;

        [Header("SLIDER")]
        [SerializeField] float duration = 0.5f;

        public void Decrease(float dest) {
            if(mainSlider == null) {
                throw new System.Exception("Sldier Component is Null.");
            }

            mainSlider.DOValue(dest, duration);
        }
    }
}
