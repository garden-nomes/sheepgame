using System;
using System.Collections.Generic;
using UnityEngine;

public class EventBus
{
    private static EventBus instance;

    public static EventBus Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EventBus();
            }

            return instance;
        }
    }

    private Dictionary<Event, List<Action<GameObject>>> eventListeners
        = new Dictionary<Event, List<Action<GameObject>>>();

    private List<Action<Event, GameObject>> generalListeners = new List<Action<Event, GameObject>>();

    public void Listen(Event event_, Action<GameObject> listener)
    {
        if (!eventListeners.ContainsKey(event_))
        {
            eventListeners.Add(event_, new List<Action<GameObject>>());
        }

        eventListeners[event_].Add(listener);
    }

    public void ListenAll(Action<Event, GameObject> listener)
    {
        generalListeners.Add(listener);
    }

    public void Notify(Event event_, GameObject gameObject)
    {
        foreach (var listener in generalListeners)
        {
            listener.Invoke(event_, gameObject);
        }

        if (eventListeners.ContainsKey(event_))
        {
            foreach (var listener in eventListeners[event_])
            {
                listener.Invoke(gameObject);
            }
        }
    }
}
