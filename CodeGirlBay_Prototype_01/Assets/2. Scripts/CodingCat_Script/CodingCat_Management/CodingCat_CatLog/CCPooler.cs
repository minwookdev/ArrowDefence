namespace CodingCat_Games
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using System.Linq;
    using UnityEngine.UIElements;
    using CodingCat_Scripts;
    using CodingCat_Games.Data;

#if UNITY_EDITOR
    [CustomEditor(typeof(CCPooler))]
    public class CCPoolerEditor : Editor
    {
        //const string info = "** The following code is written in Pooling Object \n" +
        //                    "void OnDisable()\n" +
        //                    "{\n" +
        //                    "   CatObjectPooler.ReturnToPool(this.gameObject); //Once Per Object \n" +
        //                    "   CancelInvoke(); //If the object has an Invoke Method \n" +
        //                    "}";

        const string rules = "Coding Cat Object Pooler Rules \n" +
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

        [SerializeField]
        private Pool[] pools;

        [SerializeField]
        private List<Transform> ParentList;

        private string parentStr = "_Pool";

        [Header("TESTING POOL OBJECTS")]
        [SerializeField] private List<Pool> testingPools = new List<Pool>();

        public static bool IsInitialized { get; private set; }

        [ContextMenu("GetSpawnObjectsInfo")]
        void GetSpawnObjectInfo()
        {
            foreach (var pool in pools)
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
                Pool pool = Array.Find(pools, x=> x.tag == tag);
                var obj = CreateNewObject(pool.tag, pool.prefab, ParentList.Find(tr => tr.name == tag + parentStr));

                //Pool Class 자체에 Transform 형태의 부모를 껴주는것도 나쁘지 않을듯..
                //Parent List에 등록된 Transform 의 Name 으로 이름을 찾아온다
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
                Pool pool = Array.Find(pools, x => x.tag == tag);
                var obj = CreateNewObject(pool.tag, pool.prefab, ParentList.Find(tr => tr.name == tag + parentStr));
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

        //Parent가 움직이는 개체에 대한 함수
        public static void ReturnToPool(GameObject obj, byte parentNum)
        {
            if (!_inst.poolDictionary.ContainsKey(obj.name))
                throw new Exception($"Pool With Tag {obj.name} doesn't Exist.");

            //테스트 필요
            obj.SetActive(false);
            obj.transform.SetParent(_inst.ParentList[parentNum]);

            _inst.poolDictionary[obj.name].Push(obj);

            //굳이 코루틴으로 한프레임 Waiting 해서 사용하는 이유는
            //대상 PoolObjec의 Disable 처리와 ReturnToPool 메서드에서 Parent 바꿔주는 프레임이 동일해서
            //Unity 자체에서 에러로그를 띄워버린다
            //같은 프레임에 Object Disable 처리와 SetParent 과정이 동일하게 있으면 오류를 띄우는듯 하다
            //PoolObject와 커플링이 너무 심하다..
            //_inst의 gameObject가 살아있는지 확인하는 이유는 CCPooler 오브젝트가 죽었을때 StartCoroutine을
            //시도하려 한다는 에러메세지가 출력되었기 때문에 살아있는지 체크하고 코루틴을 돌린다
            //한 프레임 기다리는게 말이 안되고 코드도 더러워지기 때문에 기다릴일이 없도록 수정
        }

        //Parent가 움직이지 않는 개체에 대한 함수
        public static void ReturnToPool(GameObject obj)
        {
            if (!_inst.poolDictionary.ContainsKey(obj.name))
                throw new Exception($"Pool With Tag {obj.name} doesn't Exist.");

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

            foreach (Pool pool in pools)
            {
                //현재는 갯수 할당되지 않은 오브젝트는 돌려주지 않음
                if (pool.size <= 0) continue;

                poolDictionary.Add(pool.tag, new Stack<GameObject>());

                var parentObj = new GameObject(pool.tag + parentStr).transform;
                parentObj.SetParent(this.transform);
                ParentList.Add(parentObj);

                for(int i =0; i < pool.size; i++)
                {
                    var obj = CreateNewObject(pool.tag, pool.prefab, parentObj.transform);
                }

                //Pool 대상 Object에 ReturnToPool 메서드 검사
                if (poolDictionary[pool.tag].Count <= 0)
                    CatLog.ELog($"{pool.tag} ReturnToPool 메서드 누락, Stack에 Pull 되지 않았습니다.");
                else if (poolDictionary[pool.tag].Count != pool.size)
                    CatLog.ELog($"{pool.tag} ReturnToPool 메서드가 중복 작성.");
            }

            IsInitialized = true;

            //pools에 캐싱된 정보로 만드는 Dictionary가 아닌 코드로 직접 풀 배열을 만들어주는 기능 필요
            //var equipment = CCPlayerData.equipments;
            //
            //if (equipment.IsEquippedArrowMain())
            //{
            //    Pool arrowPool = new Pool();
            //    arrowPool.tag = AD_Data.TAG_MAINARROW;
            //    arrowPool.size = 30;
            //    arrowPool.prefab = equipment.GetMainArrow().GetObject_MainArrow();
            //
            //    testingPools.Add(arrowPool);
            //
            //    Pool lessPool = new Pool();
            //    lessPool.tag = AD_Data.TAG_MAINARROW_LESS;
            //    lessPool.size = 30;
            //    lessPool.prefab = equipment.GetMainArrow().GetObject_LessArrow();
            //
            //    testingPools.Add(lessPool);
            //}
        }

        GameObject CreateNewObject(string tag, GameObject prefab, Transform parent)
        {
            var obj = Instantiate(prefab, parent);
            obj.name = tag;
            obj.SetActive(false);   //Spawn 하고 비활성화 시 ReturnToPool을 실행하므로 Stack에 Push 된다
            ReturnToPool(obj);
            return obj;
        }

        public static void DestroyCCPooler()
        {
            Destroy(CCPooler._inst.gameObject);
            CCPooler._inst = null;
        }

        public static void AddPoolList(string tag, int size, GameObject prefab)
        {
            Pool pool = new Pool() { tag = tag, size = size, prefab = prefab };
            _inst.testingPools.Add(pool);

        }
    }
}
