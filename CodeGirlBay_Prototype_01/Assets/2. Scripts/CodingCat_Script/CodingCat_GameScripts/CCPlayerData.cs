namespace CodingCat_Games.Data
{
    using CodingCat_Scripts;

    public static class CCPlayerData
    {
        public static AD_Inventory      inventory  = new AD_Inventory();
        public static Player_Equipments equipments = new Player_Equipments();

        public static int user_int;

        static readonly string KEY_INVENTORY = "KEY_INVENTORY";
        static readonly string KEY_EQUIPMENT = "KEY_EQUIPMENT";

        public static void SetUserInt(int data) => user_int = data;

        public static void SaveUserData()
        {
            //ES3.Save<AD_Inventory>(inventoryKey, inventory);

            try
            {
                ES3.Save(KEY_INVENTORY, inventory);
                ES3.Save(KEY_EQUIPMENT, equipments);

                CatLog.Log("UserData 저장 성공.");
            }
            catch (System.Exception)
            {
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
                CatLog.Log("성공적으로 Inventory를 불러왔습니다.");
            }
            else CatLog.WLog("ES3 inventory Key 값이 없습니다.");

            //Load Equipments
            if (ES3.KeyExists(KEY_EQUIPMENT))
            {
                equipments = ES3.Load<Player_Equipments>(KEY_EQUIPMENT);
                CatLog.Log("성공적으로 Equipments를 불러왔습니다.");
            }
            else CatLog.WLog("ES3 equipment key 값이 없습니다.");
        }
    }
}
