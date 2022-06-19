namespace ActionCat.Data {
    using System.Collections.Generic;
    using StageData;

    public class GameSettings {
        //FIELD
        Dictionary<string, StageSetting> stageSettings = new Dictionary<string, StageSetting>();
        private float bgmVolumeParamsValue = 0f;
        private float seVolumeParamsValue  = 0f;
        public PULLINGTYPE PullingType { 
            get; private set; 
        } = PULLINGTYPE.FREE_TOUCH;

        //PROPERTY
        public bool GetPullTypeToBoolean {
            get {
                return (PullingType == PULLINGTYPE.FREE_TOUCH) ? true : false;
            }
        }
        public float BgmVolumeParamsValue {
            get => bgmVolumeParamsValue;
            set {
                bgmVolumeParamsValue = (value >= -80f && value <= 0f) ? value : bgmVolumeParamsValue;
                CatLog.Log($"Set BGM SoundFitch Value: {bgmVolumeParamsValue}");
            }
        }
        public float SeVolumeParamsValue {
            get => seVolumeParamsValue;
            set {
                seVolumeParamsValue = (value >= -80f && value <= 0f) ? value : seVolumeParamsValue;
                CatLog.Log($"Set SE SoundFitch Value: {seVolumeParamsValue}");
            }
        }

        public StageSetting GetStageSetting(string key) {
            if(stageSettings.ContainsKey(key)) {
                return stageSettings[key];
            }
            else {
                stageSettings.Add(key, new StageSetting());
                return stageSettings[key];
            }
        }

        public bool TryGetStageSetting(string key, out StageSetting setting) {
            return stageSettings.TryGetValue(key, out setting);
        }

        /// <summary>
        /// Bow Controller Type Change
        /// </summary>
        /// <param name="isChangeType"> false = Around Bow, true = Free Touch </param>
        public void SetPullType(bool isChangeType) {
            PullingType = (isChangeType) ? PULLINGTYPE.FREE_TOUCH : PULLINGTYPE.AROUND_BOW_TOUCH;
            CatLog.Log($"Currnet Control Type: {PullingType}");
        }

        public static GameSettings defaultSettings {
            get {
                return new GameSettings() {
                    stageSettings = new Dictionary<string, StageSetting>(),
                    PullingType = PULLINGTYPE.FREE_TOUCH,
                    bgmVolumeParamsValue = 1.0f,
                    seVolumeParamsValue  = 1.0f
                };
            }
        }
        public GameSettings() { }
        ~GameSettings() { }
    }
}

namespace ActionCat.Data.StageData {
    public sealed class StageSetting {
        public bool isOnAutoMode { get; private set; }   = false;
        public bool isOnEliteSpawn { get; private set; } = false;

        public void SetAutoMode(bool isOn) {
            isOnAutoMode = isOn;
            CatLog.Log($"Stage Settigns AutoMode : {isOnAutoMode}");
        }

        public void SetMutant(bool isOn) {
            isOnEliteSpawn = isOn;
            CatLog.Log($"Stage Settings Spawn Mutant : {isOnEliteSpawn}");
        }
    }
}
