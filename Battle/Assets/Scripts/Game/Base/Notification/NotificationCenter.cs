using System;
using System.Collections.Generic;


public delegate void OnNotificationDelegate(object note);

public class NotificationCenter
{
    private static NotificationCenter instance = null;
      
    //Single 
    public static NotificationCenter Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NotificationCenter();
                return instance;
            }
            return instance;
        }
    }

    private NotificationCenter()
    {
    }

    private Dictionary<int, LinkedList<OnNotificationDelegate>> eventListerners = new Dictionary<int, LinkedList<OnNotificationDelegate>>();

    private Queue<Event> eventQueue = new Queue<Event>();

    private sealed class Event
    {
        public int id;
        public object args;

        public Event()
        {
        }

        public void Clear()
        {
            id = -1;
            args = null;
        }

        public static Event Create(int id, object args)
        {
            Event eventNode = ClassObjectPool.instance.Pop<Event>();
            eventNode.id = id;
            eventNode.args = args;
            return eventNode;
        }

        public static void Release(Event eventNode)
        {
            eventNode.Clear();
            ClassObjectPool.instance.Recyle(eventNode);
        }
    }

    public void AddEventListener(object otype, OnNotificationDelegate listener)
    {
        int type = (int) otype;
        if (!eventListerners.ContainsKey(type))
        {
            eventListerners[type] = new LinkedList<OnNotificationDelegate>();
        }
        eventListerners[type].AddLast(listener);
    }

    public void RemoveEventListener(object otype, OnNotificationDelegate listener)
    {
        int type = (int) otype;
        if (!eventListerners.ContainsKey(type))
        {
            return;
        }
        eventListerners[type].Remove(listener);

        if (eventListerners[type] == null)
            eventListerners.Remove(type);
    }

    public void DispatchEvent(object otype, object note)
    {
        int type = (int) otype;
        if (eventListerners.ContainsKey(type))
        {
            foreach (OnNotificationDelegate listener in eventListerners[type])
            {
                listener(note);
            }
        }
    }

    public void DispatchEvent(object otype)
    {
        int type = (int) otype;
        DispatchEvent(type, null);
    }

    public bool HasEventListener(object otype)
    {
        int type = (int) otype;
        return eventListerners.ContainsKey(type);
    }
        
    public void PushEvent(object otype, object args)
    {
        int type = (int)otype;
        Event eventNode = Event.Create(type, args);
        eventQueue.Enqueue(eventNode);
    }

    public void Update()
    {
        while (eventQueue.Count > 0)
        {
            Event eventNode = eventQueue.Dequeue();
            DispatchEvent(eventNode.id, eventNode.args);
            Event.Release(eventNode);
        }
    }
}