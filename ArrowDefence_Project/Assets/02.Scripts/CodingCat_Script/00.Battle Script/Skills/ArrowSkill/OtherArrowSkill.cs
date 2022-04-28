namespace ActionCat {
    public class OtherArrowSkill { }

    //======================================================================================================================================================
    //======================================================================================================================================================
    public class EmptyTypeArrowSkill : ArrowSkill {
        float[] values;

        public override void Clear() {
            throw new System.NotImplementedException();
        }
        public override string GetDesc(string localizedString) {
            System.Collections.Generic.List<string> tempString = new System.Collections.Generic.List<string>();
            foreach (var value in values) {
                tempString.Add(value.ToString().GetColor(StringColor.GREEN));
            }
            return string.Format(localizedString, tempString.ToArray());
        }
        public EmptyTypeArrowSkill(SkillEntity_Empty entity) {
            this.values = entity.Values;
        }
        public EmptyTypeArrowSkill(EmptyTypeArrowSkill origin) {
            this.values = origin.values;
        }
        #region ES3
        public EmptyTypeArrowSkill() { }
        #endregion
    }
    //======================================================================================================================================================
    //======================================================================================================================================================
    public abstract class BuffTypeArrowSkill : ArrowSkill {
        public ARROWBUFFTYPE BuffType { get; private set; }
        public abstract float GetValue();
        protected BuffTypeArrowSkill(ARROWBUFFTYPE type) {
            BuffType = type;
        }
        public BuffTypeArrowSkill() { }
    }

    public class ElementalAmplification : BuffTypeArrowSkill {
        private float value = 1f;

        public override float GetValue() => value;
        public override string GetDesc(string localizedString) {
            return string.Format(localizedString, ((value * 100) - 100).ToString().GetColor(StringColor.GREEN));
        }
        #region NOT-USED
        public override void Clear() {
            throw new System.NotImplementedException();
        }

        #endregion
        #region ES3
        public ElementalAmplification() { }
        #endregion
        public ElementalAmplification(SkillEntity_ElementalAmp entity) : base(ARROWBUFFTYPE.ELEMENTALAMPLIFICATION) {
            var entityValue = entity.ElementalAmplificationValue;
            if (entityValue < 1f) {
                throw new System.Exception("This value cannot be assigned less than 1.");
            }
            this.value = entity.ElementalAmplificationValue;
        }
        public ElementalAmplification(ElementalAmplification origin) : base(ARROWBUFFTYPE.ELEMENTALAMPLIFICATION) {
            this.value = origin.value;
        }
    }

    public class PhysicalAmplification : BuffTypeArrowSkill {
        private float value = 1f;
        public override float GetValue() => value;
        public override string GetDesc(string localizedString) {
            return string.Format(localizedString, value.ToString());
        }
        #region NOT-USED
        public override void Clear() {
            throw new System.NotImplementedException();
        }

        #endregion
        #region ES3
        public PhysicalAmplification() { }
        #endregion
        public PhysicalAmplification(float value) : base(ARROWBUFFTYPE.PHYSICALAMPLIFICATION) {
            this.value = value;
        }
    }

    public enum ARROWBUFFTYPE {
        NONE,
        ELEMENTALAMPLIFICATION,
        PHYSICALAMPLIFICATION
    }
}
