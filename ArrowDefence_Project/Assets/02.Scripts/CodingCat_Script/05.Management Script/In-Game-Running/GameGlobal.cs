namespace ActionCat {
    using System.Collections.Generic;
    using UnityEngine;

    public static class GameGlobal {
        public static Vector2 ScreenOffset = new Vector2(2f, 3f);
        public static Vector3 ArrowScale   = new Vector3(1.5f, 1.5f, 1f);
        public static readonly int RandomRangeCorrection = 1;

        //COMBO
        public static readonly short MaxComboCount = 9999;
        public static readonly float ComboDuration = 1.5f;

        //BOW
        public static readonly float CHARGINGTIME = 1f;

        public static readonly string EMPTYSTR = "";

        /// <summary>
        /// int 배열에서 무작위 "index"를 반환합니다.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int RandomIndexInArray(int[] value)
        {
            int randomIndexInRange = UnityEngine.Random.Range(0, value.Length);
            return randomIndexInRange;
        }

        /// <summary>
        /// int 배열에서 무작위 요소를 result에 할당합니다.
        /// </summary>
        /// <param name="intarray">Length가 1이상인 배열</param>
        /// <param name="result"></param>
        public static void RandomIntInRange(int[] intarray, ref int result)
        {
            if (intarray.Length <= 0) return;

            int randomIndex = GameGlobal.RandomIndexInArray(intarray);
            int valueOfRange = intarray[randomIndex];
            result = valueOfRange;
        }

        /// <summary>
        /// Int배열에서 랜덤한 Index의 값을 반환합니다.
        /// </summary>
        /// <param name="intArray"></param>
        /// <returns></returns>
        public static int RandomIntInArray(int[] intArray)
        {
            if (intArray.Length <= 0)
            {
                CatLog.WLog("Int Array Parameter Size 0, return 1");
                return 1;
            }

            int valueOfArray = intArray[GameGlobal.RandomIndexInArray(intArray)];
            return valueOfArray;
        }

        public static int RandomQauntityInRange(int[] quantityArray)
        {
            if (quantityArray.Length <= 0)
            {
                //DropItem Class Quantity Array Size 가 0인 경우
                CatLog.WLog("Quantity Array Size is 0, return int value 1");
                return 1;
            }
            else if (quantityArray.Length == 1)
            {
                //DropItem Class Quantity Array Size 1의 경우 (주로 장비아이템)
                return quantityArray[0];
            }
            else
            {
                //DropItem Class Quantity Array Size 2의 경우 (소모품, 재료 아이템) MinQuantity ~ MaxQuantity 사이의 값 Return
                int quantityInArray = Random.Range(quantityArray[0], quantityArray[1] + RandomRangeCorrection);
                return quantityInArray;
            }
        }

        public static T GetRandom<T>(this T[] array) {
            return array[Random.Range(0, array.Length)];
        }

        public static T GetRandom<T>(this T[] array, out int index) {
            index = Random.Range(0, array.Length);
            return array[index];
        }

        public static int GetRandIdx<T>(this T[] array) {
            return Random.Range(0, array.Length);
        }

        public static GameObject GetControllerByTag() {
            GameObject BowGameObject = GameObject.FindWithTag(AD_Data.OBJECT_TAG_BOW);
            if (BowGameObject) return BowGameObject;
            else throw new System.Exception("the Controller does not exist.");
        }

        public static Vector3 FixedVectorOnScreen(Vector2 position) {
            return new Vector3(position.x, position.y, 0f);
        }

        public static void FixedPosOnScreen(ref Vector3 pos) {
            pos = new Vector3(pos.x, pos.y, 0f);
        }

        public static T[] ArrayRemoveAll<T>(T[] array, System.Predicate<T> predicate)
        {
            List<T> list = new List<T>(array);
            list.RemoveAll(predicate);

            //for (int i = 0; i < list.Count; i++)
            //{
            //    if(predicate(list[i]))
            //    {
            //        list.Remove(list[i]);
            //        i = 0;
            //    }
            //}

            return list.ToArray();
        }

        public static T[] ArrayRemoveAt<T>(T[] array, int index)
        {
            if (array.Length <= index)
            {
                CatLog.WLog("Target Index Number is bigger than, Array Size");
                return array;
            }

            List<T> list = new List<T>(array);
            list.RemoveAt(index);
            return list.ToArray();
        }

        /// <summary>
        /// 매개변수로 들어온 Array는 본 메서드에서 직접 수정될 수 없습니다. 선언적 루프입니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="action"></param>
        public static void ArrayForeach<T>(T[] array, System.Action<T> action)
        {
            if (array.Length <= 0 || action is null)
                return;

            for (int i = 0; i < array.Length; i++)
            {
                action(array[i]);
            }
        }

        /// <summary>
        /// 배열을 직접 조건에 따라 수정하여 반환합니다.
        /// 요소제거는 RemoveAll, RemoveAt을 사용합시다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        //public static T[] ReturnArrayForeach<T>(T[] array, System.Action<T> action)
        //{
        //    if (array is null || action is null) return array;
        //
        //    List<T> tempList = new List<T>(array);
        //    //tempList.ForEach((x) => action(x));
        //
        //    for (int i = 0; i < tempList.Count; i++)
        //    {
        //        action(tempList[i]);
        //    }
        //
        //    return tempList.ToArray();
        //} // -> 이 방법도 안댐 ㅎ

        /// <summary>
        /// Swap Value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs  = rhs;
            rhs  = temp;
        }

        /// <summary>
        /// Get Random int 0~100 [include 100]
        /// </summary>
        /// <returns></returns>
        public static int GetCritChance() {
            return UnityEngine.Random.Range(0, 100 + 1);
        }

        public static int GetItemDropRollChance() {
            return UnityEngine.Random.Range(1, 100 + 1);
        }

        public static Vector2 RotateToVector2(float degree) {
            Quaternion rotation = Quaternion.Euler(0f, 0f, degree);
            Vector2 vector = rotation * Vector2.down;
            return vector;
        }

        //Place rect transform to have the same dimensions as 'other'. even if they don't have same Parent.
        //Relatively non-extensive.
        //NOTICE - also modifies scale of your RectTransform to match the scale of other
        public static void MatchOther(this RectTransform rt, RectTransform other) {
            Vector2 myPrevPivot = rt.pivot; //?
            myPrevPivot = other.pivot;
            rt.position = other.position;

            rt.localScale = other.localScale;

            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, other.rect.width);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, other.rect.height);
            //rectTrasnform.ForceUpdateRectTransform(); -> needed before we adjust pivot a second time?
            rt.pivot = myPrevPivot;
        }

        public static void RectResizer(this RectTransform rect, Vector2 leftBottom, Vector2 rightTop, Vector3 scale) {
            rect.offsetMin  = leftBottom;
            rect.offsetMax  = rightTop;
            rect.localScale = scale;
        }

        public static string GetStageKey(STAGETYPE stageType) {
            switch (stageType) {
                case STAGETYPE.FOREST_SECLUDED_E: return AD_Data.STAGE_KEY_FOREST_SECLUDED_EASY;
                case STAGETYPE.FOREST_SECLUDED_N: return AD_Data.STAGE_KEY_FOREST_SECLUDED_NORMAL;
                case STAGETYPE.FOREST_SECLUDED_H: return AD_Data.STAGE_KEY_FOREST_SECLUDED_HARD;
                case STAGETYPE.DUNGEON_E:         return AD_Data.STAGE_KEY_DUNGEON_ENTRANE_EASY;
                case STAGETYPE.DUNGEON_N:         return AD_Data.STAGE_KEY_DUNGEON_ENTRANE_NORMAL;
                case STAGETYPE.DUNGEON_H:         return AD_Data.STAGE_KEY_DUNGEON_ENTRANE_HARD;
                case STAGETYPE.STAGE_DEV:         return AD_Data.STAGE_KEY_DEV;
                default: throw new System.NotImplementedException("Not Implemented this Stage Type");
            }
        }

        public static List<T> GetComponentAll<T>(this List<GameObject> list) where T : Component {
            List<T> tempList = new List<T>();
            for (int i = 0; i < list.Count; i++) {
                if(list[i].TryGetComponent<T>(out T component)) {
                    tempList.Add(component);
                }
                else {
                    CatLog.WLog($"{list[i].name} is not have Component : {component.name}.");
                    continue;
                }
            }

            return tempList;
        }

        /// <summary>
        /// return -180~180 degree (for Unity)
        /// </summary>
        /// <returns></returns>
        public static float AngleBetweenVec3(Vector3 _start, Vector3 _end) {
            Vector3 direction = _end - _start;
            return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }

        public static float AngleBetweenVec2(Vector2 _start, Vector2 _end) {
            Vector2 direction = _end - _start;
            return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }


        /// <summary>
        /// Number is Included Range [include min, max]
        /// </summary>
        /// <param name="number"></param>
        /// <param name="range">-range ~ range</param>
        /// <returns></returns>
        public static bool IsRange(float number, float range) {
            return (number >= -range && number <= range);
        }

        public static T RandIndex<T>(this T[] array) {
            return array[Random.Range(0, array.Length)];
        }

        public static float RandomAngleDeg() {
            return Random.Range(0, 360f);
        }

        public static float RandomAngleRad() {
            return Random.Range(0f, 360f) * Mathf.Deg2Rad;
        }

        #region COLLIDER

        public static Collider2D[] OverlapCircleAll2D(Transform centerTr, float radius, string layerName, System.Predicate<Collider2D> predicate = null) {
            List<Collider2D> list = new List<Collider2D>(Physics2D.OverlapCircleAll(centerTr.position, radius, 1 << LayerMask.NameToLayer(layerName)));
            if (predicate != null) list.RemoveAll(predicate);
            return list.ToArray();
        }

        public static bool TryGetOverlapCircleAll2D(out Collider2D[] array, Vector2 centerPos, float radius, string layerName, System.Predicate<Collider2D> predicate = null) {
            var colliderList = new List<Collider2D>(Physics2D.OverlapCircleAll(centerPos, radius, 1 << LayerMask.NameToLayer(layerName)));
            if (predicate != null) colliderList.RemoveAll(predicate);

            if (colliderList.Count <= 0) {
                array = null;
                return false;
            }

            array = colliderList.ToArray();
            return true;
        }

        #endregion

        #region ARRAY

        /// <summary>
        /// 검증 / 테스트 필요함 (실패)
        /// </summary>
        ///public static void Add<T>(this T[] array, T newelement) {
        ///    List<T> temp = new List<T>(array);
        ///    temp.Add(newelement);
        ///    array = new List<T>(temp).ToArray();
        ///}

        public static T[] AddArray<T>(T[] array, T element) {
            if(array == null) {
                throw new System.Exception("This Array is Null.");
            }

            List<T> tempList = new List<T>(array);
            tempList.Add(element);
            return tempList.ToArray();
        }

        public static void AddArray<T>(ref T[] array, T element) {
            if(array == null) {
                throw new System.Exception("This Array is Null.");
            }

            List<T> tempList = new List<T>(array);
            tempList.Add(element);
            array = tempList.ToArray();
        }

        /// <summary>
        /// 값 형식은 지원한다. 확장 메서드.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="element"></param>
        public static void Add<T>(this ref T array, T element) where T: struct {

        }

        public static List<T> ToList<T>(this T[] array) {
            return new List<T>(array);
        }

        public static void Foreach<T>(this T[] array, System.Action<T> action) {
            foreach (var item in array) {
                action(item);
            }
        }

        /// <summary>
        /// Returns false if the array length is 0.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool TrueForAll<T>(this T[] array, System.Predicate<T> predicate) {
            if(array.Length <= 0) {
                return false;
            }

            foreach (var item in array) {
                if(predicate(item) == false) {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region TIME

        public static int GetDay(float _seconds) {
            return (int)(_seconds / (60 * 60 * 24) % 365);
        }

        public static int GetHour(float _seconds) {
            return (int)(_seconds / (60 * 60) % 24);
        }

        public static int GetMinute(float _seconds) {
            return (int)((_seconds / 60) % 60);
        }

        public static int GetSeconds(float _seconds) {
            return (int)(_seconds % 60);
        }

        public static string GetMiliSeconds(float _seconds) {
            return string.Format("{0:.00}", (_seconds % 1));
        }

        #endregion

        #region COLOR
        public static void AlphaZero(this TMPro.TextMeshProUGUI text) {
            var tempColor = text.color;
            tempColor.a   = 0f;
            text.color    = tempColor;
        }

        public static void AlphaOne(this TMPro.TextMeshProUGUI text) {
            Color tempColor = text.color;
            tempColor.a = 1f;
            text.color = tempColor;
        }
        #endregion

        #region UPGRADE

        public static bool TryUpgrade(float failedProb, ref bool adsApplied) {
            float totalProb = 200f; // Real Failed probablity: failedProb / totalProb * 100
            bool result = (RandomEx.RangeFloat(StNum.floatZero, totalProb) < GameGlobal.GetUpgradeFailedProb(failedProb, adsApplied)) ? false : true;
            if (adsApplied) { //효과 적용중이라면 해제
                adsApplied = false;
            }
            return result;
        }

        public static float GetUpgradeFailedProb(float failedProb, bool isAdsApplied) {
            return (isAdsApplied) ? failedProb - (failedProb * 0.5f) : failedProb;
        }

        #endregion
    }

    public static class StNum {
        public static readonly float floatOne  = 1f;
        public static readonly float floatZero = 0f;
        public static readonly float DegreeFull = 360f;
        public static readonly int intOne  = 1;
        public static readonly int intZero = 0;
        public static readonly string stringEmpty = "";
    }

    public struct RandomEx {
        /// <summary>
        /// 최소 ~ 최대 모든 범위 포함
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RangeInt(int min, int max) {
            return Random.Range(min, max + 1);
        }

        public static float RangeFloat(float min, float max) {
            return Random.Range(min, max);
        }

        public static T GetRandomIndex<T>(T[] array) {
            return array[UnityEngine.Random.Range(0, array.Length)];
        }

        /// <summary>
        /// included max value
        /// </summary>
        /// <param name="quantityRange"></param>
        /// <returns></returns>
        public static int GetRandomItemAmount(int[] quantityRange) {
            return RangeInt(quantityRange[0], quantityRange[quantityRange.Length - 1]);
        }
    }

    #region ENUMS_BATTLE

    /// <summary>
    ///  정의. Bow Controller 화살 장전 타입
    /// </summary>
    public enum ARROWTYPE {
        NONE          = 0,  
        ARROW_MAIN    = 1,
        ARROW_SUB     = 2,
        ARROW_SPECIAL = 3,
    }

    #endregion

    #region ENUMS_SKILL
    /// <summary>
    /// 정의. Bow, Accessory Special Skill Level
    /// </summary>
    public enum SKILL_LEVEL
    {
        LEVEL_LOW    = 0,
        LEVEL_MEDIUM = 1,
        LEVEL_HIGH   = 2,
        LEVEL_UNIQUE = 3
    }
    /// <summary>
    /// 정의. 구현된 Bow Skill Type
    /// 추가 화살 갯수, 방향, 패턴관여
    /// </summary>
    public enum BOWSKILL_TYPE
    {
        SKILL_EMPTY,
        SKILL_SPREAD_SHOT,
        SKILL_RAPID_SHOT,
        SKILL_ARROW_RAIN
    }
    /// <summary>
    /// 정의. 구현된 Accessory Special Effect Type
    /// 전투 중, 독자적인 효과 부여 (중첩 불가)
    /// </summary>
    public enum ACSP_TYPE
    {
        SPEFFECT_NONE,
        SPEFFECT_AIMSIGHT,
        SPEEFECT_SLOWTIME
    }
    /// <summary>
    /// 정의. 구현된 ReinForcement Effect Type 
    /// 주로 수치 상향, 조정에 적용 (중첩 가능)
    /// </summary>
    public enum RFEF_TYPE
    {
        RFEFFECT_NONE,
        RFEFFECT_INCREASE_DAMAGE,
        RFEFFECT_INCREASE_PHYSICAL_DAMAGE,
        RFEFFECT_INCREASE_MAGICAL_DAMAGE,
        RFEFFECT_INCREASE_HEALTH
    }
    /// <summary>
    /// 정의. 구현된 Arrow Skill Type
    /// </summary>
    public enum ARROWSKILL {
        NONE,
        SKILL_REBOUND,
        SKILL_HOMING,
        SKILL_SPLIT,
        SKILL_PIERCING,
        SPLIT_DAGGER,
        ELEMENTAL_FIRE,
        EXPLOSION,
        WINDPIERCING,
        BUFF,
    }
    /// <summary>
    /// 정의. Skill 발동 UI 타입
    /// </summary>
    public enum ACSPACTIVETYPE {
        NONE,
        COOLDOWN,
        CHARGING,
        KILLCOUNT,
        HITCOUNT
    }
    /// <summary>
    /// 정의. ARROW SKILL 발동 타입
    /// </summary>
    public enum ARROWSKILL_ACTIVETYPE {
        NONE,
        FULL,
        ATTACK_AIR,
        ATTACK_ADDPROJ,
        ATTACK,
        AIR_ADDPROJ,
        AIR,
        ADDPROJ,
        EMPTY,
        SP_EXPLOSION,
        SP_WINDPIERCING,
        BUFF,
    }
    ///[7] TTT : FULL SKILL
    ///[8] TTF : ATK, AIR
    ///[9] TFT : ATK, ADD PROJ
    ///[10] TFF : ATK
    ///[11] FTT : AIR, ADD PROJ
    ///[12] FTF : AIR
    ///[13] FFT : ADD PROJ
    ///[14] FFF : EMPTY
    #endregion
}
