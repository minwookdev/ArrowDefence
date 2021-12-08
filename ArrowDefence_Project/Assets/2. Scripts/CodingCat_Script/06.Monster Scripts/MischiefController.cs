namespace ActionCat {
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
    public class MischiefController : MonsterController {
        [Header("COMPONENT")]
        [SerializeField] Animator    anim;
        [SerializeField] Rigidbody2D rigid;

        [Header("MOVEMENT")]
        public float InitMoveSpeed = 0.5f;

        //Animator State
        int animState = Animator.StringToHash("state");

        //Monster Property
        float speed = 0f;

        void Start() {
            //Init-Property
            speed = InitMoveSpeed;
            StateStart(MONSTERSTATE.IDLE);
        }

        void OnEnable() {
            //State Start
            ChangeState(MONSTERSTATE.IDLE);
        }

        protected override void STATE_IDLE(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  IdleStart();  break;
                case STATEFLOW.UPDATE: IdleUpdate(); break;
                case STATEFLOW.EXIT:   break;
            }
        }

        protected override void STATE_MOVE(STATEFLOW flow) {
            switch (flow)
            {
                case STATEFLOW.ENTER:  WalkStart(); break;
                case STATEFLOW.UPDATE: break;
                case STATEFLOW.EXIT:   break;
            }
        }

        protected override void STATE_DEATH(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER: DeathStart(); break;
                case STATEFLOW.UPDATE: break;
                case STATEFLOW.EXIT:   break;
            }
        }

        void WalkStart() {
            anim.SetInteger(animState, 1);
            rigid.velocity = Vector2.down * speed;
        }

        void IdleStart() {
            anim.SetInteger(animState, 0);
            rigid.velocity = Vector2.zero;
        }

        void IdleUpdate() {
            if(GameManager.Instance.GameState == GAMESTATE.STATE_INBATTLE) {
                ChangeState(MONSTERSTATE.MOVE);
            }
        }

        void DeathStart() {
            anim.SetInteger(animState, 3);
            rigid.velocity = Vector2.zero;
        }
    }
}
