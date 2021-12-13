namespace ActionCat {
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
    public class WalkerState : MonsterState {
        [Header("COMPONENET")]
        [SerializeField] CapsuleCollider2D coll;
        [SerializeField] Animator          anim;
        [SerializeField] Rigidbody2D       rigidBody;

        [Header("MOVEMENT")]
        public float InitMoveSpeed  = 0.5f;
        public float AttackInterval = 2f;

        //Animator Parameters
        int animState = Animator.StringToHash("state");
        int atkState  = Animator.StringToHash("attack");
        int hitParams = Animator.StringToHash("hit");
        int runParams = Animator.StringToHash("isRun");

        //Walker Properties
        float moveSpeed      = 0f;
        float attackTimer    = 0f;
        float attackInterval = 0f;
        bool isFindWall      = false;

        void ComponentInit() {
            if(anim == null) {
                anim = GetComponent<Animator>();
            }

            if(rigidBody == null) {
                rigidBody = GetComponent<Rigidbody2D>();
            }

            if(coll == null) {
                coll = GetComponent<CapsuleCollider2D>();
            }
        }

        private void Start() {
            ComponentInit();
            moveSpeed      = InitMoveSpeed;
            attackInterval = AttackInterval;
        }

        private void OnEnable() {
            ChangeState(STATETYPE.IDLE);
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if(collision.gameObject.layer == LayerMask.NameToLayer("AttackGuideLine")) {
                isFindWall = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision) {
            if(collision.gameObject.layer == LayerMask.NameToLayer("AttackGuideLine")) {
                isFindWall = false;
            }
        }

        #region OVERRIDE_STATE

        protected override void State_Idle(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  IdleStart();  break;
                case STATEFLOW.UPDATE: IdleUpdate(); break;
                case STATEFLOW.EXIT:                 break;
            }
        }

        protected override void State_Move(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  WalkStart();  break;
                case STATEFLOW.UPDATE: WalkUpdate(); break;
                case STATEFLOW.EXIT:   WalkExit();   break;
            }
        }

        protected override void State_Death(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER: DeathStart(); break;
                case STATEFLOW.UPDATE:              break;
                case STATEFLOW.EXIT:  DeathExit();  break;
            }
        }

        protected override void State_Attack(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  AttackStart();  break;
                case STATEFLOW.UPDATE: AttackUpdate(); break;
                case STATEFLOW.EXIT:   AttackExit();   break;
            }
        }

        #endregion

        #region ACTION

        void WalkStart() {
            anim.SetInteger(animState, 1);
            rigidBody.velocity = Vector2.down * moveSpeed;
        }

        void WalkUpdate() {
            if (isFindWall == true) {
                ChangeState(STATETYPE.ATTACK);
            }
        }

        void WalkExit() {
            rigidBody.velocity = Vector2.zero;
        }

        void IdleStart() {
            anim.SetInteger(animState, 0);
        }

        void IdleUpdate() {
            if (GameManager.Instance.GameState == GAMESTATE.STATE_INBATTLE) {
                ChangeState(STATETYPE.MOVE);
            }
        }

        void AttackStart() {
            //Set Animation on Idle
            anim.SetInteger(animState, 0);
        }

        void AttackUpdate() {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval) {
                anim.SetTrigger(atkState);
                attackTimer = 0f;
            }
        }

        void AttackExit() {
            attackTimer = 0f;
        }

        void DeathStart() {
            anim.SetInteger(animState, 3);
            coll.enabled = false;
        }

        void DeathExit() {
            coll.enabled = true;
        }

        #endregion

        #region ANIMATION_EVENT
        public override void OnHit() {
            anim.SetTrigger(hitParams);
        }
        #endregion
    }
}
