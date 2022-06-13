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

        #region LIFE_CYCLE

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
            audioSource.volume = volumeScale;
        }

        private void OnDestroy() {
            if (SoundManager.IsExist) {
                SoundManager.Instance.RemoveSound(soundKey);
            }
        }

        #endregion

        #region OPTIONS

        public void SetVolumeScale(float volume) {
            volumeScale = volume;
            audioSource.volume = volumeScale;
        }

        public void ChangeKey(int index) {
            this.soundKey = string.Format("{0}_{1}", this.assignSoundKey, index.ToString());
        }

        #endregion

        #region STOP_SOUND

        public void StopSound() => audioSource.Stop();

        public void StopSoundWithFadeIn(float fadeTime = 1f, bool isEndWithSoundStop = true) {
            StartCoroutine(FadeInVolume(fadeTime, isEndWithSoundStop));
        }

        #endregion

        #region PLAY

        /// <summary>
        /// Awake에서 사용하는 것은 안전하지 않음.
        /// </summary>
        /// <param name="isLoop"></param>
        public void PlaySound(bool isLoop = false) {
            if (audioSource.clip == null) {
                if (sound == null) {
                    CatLog.WLog("Action Sound: AudioClip is Null."); return;
                }
                audioSource.clip = sound;
            }

            audioSource.volume = volumeScale;
            audioSource.loop   = isLoop;
            audioSource.Play();
        }

        /// <summary>
        /// Delay 시간동안 대기 후 사운드 출력 # delay 매개변수는 seconds 단위
        /// </summary>
        /// <param name="delay">Seconds</param>
        /// <param name="isLoop"></param>
        public void PlaySoundWithDelayed(float delay, bool isLoop = false) {
            if (audioSource.clip == null) {
                if (sound == null) {
                    CatLog.WLog("Action Sound: AudioClip is Null."); return;
                }
                audioSource.clip = sound;
            }

            audioSource.loop = isLoop;
            audioSource.volume = volumeScale;
            audioSource.PlayDelayed(delay);
        }

        /// <summary>
        /// Volume이 0에서 시작하고 점점 커짐. Sound FadeOut효과
        /// </summary>
        /// <param name="isLoop"></param>
        public void PlaySoundWithFadeOut(float fadeTime = 1f, bool isLoop = false) {
            if (audioSource.clip == null) {
                if (sound == null) {
                    CatLog.WLog("Action Sound: AudioClip is Null."); return;
                }
                audioSource.clip = sound;
            }

            StartCoroutine(FadeOutVolume(fadeTime));
            audioSource.loop = isLoop;
            audioSource.Play();
        }

        #endregion

        #region PLAYONESHOT

        public void PlayOneShot() {
            audioSource.PlayOneShot(sound, volumeScale);
        }

        #endregion

        #region COROUTINE

        System.Collections.IEnumerator FadeOutVolume(float fadeTime) {
            float progress    = 0f;
            float fadeSpeed   = 1 / fadeTime;
            while (progress < 1f) {
                progress += Time.deltaTime * fadeSpeed;
                audioSource.volume = Mathf.Lerp(StNum.floatZero, volumeScale, progress);
                yield return null;
            }
        }

        System.Collections.IEnumerator FadeInVolume(float fadeTime, bool isEndWithSoundStop = false) {
            float progress    = 0f;
            float fadeSpeed   = 1 / fadeTime;
            while (progress < 1f) {
                progress += Time.deltaTime * fadeSpeed;
                audioSource.volume = Mathf.Lerp(volumeScale, StNum.floatZero, progress);
                yield return null;
            }
            volumeScale = StNum.floatZero;
            if (isEndWithSoundStop) {
                audioSource.Stop();
            }
        }

        #endregion
    }

    public enum SOUNDTYPE {
        NONE = 0,
        SE   = 1,
        BGM  = 2,
    }
}
