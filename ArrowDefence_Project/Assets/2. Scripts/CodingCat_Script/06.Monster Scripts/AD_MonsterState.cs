namespace CodingCat_Games
{
    using CodingCat_Scripts;
    using UnityEngine;

    public class AD_MonsterState : MonoBehaviour
    {
        private Rigidbody2D rbody;

        public enum MONSTERSTATE
        {
            STATE_NONE,
            STATE_MOVE,
            STATE_ATTACK,
            STATE_DEATH
        }

        [Header("MONSTER STATE")]
        [ReadOnly] public MONSTERSTATE monsterState = MONSTERSTATE.STATE_NONE;

        [Header("MONSTER MOVEMENT")]
        public float MoveSpeedMin = .5f; 
        public float MoveSpeedMax = 1f;

        private Vector2 movePos;
        private float moveSpeed;

        private void Update()
        {
            switch (monsterState)
            {
                case MONSTERSTATE.STATE_NONE:   StateNone();   break;
                case MONSTERSTATE.STATE_MOVE:   StateMove();   break;
                case MONSTERSTATE.STATE_ATTACK: StateAttack(); break;
                case MONSTERSTATE.STATE_DEATH:  StateDeath();  break;
            }
        }

        private void FixedUpdate()
        {
            switch (monsterState)
            {
                case MONSTERSTATE.STATE_NONE:                    break;
                case MONSTERSTATE.STATE_MOVE: MonsterMovement(); break;
                case MONSTERSTATE.STATE_ATTACK:                  break;
                case MONSTERSTATE.STATE_DEATH:                   break;
            }
        }

        private void Start()
        {
            rbody     = GetComponent<Rigidbody2D>();
            movePos   = Vector2.down; 
            moveSpeed = Random.Range(MoveSpeedMin, MoveSpeedMax);

            monsterState = MONSTERSTATE.STATE_MOVE;
        }

        void StateNone()
        {

        }

        void StateMove()
        {
            
        }

        void StateAttack()
        {

        }

        void StateDeath()
        {

        }

        void MonsterMovement()
        {
            rbody.velocity = movePos * moveSpeed;
        }
    }
}
