namespace ActionCat {
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class GOSO : ScriptableObject {
        private const string FileDirectory = "Assets/Resources";
        private const string FilePath      = "Assets/Resources/GOSO.asset";

        private static GOSO _inst;

        [Header("POOLTAG")]
        public string POOLTAG_RAPIDSHOT_EFFECT = "";
        public string POOLTAG_MUZZLE = "Eff_Muzzle_0";

        [Header("SORTING ORDER")]
        public string SORTINGLAYER_EFFECT = "";

        [Header("ITEM")]
        public ItemData[] supplyItems;

        public static GOSO Inst {
            get {
                if(_inst != null) {
                    return _inst;
                }

                _inst = Resources.Load<GOSO>("GOSO");

#if UNITY_EDITOR
                if (_inst == null) {
                    if(AssetDatabase.IsValidFolder(FileDirectory) == false) {
                        AssetDatabase.CreateFolder("Assets", "Resources");
                    }

                    _inst = AssetDatabase.LoadAssetAtPath<GOSO>(FilePath);

                    if(_inst == null) {
                        _inst = CreateInstance<GOSO>();
                        AssetDatabase.CreateAsset(_inst, FilePath);
                    }
                }
#endif
                return _inst;
            }
        }
    }
}
