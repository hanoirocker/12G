using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "SO's/GameEventSO", order = 0)]
// public class GameEventSO<T> : ScriptableObject
public class GameEventSO : ScriptableObject
{
    // TODO: maybe in future, a more generic List<GameEventListener<T>> could be better
    // if garbage collection starts being a problem. Atm no more than 10 GameEventSO / frame are
    // currently being send, and only when event starts .. 
    // so there's no problem with passing object as data. (THIS AINT NO RPG GAME)
    public List<GameEventListener> listeners = new List<GameEventListener>();

    // Generic T Raise method
    // public void Raise(Component sender, T data)
    // {
    //     for (int i = listeners.Count - 1; i >= 0; i--)
    //         listeners[i].OnEventRaised(sender, data);
    //     // No boxing, no GC
    // }

    public void Raise(Component sender, object data)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(sender, data);
        }
    }

    public void RegisterListener(GameEventListener listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    public void UnRegisterListener(GameEventListener listener)
    {
        if (listeners.Contains(listener))
        {
            listeners.Remove(listener);
        }
    }

}