namespace ActionCat.Data
{
    public static class CCPlayerData
    {
        public static AD_Inventory inventory       = new AD_Inventory();
        public static Player_Equipments equipments = new Player_Equipments();
        public static PlayerAbility ability        = new PlayerAbility();
        public static GameSettings settings        = new GameSettings();

        static readonly string KEY_INVENTORY = "KEY_INVENTORY";
        static readonly string KEY_EQUIPMENT = "KEY_EQUIPMENT";
        static readonly string KEY_SETTINGS  = "KEY_SETTINGS";

        public static void SaveUserData() {
            //ES3.Save<AD_Inventory>(inventoryKey, inventory);

            try {
                ES3.Save(KEY_INVENTORY, inventory);
                ES3.Save(KEY_EQUIPMENT, equipments);
                ES3.Save(KEY_SETTINGS, settings);

                CatLog.Log("UserData 저장 성공.");
            }
            catch (System.Exception) {
                CatLog.ELog("UserData 저장 실패.");
                throw;
            }


        }

        public static void LoadUserData()
        {
            //Load Inventory
            if (ES3.KeyExists(KEY_INVENTORY))
            {
                inventory = ES3.Load<AD_Inventory>(KEY_INVENTORY);
                //inventory = ES3.Load(KEY_INVENTORY, inventory.invenList);
                //ES3.LoadInto<AD_item>(KEY_INVENTORY, inventory.invenList);
                //ES3.Load
                //inventory.AddRangeItem(ES3.Load<List<AD_item>>(KEY_INVENTORY));
                CatLog.Log("성공적으로 Inventory를 불러왔습니다.");
            }
            else CatLog.WLog("ES3 inventory KEY값이 없습니다.");

            //Load Equipments
            if (ES3.KeyExists(KEY_EQUIPMENT))
            {
                equipments = ES3.Load<Player_Equipments>(KEY_EQUIPMENT);
                CatLog.Log("성공적으로 Equipments를 불러왔습니다.");
            }
            else CatLog.WLog("ES3 equipment KEY값이 없습니다.");

            if (ES3.KeyExists(KEY_SETTINGS))
            {
                settings = ES3.Load<GameSettings>(KEY_SETTINGS);
                CatLog.Log("성공적으로 Settings를 불러왔습니다.");
            }
            else CatLog.WLog("ES3 Game Settings KEY값이 없습니다.");
        }
    }
}
