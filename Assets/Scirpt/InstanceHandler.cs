using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static class InstanceHandler
{
    private static readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();

    /// <summary>
    /// Registers a instance of the given type, in order to use GetInstance later.
    /// </summary>
    /// <param name="instance">Instance to register</param>
    /// <typeparam name="T"></typeparam>
    public static void RegisterInstance<T>(T instance) where T : class
    {
        _instances[typeof(T)] = instance;
    }

    /// <summary>
    /// Unregisters a instance of the given type.
    /// </summary>
    /// <typeparam name="T">Type to unregister</typeparam>
    public static void UnregisterInstance<T>() where T : class
    {
        _instances.Remove(typeof(T));
    }

    /// <summary>
    /// Get a registered instance of the given type
    /// </summary>
    /// <typeparam name="T">Type to get the instance of</typeparam>
    /// <returns>Instance of the given type</returns>
    /// <exception cref="KeyNotFoundException">Throws an exception if the given type has not been registered</exception>
    public static T GetInstance<T>() where T : class
    {
        if (!_instances.TryGetValue(typeof(T), out var instance))
            throw new KeyNotFoundException($"Singleton of type {typeof(T)} not found");

        return (T)instance;
    }

    /// <summary>
    /// Try and get a registered instance
    /// </summary>
    /// <param name="instance">The instance of the type. Will be null if not successful</param>
    /// <typeparam name="T">Type you're trying to get</typeparam>
    /// <returns>Whether it successfully got the instance</returns>
    public static bool TryGetInstance<T>(out T instance) where T : class
    {
        if (!_instances.TryGetValue(typeof(T), out var obj))
        {
            instance = null;
            return false;
        }

        instance = (T)obj;
        return true;
    }
}
