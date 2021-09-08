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

        public void DisableObject_Req(GameObject target) => CCPooler.ReturnToPool(target);

        public void OnHitObject(float value)
        {
            monsterHp -= value;

            if(monsterHp <= 0)
            {
                BattleProgresser.OnIncreaseClearGauge(10f);
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
