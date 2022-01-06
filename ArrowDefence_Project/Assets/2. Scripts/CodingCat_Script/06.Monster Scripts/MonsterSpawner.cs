namespace ActionCat
{
    using UnityEngine;
    using System;
    using System.Collections;

    public class MonsterSpawner : MonoBehaviour
    {
        [Header("STAGE DIFFICULTY")]
        [SerializeField] STAGEDIFF stageDiff = STAGEDIFF.EASY;

        [Header("SPAWN POINT")]
        public Vector3 TopLeft;
        public Vector3 BottomRight;

        [Header("MONSTER DATA")]
        [SerializeField] GameObject monsterPrefNormal;
        [SerializeField] GameObject monsterPrefFreq;
        [SerializeField] GameObject monsterPrefElite;

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
                case GAMESTATE.STATE_GAMEOVER:                     break;
            }
        }

        private void SpawnMonster() {
            spawnTimer -= Time.deltaTime;

            if(spawnTimer <= 0) {
                StartCoroutine(Spawn());
                spawnTimer = UnityEngine.Random.Range(spawnIntervalMin, spawnIntervalMax + 1);
            }
        }

        private IEnumerator Spawn()
        {
            //몬스터 소환해놓고 각종 변수 (위치, 회전 등) 설정해주고
            randomPosX = UnityEngine.Random.Range(topLeftPoint.x, bottomRightPoint.x);
            randomPosY = UnityEngine.Random.Range(bottomRightPoint.y, topLeftPoint.y);
            Vector3 spawnPos = new Vector3(randomPosX, randomPosY, 0f);

            yield return null;

            //Init-Type Monster Spawn -> 스폰 빈도, 난이도 등등 조건 조합해서 스폰식 구현해주기
            //현재는 Init된 Pref에 따라 스폰 몬스터 결정.
            switch (stageDiff) {
                case STAGEDIFF.EASY:   EasyDiffSpawn(spawnPos);   break;
                case STAGEDIFF.NORMAL: NormalDiffSpawn(spawnPos); break;
                case STAGEDIFF.HARD:   HardDiffSpawn(spawnPos);   break;
                case STAGEDIFF.WARFIELD: break;
                case STAGEDIFF.HELL:     break;
            }

            //여기서 CCPooler Spawn처리, 되고나서 컴포넌트 가져와서 뭐 할거있으면 해줌 [Origin]
            //CCPooler.SpawnFromPool(AD_Data.POOLTAG_MONSTER_NORMAL, spawnPos, Quaternion.identity);
        }

        private void MonsterInPool()
        {
            //Init-Monster in Object Pooler
            //Difficulty configuration according to cached monsters.

            if (monsterPrefNormal != null) {
                CCPooler.AddPoolList(AD_Data.POOLTAG_MONSTER_NORMAL, 10, monsterPrefNormal, iscount:false);
                SetStageDiff();
            }

            if(monsterPrefFreq != null) {
                CCPooler.AddPoolList(AD_Data.POOLTAG_MONSTER_FREQ, 10, monsterPrefFreq, iscount:false);
                SetStageDiff();
            }

            if(monsterPrefElite != null) {
                CCPooler.AddPoolList(AD_Data.POOLTAG_MONSTER_ELITE, 10, monsterPrefElite, iscount:false);
                SetStageDiff();
            }
        }

        void EasyDiffSpawn(Vector3 spawnpos) {
            CCPooler.SpawnFromPool(AD_Data.POOLTAG_MONSTER_NORMAL, spawnpos, Quaternion.identity);
        }

        void NormalDiffSpawn(Vector3 spawnpos) {
            switch (UnityEngine.Random.Range(0, 1 + 1)) {
                case 0: CCPooler.SpawnFromPool(AD_Data.POOLTAG_MONSTER_NORMAL, spawnpos, Quaternion.identity); break;
                case 1: CCPooler.SpawnFromPool(AD_Data.POOLTAG_MONSTER_FREQ, spawnpos, Quaternion.identity);   break;
            }
        }

        void HardDiffSpawn(Vector3 spawnpos) {
            switch (UnityEngine.Random.Range(0, 2 + 1)) {
                case 0: CCPooler.SpawnFromPool(AD_Data.POOLTAG_MONSTER_NORMAL, spawnpos, Quaternion.identity); break;
                case 1: CCPooler.SpawnFromPool(AD_Data.POOLTAG_MONSTER_FREQ, spawnpos, Quaternion.identity);   break;
                case 2: CCPooler.SpawnFromPool(AD_Data.POOLTAG_MONSTER_ELITE, spawnpos, Quaternion.identity);  break;
            }
        }

        void SetStageDiff() {
            stageDiff += 1;

            ///Example
            ///var countOfDiff = System.Enum.GetValues(typeof(STAGEDIFF)).Length;
            ///stageDiff += 1;
            ///if((int)stageDiff == countOfDiff) {
            ///    stageDiff = 0;
            ///}
            ///else...
        }

#if UNITY_EDITOR
        public void MonsterDebug(string monsterTag, Vector2 spawnPos)
        {
            CCPooler.SpawnFromPool(monsterTag, spawnPos, Quaternion.identity);
        }
#endif
    }
}
