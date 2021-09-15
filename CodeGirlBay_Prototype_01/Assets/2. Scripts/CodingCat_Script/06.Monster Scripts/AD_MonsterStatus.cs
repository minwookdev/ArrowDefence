namespace CodingCat_Games
{
    using UnityEngine;

    public class AD_MonsterStatus : MonoBehaviour, IPoolObject, IDamageable
    {
        [Header("MONSTER STATUS DATA")]
        public float MaxMonsterHP;
        public float MaxMonsterMP;

        private float monsterHp;
        private float monsterMp;

        private float dropCorrection = 5f;
        private float clearGaugeIncreaseValue = 10f;

        private void Update()
        {
            //if(GameManager.Instance.GameState == GAMESTATE.STATE_ENDBATTLE)
            //{
            //    //Battle이 끝남에 따라 자체적으로 비활성화 처리로직 들어가도 된다 
            //}
        }

        public void DisableObject_Req(GameObject target) => CCPooler.ReturnToPool(target);

        public void OnStageClear() => DisableObject_Req(this.gameObject);

        public void OnHitObject(float value)
        {
            monsterHp -= value;

            if(monsterHp <= 0)
            {
                BattleProgresser.OnIncreaseClearGauge(clearGaugeIncreaseValue);
                BattleProgresser.OnDropItemChance(dropCorrection);
                DisableObject_Req(this.gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_DISABLELINE))
            {
                DisableObject_Req(this.gameObject);
            }
        }

        private void OnCollisionEnter2D(Collision2D coll)
        {
            if (coll.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_ARROW))
            {
                float damageCount = Random.Range(70f, 130f + 1f);
                OnHitObject(damageCount);
            }
        }

        private void OnEnable()
        {
            this.monsterHp = MaxMonsterHP;
            this.monsterMp = MaxMonsterMP;
        }
    }
}
