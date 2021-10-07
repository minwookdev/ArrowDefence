namespace CodingCat_Games
{
    using CodingCat_Scripts;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

#if UNITY_EDITOR
    [CustomEditor(typeof(CatObjectPooler))]
    public class ObjectPoolerEditor : Editor
    {
        const string info = "** The following code is written in Pooling Object \n" +
                            "void OnDisable()\n" +
                            "{\n" +
                            "   CatObjectPooler.ReturnToPool(this.gameObject); //Once Per Object \n" +
                            "   CancelInvoke(); //If the object has an Invoke Method \n" +
                            "}";

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(info, MessageType.Info);
            base.OnInspectorGUI();
        }
    }
#endif

    public class CatObjectPooler : MonoBehaviour
    {
        static CatObjectPooler Instance;
        private void Awake() => Instance = this;

        [Serializable]
        public class Pool
        {
            public string Tag;
            public GameObject prefab;
            public int size;
        }

        [SerializeField]
        private Pool[] Pools;

        List<GameObject> spawnObjects;  //Transform 의 Child 순서 및 Count 그대로 따라감
        Dictionary<string, Queue<GameObject>> poolDictionary;
        readonly string info = "** The following code is written in Pooling Object \n" +
                            "void OnDisable()\n" +
                            "{\n" +
                            "   CatObjectPooler.ReturnToPool(this.gameObject); //Once Per Object \n" +
                            "   CancelInvoke(); //If the object has an Invoke Method \n" +
                            "}";

        #region ORIGIN_SPAWN_FROM_POOL_METHOD

        public static GameObject SpawnFromPool(string tag, Vector3 pos) =>
            Instance._SpawnFromPool(tag, pos, Quaternion.identity);

        public static GameObject SpawnFromPool(string tag, Vector3 pos, Quaternion rot) =>
            Instance._SpawnFromPool(tag, pos, rot);

        public static T SpawnFromPool<T>(string tag, Vector3 pos) where T : Component
        {
            GameObject obj = Instance._SpawnFromPool(tag, pos, Quaternion.identity);

            if (obj.TryGetComponent(out T component))
                return component;
            else
            {
                obj.SetActive(false);
                throw new Exception($"{tag} Component is Not Found !");
            }
        }

        public static T SpawnFromPool<T>(string tag, Vector3 pos, Quaternion rot) where T : Component
        {
            GameObject obj = Instance._SpawnFromPool(tag, pos, rot);

            if (obj.TryGetComponent(out T component))
                return component;
            else
            {
                obj.SetActive(false);
                throw new Exception($"{tag} Object Not Found Target Component !");
            }
        }

        #endregion

        #region EXTENDED_SPAWN_FROM_POOL_METHOD

        public static GameObject SpawnFromPool(string tag, Transform parent, Vector3 pos, Quaternion rot) =>
            Instance._SpawnFromPool(tag, parent, pos, rot);

        public static T SpawnFromPool<T>(string tag, Transform parent, Vector3 pos, Quaternion rot)
        {
            GameObject obj = Instance._SpawnFromPool(tag, parent, pos, rot);

            if (obj.TryGetComponent(out T component))
                return component;
            else
            {
                obj.SetActive(false);
                throw new Exception($"{tag} Object Not Found Target Component !");
            }
        }
        

        #endregion

        /// <summary>
        /// Origin _SpawnFromPool Method
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="pos"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        GameObject _SpawnFromPool(string tag, Vector3 pos, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
                throw new Exception($"Pool with tag {tag} doesn't exist.");

            //Spawn 하려는 Object 가 Queue에 없으면 새로 추가
            Queue<GameObject> poolQueue = poolDictionary[tag];

            if(poolQueue.Count <= 0)
            {
                Pool pool = Array.Find(Pools, x => x.Tag == tag);
                var obj = CreateNewObject(pool.Tag, pool.prefab);
                ArrangePool(obj);
            }

            //큐에서 꺼내서 사용
            GameObject objectToSpawn = poolQueue.Dequeue();
            objectToSpawn.transform.position = pos;
            objectToSpawn.transform.rotation = rotation;
            objectToSpawn.SetActive(true);

            return objectToSpawn;
        }

        /// <summary>
        /// Extra _SpawnToPool Method (Change Parnet Response)
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="parent"></param>
        /// <param name="pos"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        GameObject _SpawnFromPool(string tag, Transform parent, Vector3 pos, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
                throw new Exception($"Pool with tag {tag} doesn't exist.");

            if (!poolDictionary.ContainsKey(tag))
                throw new Exception($"Pool with tag {tag} doesn't exist.");

            //Spawn 하려는 Object 가 Queue에 없으면 새로 추가
            Queue<GameObject> poolQueue = poolDictionary[tag];

            if (poolQueue.Count <= 0)
            {
                Pool pool = Array.Find(Pools, x => x.Tag == tag);
                var obj = CreateNewObject(pool.Tag, pool.prefab);
                ArrangePool(obj);
            }

            //큐에서 꺼내서 사용
            GameObject objectToSpawn = poolQueue.Dequeue();
            objectToSpawn.transform.SetParent(parent, false);
            objectToSpawn.transform.position = pos;
            objectToSpawn.transform.rotation = rotation;
            objectToSpawn.SetActive(true);

            return objectToSpawn;
        }

        public static void ReturnToPool(GameObject obj)
        {
            if (!Instance.poolDictionary.ContainsKey(obj.name))
                throw new Exception($"Pool with Tag {obj.name} doesn't exist.");
            //Test Func
            //if(obj.transform.parent != transform)

            Instance.poolDictionary[obj.name].Enqueue(obj);
        }

        private void Start()
        {
            spawnObjects = new List<GameObject>();
            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            //Create PoolObject From Start Method
            foreach (Pool pool in Pools)
            {
                poolDictionary.Add(pool.Tag, new Queue<GameObject>());

                for(int i =0;i<pool.size;i++)
                {
                    var obj = CreateNewObject(pool.Tag, pool.prefab);
                    ArrangePool(obj);
                }

                //Pooling Objectdp ReturnToPoolObject 구현여부와 중복구현 검사코드
                if (poolDictionary[pool.Tag].Count <= 0)
                {
                    CatLog.ELog($"{pool.Tag}{info}");
                }
                else if (poolDictionary[pool.Tag].Count != pool.size)
                {
                    CatLog.ELog($"{pool.Tag} Object에 ReturnToPool구현이 중복되었습니다");
                }
            }
        }

        GameObject CreateNewObject(string tag, GameObject prefab)
        {
            var obj = Instantiate(prefab, transform);
            obj.name = tag;
            obj.SetActive(false);   //스크립트의 Disable Method에 ReturnToPool가 있다면 Enqueue된다
            return obj;
        }

        private void ArrangePool(GameObject obj)
        {
            //추가된 오브젝트를 묶어서 정렬
            bool isFind = false;
            for(int i =0; i < transform.childCount; i++)
            {
                if(i == transform.childCount - 1)
                {
                    obj.transform.SetSiblingIndex(i);
                    spawnObjects.Insert(i, obj);
                    break;
                }
                else if (transform.GetChild(i).name == obj.name)
                {
                    isFind = true;
                }
                else if (isFind)
                {
                    obj.transform.SetSiblingIndex(i);
                    spawnObjects.Insert(i, obj);
                    break;
                }
            }
        }

    }
}
