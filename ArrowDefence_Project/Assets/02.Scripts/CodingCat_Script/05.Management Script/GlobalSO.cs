﻿namespace ActionCat {
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class GlobalSO : ScriptableObject {
        private const string FileDirectory = "Assets/Resources";
        private const string FilePath      = "Assets/Resources/GlobalSO.asset";

        private static GlobalSO _inst;

        [Header("POOLTAG")]
        public string POOLTAG_RAPIDSHOT_EFFECT = "";
        public string POOLTAG_MUZZLE = "Eff_Muzzle_0";

        [Header("SORTING ORDER")]
        public string SORTINGLAYER_EFFECT = "";

        public static GlobalSO Inst {
            get {
                if(_inst != null) {
                    return _inst;
                }

                _inst = Resources.Load<GlobalSO>("GlobalSO");

#if UNITY_EDITOR
                if (_inst == null) {
                    if(AssetDatabase.IsValidFolder(FileDirectory) == false) {
                        AssetDatabase.CreateFolder("Assets", "Resources");
                    }

                    _inst = AssetDatabase.LoadAssetAtPath<GlobalSO>(FilePath);

                    if(_inst == null) {
                        _inst = CreateInstance<GlobalSO>();
                        AssetDatabase.CreateAsset(_inst, FilePath);
                    }
                }
#endif
                return _inst;
            }
        }
    }
}