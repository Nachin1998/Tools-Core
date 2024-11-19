using System;
using UnityEngine;

namespace Nach.Tools.Singleton
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T instance = null;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        Type singletonType = typeof(T);
                        instance = new GameObject(singletonType.Name, singletonType).GetComponent<T>();
                        DontDestroyOnLoad(instance.gameObject);
                    }
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}