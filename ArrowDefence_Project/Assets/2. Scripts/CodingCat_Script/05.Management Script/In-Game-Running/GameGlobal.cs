namespace ActionCat
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

        public static int RandomIntInArray(int[] intArray)
        {
            if (intArray.Length <= 0)
            {
                CatLog.WLog("Int Array Parameter Size 0, return 1");
                return 1;
            }

            int valueOfArray = intArray[GameGlobal.RandomIndexInRange(intArray)];
            return valueOfArray;
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

        public static Vector3 FixedVectorOnScreen(Vector2 position)
        {
            Vector3 vector = new Vector3(position.x, position.y, 90f);
            return vector;
        }
    }

    #region ENUMS_SKILL
    public enum SKILL_LEVEL
    {
        LEVEL_LOW    = 0,
        LEVEL_MEDIUM = 1,
        LEVEL_HIGH   = 2,
        LEVEL_UNIQUE = 3
    }

    public enum BOWSKILL_TYPE
    {
        SKILL_EMPTY,
        SKILL_SPREAD_SHOT,
        SKILL_RAPID_SHOT,
        SKILL_ARROW_RAIN
    }

    public enum ACSP_TYPE
    {
        SPEFFECT_NONE,
        SPEFFECT_AIMSIGHT,
        SPEEFECT_SLOWTIME
    }

    public enum RFEF_TYPE
    {
        RFEFFECT_NONE,
        RFEFFECT_INCREASE_DAMAGE,
        RFEFFECT_INCREASE_PHYSICAL_DAMAGE,
        RFEFFECT_INCREASE_MAGICAL_DAMAGE,
        RFEFFECT_INCREASE_HEALTH
    }
    #endregion

    #region ENUMS_ITEM

    #endregion
}
