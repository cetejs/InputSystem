using UD.Generic;
using UnityEngine;

namespace UD.Globals
{
    public class Global : MonoBehaviour
    {
        private ServiceCollection services = new ServiceCollection(10);
        private static Global instance;
        
        public static bool IsApplicationQuitting { get; private set; }

        public static bool IsShowDevConsole { get; private set; } = true;

        public static Global Instance
        {
            get
            {
                if (IsApplicationQuitting)
                {
                    return null;
                }

                if (!Application.isPlaying)
                {
                    return FindObjectOfType<Global>();
                }

                if (instance == null)
                {
                    InitGlobal();
                }

                return instance;
            }
        }

        public static Camera MainCamera { get; set; }

        public static void InitGlobal()
        {
            if (instance)
            {
                Debug.LogError("There are multiple instances of the game");
                return;
            }

            instance = new GameObject("Global").AddComponent<Global>();
            DontDestroyOnLoad(instance.gameObject);
        }

        public static T GetService<T>() where T : Service, new()
        {
            if (IsApplicationQuitting)
            {
                return null;
            }

            if (!Instance)
            {
                return null;
            }

            var result = Instance.services.GetService<T>();
            if (!result)
            {
                result = Instance.services.AddService<T>(Instance.transform);
            }

            return result;
        }

        public static T AddService<T>() where T : Service, new()
        {
            if (IsApplicationQuitting)
            {
                return null;
            }
            
            if (!Instance)
            {
                return null;
            }

            return Instance.services.AddService<T>(Instance.transform);
        }

        public static void AddService(Service service)
        {
            if (IsApplicationQuitting)
            {
                return;
            }

            if (!Instance)
            {
                return;
            }

            if (!service)
            {
                return;
            }

            var result = Instance.services.GetService(instance.GetType());
            if (result == null)
            {
                Instance.services.AddService(Instance.transform, service);
            }
        }

        private void OnDestroy()
        {
            IsApplicationQuitting = true;
        }
    }
}