namespace ActionCat {
    using ActionCat.Interface;
    using UnityEngine;

    public class DamageFloater : MonoBehaviour, IPoolUser {
        [Header("DAMAGE FLOATER")]
        public int PoolCounter = 10;
        [SerializeField] Transform parentTransform = null;
        [SerializeField] GameObject floatingDamagePref = null;

        string format = "";

        private static DamageFloater _inst = null;
        public static DamageFloater Instance {
            get {
                if(_inst != null){
                    return _inst;
                }
                else {
                    CatLog.ELog("Not Instantiated Damage Floater.");
                    return null;
                }
            }
        }

        void Awake() {
            _inst = this;
        }

        public System.Collections.IEnumerator Start() {
            //Wait Until CCPooler Initialized
            yield return new WaitUntil(() => CCPooler.IsInitialized == true);
            //Init Object Pooler in FloatingDamage Object..
            CCPooler.AddPoolList(AD_Data.POOLTAG_FLOATING_DAMAGE, 20, floatingDamagePref, parentTransform, iscount:false);
        }

        void OnDestroy() {
            _inst = null;
        }

        public void OnFloating(float value, Vector3 position, Vector2 direction, bool iscritical) {
            var damage = CCPooler.SpawnFromPool<FloatingDamage>(AD_Data.POOLTAG_FLOATING_DAMAGE, position, Quaternion.identity);
            damage.OnFloating(value.ToString(), direction, iscritical);
        }

        public void OnFloatingWithScale(float value, Vector3 position, Vector2 direction, bool iscritical) {
            var damage = CCPooler.SpawnFromPool<FloatingDamage>(AD_Data.POOLTAG_FLOATING_DAMAGE, position, Quaternion.identity);
            damage.OnFloatingWithScale(value.ToString(), direction, iscritical);
        }
    }
}
