namespace ActionCat {
    using UnityEngine;
    using ActionCat.Data;
    using ActionCat.Audio;
    using System.Collections.Generic;
    using System.Linq;

    public class SoundManager : Singleton<SoundManager> {
        Dictionary<string, ACSound> soundDic = new Dictionary<string, ACSound>();
        Dictionary<string, int> numberCacheDic  = new Dictionary<string, int>();
        [Header("DEBUG")]
        [SerializeField] [Tooltip("ContextMenu를 사용하여 Inspector에 표현할 수 있습니다.")] List<ACSound> soundList = null;
        [SerializeField] [Tooltip("ContectMenu를 사용하여 Inspector에 표현할 수 있습니다.")] string[] keys           = null;
        [SerializeField] int[] numberCacheArray = null;

        [Header("OPTION")]
        [SerializeField] [RangeEx(1, 10, 1)] int numberCacheStart = 3;

        [Header("CHANNEL")]
        [SerializeField] List<ACSound> channels = new List<ACSound>();
        Dictionary<CHANNELTYPE, ACSound> channelDictionary = new Dictionary<CHANNELTYPE, ACSound>();

        #region UNITY_CYCLE

        private void Awake() {
#if UNITY_EDITOR
            soundList = new List<ACSound>();
            numberCacheArray = new int[] { };
#endif
        }

        private void Start() {
            SceneLoader.SceneChangeCallback += ClearNumberCache;
            SceneLoader.SceneChangeCallback += ClearChannels;

            var globalMixer = GOSO.Inst.GlobalAudioMixer;
            if (globalMixer) {
                globalMixer.SetFloat(GOSO.Inst.BgmVolumeParameter, CCPlayerData.settings.BgmParamVolumeValue);
                globalMixer.SetFloat(GOSO.Inst.SeVolumeParameter, CCPlayerData.settings.SeParamVolumeValue);
            }
            else {
                CatLog.ELog("SoundManager: GlobalAudioMixer is Null.");
            }
        }

        private void OnDestroy() {
            if (SceneLoader.IsExist) {
                SceneLoader.SceneChangeCallback -= ClearNumberCache;
                SceneLoader.SceneChangeCallback -= ClearChannels;
            }
        }

        #endregion

        /// <summary>
        /// Start 메서드에서 사용을 권장
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public float GetVolumeScale(SOUNDTYPE type) {
            if (!GameManager.Instance.IsInitialized) {
                CatLog.WLog("Settings Data가 로드되지 않았습니다. 기본값이 할당됩니다.");
                return StNum.floatOne;
            }

            switch (type) {
                case SOUNDTYPE.SE:  return CCPlayerData.settings.SeSoundValue;
                case SOUNDTYPE.BGM: return CCPlayerData.settings.BgmSoundValue;
                default:            return StNum.floatOne;
            }
        }

        /// <summary>
        /// 관리되는 Sound Dictionary에 ACSound 컴포넌트를 추가
        /// </summary>
        /// <param name="source"></param>
        public void AddSound(ACSound source) {
            if (soundDic.ContainsKey(source.SoundKey)) {                                                   //이미 동일한 SoundKey가 존재하면 새로운 SoundKey를 부여
                int loopCounts = (numberCacheDic.TryGetValue(source.SoundKey, out int value)) ? value : 1; //넘버캐시에 저장된 넘버가 있는지 확인. ※ 너무 많은 Sound객체의 Loop방지
                while (soundDic.ContainsKey(source.SoundKey)) {                                            //중복되는 key가 아닐 때 까지 Loop
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

            soundDic.Add(source.SoundKey, source);
        }

        /// <summary>
        /// Sound Dictionary에서 해당 요소를 제거. 
        /// [Dictionary에 Add되었다면, 씬 종료시 반드시 호출]
        /// </summary>
        /// <param name="key"></param>
        public void RemoveSound(string key) {
            if (soundDic.ContainsKey(key) == false) {
                CatLog.WLog($"this SoundKey is Not Exist In SoundDictionary. KEY: {key}");
                return;
            }

            soundDic.Remove(key);
        }

        /// <summary>
        /// Set Volume By Sound Type 
        /// </summary>
        /// <param name="type"></param>
        public void SetVolumeScale(SOUNDTYPE type) {
            float targetVolume = GetVolumeScale(type);
            foreach (var pair in soundDic) {
                if (pair.Value.SoundType == type) {
                    pair.Value.SetVolume(targetVolume);
                }
            }
        }

        /// <summary>
        /// Set All Sounds Volume
        /// </summary>
        public void SetVolumeScale() {
            foreach (var pair in soundDic) {
                pair.Value.SetVolume(GetVolumeScale(pair.Value.SoundType));
            }
        }

        void ClearNumberCache() => numberCacheDic.Clear();

        #region CHANNEL

        public void AddChannel(ACSound audioSource) {
            channels.Add(audioSource);
        }

        public void AddChannel2Dic(CHANNELTYPE type, ACSound audioSource) {
            if (channelDictionary.ContainsKey(type)) {
                throw new System.Exception("Duplicated Channel Key.");
            }
            channelDictionary.Add(type, audioSource);
        }

        public void ClearChannels() {
            channels.Clear();
            channelDictionary.Clear();
        }

        public ACSound GetChannel(CHANNELTYPE type) {
            switch (type) {
                case CHANNELTYPE.ARROW:      return (channels[0] != null) ? channels[0] : throw new System.Exception("Failed to Get Channel !");
                case CHANNELTYPE.PROJECTILE: return (channels[1] != null) ? channels[1] : throw new System.Exception("Failed to Get Channel !");
                case CHANNELTYPE.PLAYER:     return (channels[2] != null) ? channels[2] : throw new System.Exception("Failed to Get Channel !");
                case CHANNELTYPE.MONSTER:    return (channels[3] != null) ? channels[3] : throw new System.Exception("Failed to Get Channel !");
                case CHANNELTYPE.NONE:  throw new System.NotImplementedException();
                default:                throw new System.NotImplementedException();
            }
        }

        public bool TryGetChannel(CHANNELTYPE type, out ACSound result) {
            return channelDictionary.TryGetValue(type, out result);
        }

        #endregion

#if UNITY_EDITOR
        [ContextMenu("Get Dictionary to List")]
        public void GetSoundList() {
            //Get Sound List
            soundList.Clear();
            foreach (var pair in soundDic) {
                soundList.Add(pair.Value);
            }

            //Get Keys Array
            keys = soundDic.Keys.ToArray();

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
