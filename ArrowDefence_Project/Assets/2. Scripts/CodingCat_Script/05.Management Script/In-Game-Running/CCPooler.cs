namespace ActionCat
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using System.Linq;

#if UNITY_EDITOR
    [CustomEditor(typeof(CCPooler))]
    public class CCPoolerEditor : Editor
    {
        const string rules = "Coding Cat Notification ! Object Pooler Rules \n" +
                                     "1st Pool Object : Main Arrow \n" + 
                                     "2nd Pool Object : Main Arrow-Less \n" + 
                                     "3rd Pool Object : Sub Arrow \n" +
                                     "4th Pool Object : Sub Arrow-Less \n" + 
                                     "5th Pool Object : Special Arrow \n" +
                                     "~~ The Pool target Object must contain a ReturnToPool() method.";

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(rules, MessageType.Warning);
            base.OnInspectorGUI();
        }
    }
#endif

    public class CCPooler : MonoBehaviour
    {
        //특정 오브젝트의 부모가 바뀌는 상황에서 반드시 개체는 Disable 된 상황이어야 한다.
        static CCPooler _inst;

        private void Awake() => _inst = this;

        Dictionary<string, Stack<GameObject>> poolDictionary;

        [Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
            [ReadOnly] public Transform defalutParent;
        }

        [Header("PARENT LIST")]
        [SerializeField] private List<Transform> ParentList;

        [Header("POOL OBJECTS")]
        [SerializeField] private List<Pool> poolInstanceList = new List<Pool>();

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

        public static GameObject SpawnFromPool(string tag, Transform parent ,
                                               Vector3 scale, Vector3 pos, Quaternion rot) =>
            _inst._SpawnFromPool(tag, parent, scale, pos, rot);

        public static T SpawnFromPool<T>(string tag, Transform parent, 
                                Vector3 scale, Vector3 pos, Quaternion rot) where T : Component
        {
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
        GameObject _SpawnFromPool(string tag, Vector3 pos, Quaternion rot)
        {
            if (!poolDictionary.ContainsKey(tag))
                throw new Exception($"Pool With Tag {tag} doesn't Exist");

            Stack<GameObject> poolStack = poolDictionary[tag];

            //꺼내려는 Object가 Stack에 없을 경우 새로 추가
            if(poolStack.Count <= 0)
            {
                //Pool pool = Array.Find(pools, x=> x.tag == tag);
                Pool pool = poolInstanceList.Find(x => x.tag == tag);
                var obj = CreateNewObject(pool.tag, pool.prefab, FindParentTransform(pool.tag));
            }

            //Stack 에서 꺼내서 사용
            GameObject objectToSpawn = poolStack.Pop();
            objectToSpawn.transform.position = pos;
            objectToSpawn.transform.rotation = rot;
            objectToSpawn.SetActive(true);

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
        GameObject _SpawnFromPool(string tag, Transform parent, Vector3 scale, Vector3 pos, Quaternion rot)
        {
            if (!poolDictionary.ContainsKey(tag))
                throw new Exception($"Pool With Tag {tag} doesn't Exist");

            Stack<GameObject> poolStack = poolDictionary[tag];

            //Spawn 하려는 Object가 Pool에 없는 경우 새로 만듦
            if(poolStack.Count <= 0)
            {
                //부모찾는방법 최적화
                //Pool pool = Array.Find(pools, x => x.tag == tag);
                Pool pool = poolInstanceList.Find(x => x.tag == tag);
                var obj = CreateNewObject(pool.tag, pool.prefab, FindParentTransform(pool.tag));
            }

            GameObject objectToSpawn = poolStack.Pop();
            objectToSpawn.transform.SetParent(parent);
            objectToSpawn.transform.localScale = scale;
            objectToSpawn.transform.position   = pos;
            objectToSpawn.transform.rotation   = rot;
            objectToSpawn.SetActive(true);

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
        /// 관리 부모 Object가 있는경우에 회수처리 Method
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="parentNum"></param>
        public static void ReturnToPool(GameObject obj, byte num)
        {
            if (!_inst.poolDictionary.ContainsKey(obj.name))
                throw new Exception($"Pool With Tag {obj.name} doesn't Exist.");

            obj.SetActive(false);
            obj.transform.SetParent(_inst.FindParentTransform(obj.name));
            //obj.transform.SetParent(_inst.FindParentTransform(tag)); //이거 못씀;; Main, Sub Arrow 회수할때 중복됨
            //ParentList에서 GameObject.name 이랑 싱크 맞춰놓았으니 굳이 사용할 필요도 없음

            //사용할 준비가 완벽하게 정리되면 Push 해주기
            _inst.poolDictionary[obj.name].Push(obj);
        }

        /// <summary>
        /// 부모 Object가 없는경우 또는 CreateNewObject 메서드제외 사용금지
        /// </summary>
        /// <param name="obj"></param>
        public static void ReturnToPool(GameObject obj)
        {
            if (!_inst.poolDictionary.ContainsKey(obj.name))
                throw new Exception($"Pool With Tag {obj.name} doesn't Exist.");

            obj.SetActive(false);

            _inst.poolDictionary[obj.name].Push(obj);
        }

        static IEnumerator ChangeParent(GameObject obj, Transform parent)
        {
            yield return null;

            obj.transform.SetParent(parent);
        }

        private void Start()
        {
            poolDictionary = new Dictionary<string, Stack<GameObject>>();
            ParentList = new List<Transform>();

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

        GameObject CreateNewObject(string tag, GameObject prefab, Transform parent)
        {
            var obj = Instantiate(prefab, parent);
            obj.name = tag;
            //obj.SetActive(false);   //Spawn 하고 비활성화 시 ReturnToPool을 실행하므로 Stack에 Push 된다
            ReturnToPool(obj);
            return obj;
        }

        public static void DestroyCCPooler()
        {
            Destroy(CCPooler._inst.gameObject);
            CCPooler._inst = null;
        }

        /// <summary>
        /// Pool Object Prefab을 등록하고 Pool 관리대상 Pool Dictionary에 추가합니다.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="size"></param>
        /// <param name="prefab"></param>
        public static void AddPoolList(string tag, int size, GameObject prefab)
        {
            Pool pool = new Pool() { tag = tag, size = size, prefab = prefab };
            _inst.poolInstanceList.Add(pool);

            _inst.poolDictionary.Add(pool.tag, new Stack<GameObject>());

            var parentObj = new GameObject(pool.tag).transform;
            parentObj.SetParent(_inst.transform);
            _inst.ParentList.Add(parentObj);

            for (int i = 0; i < pool.size; i++)
            {
                _inst.CreateNewObject(pool.tag, pool.prefab, parentObj);
            }

            if (_inst.poolDictionary[pool.tag].Count <= 0)
                CatLog.ELog($"{pool.tag} : ReturnToPool 메서드 누락되어, Stack에 Pull 되지 않았습니다.");
            else if (_inst.poolDictionary[pool.tag].Count != pool.size)
                CatLog.ELog($"{pool.tag} : ReturnToPool 메서드가 중복 작성되었습니다.");
        }

        private Transform FindParentTransform(string tag)
        {
            Transform tr = ParentList.Find(x => x.name == tag);

            if (tr) return tr;
            else    return null;
        }

        #region FIND_POOL_OBJECT

        /// <summary>
        /// Pool Tag로 Pool 관리 대상 Object를 반환합니다
        /// </summary>
        public static GameObject[] FindPoolObjectsWithTag(string pooltag)
        {
            if (_inst.poolDictionary.ContainsKey(pooltag) == false)
                throw new Exception($"Pool With Tag {pooltag} doesn't Exist");
        
            Stack<GameObject> poolStack = _inst.poolDictionary[pooltag];
            return poolStack.ToArray();
        }

        public static int GetPoolStackSize(string tag)
        {
            if (_inst.poolDictionary.ContainsKey(tag) == false)
                throw new Exception($"Pool With Tag {tag} doesn't Exist");

            Stack<GameObject> poolStack = _inst.poolDictionary[tag];
            return poolStack.Count;
        }

        // -> 활성화 해서 사용하려는 Object는 Pool Stack에서 Pop되어 사용하기 때문에
        //    현재로써는 활성화된 Object만 따로 잡아낼 수 없다. 결과는 항상 Length가 0인 배열을 반환.
        /// <summary>
        /// Pool Tag로 Pool관리 대상 [활성화된] Object를 반환합니다. 
        /// </summary>
        /// <param name="pooltag"></param>
        /// <returns></returns>
        //public static GameObject[] FindAlivePoolObjectsWidthTag(string pooltag)
        //{
        //    if(_inst.poolDictionary.ContainsKey(pooltag) == false)
        //        throw new Exception($"Pool With Tag {pooltag} doesn't Exist");
        //
        //    List<GameObject> poolList = _inst.poolDictionary[pooltag].ToList();
        //    for (int i = poolList.Count - 1; i >= 0; i--)
        //    {
        //        if (poolList[i].activeSelf == false)
        //            poolList.Remove(poolList[i]);
        //    }
        //
        //    return poolList.ToArray();
        //}
        //

        #endregion

    }
}
