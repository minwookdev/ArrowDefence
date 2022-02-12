namespace ActionCat
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEditor;

#if UNITY_EDITOR
    [CustomEditor(typeof(CCPooler))]
    public class CCPoolerEditor : Editor
    {
        const string rules = "Coding Cat Notification ! Object Pooler Rules \n" +
                             "CCPooler의 모든 동작은 코드 컨트롤 합니다. \n" +
                             "Pooler를 사용하여 Instantiate되는 모든 오브젝트는 비활성화 시 \n" +
                             "IPoolObject.DisableRequest 메서드를 사용하여 비활성화를 요청합니다. \n" +
                             "**인스펙터의 항목을 수정하지 않습니다**";

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(rules, MessageType.Warning);
            base.OnInspectorGUI();
        }
    }
#endif

    public class CCPooler : MonoBehaviour {
        static CCPooler _inst;

        private void Awake() => _inst = this;

        Dictionary<string, Stack<GameObject>> poolDictionary;

        [Serializable]
        public class Pool {
            public string tag;
            public GameObject prefab;
            public int size;
            [ReadOnly] public Transform defalutParent;
            public void SizeInc(int value) {
                size += value;
            }
        }

        [Header("PARENT LIST")]
        [SerializeField] private List<Transform> ParentList;

        [Header("POOL OBJECTS")]
        [SerializeField] private List<Pool> poolInstanceList = new List<Pool>();

        [Header("ENABLE TRACKING")]
        Dictionary<string, List<GameObject>> aliveTrackDic = null;

        public static bool IsInitialized { get; private set; }

        [ContextMenu("GetSpawnObjectsInfo")]
        void GetSpawnObjectInfo()
        {
            foreach (var pool in poolInstanceList)
            {
                int count = poolDictionary[pool.tag].Count;
                CatLog.Log($"{pool.tag} Count : {count}");
            }
        }

        private void Start() {
            //Init Collections
            poolDictionary = new Dictionary<string, Stack<GameObject>>();
            aliveTrackDic  = new Dictionary<string, List<GameObject>>();
            ParentList     = new List<Transform>();

            #region LEGACY_CODE
            //foreach (Pool pool in pools)
            //{
            //    //현재는 갯수 할당되지 않은 오브젝트는 돌려주지 않음
            //    if (pool.size <= 0) continue;
            //
            //    poolDictionary.Add(pool.tag, new Stack<GameObject>());
            //
            //    var parentObj = new GameObject(pool.tag + parentStr).transform;
            //    parentObj.SetParent(this.transform);
            //    ParentList.Add(parentObj);
            //
            //    for(int i =0; i < pool.size; i++)
            //    {
            //        var obj = CreateNewObject(pool.tag, pool.prefab, parentObj.transform);
            //    }
            //
            //    //Pool 대상 Object에 ReturnToPool 메서드 검사
            //    if (poolDictionary[pool.tag].Count <= 0)
            //        CatLog.ELog($"{pool.tag} ReturnToPool 메서드 누락, Stack에 Pull 되지 않았습니다.");
            //    else if (poolDictionary[pool.tag].Count != pool.size)
            //        CatLog.ELog($"{pool.tag} ReturnToPool 메서드가 중복 작성.");
            //}
            #endregion

            IsInitialized = true;
        }

        void OnDestroy() {
            //Release Instance CCPooler
            _inst = null;
        }

        #region SPAWN_FROM_POOL_METHOD

        public static GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation) =>
            _inst._SpawnFromPool(tag, position, rotation);

        public static T SpawnFromPool<T>(string tag, Vector3 position, Quaternion rotation) where T : Component
        {
            GameObject obj = _inst._SpawnFromPool(tag, position, rotation);

            if (obj.TryGetComponent(out T component))
                return component;
            else
            {
                obj.SetActive(false);
                throw new Exception($"{tag} Object Not Found Target Component");
            }
        }

        #endregion

        #region EXTENDED_SPAWN_FROM_POOL

        /// <summary>
        /// Parent Change를 사용하는 Pool Object Spawn Method, Object 회수에 반드시 ReturnToPool(GameObject, byte)를 사용할 것.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="parent"></param>
        /// <param name="scale"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <returns></returns>
        public static GameObject SpawnFromPool(string tag, Transform parent ,
                                               Vector3 scale, Vector3 pos, Quaternion rot) =>
            _inst._SpawnFromPool(tag, parent, scale, pos, rot);

        /// <summary>
        /// Parent Change를 사용하는 Pool Object Spawn Method, Object 회수에 반드시 ReturnToPool(GameObject, byte)를 사용할 것.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <param name="parent"></param>
        /// <param name="scale"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <returns></returns>
        public static T SpawnFromPool<T>(string tag, Transform parent, 
                                Vector3 scale, Vector3 pos, Quaternion rot) where T : Component {
            GameObject obj = _inst._SpawnFromPool(tag, parent, scale, pos, rot);

            if (obj.TryGetComponent(out T component))
                return component;
            else
            {
                obj.SetActive(false);
                throw new Exception($"{tag} Object Not Found Target Component");
            }
        }

            #endregion

        /// <summary>
        /// Spawn Object From Pool Stack.
        /// </summary>
        /// <param name="tag">Spawn Target Object Name</param>
        /// <param name="pos">Position of Object Spawn</param>
        /// <param name="rot">Rotation of Object Spawn</param>
        /// <returns></returns>
        GameObject _SpawnFromPool(string tag, Vector3 pos, Quaternion rot) {
            if (!poolDictionary.ContainsKey(tag))
                throw new Exception($"Pool With Tag {tag} doesn't Exist");

            Stack<GameObject> poolStack = poolDictionary[tag];

            //Spawn 하려는 PoolObject가 Dictionary Stack에 없는 경우 생성
            if(poolStack.Count <= 0) {
                Pool pool = poolInstanceList.Find(x => x.tag == tag);
                var obj = CreateNewObject(pool.tag, pool.prefab, FindParentOrNull(pool.tag));
            }

            //Stack Pop
            GameObject objectToSpawn = poolStack.Pop();
            objectToSpawn.transform.position = pos;
            objectToSpawn.transform.rotation = rot;
            objectToSpawn.SetActive(true);

            //Try Add Alive Dictionary
            TryAddAliveTrackDic(tag, objectToSpawn);

            return objectToSpawn;
        }

        /// <summary>
        /// Spawn Object From Pool Stack, Set a New Parent Object
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="parent"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <returns></returns>
        GameObject _SpawnFromPool(string tag, Transform parent, Vector3 scale, Vector3 pos, Quaternion rot) {
            if (!poolDictionary.ContainsKey(tag))
                throw new Exception($"Pool With Tag {tag} doesn't Exist");

            Stack<GameObject> poolStack = poolDictionary[tag];

            //Spawn 하려는 PoolObject가 Dictionary Stack에 없는 경우 생성
            if (poolStack.Count <= 0) {
                Pool pool = poolInstanceList.Find(x => x.tag == tag);
                var obj = CreateNewObject(pool.tag, pool.prefab, FindParentOrNull(pool.tag));
            }

            //Stack Pop
            GameObject objectToSpawn = poolStack.Pop();
            objectToSpawn.transform.SetParent(parent);
            objectToSpawn.transform.localScale = scale;
            objectToSpawn.transform.position   = pos;
            objectToSpawn.transform.rotation   = rot;
            objectToSpawn.SetActive(true);

            //Try Add Alive Dictionary
            TryAddAliveTrackDic(tag, objectToSpawn);

            return objectToSpawn;
        }

        #region 테스트_요망

        public static Stack<GameObject> GetAllPools(string tag)
        {
            if (!_inst.poolDictionary.ContainsKey(tag))
                throw new Exception($"Pool with Tag {tag} doesn't exist.");

            Stack<GameObject> objectStack = _inst.poolDictionary[tag];
            return objectStack;
        }

        public static List<T> GetAllPools<T>(string tag) where T : Component
        {
            //Stack에서 제대로 빠지는 확인. 기존 Stack에 영향을 주지 않는지 확인
            //예상되는 문제_GetAllPools를 실행한뒤에 새로 생성하는 오브젝트들에 대해
            List<GameObject> objects = GetAllPools(tag).ToList();

            if (!objects[0].TryGetComponent(out T component))
                throw new Exception($"{objects[0].name} Object Component Not Found");

            return objects.ConvertAll(x => x.GetComponent<T>());
        }

        #endregion

        /// <summary>
        /// Object 회수. (부모가 변경된 객체 해당)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="parentNum"></param>
        public static void ReturnToPool(GameObject obj, byte num)
        {
            if (!_inst.poolDictionary.ContainsKey(obj.name))
                throw new Exception($"Pool With Tag {obj.name} doesn't Exist.");

            obj.SetActive(false);
            obj.transform.SetParent(_inst.FindParentOrNull(obj.name));
            //obj.transform.SetParent(_inst.FindParentTransform(tag)); //이거 못씀;; Main, Sub Arrow 회수할때 중복됨
            //ParentList에서 GameObject.name 이랑 싱크 맞춰놓았으니 굳이 사용할 필요도 없음

            //Try Remove AliveTracking Dictionary.
            _inst.TryRemoveAliveTrackDic(obj.name, obj);

            //사용할 준비가 완벽하게 정리되면 Push 해주기
            _inst.poolDictionary[obj.name].Push(obj);
        }

        /// <summary>
        /// Object 회수. (부모가 변경되지 않은 객체 해당)
        /// </summary>
        /// <param name="obj"></param>
        public static void ReturnToPool(GameObject obj) {
            if (!_inst.poolDictionary.ContainsKey(obj.name))
                throw new Exception($"Pool With Tag {obj.name} doesn't Exist.");

            //Disable Pool Object
            obj.SetActive(false);

            //Try Remove AliveTracking Dictionary.
            _inst.TryRemoveAliveTrackDic(obj.name, obj);

            _inst.poolDictionary[obj.name].Push(obj);
        }

        GameObject CreateNewObject(string tag, GameObject prefab, Transform parent)
        {
            var obj = Instantiate(prefab, parent);
            obj.name = tag;
            //obj.SetActive(false);   //Spawn 하고 비활성화 시 ReturnToPool을 실행하므로 Stack에 Push 된다
            ReturnToPool(obj);
            return obj;
        }

        /// <summary>
        /// Pool Instance List와 Pool Dictionary에 객체 등록. (부모객체를 지정하지 않음)
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="size"></param>
        /// <param name="prefab"></param>
        /// <param name="isTracking">active된 객체 수 추적 여부</param>
        public static void AddPoolList(string tag, int size, GameObject prefab, bool isTracking) {
            if(_inst.poolDictionary.ContainsKey(tag) == true) { //Exception: Duplicate PoolTag
                ///CatLog.WLog(StringColor.YELLOW, "Same PoolTag found in Dictionary.");
                ///Pool origin = _inst.poolInstanceList.Find(element => element.tag == tag);
                ///Transform originParent = _inst.FindParentOrNull(tag);
                ///if(origin == null || originParent == null) {
                ///    throw new Exception("Origin Pool or Parent is Null.");
                ///}
                ///origin.SizeInc(size);
                ///for (int i = 0; i < size; i++) { //Create the Size to be added
                ///    _inst.CreateNewObject(origin.tag, origin.prefab, originParent);
                ///}
                ///
                ///if(_inst.poolDictionary[origin.tag].Count != origin.size) {
                ///    CatLog.ELog($"{origin.tag} Not Equal Pool Dictionary Count");
                ///}
                ///return;
                throw new Exception("a Duplicate Pool Tag was Detected.");
            }

            Pool pool = new Pool() { tag = tag, size = size, prefab = prefab };
            _inst.poolInstanceList.Add(pool);                            //1. Add PoolInstance List

            _inst.poolDictionary.Add(pool.tag, new Stack<GameObject>()); //2. Add PoolDictionary

            var parentTr = new GameObject(pool.tag).transform;          //3. New Parent GameObject Created and Add Parent List.
            parentTr.SetParent(_inst.transform);
            _inst.ParentList.Add(parentTr);

            if(isTracking == true) {                                     //4. if Tracking Alive options ture, Add AliveDictionary this PoolObject. 
                _inst.NewAliveTrackDic(pool.tag);
            }

            for (int i = 0; i < pool.size; i++) {                        //5. Create New Pool Object. 
                _inst.CreateNewObject(pool.tag, pool.prefab, parentTr);
            }

            if (_inst.poolDictionary[pool.tag].Count <= 0)
                CatLog.ELog($"{pool.tag} : ReturnToPool 메서드 누락되어, Stack에 Pull 되지 않았습니다.");
            else if (_inst.poolDictionary[pool.tag].Count != pool.size)
                CatLog.ELog($"{pool.tag} : ReturnToPool 메서드가 중복 작성되었습니다.");
        }

        /// <summary>
        /// Pool Instance List와 Pool Dictionary에 객체 등록. (부모객체를 지정)
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="size"></param>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <param name="isTracking">active된 객체 수 추적 여부</param>
        public static void AddPoolList(string tag, int size, GameObject prefab, Transform parent, bool isTracking) {
            Pool pool = new Pool() { tag = tag, size = size, prefab = prefab };
            _inst.poolInstanceList.Add(pool);                             //1. Add PoolInstance List.

            _inst.poolDictionary.Add(pool.tag, new Stack<GameObject>());  //2. Add PoolDictaionary.

            _inst.ParentList.Add(parent);                                 //3. Add Parent Transform to Parent List.

            if(isTracking == true) {                                      //4. if Tracking Alive options true, Add AliveDictionary this PoolObject
                _inst.NewAliveTrackDic(pool.tag);
            }

            for (int i = 0; i < pool.size; i++) {                         //5. Create New PoolObject.
                _inst.CreateNewObject(pool.tag, pool.prefab, parent);
            }

            if (_inst.poolDictionary[pool.tag].Count <= 0)
                CatLog.ELog($"{pool.tag} : Missing Return to Pool Method.");
            else if (_inst.poolDictionary[pool.tag].Count != pool.size)
                CatLog.ELog($"{pool.tag} : Method has been Duplicated.");
        }

        /// <summary>
        /// This Method is Experiment, Not Used Yet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <param name="size"></param>
        /// <param name="prefab"></param>
        /// <param name="action"></param>
        /// <param name="isTracking"></param>
        public static void AddPoolListExtended<T>(string tag, int size, T prefab, Action<T> action, bool isTracking) where T : Component {
            Transform tempParent = null; // Temp GameObejct Parent, Create New Temp Transform
            var tempPrefab  = Instantiate<T>(prefab, new Vector3(100f, 100f, 0f), Quaternion.identity, tempParent);
            tempPrefab.name = "temp_";
            action(tempPrefab);

            Pool pool = new Pool() { tag = tag, prefab = tempPrefab.gameObject, size = size };
            _inst.poolInstanceList.Add(pool);
            _inst.poolDictionary.Add(pool.tag, new Stack<GameObject>());

            var parentTr = new GameObject(pool.tag).transform;
            parentTr.SetParent(_inst.transform);
            _inst.ParentList.Add(parentTr);

            if (isTracking == true) {
                _inst.NewAliveTrackDic(pool.tag);
            }

            for (int i = 0; i < pool.size; i++) {
                _inst.CreateNewObject(pool.tag, pool.prefab, parentTr);
            }

            tempPrefab.gameObject.SetActive(false);

            if(_inst.poolDictionary[pool.tag].Count <= 0) {
                CatLog.ELog("");
            }
            else if (_inst.poolDictionary[pool.tag].Count != pool.size) {
                CatLog.ELog("");
            }
        }

        #region PARENT

        private Transform FindParentOrNull(string tag) {
            return ParentList.Find(element => element.name == tag);
        }

        #endregion

        #region ALIVE_TRACKER_DICTIONARY

        /// <summary>
        /// Create New Key AliveTrack Dictionary
        /// </summary>
        /// <param name="key">string type key</param>
        void NewAliveTrackDic(string key) {
            aliveTrackDic.Add(key, new List<GameObject>());
        }

        /// <summary>
        /// non-safety Method
        /// </summary>
        /// <param name="key"></param>
        /// <param name="go"></param>
        void AddAliveTrackDic(string key, GameObject go) {
            aliveTrackDic[key].Add(go);
        }

        /// <summary>
        /// non-safety Method
        /// </summary>
        /// <param name="key"></param>
        /// <param name="go"></param>
        void RemoveAliveTrackDic(string key, GameObject go) {
            var target = aliveTrackDic[key].Find(element => ReferenceEquals(element, go));
            if(target) {
                aliveTrackDic[key].Remove(target);
            }
            else {
                CatLog.ELog("target is Null. [CCPooler]");
            }
        }

        bool TryAddAliveTrackDic(string tag, GameObject go) {
            if (aliveTrackDic.ContainsKey(tag) == false) return false;
            aliveTrackDic[tag].Add(go);
            return true;
        }

        bool TryRemoveAliveTrackDic(string tag, GameObject go) {
            if (aliveTrackDic.ContainsKey(tag) == false) return false;
            var target = aliveTrackDic[tag].Find(element => ReferenceEquals(element, go));
            if(target) {
                aliveTrackDic[tag].Remove(target);
                return true;
            }
            else {
                return false;
            }
        }

        bool TryAliveTrackKey(string key) {
            return aliveTrackDic.ContainsKey(key);
        }

        public static int GetCountAliveTrackDic(string key) {
            if(_inst.aliveTrackDic.ContainsKey(key) == false) {
                throw new Exception($"No Has key : {key} in AliveTracking Dictionary");
            }

            return _inst.aliveTrackDic[key].Count;
        }

        public static int GetAllAliveMonsterCount() {
            int totalMonsterCount = 0;

            foreach (var dic in _inst.aliveTrackDic) {
                //Get Count of Normal, Elite, Frequency Monsters List
                if (dic.Key == AD_Data.POOLTAG_MONSTER_NORMAL || dic.Key == AD_Data.POOLTAG_MONSTER_ELITE || dic.Key == AD_Data.POOLTAG_MONSTER_FREQ) {
                    totalMonsterCount += dic.Value.Count;
                }
                else {
                    continue;
                }
            }

            return totalMonsterCount;
        }

        public static int GetAllAliveTrackCount() {
            int total = 0;
            //Type 1. access directly on Dictionary
            foreach (var dic in _inst.aliveTrackDic) {  
                total += dic.Value.Count;
            }
            return total;
            //Type 2.use foreach loop on Keys, then access Values
            //foreach (string key in _inst.aliveTrackDic.Keys) {
            //    total += _inst.aliveTrackDic[key].Count;
            //}
        }

        public static GameObject[] GetAliveMonsters() {
            List<GameObject> resultList   = new List<GameObject>();
            List<GameObject> monstersList = new List<GameObject>();

            //Get Normal Monster List
            if(_inst.aliveTrackDic.TryGetValue(AD_Data.POOLTAG_MONSTER_NORMAL, out monstersList)) {
                resultList.AddRange(monstersList);
            }
            //Get Elite Monster List
            if(_inst.aliveTrackDic.TryGetValue(AD_Data.POOLTAG_MONSTER_ELITE, out monstersList)) {
                resultList.AddRange(monstersList);
            }
            //Get Frequency Monster List
            if(_inst.aliveTrackDic.TryGetValue(AD_Data.POOLTAG_MONSTER_FREQ, out monstersList)) {
                resultList.AddRange(monstersList);
            }

            return resultList.ToArray();
        }

        public static void GetAliveMonstersOut(out GameObject[] array) {
            List<GameObject> resultList   = new List<GameObject>();
            List<GameObject> tempMonsters = new List<GameObject>();
            if(_inst.aliveTrackDic.TryGetValue(AD_Data.POOLTAG_MONSTER_NORMAL, out tempMonsters)) {
                resultList.AddRange(tempMonsters);
            }
            if(_inst.aliveTrackDic.TryGetValue(AD_Data.POOLTAG_MONSTER_ELITE, out tempMonsters)) {
                resultList.AddRange(tempMonsters);
            }
            if(_inst.aliveTrackDic.TryGetValue(AD_Data.POOLTAG_MONSTER_FREQ, out tempMonsters)) {
                resultList.AddRange(tempMonsters);
            }

            array = resultList.ToArray();
        }

        public static void OutEnableMonsters(out List<Transform> result) {
            List<GameObject> tempList  = new List<GameObject>();
            result = new List<Transform>();
            if(_inst.aliveTrackDic.TryGetValue(AD_Data.POOLTAG_MONSTER_NORMAL, out tempList)) {
                result.AddRange(tempList.GetComponentAll<Transform>());
            }
            if(_inst.aliveTrackDic.TryGetValue(AD_Data.POOLTAG_MONSTER_ELITE, out tempList)) {
                result.AddRange(tempList.GetComponentAll<Transform>());
            }
            if (_inst.aliveTrackDic.TryGetValue(AD_Data.POOLTAG_MONSTER_FREQ, out tempList)) {
                result.AddRange(tempList.GetComponentAll<Transform>());
            }
        }

        #endregion
    }
}
