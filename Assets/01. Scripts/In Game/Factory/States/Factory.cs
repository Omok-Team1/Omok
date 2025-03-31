using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assembly = System.Reflection.Assembly;

public static class Factory
{
    public static Dictionary<Type, IState> CreateItems<T>()
    {
        if(_factories is null)
            GetFactoryInstance();
        
        if(_factories.TryGetValue(typeof(T), out IConcreteFactory factory))
            return factory.CreateItems();
        else
            throw new KeyNotFoundException($"No factory found for type {typeof(T).Name}");
    }

    private static void GetFactoryInstance()
    {
        _factories = new Dictionary<Type, IConcreteFactory>();
        
        var factoryTypes = Assembly.GetExecutingAssembly().GetTypes().
            Where(types => typeof(IConcreteFactory).IsAssignableFrom(types) && !types.IsAbstract && !types.IsInterface);

        foreach (var factoryType in factoryTypes)
        {
            if (Activator.CreateInstance(factoryType) is IConcreteFactory factory)
            {
                _factories.Add(factoryType, factory);
            }
        }
    }

    private static IDictionary<Type, IConcreteFactory> _factories;
}
