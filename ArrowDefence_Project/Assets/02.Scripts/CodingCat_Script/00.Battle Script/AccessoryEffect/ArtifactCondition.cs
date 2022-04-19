namespace ActionCat {

    public enum ARTCONDITION {
        NONE     = 0,
        TRIGGER  = 1,
        BUFF     = 2,
        DEBUFF   = 3,
        PASSIVE  = 4,
    }

    [System.Serializable]
    public abstract class ArtifactCondition {
        protected ARTCONDITION conditionType = ARTCONDITION.NONE;
        protected int maxStack       = 0;
        protected int currentStack   = 0;
        protected float maxCost      = 0f;
        protected float currentCost  = 0f;
        protected float cooldownTime = 0f;

        public abstract void IncreaseCurrentCost();

        protected abstract void IncreaseStack();

        public abstract void Clear();

        protected ArtifactCondition(int maxstack, float maxcost, float cooltime, ARTCONDITION type) {
            conditionType = type;
            maxStack      = maxstack;
            maxCost       = maxcost;
            cooldownTime  = cooltime;
        }
        protected ArtifactCondition() { }
    }

    public sealed class ArtCondition_Trigger : ArtifactCondition {
        private float costIncreaseValue = 0f;

        public ArtCondition_Trigger(int maxStack, float maxCost, float cooldownTime, float costincreasevalue) : base(maxStack, maxCost, cooldownTime, ARTCONDITION.TRIGGER) {
            this.costIncreaseValue = costincreasevalue;
        }
        public ArtCondition_Trigger() { }

        public override void Clear() {
            throw new System.NotImplementedException();
        }

        public override void IncreaseCurrentCost() {
            throw new System.NotImplementedException();
        }

        protected override void IncreaseStack() {
            throw new System.NotImplementedException();
        }
    }

    public sealed class ArtCondition_Buff : ArtifactCondition { 
        //MaxCost = 1스택 모이는데 걸리는 시간

        public ArtCondition_Buff(float maxCost, float cooldownTime) : base(1, maxCost, cooldownTime, ARTCONDITION.BUFF) { }
        public ArtCondition_Buff() { }

        public override void Clear() {
            throw new System.NotImplementedException();
        }

        public override void IncreaseCurrentCost() {
            throw new System.NotImplementedException();
        }

        protected override void IncreaseStack() {
            throw new System.NotImplementedException();
        }
    }

    public sealed class ArtCondition_Debuff : ArtifactCondition {
        public ArtCondition_Debuff(int maxStack, float maxCost, float cooldownTime) : base(maxStack, maxCost, cooldownTime, ARTCONDITION.DEBUFF) { }
        public ArtCondition_Debuff() { }

        public override void Clear() {
            throw new System.NotImplementedException();
        }

        public override void IncreaseCurrentCost() {
            throw new System.NotImplementedException();
        }

        protected override void IncreaseStack() {
            throw new System.NotImplementedException();
        }
    }
}
