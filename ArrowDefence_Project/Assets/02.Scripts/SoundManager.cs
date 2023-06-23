namespace ActionCat {
    using UnityEngine;
    using ActionCat.Data;
    using ActionCat.Audio;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 게임 내 사운드 효과 담당 클래스
    /// </summary>
    public class SoundManager : Singleton<SoundManager> {
        [Header("DEBUG")]
        [SerializeField] List<ACSound> soundList = null;
        [SerializeField] string[] keys = null;
        [SerializeField] int[] numberCacheArray = null;

        [Header("OPTION")]
        [SerializeField][RangeEx(1, 10, 1)] int numberCacheStart = 3;

        [Header("SOUND MANAGED SYSTEM")]
        Dictionary<string, int> numberCacheDic = new Dictionary<string, int>();
        Dictionary<string, ACSound> managedSoundDic = new Dictionary<string, ACSound>();

        [Header("CHANNEL SYSTEM")]
        Dictionary<CHANNELTYPE, ACSound> channelDictionary = new Dictionary<CHANNELTYPE, ACSound>();

        protected override void Init() {
            // Global AudioMixer 초기화
            var globalMixer = GOSO.Inst.GlobalAudioMixer;
            if (globalMixer) {  // Audio Mixer에 저장된 Settings의 Volume변수 적용해줌
                globalMixer.SetFloat(GOSO.Inst.BgmVolumeParameter, CCPlayerData.settings.BgmVolumeParamsValue);
                globalMixer.SetFloat(GOSO.Inst.SeVolumeParameter, CCPlayerData.settings.SeVolumeParamsValue);
            }
            else {
                CatLog.ELog("SoundManager: GlobalAudioMixer is Null.");
            }
        }

        public void Initialize() {
            // Global AudioMixer 초기화
            var globalMixer = GOSO.Inst.GlobalAudioMixer;
            if (globalMixer) {  // Audio Mixer에 저장된 Settings의 Volume변수 적용해줌
                globalMixer.SetFloat(GOSO.Inst.BgmVolumeParameter, CCPlayerData.settings.BgmVolumeParamsValue);
                globalMixer.SetFloat(GOSO.Inst.SeVolumeParameter, CCPlayerData.settings.SeVolumeParamsValue);
            }
            else {
                CatLog.ELog("SoundManager: GlobalAudioMixer is Null.");
            }
        }

        /// <summary>
        /// 매개변수 type의 AudioSource의 Volume값 수정.
        /// </summary>
        /// <param name="type"></param>
        public void SetVolumeScale(SOUNDTYPE type, float audioSourceVolume) {
            foreach (var pair in managedSoundDic) {
                if (pair.Value.SoundType == type) {
                    pair.Value.SetVolume(audioSourceVolume);
                }
            }
        }

        #region UNITY_CYCLE

        private void Awake() {
#if UNITY_EDITOR
            soundList = new List<ACSound>();
            numberCacheArray = new int[] { };
#endif
        }

        private void Start() {
            SceneLoader.SceneChangeCallback += ClearNumberCache;
            SceneLoader.SceneChangeCallback += ClearManagedSoundDic;
            SceneLoader.SceneChangeCallback += ClearChannelDic;
        }

        private void OnDestroy() {
            if (SceneLoader.IsExist) {
                SceneLoader.SceneChangeCallback -= ClearNumberCache;
                SceneLoader.SceneChangeCallback -= ClearManagedSoundDic;
                SceneLoader.SceneChangeCallback -= ClearChannelDic;
            }
        }

        #endregion

        #region SOUNDMANAGER_FUNCTION


        /// <summary>
        /// 모든 ManagedSoundDictionary의 AudioSource컴포넌트 Volume값 수정.
        /// </summary>
        public void SetVolumeScale(float audioSourceVolume) {
            foreach (var pair in managedSoundDic) {
                pair.Value.SetVolume(audioSourceVolume);
            }
        }

        #endregion

        #region MANAGED_SOUND

        /// <summary>
        /// 관리되는 Sound Dictionary에 ACSound 컴포넌트를 추가
        /// </summary>
        /// <param name="source"></param>
        public void AddSound(ACSound source) {
            if (managedSoundDic.ContainsKey(source.SoundKey)) {                                                   //이미 동일한 SoundKey가 존재하면 새로운 SoundKey를 부여
                int loopCounts = (numberCacheDic.TryGetValue(source.SoundKey, out int value)) ? value : 1; //넘버캐시에 저장된 넘버가 있는지 확인. ※ 너무 많은 Sound객체의 Loop방지
                while (managedSoundDic.ContainsKey(source.SoundKey)) {                                            //중복되는 key가 아닐 때 까지 Loop
                    source.ChangeKey(loopCounts);
                    loopCounts++;
                }

                if (loopCounts > numberCacheStart) {                    //체크 루프 카운트 넘어가면 해당 사운드의 루프 횟수를 저장해 둠
                    if (numberCacheDic.ContainsKey(source.SoundKey)) {   //이미 해당 키의 루프가 존재하면 덮어씀
                        numberCacheDic[source.SoundKey] = loopCounts;
                    }
                    else {                                               //없으면 새로 저장
                        numberCacheDic.Add(source.SoundKey, loopCounts);
                    }
                }
            }

            managedSoundDic.Add(source.SoundKey, source);
        }

        /// <summary>
        /// Managed Sound Dictionary에서 해당 관리되고 있는 사운드 요소 제거
        /// </summary>
        /// <param name="key"></param>
        public void RemoveSound(string key) {
            if (managedSoundDic.ContainsKey(key) == false) {
                CatLog.WLog($"this SoundKey is Not Exist In SoundDictionary. KEY: {key}");
                return;
            }

            managedSoundDic.Remove(key);
        }

        /// <summary>
        /// 등록된 SoundKey로 해당되는 사운드를 찾음
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ACSound GetManagedSound(string key) {
            throw new System.NotImplementedException();
        }

        void ClearNumberCache() => numberCacheDic.Clear();

        void ClearManagedSoundDic() => managedSoundDic.Clear();

        #endregion

        #region CHANNEL

        public void AddChannel2Dic(CHANNELTYPE type, ACSound audioSource) {
            if (channelDictionary.ContainsKey(type)) {
                throw new System.Exception("Duplicated Channel Key.");
            }
            channelDictionary.Add(type, audioSource);
        }

        void ClearChannelDic() {
            channelDictionary.Clear();
        }

        public bool TryGetChannel2Dic(CHANNELTYPE type, out ACSound result) {
            return channelDictionary.TryGetValue(type, out result);
        }

        #endregion

#if UNITY_EDITOR
        [ContextMenu("Get Dictionary to List")]
        public void GetSoundList() {
            //Get Sound List
            soundList.Clear();
            foreach (var pair in managedSoundDic) {
                soundList.Add(pair.Value);
            }

            //Get Keys Array
            keys = managedSoundDic.Keys.ToArray();

            //Get NumberCache Array
            numberCacheArray = numberCacheDic.Values.ToArray();
        }
#endif
    }

    public enum CHANNELTYPE {
        NONE,
        ARROW,
        PROJECTILE,
        PLAYER,
        MONSTER,
        BUTTON_DEFAULT,
        BUTTON_SEPARATION,
        BATTLESTART,
    }
}
