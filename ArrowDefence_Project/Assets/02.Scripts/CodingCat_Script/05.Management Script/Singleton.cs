namespace ActionCat
{
    using UnityEngine;

    ///inherit from this base class to Create a Singleton
    ///e.g. public class MyCustomScript : Singleton {}

    ///Untiy Generic Singleton Template Codes
    ///Scene에 배치하여 미리 어떤 참조를 캐싱하는 것처럼 사용하는 형태에는 맞지 않다
    ///이 싱글턴클래스를 상속받아서 일처리 하는 경우는 오로지 모든 동작이 코드로 작동되어야 맞다
    ///Arrow & Defence Project에서는 Scene UI와 내부 Data들과 소통하는 형태로 사용중임

    /// <summary>
    /// Make Singleton Class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        //Check to see if we're about to be Destoryed
        private static bool _shuttingdown = false;
        private static object _lock = new object();
        private static T _instance;

        /// <summary>
        /// Access Singleton Instance through this Property
        /// </summary>
        public static T Instance
        {
            get
            {
                //게임종료 시 object보다 싱글턴의 OnDestroy가 먼저 실행될 수 있다
                //해당 싱글턴을 gameObject.OnDestroy() 에서는 사용하지 않거나 사용한다면 null체크 해주자
                if (_shuttingdown)
                {
                    Debug.Log("[Singleton] Instance '" + typeof(T) + "' already Destroyed. Returning Null.");
                    return null;
                }

                lock (_lock) //Thread Safe
                {
                    if (_instance == null)
                    {
                        //인스턴스 존재 여부 확인
                        _instance = (T)FindObjectOfType(typeof(T));

                        //아직 생성되지 않았다면 인스턴스를 생성함
                        if (_instance == null)
                        {
                            //새로운 싱글턴 게임오브젝트를 만들어서 싱글턴 Attach
                            var singletonObject = new GameObject();
                            _instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).ToString() + " [Singleton]";

                            //Make Instance Persistence
                            DontDestroyOnLoad(singletonObject);
                        }
                    }

                    return _instance;
                }
            }
        }

        public static bool IsActive {
            get {
                if(_instance == null) {
                    return false;
                }
                return true;
            }
        }

        private void Awake()
        {
#if UNITY_EDITOR
            //InitMessage Output :: Unity Debug
            string initMsg = string.Format("{0} :: {1}", "Init Singleton Instance", "<color=yellow>" + this + "</color>");
            Debug.Log(initMsg);
#endif
        }

        private void OnApplicationQuit()
        {
            _shuttingdown = true;
        }

        private void OnDestroy()
        {
            _shuttingdown = true;
        }
    }
}
