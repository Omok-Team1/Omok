using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConcreteFactory
{
    Dictionary<Type, IState> CreateItems();
}
