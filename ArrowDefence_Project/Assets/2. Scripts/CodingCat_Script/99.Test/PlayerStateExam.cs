namespace ActionCat.Experimantal
{
    using UnityEngine;

    public class PlayerStateExam : MonoBehaviour
    {
        private CharacterState stateTypeA;

        private BaseState stateTypeB;

        private void Start()
        {
            stateTypeA = new IdleState();
        }

        private void Update()
        {
            stateTypeA = stateTypeA.handleInput();

            if(stateTypeB != null) {
                stateTypeB.UpdateState();
            }
        }

        public void ChangeState(BaseState targetState) {
            if(stateTypeB != null) {
                stateTypeB.DestroyState();
            }

            stateTypeB = targetState;

            if(stateTypeB != null) {
                stateTypeB.owner = this;
                stateTypeB.PreparedState();
            }
        }
    }

    #region Type_A

    class CharacterState
    {
        public virtual CharacterState handleInput()
        {
            return this;
        }
    }

    class IdleState : CharacterState
    {
        public override CharacterState handleInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                return new JumpingState();
            }

            return this;
        }
    }

    class JumpingState : CharacterState
    {
        public override CharacterState handleInput()
        {
            return this;
        }
    }

    #endregion

    #region TYPE_B

    ///Possibility of writing as an interface

    public abstract class BaseState {
        //Reference to our State Machine
        public PlayerStateExam owner;

        /// <summary>
        /// Start
        /// </summary>
        public virtual void PreparedState() {

        }

        /// <summary>
        /// Update
        /// </summary>
        public virtual void UpdateState() {

        }

        /// <summary>
        /// OnDestroy
        /// </summary>
        public virtual void DestroyState() {

        }
    }

    public class WaitState : BaseState
    {
        public override void PreparedState() {
            base.PreparedState();
        }

        public override void UpdateState() {
            base.UpdateState();

            if(Input.GetKeyDown(KeyCode.LeftArrow)) {
                owner.ChangeState(new WalkState());
                //Write Movement Logic..
            }
        }

        public override void DestroyState() {
            base.DestroyState();
        }
    }

    public class WalkState : BaseState {
        public override void PreparedState() {
            base.PreparedState();
        }

        public override void UpdateState() {
            base.UpdateState();
        }

        public override void DestroyState() {
            base.DestroyState();
        }
    }

    #endregion
}



