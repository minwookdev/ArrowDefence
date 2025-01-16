namespace ActionCat {
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    public class AccessorySkillSlot : MonoBehaviour {
        [Header("DEFAULT")]
        [SerializeField] protected Image skillIcon;
        [SerializeField] protected EventTrigger eventTrigger;
        protected AccessorySPEffect artifactEffect = null;

        protected bool isEffectActivation = false;
        protected bool isPreparedActive   = false;

        //Skill Callback
        protected Action notifyPlayAction = null;             //SKill Prepared Notify
        protected Coroutine effectActivationCoroutine = null; //Skill Effect Coroutine

        [Header("SOUND COMPATIBILITY")]
        [SerializeField] protected bool isSoundCompatibility = false;
        [SerializeField] protected CHANNELTYPE channelKey    = CHANNELTYPE.NONE;
        [SerializeField] protected Audio.ACSound channel     = null;
        [SerializeField] protected AudioClip soundEffectClip = null;
        protected bool isInitChannel {
            get {
                return channel != null;
            }
        }

        public void IconSpriteOverride(Sprite sprite) {
            skillIcon.sprite = sprite;
        }

        protected void InitChannel() {
            channel = isSoundCompatibility && SoundManager.Instance.TryGetChannel2Dic(channelKey, out Audio.ACSound result) ? result : channel;
            if (isSoundCompatibility && channel == null) {
                CatLog.ELog("Channel Not Found !");
            }
        }

        public void PlayDownSound() {
            if (isSoundCompatibility) {
                channel.PlayOneShot(0);
            }
        }

        public void PlayClickedSound() {
            if (isSoundCompatibility) {
                channel.PlayOneShot(1);
            }
        }

        protected void PlaySoundEffect() {
            if (isSoundCompatibility) {
                channel.PlayOneShot(soundEffectClip);
            }
        }
    }
}
