namespace ActionCat {

    public abstract class ProjectileType : ArrowSkill {
        public abstract void OnHit();
    }

    public class SplitArrow : ProjectileType {
        public override void Clear() {
            throw new System.NotImplementedException();
        }

        public override void OnHit() {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Copy Class Constructor
        /// </summary>
        /// <param name="splitArrow"></param>
        public SplitArrow(DataSplit data) {

        }

        public SplitArrow(SplitArrow origin) {

        }
        #region ES3
        public SplitArrow() { }
        ~SplitArrow() { }
        #endregion
    }

    public class SplitDagger : ProjectileType {
        public override void OnHit() {
            throw new System.NotImplementedException();
        }

        public override void Clear() {
            throw new System.NotImplementedException();
        }

        public SplitDagger(DataSplitDagger data) {

        }

        public SplitDagger(SplitDagger origin) {

        }
        #region ES3
        public SplitDagger() { }
        ~SplitDagger() { }
        #endregion
    }

    public class ElementalFire : ProjectileType {
        public override void OnHit() {
            throw new System.NotImplementedException();
        }
        public override void Clear() {
            throw new System.NotImplementedException();
        }

        public ElementalFire(DataEltalFire data) {

        }

        public ElementalFire(ElementalFire origin) {

        }
        #region ES3
        public ElementalFire() { }
        ~ElementalFire() { }
        #endregion
    }
}
