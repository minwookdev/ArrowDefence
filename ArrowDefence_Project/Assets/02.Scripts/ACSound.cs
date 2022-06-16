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
        public AudioClip[] Sounds {
            get => sounds;
        }

        #region PROPERTY
        public string SoundKey {
            get => soundKey;
        }
        public SOUNDTYPE SoundType {
            get => soundType;
        }
        private bool isReadyToPlay {
            get {
                if (this.audioSource.clip == null) {
                    if (this.sound == null) {
                        CatLog.WLog("ActionSound Component: audio clip is null. not to play Sound.");
                        return false;
                    }
                    this.audioSource.clip = this.sound;
                }
                return true;
            }
        }
        public AudioClip SetClip {
            set {
                this.audioSource.clip = value;
            }
        }
        #endregion

        #region LIFE_CYCLE

        private void Awake() {
            if (this.audioSource == null) {
                audioSource = GetComponent<AudioSource>();
            }

            soundKey = assignSoundKey;              // Apply SoundKey
            SoundManager.Instance.AddSound(this);   // AddSound Dictionary by SoundKey
            audioSource.clip = (sound != null) ? sound : null;
        }

        private void Start() {
            volumeScale = SoundManager.Instance.GetVolumeScale(soundType); // Get Volume Variable
            audioSource.volume = volumeScale;                              // Set Volume Variable
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
            if (isReadyToPlay == false) {
                return;
            }

            audioSource.loop = isLoop;
            audioSource.Play();
        }

        /// <summary>
        /// Delay 시간동안 대기 후 사운드 출력 # delay 매개변수는 seconds 단위
        /// </summary>
        /// <param name="delay">Seconds</param>
        /// <param name="isLoop"></param>
        public void PlaySoundWithDelayed(float delay, bool isLoop = false) {
            if (isReadyToPlay == false) {
                return;
            }

            audioSource.loop = isLoop;
            audioSource.PlayDelayed(delay);
        }

        /// <summary>
        /// Volume이 0에서 시작하고 점점 커짐. Sound FadeOut효과
        /// </summary>
        /// <param name="isLoop"></param>
        public void PlaySoundWithFadeOut(float fadeTime = 1f, bool isLoop = false) {
            if (isReadyToPlay == false) {
                return;
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

        /// <summary>
        /// 사운드의 인덱스 넘버를 확실하게 알고있는 상태에서 사용할 것.
        /// </summary>
        /// <param name="idx"></param>
        public void PlayOneShot(int idx) => audioSource.PlayOneShot(sounds[idx], volumeScale);

        /// <summary>
        /// 재생항 AudioClip을 지정
        /// </summary>
        /// <param name="audio"></param>
        public void PlayOneShot(AudioClip audio) => audioSource.PlayOneShot(audio, volumeScale);

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
