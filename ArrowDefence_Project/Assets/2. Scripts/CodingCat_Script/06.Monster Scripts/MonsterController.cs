namespace ActionCat {
    using UnityEngine;

    public class MonsterController : MonoBehaviour {
        MONSTERSTATE currentState = MONSTERSTATE.IDLE;

        public MONSTERSTATE GetCurrentState {
            get {
                return currentState;
            }
        }

        #region MACHINE

        protected void StateStart(MONSTERSTATE state) {
            currentState = state;

            switch (currentState) {
                case MONSTERSTATE.IDLE:   STATE_IDLE(STATEFLOW.ENTER);   break;
                case MONSTERSTATE.MOVE:   STATE_MOVE(STATEFLOW.ENTER);   break;
                case MONSTERSTATE.ATTACK: STATE_ATTACK(STATEFLOW.ENTER); break;
                case MONSTERSTATE.DEATH:  STATE_DEATH(STATEFLOW.ENTER);  break;
                default: break;
            }
        }

        public void ChangeState(MONSTERSTATE targetState) {
            switch (currentState) {
                case MONSTERSTATE.IDLE:   STATE_IDLE(STATEFLOW.EXIT);   break;
                case MONSTERSTATE.MOVE:   STATE_MOVE(STATEFLOW.EXIT);   break;
                case MONSTERSTATE.ATTACK: STATE_ATTACK(STATEFLOW.EXIT); break;
                case MONSTERSTATE.DEATH:  STATE_DEATH(STATEFLOW.EXIT);  break;
            }

            currentState = targetState;

            switch (currentState) {
                case MONSTERSTATE.IDLE:   STATE_IDLE(STATEFLOW.ENTER);   break;
                case MONSTERSTATE.MOVE:   STATE_MOVE(STATEFLOW.ENTER);   break;
                case MONSTERSTATE.ATTACK: STATE_ATTACK(STATEFLOW.ENTER); break;
                case MONSTERSTATE.DEATH:  STATE_DEATH(STATEFLOW.ENTER);  break;
            }
        }

        void UpdateState() {
            switch (currentState) {
                case MONSTERSTATE.IDLE:   STATE_IDLE(STATEFLOW.UPDATE);   break;
                case MONSTERSTATE.MOVE:   STATE_MOVE(STATEFLOW.UPDATE);   break;
                case MONSTERSTATE.ATTACK: STATE_ATTACK(STATEFLOW.UPDATE); break;
                case MONSTERSTATE.DEATH:  STATE_DEATH(STATEFLOW.UPDATE);  break;
            }
        }

        private void Update() {
            CommonUpdate();
            UpdateState();
        }

        void CommonUpdate() {

        }

        #endregion

        #region STATE

        protected virtual void STATE_IDLE(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  break;
                case STATEFLOW.UPDATE: break;
                case STATEFLOW.EXIT:   break;
                default: CatLog.ELog("this Flow is Not Implemented."); break;
            }
        }

        protected virtual void STATE_MOVE(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  break;
                case STATEFLOW.UPDATE: break;
                case STATEFLOW.EXIT:   break;
                default: CatLog.ELog("this Flow is Not Implemented."); break;
            }
        }

        protected virtual void STATE_DEATH(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  break;
                case STATEFLOW.UPDATE: break;
                case STATEFLOW.EXIT:   break;
                default: CatLog.ELog("this Flow is Not Implemented."); break;
            }
        }

        protected virtual void STATE_ATTACK(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  break;
                case STATEFLOW.UPDATE: break;
                case STATEFLOW.EXIT:   break;
                default: CatLog.ELog("this Flow is Not Implemented."); break;
            }
        }

        #endregion

        #region HIT

        public virtual void OnHit() {
            new System.NotImplementedException("OnHit function Not override on Controller.");
        }

        #endregion
    }
}
