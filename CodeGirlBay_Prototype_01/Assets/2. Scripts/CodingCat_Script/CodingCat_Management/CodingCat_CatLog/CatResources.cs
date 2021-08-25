namespace CodingCat_Scripts
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class CatResources : ScriptableObject
    {
        [System.Serializable]
        private class ResourceInfo
        {
            public string path;
            public Object asset;
        }

        [SerializeField]
        private List<ResourceInfo> resources;

        private static CatResources instance;

        private static CatResources Instance { 
            get 
            { 
                if(instance == null)
                {
                    instance = Resources.Load("CatResources") as CatResources;
                }
                return instance; 
            } 
        }

        static public Object Load(string path)
        {
            var record = Instance.resources.FirstOrDefault(resource => resource.path == path);
            return record == null ? null : record.asset;
        }

        static public T Load<T>(string path) where T : Object
        {
            return Load(path) as T;
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Asset/Create/CodingCat_Resources")]
        static void Create()
        {
            var asset = ScriptableObject.CreateInstance<CatResources>();
            var path = "Assets/Resources/CatResources.asset";
            UnityEditor.AssetDatabase.CreateAsset(asset, path);
            UnityEditor.AssetDatabase.ImportAsset(path);
        }
#endif
    }
}
