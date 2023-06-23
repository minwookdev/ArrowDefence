namespace ActionCat {
    using UnityEngine;
    using ActionCat.Interface;

    public abstract class ProjectilePref : MonoBehaviour, IPoolObject {
        [Header("PROJECTILE VALUE")]
        [SerializeField] [ReadOnly] protected short finCalcDamage;

        [Header("COMPONENT")]
        [SerializeField] protected Transform tr = null;

        //protected List<IDamageable> targetList = new List<IDamageable>();
        protected DamageStruct damageStruct;
        protected Vector2 tempPos;
        protected Vector2 PointLeftTop;
        protected Vector2 PointRightBottom;
        protected Vector2 screenOffset = GameGlobal.ScreenOffset;
        protected bool xIn, yIn;

        public string PrefName {
            get {
                return gameObject.name;
            }
        }

        public virtual void ReturnToPoolRequest() {
            CCPooler.ReturnToPool(gameObject);
        }

        public abstract void Shot(DamageStruct damage, short projectileDamage = 0);

        public virtual void CheckBounds() {
            tempPos = tr.position;
            xIn = (tempPos.x >= PointLeftTop.x - screenOffset.x && tempPos.x <= PointRightBottom.x + screenOffset.x);
            yIn = (tempPos.y >= PointRightBottom.y - screenOffset.y && tempPos.y <= PointLeftTop.y + screenOffset.y);
            if (!(xIn && yIn)) { //Out of Screen
                ReturnToPoolRequest();
            }
        }

        protected abstract void CheckComponent();

        public void SetScreenBound() {
            PointLeftTop     = Camera.main.ScreenToWorldPoint(new Vector2(0f, Screen.height));
            PointRightBottom = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0f));
        }

        public virtual void SetAbility(PlayerAbilitySlot ability) { }

    }
}
