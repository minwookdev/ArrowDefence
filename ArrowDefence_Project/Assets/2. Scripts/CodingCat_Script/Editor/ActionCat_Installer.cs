namespace ActionCat.Editor {
    using UnityEngine;

    public class ActionCat_Installer {

        [UnityEditor.MenuItem("ActionCat/Installer/Damage Floater")]
        public static void CreateDamageFloater() {
            GameObject tempGameObject = new GameObject("DamageFloater");
            tempGameObject.transform.position = new Vector3(0f, 0f, 0f);
            tempGameObject.transform.rotation = Quaternion.identity;
            tempGameObject.AddComponent<DamageFloater>();
        }
    }
}
