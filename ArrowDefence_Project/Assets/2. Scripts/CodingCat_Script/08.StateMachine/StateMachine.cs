namespace ActionCat {
    using System;
    using UnityEngine;

    public class StateMachine : MonoBehaviour {
        STATETYPE currentState = STATETYPE.IDLE;

        void NotImplementedState() {
            throw new NotImplementedException("This State is Not Implemented.");
        }

        #region MACHINE

        protected void ChangeState(STATETYPE targetState)
        {
            switch (currentState) {
                case STATETYPE.IDLE:   State_Idle(STATEFLOW.EXIT);   break;
                case STATETYPE.MOVE:   State_Move(STATEFLOW.EXIT);   break;
                case STATETYPE.ATTACK: State_Attack(STATEFLOW.EXIT); break;
                case STATETYPE.DEATH:  State_Death(STATEFLOW.EXIT);  break;
                default: NotImplementedState(); break;
            }

            currentState = targetState;

            switch (currentState) {
                case STATETYPE.IDLE:   State_Idle(STATEFLOW.ENTER);   break;
                case STATETYPE.MOVE:   State_Move(STATEFLOW.ENTER);   break;
                case STATETYPE.ATTACK: State_Attack(STATEFLOW.ENTER); break;
                case STATETYPE.DEATH:  State_Death(STATEFLOW.ENTER);  break;
                default: NotImplementedState(); break;
            }
        }

        protected void StartState(STATETYPE state) {
            currentState = state;

            switch (currentState) {
                case STATETYPE.IDLE:   State_Idle(STATEFLOW.ENTER);   break;
                case STATETYPE.MOVE:   State_Move(STATEFLOW.ENTER);   break;
                case STATETYPE.ATTACK: State_Attack(STATEFLOW.ENTER); break;
                case STATETYPE.DEATH:  State_Death(STATEFLOW.ENTER);  break;
                default: NotImplementedState(); break;
            }
        }
        
        void UpdateState() {
            switch (currentState) {
                case STATETYPE.IDLE:   State_Idle(STATEFLOW.UPDATE);   break;
                case STATETYPE.MOVE:   State_Move(STATEFLOW.UPDATE);   break;
                case STATETYPE.ATTACK: State_Attack(STATEFLOW.UPDATE); break;
                case STATETYPE.DEATH:  State_Death(STATEFLOW.UPDATE);  break;
                default: NotImplementedState(); break;
            }
        }

        protected void Update() {
            UpdateState();
            CommonUpdate();
        }

        void CommonUpdate() {

        }

        #endregion

        #region STATE

        protected virtual void State_Idle(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  break;
                case STATEFLOW.UPDATE: break;
                case STATEFLOW.EXIT:   break;
            }
        }

        protected virtual void State_Move(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  break;
                case STATEFLOW.UPDATE: break;
                case STATEFLOW.EXIT:   break;
            }
        }

        protected virtual void State_Death(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  break;
                case STATEFLOW.UPDATE: break;
                case STATEFLOW.EXIT:   break;
            }
        }

        protected virtual void State_Attack(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  break;
                case STATEFLOW.UPDATE: break;
                case STATEFLOW.EXIT:   break;
            }
        }

        #endregion
    }
}
