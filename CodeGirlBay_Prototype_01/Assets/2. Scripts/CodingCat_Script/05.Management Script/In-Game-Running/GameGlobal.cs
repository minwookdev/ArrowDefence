namespace CodingCat_Games
{
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
    }
}
