namespace ActionCat {
    using UnityEngine;
    public class MonsterStatusEntity : ScriptableObject {
        [Header("BASE STATUS")]
        public float HealthPointAmount = 0f;
        public float ManaPointAmount   = 0f;

        [Header("ATTACK STATUS")]
        public float DamageAmount = 0f;

        [Header("DEFENCE STATUS")]
        public short Armorating    = 0;
        public byte CriticalResist = 0;

        [Header("ACTION")]
        public float MoveSpeed      = 0f;
        public float AttackInterval = 0f;

        [Header("ETC")]
        public float ItemDropCorrection  = 0f;
        public float GaugeIncreaseAmount = 0f;
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(MonsterStatus))]
    public class MonsterStatusEntityEditor : UnityEditor.Editor {
        [UnityEditor.MenuItem("ActionCat/Scriptable Object/Monster Status Entity")]
        public static void CreateMonsterStatusEntity() {
            string assetCreatePath = "Assets/05.SO/StatusEntity/name_stage_entity.asset";
            var asset = ScriptableObject.CreateInstance<MonsterStatusEntity>();
            UnityEditor.AssetDatabase.CreateAsset(asset, assetCreatePath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = asset;
        }
    }
#endif
}
