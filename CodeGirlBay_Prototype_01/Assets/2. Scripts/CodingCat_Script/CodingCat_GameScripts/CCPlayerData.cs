namespace CodingCat_Games.Data
{
    using CodingCat_Scripts;

    public static class CCPlayerData
    {
        public static AD_Inventory inventory = new AD_Inventory();
        public static int user_int;

        static readonly string inventoryKey = "KEY_INVENTORY";

        public static void SetUserInt(int data) => user_int = data;

        public static void Saveinventory()
        {
            //ES3.Save<AD_Inventory>(inventoryKey, inventory);
            ES3.Save(inventoryKey, inventory);
            CatLog.Log("성공적으로 저장하였습니다.");
        }

        public static void LoadInventory()
        {
            if (ES3.KeyExists(inventoryKey))
            {
                inventory = ES3.Load<AD_Inventory>(inventoryKey);
                CatLog.Log("성공적으로 Inventory를 불러왔습니다.");
            }
            else CatLog.WLog("ES3 에 inventory Key 값이 없습니다.");
        }
    }
}
