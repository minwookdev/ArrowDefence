namespace ActionCat {
    using UnityEngine;

    /// <summary>
    /// Generic Singleton Class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
        private static bool _shuttingdown = false; // Singleton Object가 파괴되었는지 확인하기 위한 변수
        private static object _lock = new object();
        private static T _instance;

        /// <summary>
        /// Access Singleton Instance through this Property
        /// </summary>
        public static T Instance {
            get {
                // gameObject.Destory() 메서드에서 사용 금지 사용한다면 IsExist를 통해서 체크
                if (_shuttingdown) {
                    Debug.Log("Singleton Instance '" + typeof(T) + "' is already Destroyed. Returning Null.");
                    return null;
                }

                lock (_lock) { //Thread Safe 
                    if (_instance == null) {
                        // 이미 생성된 Instance가 있는지 확인
                        _instance = (T)FindObjectOfType(typeof(T));

                        // 아직 생성되지 않았다면 인스턴스를 생성
                        if (_instance == null) {
                            //새로운 싱글턴 게임오브젝트를 만들어서 싱글턴화
                            var singletonObject = new GameObject();
                            _instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).ToString() + " [Singleton]";
                            _instance.GetComponent<Singleton<T>>().Init();

                            // 씬이 넘어감에 따라 파괴되지않도록 하는 처리
                            DontDestroyOnLoad(singletonObject);
                        }
                    }
                    return _instance;
                }
            }
        }
        /// <summary>
        /// Singleton<T>.Instnace를 거치지 않고 존재하는지 확인하기 위함. (Instance로 체크 시 무조건 생성하기 때문)
        /// </summary>
        public static bool IsExist => _instance != null && _shuttingdown == false; 

        /// <summary>
        /// Singleton Instance가 생성되고 바로 호출되는 가상함수. (대형 로직 작성 금지)
        /// </summary>
        protected virtual void Init() { }

        private void Awake() {
#if UNITY_EDITOR
            //InitMessage Output :: Unity Debug
            string initMsg = string.Format("{0} :: {1}", "Init Singleton Instance", "<color=yellow>" + this + "</color>");
            Debug.Log(initMsg);
#endif
        }

        private void OnApplicationQuit() {
            _shuttingdown = true;
        }

        private void OnDestroy() {
            _shuttingdown = true;
        }
    }
}
