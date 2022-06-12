namespace ActionCat.Audio {
    using UnityEngine;

    [RequireComponent(typeof(AudioSource))]
    public class ACSound : MonoBehaviour {
        [Header("OPTIONS")]
        [SerializeField] AudioSource audioSource      = null;
        [SerializeField] SOUNDTYPE soundType          = SOUNDTYPE.NONE;
        [SerializeField] string assignSoundKey        = "defaultSound";
        [SerializeField] [ReadOnly] string soundKey   = "";
        [SerializeField] [ReadOnly] float volumeScale = 0f;

        [Header("SOUND")]
        [SerializeField] AudioClip sound              = null;
        [SerializeField] AudioClip[] sounds           = null;
        public string SoundKey {
            get => soundKey;
        }
        public SOUNDTYPE SoundType {
            get => soundType;
        }

        private void Awake() {
            if (this.audioSource == null) {
                audioSource = GetComponent<AudioSource>();
            }
            audioSource.clip = sound;
            soundKey  = assignSoundKey;
        }

        private void Start() {
            SoundManager.Instance.AddSound(this);
            volumeScale = SoundManager.Instance.GetVolumeScale(soundType);
        }

        private void OnDestroy() {
            if (SoundManager.Instance != null) {
                SoundManager.Instance.RemoveSound(soundKey);
            }
        }

        /// <summary>
        /// Awake 에서 사용하는 것은 안전하지 않음.
        /// </summary>
        /// <param name="isLoop"></param>
        public void PlaySound(bool isLoop = false, bool isFade = false) {
            if (audioSource.clip == null) {
                CatLog.WLog("ActionCat Sound: Audio Clip is Null.");
                return;
            }

            if (isFade) {
                StartCoroutine(SoundFade(true, 2f));
            }
            else {
                audioSource.volume = volumeScale;
            }
            
            audioSource.loop = isLoop;
            audioSource.Play();
        }

        public void PlayOneShot() {
            audioSource.PlayOneShot(sound, volumeScale);
        }

        public void StopSound(bool isFade = false) {
            if (isFade) {
                StartCoroutine(SoundFade(false, 1f));
                return;
            }

            audioSource.Stop();
        }

        public void SetVolumeScale(float volume) {
            volumeScale = volume;
            audioSource.volume = volumeScale;
        }

        public void ChangeKey(int index) {
            this.soundKey = string.Format("{0}_{1}", this.assignSoundKey, index.ToString());
        }

        System.Collections.IEnumerator SoundFade(bool isOn, float fadeTime) { //분리하기
            float progress  = 0f;
            float fadeSpeed = 1 / fadeTime;
            float destVolume  = (isOn) ? volumeScale : StNum.floatZero;
            float startVolume = (isOn) ? StNum.floatZero : volumeScale;
            while (progress < 1f) {
                progress += Time.deltaTime * fadeSpeed;
                audioSource.volume = Mathf.Lerp(startVolume, destVolume, progress);
                yield return null;
            }

            if(!isOn) {
                audioSource.Stop();
            }
        }
    }

    public enum SOUNDTYPE {
        NONE = 0,
        SE   = 1,
        BGM  = 2,
    }
}
