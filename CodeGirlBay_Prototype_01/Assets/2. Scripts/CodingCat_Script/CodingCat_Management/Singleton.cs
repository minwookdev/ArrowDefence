using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Need To Modified List
// 어떤 오브젝트나 스크립트에서 Singleton Object를 호출하는 시점이 곧 생성되는 순간임
// Singleton이 될 대상에 미리 스크립트를 붙여놓는 것 만으로 Singleton이 되지 못한다
// 이는 특정 Singleton이 될 스크립트의 값을 수정하여 테스트할 때 문제가 되고있다 2021/04/30

namespace CodingCat_Scripts
{
    /// <summary>
    /// Make Singleton Class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        //Destroy Check
        private static bool _shuttingdown = false;
        private static object _lock = new object();
        private static T _instance;

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
