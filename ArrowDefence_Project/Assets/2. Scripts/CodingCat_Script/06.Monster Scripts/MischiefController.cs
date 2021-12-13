namespace ActionCat {
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
    public class MischiefController : MonsterController {
        [Header("COMPONENT")]
        [SerializeField] Animator    anim;
        [SerializeField] Rigidbody2D rigid;
        [SerializeField] CapsuleCollider2D coll;

        [Header("MOVEMENT")]
        public float InitMoveSpeed  = 0.5f;
        public float AttackInterval = 2f;

        //Animator Parameters
        int animState = Animator.StringToHash("state");
        int atkState  = Animator.StringToHash("attack");
        int hitParams = Animator.StringToHash("hit");
        int runParams = Animator.StringToHash("isRun");

        //Monster Property
        float speed = 0f;
        float attackTimer    = 0f;
        float attackInterval = 0f;
        bool isFindWall = false;

        void Start() {
            //Init-Component
            ComponentInit();

            //Init-Property
            speed          = InitMoveSpeed; 
            attackInterval = AttackInterval;
            StateStart(MONSTERSTATE.IDLE);
        }

        void OnEnable() {
            //State Start
            ChangeState(MONSTERSTATE.IDLE);
        }

        private void OnTriggerEnter2D(Collider2D coll) {
            if(coll.gameObject.layer == LayerMask.NameToLayer("AttackGuideLine")) {
                isFindWall = true;
            }
        }

        private void OnTriggerExit2D(Collider2D coll) {
            if(coll.gameObject.layer == LayerMask.NameToLayer("AttackGuideLine")) {
                isFindWall = false;
            }
        }

        void ComponentInit() {
            if(anim == null) {
                CatLog.ELog("Animator Controller Not Caching.");
            }
            if(rigid == null) {
                rigid = GetComponent<Rigidbody2D>();
            }
            if(coll == null) {
                coll = GetComponent<CapsuleCollider2D>();
            }
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
                case STATEFLOW.ENTER:  WalkStart();  break;
                case STATEFLOW.UPDATE: WalkUpdate(); break;
                case STATEFLOW.EXIT:   WalkExit();   break;
            }
        }

        protected override void STATE_DEATH(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER: DeathStart(); break;
                case STATEFLOW.UPDATE:              break;
                case STATEFLOW.EXIT:  DeathExit();  break;
            }
        }

        protected override void STATE_ATTACK(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  AttackStart();  break;
                case STATEFLOW.UPDATE: AttackUpdate(); break;
                case STATEFLOW.EXIT:   AttackExit();   break;
            }
        }

        void WalkStart() {
            anim.SetInteger(animState, 1);
            rigid.velocity = Vector2.down * speed;
        }

        void WalkUpdate() {
            if(isFindWall == true) {
                ChangeState(MONSTERSTATE.ATTACK);
            }
        }

        void WalkExit() {
            rigid.velocity = Vector2.zero;
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

        void AttackStart() {
            //Set Animation on Idle
            anim.SetInteger(animState, 0);
        }

        void AttackUpdate() {
            attackTimer += Time.deltaTime;
            if(attackTimer >= attackInterval) {
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

        #region ANIMATION_EVENT
        public void WallAttack() {
            //Damage On Player Health Point !!!
        }

        public override void OnHit() {
            anim.SetTrigger(hitParams);
        }
        #endregion
    }
}
