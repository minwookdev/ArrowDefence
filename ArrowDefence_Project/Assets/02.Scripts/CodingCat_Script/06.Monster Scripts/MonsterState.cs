﻿namespace ActionCat
{
    using UnityEngine;
    using System;

    public class MonsterState : MonoBehaviour
    {
        [Header("MONSTER STATE")]
        [SerializeField][ReadOnly] protected STATETYPE currentState = STATETYPE.IDLE;
        [SerializeField][ReadOnly] protected float defaultActionSpeed = 1f;
        [SerializeField][ReadOnly] protected float currentActionSpeed = 1f;

        void NotImplementedState()
        {
            throw new NotImplementedException("This State is Not Implemented.");
        }

        protected virtual void SetStateValue() => throw new System.NotImplementedException();

        public virtual void BreakState() => throw new NotImplementedException();

        public virtual void ValActionSpeed(float ratio, float duration) => throw new NotImplementedException();

        public virtual void PlayHitSound() => throw new System.NotImplementedException();

        public void StateChanger(STATETYPE target)
        {
            ChangeState(target);
        }

        public virtual void OnHit()
        {
            throw new NotImplementedException("OnHit function Not override on Controller.");
        }

        /// <summary>
        /// Use Get Message when Clear Battle
        /// </summary>
        public void SetStateDeath()
        {
            if (currentState == STATETYPE.DEATH) return;
            StateChanger(STATETYPE.DEATH);
        }

        #region MACHINE

        protected void ChangeState(STATETYPE targetState)
        {
            switch (currentState)
            {
                case STATETYPE.IDLE:   State_Idle(STATEFLOW.EXIT);   break;
                case STATETYPE.MOVE:   State_Move(STATEFLOW.EXIT);   break;
                case STATETYPE.ATTACK: State_Attack(STATEFLOW.EXIT); break;
                case STATETYPE.DEATH:  State_Death(STATEFLOW.EXIT);  break;
                default: StateNotImplementedMessage();               break;
            }

            currentState = targetState;

            switch (currentState)
            {
                case STATETYPE.IDLE:   State_Idle(STATEFLOW.ENTER);   break;
                case STATETYPE.MOVE:   State_Move(STATEFLOW.ENTER);   break;
                case STATETYPE.ATTACK: State_Attack(STATEFLOW.ENTER); break;
                case STATETYPE.DEATH:  State_Death(STATEFLOW.ENTER);  break;
                default: StateNotImplementedMessage();                break;
            }

            void StateNotImplementedMessage()
            {

            }
        }

        protected void StartState(STATETYPE state)
        {
            currentState = state;

            switch (currentState)
            {
                case STATETYPE.IDLE: State_Idle(STATEFLOW.ENTER); break;
                case STATETYPE.MOVE: State_Move(STATEFLOW.ENTER); break;
                case STATETYPE.ATTACK: State_Attack(STATEFLOW.ENTER); break;
                case STATETYPE.DEATH: State_Death(STATEFLOW.ENTER); break;
                default: NotImplementedState(); break;
            }
        }

        protected void Update()
        {
            UpdateState();
            CommonUpdate();
        }

        void UpdateState()
        {
            switch (currentState)
            {
                case STATETYPE.IDLE:   State_Idle(STATEFLOW.UPDATE);   break;
                case STATETYPE.MOVE:   State_Move(STATEFLOW.UPDATE);   break;
                case STATETYPE.ATTACK: State_Attack(STATEFLOW.UPDATE); break;
                case STATETYPE.DEATH:  State_Death(STATEFLOW.UPDATE);  break;
                default: NotImplementedState();                        break;
            }
        }

        void CommonUpdate()
        {

        }

        #endregion

        #region STATE

        protected virtual void State_Idle(STATEFLOW flow)
        {
            switch (flow)
            {
                case STATEFLOW.ENTER: break;
                case STATEFLOW.UPDATE: break;
                case STATEFLOW.EXIT: break;
            }
        }

        protected virtual void State_Move(STATEFLOW flow)
        {
            switch (flow)
            {
                case STATEFLOW.ENTER: break;
                case STATEFLOW.UPDATE: break;
                case STATEFLOW.EXIT: break;
            }
        }

        protected virtual void State_Death(STATEFLOW flow)
        {
            switch (flow)
            {
                case STATEFLOW.ENTER: break;
                case STATEFLOW.UPDATE: break;
                case STATEFLOW.EXIT: break;
            }
        }

        protected virtual void State_Attack(STATEFLOW flow)
        {
            switch (flow)
            {
                case STATEFLOW.ENTER: break;
                case STATEFLOW.UPDATE: break;
                case STATEFLOW.EXIT: break;
            }
        }

        #endregion
    }
}
