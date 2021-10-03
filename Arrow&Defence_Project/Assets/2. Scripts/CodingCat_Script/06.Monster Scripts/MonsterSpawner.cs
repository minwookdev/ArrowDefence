namespace CodingCat_Games
{
    using UnityEngine;
    using System;
    using System.Collections;
    using CodingCat_Scripts;

    public class MonsterSpawner : MonoBehaviour
    {
        [Header("SPAWN POINT")]
        public Vector3 TopLeft;
        public Vector3 BottomRight;

        [Header("MONSTER DATA")]
        public GameObject monsterPref;

        [Header("SPAWN TIMER")]
        [ReadOnly] public float spawnTimer = 0f;

        private Vector3 topLeftPoint;
        private Vector3 bottomRightPoint;
        private float randomPosX;
        private float randomPosY;
        private int spawnIntervalMin;
        private int spawnIntervalMax;

        private IEnumerator Start()
        {
            //Init Monster Spawn Point
            topLeftPoint     = TopLeft;
            bottomRightPoint = BottomRight;

            //Spawn Timer Setting
            spawnIntervalMax = 2;
            spawnIntervalMin = 1;
            spawnTimer = 3f;

            //Waiting Object Pooler for Add Object in Pooler
            yield return new WaitUntil(() => CCPooler.IsInitialized);

            //Monster Object in Pooler
            MonsterInPool();
        }

        private void Update()
        {
            switch (GameManager.Instance.GameState)
            {
                case GAMESTATE.STATE_BEFOREBATTLE:                 break;
                case GAMESTATE.STATE_INBATTLE:     SpawnMonster(); break;
                case GAMESTATE.STATE_BOSSBATTLE:                   break;
                case GAMESTATE.STATE_ENDBATTLE:                    break;
                default:                                           break;
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                Action action = () => {
                    if (monsterPref != null)
                    {
                        
                    }
                }; action();
            }

        }

        private void SpawnMonster()
        {
            spawnTimer -= Time.deltaTime;

            if(spawnTimer <= 0)
            {
                StartCoroutine(Spawn());
                spawnTimer = UnityEngine.Random.Range(spawnIntervalMin, spawnIntervalMax + 1);
            }
        }

        private IEnumerator Spawn()
        {
            //몬스터 소환해놓고 각종 변수 (위치, 회전 등) 설정해주고
            randomPosX = UnityEngine.Random.Range(topLeftPoint.x, bottomRightPoint.x);
            randomPosY = UnityEngine.Random.Range(bottomRightPoint.y, topLeftPoint.y);
            Vector3 spawnPos = new Vector3(randomPosX, randomPosY, 90f);

            yield return null;

            //여기서 CCPooler Spawn처리, 되고나서 컴포넌트 가져와서 뭐 할거있으면 해줌
            CCPooler.SpawnFromPool(AD_Data.POOLTAG_MONSTER_NORMAL, spawnPos, Quaternion.identity);
        }

        private void MonsterInPool()
        {
            if (monsterPref != null)
                CCPooler.AddPoolList(AD_Data.POOLTAG_MONSTER_NORMAL, 10, monsterPref);
        }
    }
}
