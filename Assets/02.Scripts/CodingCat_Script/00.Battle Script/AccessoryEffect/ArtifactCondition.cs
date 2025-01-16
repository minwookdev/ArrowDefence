namespace ActionCat {

    public enum ARTCONDITION {
        NONE     = 0,
        TRIGGER  = 1,
        BUFF     = 2,
        DEBUFF   = 3,
        PASSIVE  = 4,
        SWITCH   = 5,
    }

    [System.Serializable]
    public abstract class ArtifactCondition {
        protected ARTCONDITION conditionType = ARTCONDITION.NONE;
        protected int maxStack       = 0;       // 최대 사용 횟수
        protected float maxCost      = 0f;      // 스택을 쌓기위한 시간 Or 몬스터 처치 수
        protected float maxCoolDownTime = 0f;   // 재-사용 하기위한 대기 시간

        public ARTCONDITION ConditionType {
            get {
                return conditionType;
            }
        }
        public int MaxStack {
            get => maxStack;
        }
        public float MaxCost {
            get => maxCost;
        }
        public float CoolDown {
            get => maxCoolDownTime;
        }

        public abstract void IncreaseCurrentCost();

        protected abstract void IncreaseStack();

        public virtual void DecreaseCoolTime() {
            throw new System.NotImplementedException();
        }

        public abstract void Clear();

        protected ArtifactCondition(int maxstack, float maxcost, float cooltime, ARTCONDITION type) {
            conditionType = type;
            maxStack      = maxstack;
            maxCost       = maxcost;
            maxCoolDownTime  = cooltime;
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

        ///사용변수 설명
        ///1. MaxStack            : 최대 사용횟수 스택 (모았다가 여러 번 사용하기 위함)
        ///2. MaxCost             : 1 스태킹 하기위한 비용
        ///3. MaxCoolDownTime     : 재-사용 시간
        ///4. Increase Cost Value : 몬스터 처치 시 증가하는 비용
        ///공용변수
        ///ConditionType = Trigger Type
    }

    public sealed class ArtCondition_Buff : ArtifactCondition { 
        public ArtCondition_Buff(float cooldownTime) : base(1, 0, cooldownTime, ARTCONDITION.BUFF) { }
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

        ///사용변수 설명
        ///1. MaxStack = 1        : 최대 사용횟수 스택 (모았다가 여러 번 사용하기 위함) [Buff_Type은 항상 1로 고정]
        ///2. MaxCoolDownTime     : 재-사용 대기 시간
        ///공용변수
        ///ConditionType = Buff Type
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

        ///사용변수 설명
        ///1. MaxStack            : 최대 사용횟수 스택 (모았다가 여러 번 사용하기 위함)
        ///2. MaxCost             : 1 스태킹 하기위한 비용 : Debuff Type의 경우 항상 Time임, Increase Cost를 할당하지 않음
        ///3. MaxCoolDownTime     : 재-사용 시간
        ///공용변수
        ///ConditionType = Debuff Type
    }

    public sealed class ArtCondition_Empty : ArtifactCondition {
        #region ES3
        public ArtCondition_Empty() : base(0, 0f, 0f, ARTCONDITION.NONE) { }
        #endregion

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
