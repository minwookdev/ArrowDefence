namespace ActionCat {
    using UnityEngine;
    using Audio;

    public class MonsterAudio : MonoBehaviour {
        [Header("SOUND")]
        [SerializeField] [ReadOnly] ACSound audioSource = null;
        [SerializeField] AudioClip[] breathClips = null;
        [SerializeField] AudioClip[] attackClips = null;
        [SerializeField] AudioClip[] deathClips  = null;
        [SerializeField] AudioClip[] hitClips    = null;
        [SerializeField] AudioClip blockClip = null;

        private void Start() {
            //Get Channel
            this.audioSource = SoundManager.Instance.TryGetChannel2Dic(CHANNELTYPE.MONSTER, out ACSound result) ? result : audioSource;
        }

        public void PlayRandomHitSound() => audioSource.PlayOneShot(hitClips.RandIndex());

        public void PlayRandomDeathSound() => audioSource.PlayOneShot(deathClips.RandIndex());

        public void PlayRandomAttackSound() => audioSource.PlayOneShot(attackClips.RandIndex());

        public void PlayRandomBreathSound() => audioSource.PlayOneShot(breathClips.RandIndex());

        public void PlayBlockSound() => audioSource.PlayOneShot(blockClip);
    }
}
