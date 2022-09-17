using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UD.Generic
{
    public class ServiceCollection
    {
        private readonly Dictionary<Type, Service> services;
        private readonly List<Type> invalidServers;

        public ServiceCollection(int capacity = 4)
        {
            services = new Dictionary<Type, Service>(capacity);
            invalidServers = new List<Type>(capacity);
        }

        public T GetService<T>() where T : Service
        {
            var key = typeof(T);
            if (!services.TryGetValue(key, out var service))
            {
                return default;
            }

            return (T) service;
        }

        public Service GetService(Type type)
        {
            if (!services.TryGetValue(type, out var service))
            {
                return default;
            }

            return service;
        }

        public T AddService<T>(Transform parent) where T : Service, new()
        {
            var key = typeof(T);
            if (services.TryGetValue(key, out var service))
            {
                Debug.LogError($"Service is add fail, service {key} is already exist");
                return default;
            }

            service = new GameObject(key.Name).AddComponent<T>();
            service.transform.SetParent(key.IsSubclassOf(typeof(PersistentService)) ? parent : null);
            services.Add(key, service);
            return (T) service;
        }

        public Service AddService(Transform parent, Type type)
        {
            var key = type;
            if (services.TryGetValue(key, out var service))
            {
                Debug.LogError($"Service is add fail, service {key} is already exist");
                return default;
            }

            var go = new GameObject(key.Name, type);
            service = go.GetComponent<Service>();
            if (service == null)
            {
                Debug.LogError($"Service is add fail, {key} is not service");
                return default;
            }

            service.transform.SetParent(type.IsSubclassOf(typeof(PersistentService)) ? parent : null);
            services.Add(key, service);
            return service;
        }

        public void AddService(Transform parent, Service instance)
        {
            if (!instance)
            {
                Debug.LogError("Service is add fail, service instance is null");
                return;
            }

            var type = instance.GetType();
            var key = type;
            if (services.TryGetValue(key, out var service))
            {
                Debug.LogError($"Service is add fail, service {key} is already exist");
                return;
            }

            instance.transform.SetParent(type.IsSubclassOf(typeof(PersistentService)) ? parent : null);
            services.Add(key, instance);
        }

        public void RemoveService<T>() where T : Service
        {
            RemoveServices(typeof(T));
        }

        public void RemoveServices(Type type)
        {
            var key = type;
            if (!services.TryGetValue(key, out var service))
            {
                Debug.LogError($"Service is remove fail, service {key} is not exist");
                return;
            }

            Object.Destroy(service.gameObject);
            services.Remove(key);
        }

        public void CheckService()
        {
            foreach (var service in services)
            {
                if (!service.Value)
                {
                    invalidServers.Add(service.Key);
                }
            }

            foreach (var key in invalidServers)
            {
                services.Remove(key);
            }

            invalidServers.Clear();
        }

        public void Clear()
        {
            foreach (var service in services.Values)
            {
                Object.Destroy(service);
            }

            services.Clear();
        }
    }
}