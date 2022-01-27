namespace ActionCat.Auto {
    using UnityEngine;

    internal sealed class AutoMachine {
        AD_BowController controller = null;
        AUTOSTATE autoState;

        public void AutoUpdate() {
            switch (autoState) {
                case AUTOSTATE.WAIT: Wait(); break;
                case AUTOSTATE.FIND: Find(); break;
                case AUTOSTATE.TRAC: Trac(); break;
                case AUTOSTATE.SHOT: Shot(); break;
                default: throw new System.NotImplementedException();
            }
        }

        void Wait() {
            //Start AutoMode when BattleStart
            if(GameManager.Instance.GameState == GAMESTATE.STATE_INBATTLE) {
                autoState = AUTOSTATE.FIND;
            }
        }

        void Find() {

        }

        void Trac() {

        }

        void Shot() {

        }

        public AutoMachine(AD_BowController controller) {
            this.controller = controller;
        }

        #region MACHINE

        public void MachineUpdate() {
            switch (autoState) {
                case AUTOSTATE.WAIT: StateWait(STATEFLOW.UPDATE); break;
                case AUTOSTATE.FIND: StateFind(STATEFLOW.UPDATE); break;
                case AUTOSTATE.TRAC: StateTrac(STATEFLOW.UPDATE); break;
                case AUTOSTATE.SHOT: StateShot(STATEFLOW.UPDATE); break;
                default: throw new System.NotImplementedException();
            }
        }

        public void ChangeState(AUTOSTATE state) {
            switch (autoState) {
                case AUTOSTATE.WAIT: StateWait(STATEFLOW.EXIT); break;
                case AUTOSTATE.FIND: StateFind(STATEFLOW.EXIT); break;
                case AUTOSTATE.TRAC: StateTrac(STATEFLOW.EXIT); break;
                case AUTOSTATE.SHOT: StateShot(STATEFLOW.EXIT); break;
                default: throw new System.NotImplementedException();
            }

            autoState = state;

            switch (autoState) {
                case AUTOSTATE.WAIT: StateWait(STATEFLOW.ENTER); break;
                case AUTOSTATE.FIND: StateFind(STATEFLOW.ENTER); break;
                case AUTOSTATE.TRAC: StateTrac(STATEFLOW.ENTER); break;
                case AUTOSTATE.SHOT: StateShot(STATEFLOW.ENTER); break;
                default: throw new System.NotImplementedException();
            }
        }

        #endregion

        #region STATE

        void StateWait(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  break;
                case STATEFLOW.UPDATE: break;
                case STATEFLOW.EXIT:   break;
            }
        }

        void StateFind(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  break;
                case STATEFLOW.UPDATE: break;
                case STATEFLOW.EXIT:   break;
            }
        }

        void StateTrac(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  break;
                case STATEFLOW.UPDATE: break;
                case STATEFLOW.EXIT:   break;
            }
        }

        void StateShot(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  break;
                case STATEFLOW.UPDATE: break;
                case STATEFLOW.EXIT:   break;
            }
        }

        #endregion

        #region FLOW

        void WaitEnter() {

        }

        void WaitUpdate() {

        }

        void WaitExit() {

        }

        #endregion
    }
}
