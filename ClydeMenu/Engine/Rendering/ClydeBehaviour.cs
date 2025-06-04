namespace ClydeMenu.Engine.Rendering;

using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameObject_Extensions
{
    public static T CLYDE_GetOrAdd<T>(this GameObject gameObject) where T : ClydeBehaviour, new()
    {
        var instanceInfo = ClydeBehaviour.instances[gameObject];
        if (instanceInfo == null)
        {
            instanceInfo = new List<ClydeBehaviour>();
            ClydeBehaviour.instances[gameObject] = instanceInfo;
        }

        foreach (var instance in instanceInfo)
        {
            if (instance is T tInstance)
            {
                return tInstance;
            }
        }

        T newInstance = ClydeBehaviour.Create<T>();
        instanceInfo.Add(newInstance);

        return newInstance;
    }
}

public abstract class ClydeBehaviour : IDisposable //: MonoBehaviour
{
    internal static readonly Dictionary<GameObject, List<ClydeBehaviour>> instances = new();
    public static T Create<T>() where T : ClydeBehaviour, new()
    {
        var instance = new T();
        instance.Awake();
        return instance;
    }

    public static T Create<T>(params object[] args) where T : ClydeBehaviour, new()
    {
        var instance = new T();
        instance.Awake(args);
        return instance;
    }

    public static GameObject GetGameObject(ClydeBehaviour behaviour)
    {
        foreach (var kvp in instances)
        {
            if (kvp.Value.Contains(behaviour))
                return kvp.Key;
        }
        return null;
    }

    public GameObject gameObject => GetGameObject(this);

    public bool IsAlive { get; set; } = true;

    public virtual void Dispose() {}// => throw new NotImplementedException();

    public virtual void Awake(params object[] args) {}
    public virtual void Update() {}
    public virtual void LateUpdate() {}
    public virtual void FixedUpdate() {}
    public virtual void OnGUI() {}
}
