using System.Collections.Generic;
using UnityEngine.Events;

public class EventCenter : Singleton<EventCenter>
{
    private Dictionary<string, IEventInfo> eventDictionary = new Dictionary<string, IEventInfo>();

    public void AddListener(string eventName, UnityAction action)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            (eventDictionary[eventName] as EventInfo).actions += action;
        }
        else
        {
            eventDictionary.Add(eventName, new EventInfo(action));
        }
    }

    public void Broadcast(string eventName)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            if ((eventDictionary[eventName] as EventInfo) != null)
            {
                (eventDictionary[eventName] as EventInfo).actions.Invoke();
            }
        }
    }

    public void RemoveListener(string eventName, UnityAction action)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            (eventDictionary[eventName] as EventInfo).actions -= action;
        }
    }

    public void AddListener<T>(string eventName, UnityAction<T> action)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            (eventDictionary[eventName] as EventInfo<T>).actions += action;
        }
        else
        {
            eventDictionary.Add(eventName, new EventInfo<T>(action));
        }
    }

    public void Broadcast<T>(string eventName, T arg)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            if ((eventDictionary[eventName] as EventInfo<T>) != null)
            {
                (eventDictionary[eventName] as EventInfo<T>).actions.Invoke(arg);
            }
        }
    }

    public void RemoveListener<T>(string eventName, UnityAction<T> action)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            (eventDictionary[eventName] as EventInfo<T>).actions -= action;
        }
    }


    public void RemoveAllListeners()
    {
        eventDictionary.Clear();
    }
}

public interface IEventInfo { }

public class EventInfo : IEventInfo
{
    public UnityAction actions;
    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}

public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;
    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}