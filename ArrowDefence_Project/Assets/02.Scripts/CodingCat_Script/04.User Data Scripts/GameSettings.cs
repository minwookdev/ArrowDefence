namespace ActionCat.Data {
    using System.Collections.Generic;
    using StageData;

    public class GameSettings {
        //FIELD
        Dictionary<string, StageSetting> stageSettings = new Dictionary<string, StageSetting>();

        //PROPERTY
        public PULLINGTYPE PullingType { get; private set; }

        public void SetPullingType(PULLINGTYPE pullType) => PullingType = pullType;

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

        public GameSettings() { }
        ~GameSettings() { }
    }


}

namespace ActionCat.Data.StageData {
    public sealed class StageSetting {
        public bool isOnAutoMode { get; private set; }    = false;
        public bool isOnSpawnMutant { get; private set; } = false;

        public void SetAutoMode(bool isOn) {
            isOnAutoMode = isOn;
            CatLog.Log($"Stage Settigns AutoMode : {isOnAutoMode}");
        }

        public void SetMutant(bool isOn) {
            isOnSpawnMutant = isOn;
            CatLog.Log($"Stage Settings Spawn Mutant : {isOnSpawnMutant}");
        }
    }
}
