namespace ActionCat
{
    using UnityEngine;
    using System.Collections;

    public class MonsterSpawner : MonoBehaviour {
        [Header("STAGE DIFFICULTY")]
        [SerializeField] STAGEDIFF stageDiff = STAGEDIFF.EASY;

        [Header("SPAWN POINT")]
        [SerializeField] Transform[] spawnPoints = null;
        public Vector3 TopLeft;
        public Vector3 BottomRight;

        [Header("MONSTER DATA")]
        [SerializeField] GameObject monsterPrefNormal;
        [SerializeField] GameObject monsterPrefFreq;
        [SerializeField] GameObject monsterPrefElite;

        [Header("SPAWN TIMER")]
        [ReadOnly] public float spawnTimer = 0f;
        [SerializeField] [ReadOnly] float currentSpawnTime  = 0f;
        [SerializeField] [ReadOnly] float spawnIntervalTime = 0f;
        [SerializeField] [ReadOnly] int currentSpawnStack   = 0;
        [SerializeField] [ReadOnly] int spawnStackDest      = 0;

        [Header("DEBUG")]
        [SerializeField] bool isNotSpawnMonster   = false;
        [SerializeField] bool isCheckAliveMonster = false;
        [SerializeField] float checkerInterval = 2f;
        private float checkerCount = 2f;

        private Vector3 topLeftPoint;
        private Vector3 bottomRightPoint;
        private float randomPosX;
        private float randomPosY;
        private int spawnIntervalMin;
        private int spawnIntervalMax;

        private IEnumerator Start() {
            //Init Monster Spawn Point
            topLeftPoint     = TopLeft;
            bottomRightPoint = BottomRight;

            //Spawn Timer Setting
            spawnIntervalMax = 2;
            spawnIntervalMin = 1;
            spawnTimer = 3f;

            currentSpawnTime = GetRandomInterval();

            //Waiting Object Pooler for Add Object in Pooler
            yield return new WaitUntil(() => CCPooler.IsInitialized);

            //Monster Object in Pooler
            MonsterInPool();
        }

        private void Update() {
            switch (GameManager.Instance.GameState) {
                case GAMESTATE.STATE_BEFOREBATTLE:                 break;
                case GAMESTATE.STATE_INBATTLE:     SpawnMonster(); break;
                case GAMESTATE.STATE_BOSSBATTLE:                   break;
                case GAMESTATE.STATE_ENDBATTLE:                    break;
                case GAMESTATE.STATE_GAMEOVER:                     break;
            }

            //============================[ TEST : ALIVE MONSTER CHECKER ]============================
            UpdateAliveMonsterChecker();
            //========================================================================================
        }

        private void SpawnMonster() {
            if (isNotSpawnMonster == true) return;

            spawnTimer -= Time.deltaTime;

            if(spawnTimer <= 0) {
                StartCoroutine(Spawn());
                spawnTimer = UnityEngine.Random.Range(spawnIntervalMin, spawnIntervalMax + 1);
            }
        }

        void UpdateSpawner() {
            //Decrease Spawn Interval Time
            currentSpawnTime -= Time.deltaTime;

            if (currentSpawnTime <= 0f) {
                currentSpawnStack++;
                if (currentSpawnStack > spawnStackDest) {
                    SpawnGroup();
                    spawnStackDest = GetRandomStack();
                }
                else {
                    SpawnNormal();
                }
                currentSpawnTime = GetRandomInterval();
            }
        }

        void SpawnNormal() {

        }

        void SpawnGroup() {

        }

        float GetRandomInterval() {
            switch (stageDiff) {
                case STAGEDIFF.NONE: return RandomEx.RangeFloat(1f, 2f);
                case STAGEDIFF.EASY: return RandomEx.RangeFloat(2f, 3f);
                case STAGEDIFF.NOML: return RandomEx.RangeFloat(1f, 2f);
                case STAGEDIFF.HARD: return RandomEx.RangeFloat(1f, 1.5f);
                case STAGEDIFF.WARF: return RandomEx.RangeFloat(0.7f, 1.2f);
                case STAGEDIFF.HELL: return RandomEx.RangeFloat(0.5f, 1f);
                default: throw new System.NotImplementedException();
            }
        }

        int GetRandomStack() {
            switch (stageDiff) {
                case STAGEDIFF.NONE: return RandomEx.RangeInt(3, 4);
                case STAGEDIFF.EASY: return RandomEx.RangeInt(4, 4);
                case STAGEDIFF.NOML: return RandomEx.RangeInt(3, 4);
                case STAGEDIFF.HARD: return RandomEx.RangeInt(2, 3);
                case STAGEDIFF.WARF: return RandomEx.RangeInt(2, 3);
                case STAGEDIFF.HELL: return RandomEx.RangeInt(1, 2);
                default: throw new System.NotImplementedException();
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
                case STAGEDIFF.NOML: NormalDiffSpawn(spawnPos); break;
                case STAGEDIFF.HARD:   HardDiffSpawn(spawnPos);   break;
                case STAGEDIFF.WARF: break;
                case STAGEDIFF.HELL:     break;
            }

            //여기서 CCPooler Spawn처리, 되고나서 컴포넌트 가져와서 뭐 할거있으면 해줌 [Origin]
            //CCPooler.SpawnFromPool(AD_Data.POOLTAG_MONSTER_NORMAL, spawnPos, Quaternion.identity);
        }

        private void MonsterInPool() {
            //Init-Monster in Object Pooler
            //Difficulty configuration according to cached monsters.

            if (monsterPrefNormal != null) {
                CCPooler.AddPoolList(AD_Data.POOLTAG_MONSTER_NORMAL, 10, monsterPrefNormal, isTracking:true);
                SetStageDiff();
            }

            if(monsterPrefFreq != null) {
                CCPooler.AddPoolList(AD_Data.POOLTAG_MONSTER_FREQ, 10, monsterPrefFreq, isTracking:true);
                SetStageDiff();
            }

            if(monsterPrefElite != null) {
                CCPooler.AddPoolList(AD_Data.POOLTAG_MONSTER_ELITE, 10, monsterPrefElite, isTracking:false);
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

        void UpdateAliveMonsterChecker() {
            if (isCheckAliveMonster == false) return;

            checkerCount -= Time.unscaledDeltaTime;
            if(checkerCount <= 0f) {
                var tempList = CCPooler.GetAliveMonsters();
                CatLog.Log(StringColor.YELLOW, $"Current Alive Monster Checker Count : {tempList.Length}");
                checkerCount = checkerInterval;
            }
        }

#if UNITY_EDITOR
        public void MonsterDebug(string monsterTag, Vector2 spawnPos)
        {
            CCPooler.SpawnFromPool(monsterTag, spawnPos, Quaternion.identity);
        }
#endif
    }
}
