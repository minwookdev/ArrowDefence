namespace ActionCat {
    using UnityEngine;
    using ActionCat.Interface;

    public abstract class ProjectilePref : MonoBehaviour, IPoolObject {
        [Header("PROJECTILE VALUE")]
        [SerializeField] [RangeEx(0, 500, 10)] protected short baseDamage = 10;
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

        public void DisableRequest() {
            CCPooler.ReturnToPool(gameObject);
        }

        public abstract void Shot(DamageStruct damage, short projectileDamage = 0);

        public virtual void CheckBounds() {
            tempPos = tr.position;
            xIn = (tempPos.x >= PointLeftTop.x - screenOffset.x && tempPos.x <= PointRightBottom.x + screenOffset.x);
            yIn = (tempPos.y >= PointRightBottom.y - screenOffset.y && tempPos.y <= PointLeftTop.y + screenOffset.y);
            if (!(xIn && yIn)) { //Out of Screen
                DisableRequest();
            }
        }

        protected abstract void CheckComponent();

        public void SetScreenLocation(Vector2 topleft, Vector2 rightbottom) {
            PointLeftTop     = topleft;
            PointRightBottom = rightbottom;
        }

        public abstract void SetProjectileValue(PlayerAbilitySlot ability);
    }
}
