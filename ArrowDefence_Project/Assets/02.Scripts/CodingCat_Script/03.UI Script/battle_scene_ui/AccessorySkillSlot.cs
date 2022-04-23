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
    }
}
