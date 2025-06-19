using System;
using System.Collections.Generic;
using System.Linq;
using FlaxEngine;

namespace FreakyController;

/// <summary>
/// Locker struct.
/// </summary>
public readonly struct Locker
{
    private readonly HashSet<object> locks = new();

    public bool IsLocked => locks.Count != 0;

    public Locker()
    {
    }


    public void Lock(object @lock, bool toLock)
    {
        if (toLock) Lock(@lock);
        else Unlock(@lock);
    }

    public void Lock(object @lock)
    {
        locks.Add(@lock);
    }

    public void Unlock(object @lock)
    {
        locks.Remove(@lock);
    }
}
