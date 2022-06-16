namespace ActionCat.Audio {
    using UnityEngine;

    public class ControllerSound : MonoBehaviour {
        [Header("SOUND COMPATIBILITY")]
        [SerializeField] ACSound controllerAudio = null;
        [SerializeField] AudioClip[] selectedSounds    = null;
        [SerializeField] AudioClip[] fullChargedSounds = null;
        [SerializeField] AudioClip[] pullingSounds     = null;
        [SerializeField] AudioClip[] releaseSounds     = null;

        public ACSound AudioSource {
            get => controllerAudio;
        }

        private void Start() {
            if (controllerAudio == null) {
                controllerAudio = GetComponent<ACSound>();
            }
        }

        public void PlaySelectedSound() => controllerAudio.PlayOneShot(selectedSounds.RandIndex());

        public void PlayFullChargedSound() => controllerAudio.PlayOneShot(fullChargedSounds.RandIndex());

        public void PlayPullingSound() => controllerAudio.PlayOneShot(pullingSounds.RandIndex());

        public void PlayReleasedSound() => controllerAudio.PlayOneShot(releaseSounds.RandIndex());
    }
}
