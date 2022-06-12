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
        [SerializeField] [RangeEx(3, 10, 1)] int numberCacheStart = 3;

        #region UNITY_CYCLE

        private void Awake() {
#if UNITY_EDITOR
            soundList = new List<ACSound>();
            numberCacheArray = new int[] { };
#endif
        }

        private void Start() {
            SceneLoader.SceneChangeCallback += ClearNumberCache;
        }

        private void OnDestroy() {
            if (SceneLoader.Instance != null) {
                SceneLoader.SceneChangeCallback -= ClearNumberCache;
            }
        }

        #endregion

        /// <summary>
        /// Awake에서 사용하지 않는 것을 권장함.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public float GetVolumeScale(SOUNDTYPE type) {
            if (!GameManager.Instance.IsInitialized) {
                CatLog.WLog("Settings Data is Not Loaded Yet.");
                return StNum.floatOne;
            }

            switch (type) {
                case SOUNDTYPE.SE:  return CCPlayerData.settings.SeSoundValue;
                case SOUNDTYPE.BGM: return CCPlayerData.settings.BgmSoundValue;
                default:            return StNum.floatOne;
            }
        }

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

        public void RemoveSound(string key) {
            if (soundDic.ContainsKey(key) == false) {
                CatLog.WLog($"this SoundKey is Not Exist In SoundDictionary. KEY: {key}");
                return;
            }

            soundDic.Remove(key);
        }

        public void SetVolumeScale(SOUNDTYPE type) {
            float targetVolume = GetVolumeScale(type);
            foreach (var pair in soundDic) {
                if (pair.Value.SoundType == type) {
                    pair.Value.SetVolumeScale(targetVolume);
                }
            }
        }

        void ClearNumberCache() => numberCacheDic.Clear();

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
}
