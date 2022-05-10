namespace ActionCat.Data {
    public static class CCPlayerData {
        public static AD_Inventory      inventory  = new AD_Inventory();
        public static Player_Equipments equipments = new Player_Equipments();
        public static PlayerAbility     ability    = new PlayerAbility();
        public static PlayerInfo        infos      = new PlayerInfo();
        public static GameSettings      settings   = new GameSettings();

        static readonly string KEY_INVENTORY = "KEY_INVENTORY";
        static readonly string KEY_EQUIPMENT = "KEY_EQUIPMENT";
        static readonly string KEY_INFOS     = "KEY_INFOS";
        static readonly string KEY_SETTINGS  = "KEY_SETTINGS";

        public static bool IsExistsDefaultFile = false;

        public static string PersistentDataPath {
            get {
                return UnityEngine.Application.persistentDataPath;
            }
        }

        public static string DataPath {
            get {
                return UnityEngine.Application.dataPath;
            }
        }

        public static string SettingsJsonFilePath { 
            //지금 Application.DataPath로 잡고있는데 만약에 빌드해보고 문제 생기면 Persistent path 로 잡아도 상관없음
            get {
                return UnityEngine.Application.dataPath + "/ArrDef_Settings.es3";
            }
        }

        public static void CreateNewFile() {
            if (IsExistsDefaultFile) return;

            //세이브 데이터를 불러오지 못한 경우 EX)세이브 데이터 없음. 
            //게임진행에 필요한 최소의 조건 충족 EX)기초 아이템 지급.
            infos.AddCraftSlot(3); //초기에 3개 오픈
            infos.OpenSlot(0, 1, 2);

            IsExistsDefaultFile = true;
        }

        public static void SaveUserDataJson() {
            //ES3.Save<AD_Inventory>(inventoryKey, inventory);

            try {
                ES3.Save(KEY_INVENTORY, inventory);
                ES3.Save(KEY_EQUIPMENT, equipments);
                ES3.Save(KEY_INFOS, infos);
                CatLog.Log(StringColor.GREEN, "Success Save All User Data.");
            }
            catch (System.Exception ex) {
                CatLog.ELog("Failed Save UserData json. Exception: \n" + ex.Message);
            }
        }

        public static void LoadUserDataJson() {
            if (!ES3.FileExists()) {
                CatLog.WLog("User Data File is Not Exsist.");
                IsExistsDefaultFile = false;
                return;
            }

            try {
                inventory  = (ES3.KeyExists(KEY_INVENTORY)) ? ES3.Load<AD_Inventory>(KEY_INVENTORY)      : throw new System.Exception("INVENTORY KEY NOT EXISTS.");
                equipments = (ES3.KeyExists(KEY_EQUIPMENT)) ? ES3.Load<Player_Equipments>(KEY_EQUIPMENT) : throw new System.Exception("EQUIPMENT KEY NOT EXISTS.");
                infos      = (ES3.KeyExists(KEY_INFOS))     ? ES3.Load<PlayerInfo>(KEY_INFOS)            : throw new System.Exception("INFO KEY NOT EXISTS.");
                IsExistsDefaultFile = true; //ES3 로드 성공 !
                CatLog.Log(StringColor.GREEN, "User Data Json Loaded Successfully !");
            }
            catch (System.Exception ex) {
                IsExistsDefaultFile = false;
                CatLog.ELog("Failed to Load UserData Json: \n" + ex.Message);
            }
        }

        public static void TEST_CREATE_TEMP_CRAFTING_SLOT() {
            infos.AddCraftSlot(3);
            infos.OpenSlot(0);
            infos.OpenSlot(1);
            infos.OpenSlot(2);
        }

        public static void LoadSettingsJson() {
            try {
                var isExistsFile = ES3.FileExists(CCPlayerData.SettingsJsonFilePath, ES3Settings.defaultSettings);
                if (!isExistsFile) {
                    settings = GameSettings.defaultSettings;
                    ES3.Save<GameSettings>(KEY_SETTINGS, settings, CCPlayerData.SettingsJsonFilePath, ES3Settings.defaultSettings);
                    CatLog.WLog("Settings Json Not Exists in DataPath. Create New Settings Json.");
                }

                if (ES3.KeyExists(KEY_SETTINGS, CCPlayerData.SettingsJsonFilePath, ES3Settings.defaultSettings) == false) {
                    throw new System.Exception("The json file exists, but the key does not exist.");
                }

                settings = ES3.Load<GameSettings>(KEY_SETTINGS, CCPlayerData.SettingsJsonFilePath, ES3Settings.defaultSettings);
                CatLog.Log(StringColor.GREEN, "Settings Json Load Completed !");
            }
            catch (System.Exception ex) {
                CatLog.ELog("Failed to Load Settings Json. Exception: \n" + ex.Message);
            }
        }

        public static void SaveSettingsJson() {
            try { //현재 GameSettings 저장
                ES3.Save<GameSettings>(KEY_SETTINGS, settings, CCPlayerData.SettingsJsonFilePath, ES3Settings.defaultSettings);
            }
            catch (System.Exception ex) {
                CatLog.ELog("Failed to Save Settings Json. Exception: \n" + ex.Message);
            }
        }
    }
}
