namespace ActionCat {
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class MonsterSpawner : MonoBehaviour {
        [Header("STAGE DIFFICULTY")]
        [SerializeField] STAGEDIFF stageDiff = STAGEDIFF.EASY;

        [Header("SPAWN POINT")]
        [SerializeField] Transform[] spawnPointsTr = null;

        [Header("MONSTER DATA")]
        [SerializeField] [Tooltip("[0: Normal] [1: Freq] [2: Elite] [3: EMPTY] [4: EMPTY]")] GameObject[] groundMobPrefabs = null;
        [SerializeField] [Tooltip("[0: Normal] [1: Freq] [2: Elite] [3. EMPTY] [4: EMPTY]")] GameObject[] flyingMobPrefabs = null;
        [SerializeField] [ReadOnly] [Tooltip("DO NOT MODIFY THIS FIELD")] string[] groundMobTagArray;
        [SerializeField] [ReadOnly] [Tooltip("DO NOT MODIFY THIS FIELD")] string[] flyingMobTagArray;
        [SerializeField] [ReadOnly] [Tooltip("DO NOT MODIFY THIS FIELD")] List<SpawnMonster> spawnMonsterList = null;
        [SerializeField] bool isStaticGroupSpawn = false;

        [Header("SPAWN TIMER")]
        [SerializeField] [ReadOnly] float currentSpawnTime = 0f;
        [SerializeField] [ReadOnly] int currentSpawnStack  = 0;

        [Header("DEBUG")]
        [SerializeField] bool isCheckAliveMonster = false;
        [SerializeField] float checkerInterval = 2f;
        private float checkerCount = 2f;

        private Coroutine spawnCoroutine = null;
        private WaitForSeconds waitSpawnInterval = new WaitForSeconds(1.75f);
        private int[] sidePositionArray = new int[2] { -1, 1 };
        private int[] spawnTrOddNumbers;
        private short[] formationChances;

        private float totalSpawnChance = 0f;

        void InitializeMonsters(out string[] spawnStringArray) {
            var groundMonsterTagList = new List<string>();
            string tempString;
            //============================================ [ SPAWN GROUND MONSTER ] ============================================
            for (int i = 0; i < groundMobPrefabs.Length; i++) {
                if(groundMobPrefabs[i] == null) {
                    groundMonsterTagList.Add(""); //Add Empty String
                    continue;
                }

                switch (i) {
                    case 0: tempString = AD_Data.POOLTAG_MONSTER_NORMAL; break;
                    case 1: tempString = AD_Data.POOLTAG_MONSTER_FREQ;   break;
                    case 2: tempString = AD_Data.POOLTAG_MONSTER_ELITE;  break;
                    default: throw new System.NotImplementedException("Over Range Monster Prefab.");
                }

                CCPooler.AddPoolList(tempString, 10, groundMobPrefabs[i], isTracking: true);
                groundMonsterTagList.Add(tempString);
            }
            if(groundMonsterTagList.Count > 0) {
                spawnStringArray = groundMonsterTagList.ToArray();
            }
            else {
                throw new System.Exception("Monster Prefab Array is Empty.");
            }
            //============================================ [ SPAWN FLYING MONSTER ] ============================================
            for (int i = 0; i < flyingMobPrefabs.Length; i++) {
                if(flyingMobPrefabs[i] == null) {
                    continue;
                }
            }
            //==================================================================================================================
        }

        private IEnumerator Start() {
            //Get Platoon Spawn Position Array: Single Number
            if (spawnPointsTr == null || spawnPointsTr.Length < 7) {
                throw new System.Exception("Spawn Transform Arrays Not Completely Caching.");
            }
            var tempList = new List<int>();
            for (int i = 0; i < spawnPointsTr.Length; i++) {
                if (i % 2 == 1) {
                    tempList.Add(i);
                }
            }
            spawnTrOddNumbers = tempList.ToArray();
            currentSpawnTime  = GetRandomInterval();
            currentSpawnStack = GetRandomStack();

            //Waiting Object Pooler for Add Object in Pooler
            yield return new WaitUntil(() => CCPooler.IsInitialized);

            InitializeMonsters(out string[] tagsArray);
            float[] spawnChances = GetGroundUnitSpawnChances();

            spawnMonsterList = new List<SpawnMonster>();
            for (int i = 0; i < tagsArray.Length; i++) {
                if (!string.IsNullOrEmpty(tagsArray[i])) spawnMonsterList.Add(new SpawnMonster(tagsArray[i], spawnChances[i]));
                else                                     spawnMonsterList.Add(new SpawnMonster(tagsArray[i], 0f));
            }
            totalSpawnChance = GetTotalSpawnChance();
            formationChances = GetGroupSpawnChances();
        }

        #region GETTER || SETTER

        string GetRandomMonsterTag() {
            float randomPoint = Random.value * totalSpawnChance;
            for (int i = 0; i < spawnMonsterList.Count; i++) {
                if(randomPoint < spawnMonsterList[i].SpawnChance) {
                    return spawnMonsterList[i].SpawnString;
                }
                else {
                    randomPoint -= spawnMonsterList[i].SpawnChance;
                }
            }

            //if the Random.value is One, Not return String..
            //Return Last Index Element
            for (int i = spawnMonsterList.Count; i >= 0; --i) {
                if(!spawnMonsterList[i].IsEmpty()) {
                    return spawnMonsterList[i].SpawnString;
                }
            }

            throw new System.Exception();
        }

        float[] GetGroundUnitSpawnChances() {
            switch (stageDiff) {
                case STAGEDIFF.NONE: return new float[3] { 100f, 35f, 15f };
                case STAGEDIFF.EASY: return new float[3] { 100f, 30f, 10f };
                case STAGEDIFF.NOML: return new float[3] { 100f, 35f, 15f };
                case STAGEDIFF.HARD: return new float[3] { 100f, 40f, 20f };
                case STAGEDIFF.WARF: return new float[3] { 100f, 50f, 25f };
                case STAGEDIFF.HELL: return new float[3] { 100f, 60f, 30f };
                default: throw new System.NotImplementedException();
            }
        }

        float GetTotalSpawnChance() {
            float totalChance = 0f;
            foreach (var element in spawnMonsterList) {
                totalChance += element.SpawnChance;
            }
            return totalChance;
        }

        float GetRandomInterval() {
            switch (stageDiff) {
                case STAGEDIFF.NONE: return RandomEx.RangeFloat(2f, 2.5f);
                case STAGEDIFF.EASY: return RandomEx.RangeFloat(2.5f, 3.5f);
                case STAGEDIFF.NOML: return RandomEx.RangeFloat(2.5f, 3f);
                case STAGEDIFF.HARD: return RandomEx.RangeFloat(2f, 2.5f);
                case STAGEDIFF.WARF: return RandomEx.RangeFloat(1.75f, 2.25f);
                case STAGEDIFF.HELL: return RandomEx.RangeFloat(1.5f, 2f);
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

        short[] GetGroupSpawnChances() {
            switch (stageDiff) {
                case STAGEDIFF.NONE: return new short[3] { 100, 40, 20 };
                case STAGEDIFF.EASY: return new short[3] { 100, 30, 10 };
                case STAGEDIFF.NOML: return new short[3] { 100, 40, 20 };
                case STAGEDIFF.HARD: return new short[3] { 100, 50, 30 };
                case STAGEDIFF.WARF: return new short[3] { 100, 60, 40 };
                case STAGEDIFF.HELL: return new short[3] { 100, 70, 50 };
                default: throw new System.NotImplementedException();
            }
        }

        int GetRandomFormationNumber() {
            short totalChances = 0;
            foreach (var element in formationChances) {
                totalChances += element;
            }

            float randomPoint = Random.value * totalChances;
            for (int i = 0; i < formationChances.Length; i++) {
                if (randomPoint < formationChances[i]) {
                    return i;
                }
                else {
                    randomPoint -= formationChances[i];
                }
            }

            return formationChances.Length - 1;
        }

        #endregion

        private void Update() {
            switch (GameManager.Instance.GameState) {
                case GAMESTATE.STATE_BEFOREBATTLE:                    break;
                case GAMESTATE.STATE_INBATTLE:     UpdateSpawner();   break;
                case GAMESTATE.STATE_BOSSBATTLE:                      break;
                case GAMESTATE.STATE_ENDBATTLE:    UpdateGameClear(); break;
                case GAMESTATE.STATE_GAMEOVER:     UpdateGameOver();  break;
            }

            //============================ [ TEST : ALIVE MONSTER CHECKER ] ============================
            UpdateAliveMonsterChecker();
            //==========================================================================================
        }

        void UpdateSpawner() {
            //Decrease Spawn Interval Time
            currentSpawnTime -= Time.deltaTime;

            if (currentSpawnTime <= 0f) {
                currentSpawnStack--;
                if (currentSpawnStack < 0) {
                    spawnCoroutine    = StartCoroutine(SpawnGroupCo());
                    currentSpawnStack = GetRandomStack();
                }
                else {
                    SpawnSingle();
                }
                currentSpawnTime = GetRandomInterval();
            }
        }

        void SpawnSingle() {
            CCPooler.SpawnFromPool(GetRandomMonsterTag(), spawnPointsTr.GetRandom().position, Quaternion.identity);
        }

        #region SPAWNER_GROUP

        IEnumerator SpawnGroupCo() {
            var formationNumber = GetRandomFormationNumber();
            switch (formationNumber) {
                case 0: yield return StartCoroutine(SpawnSquad());   break;
                case 1: yield return StartCoroutine(SpawnPlatoon()); break;
                case 2: yield return StartCoroutine(SpawnCompany()); break;
            }
            //현재 난이도에 따라서 많은 group을 spawn할 확률 증가..
        }

        IEnumerator SpawnSquad() {
            Vector3 randomSpawnPosition = spawnPointsTr.GetRandom().position;
            string monsterSpawnTag = GetRandomMonsterTag();
            for (int i = 0; i < 3; i++) {
                CCPooler.SpawnFromPool(monsterSpawnTag, randomSpawnPosition, Quaternion.identity);
                if (!isStaticGroupSpawn) {
                    monsterSpawnTag = GetRandomMonsterTag();
                }
                yield return waitSpawnInterval;
            }
        }

        IEnumerator SpawnPlatoon() {
            var randomSpawnPosNumber = spawnTrOddNumbers.GetRandom();
            var sideNumber = randomSpawnPosNumber + sidePositionArray.GetRandom();
            byte currentSpawnedCount = 0;
            string monsterTag = GetRandomMonsterTag();
            while (currentSpawnedCount <= 4) {
                if(currentSpawnedCount % 2 == 0 && currentSpawnedCount != 0) {
                    yield return waitSpawnInterval;
                }

                CCPooler.SpawnFromPool(GetRandomMonsterTag(), spawnPointsTr[randomSpawnPosNumber].position, Quaternion.identity);
                CCPooler.SpawnFromPool(GetRandomMonsterTag(), spawnPointsTr[sideNumber].position, Quaternion.identity);

                if (!isStaticGroupSpawn) {
                    monsterTag = GetRandomMonsterTag();
                }

                currentSpawnedCount += 4;
            }
        }

        IEnumerator SpawnCompany() {
            string monsterTag = GetRandomMonsterTag();
            for (int i = 0; i < spawnPointsTr.Length; i++) {
                CCPooler.SpawnFromPool(GetRandomMonsterTag(), spawnPointsTr[i].position, Quaternion.identity);
                if (!isStaticGroupSpawn) {
                    monsterTag = GetRandomMonsterTag();
                }
            }

            yield return null;
        }

        #endregion

        void UpdateAliveMonsterChecker() {
            if (isCheckAliveMonster == false) return;

            checkerCount -= Time.unscaledDeltaTime;
            if(checkerCount <= 0f) {
                var tempList = CCPooler.GetAliveMonsters();
                CatLog.Log(StringColor.YELLOW, $"Current Alive Monster Checker Count : {tempList.Length}");
                checkerCount = checkerInterval;
            }
        }

        void UpdateGameOver() {
            if(spawnCoroutine != null) {
                StopCoroutine(spawnCoroutine);
            }
        }

        void UpdateGameClear() {
            if (spawnCoroutine != null) {
                StopCoroutine(spawnCoroutine);
            }
        }

#if UNITY_EDITOR
        public void MonsterDebug(string monsterTag, Vector2 spawnPos) {
            CCPooler.SpawnFromPool(monsterTag, spawnPos, Quaternion.identity);
        }
#endif

        internal sealed class SpawnMonster {
            public GameObject MonsterPref { get; private set; }
            public string SpawnString { get; private set; }
            public float SpawnChance { get; private set; }

            internal SpawnMonster(string tag, float spawnchance) {
                SpawnString = tag;
                SpawnChance = spawnchance;
            }

            public bool IsEmpty() {
                return string.IsNullOrEmpty(SpawnString);
            }
        }
    }
}
