namespace CodingCat_Games
{
    using CodingCat_Scripts;
    using UnityEditor;
    using UnityEngine;

    public static class GameGlobal
    {
        public static Vector2 ScreenOffset = new Vector2(2f, 2f);
        public static Vector3 ArrowScale = new Vector3(1.5f, 1.5f, 1f);

        public static int RandomIndexInRange(int[] value)
        {
            int randomIndexInRange = Random.Range(0, value.Length);
            return randomIndexInRange;
        }

        public static void RandomIntInRange(int[] randomArray, ref int value)
        {
            if (randomArray.Length <= 1) return;

            int randomIndex = GameGlobal.RandomIndexInRange(randomArray);
            int valueOfRange = randomArray[randomIndex];
            value = valueOfRange;
        }

        public static T GetRandom<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }

        public static GameObject GetBowGameObjectInScene()
        {
            GameObject BowGameObject = GameObject.FindWithTag(AD_Data.OBJECT_TAG_BOW);
            if (BowGameObject)                                        return BowGameObject;
            else CatLog.WLog("Bow GameObject Not Found This Scene."); return null;
        }
    }
}
