using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    public static int currAvailableEventId = 0;
}

public abstract class BaseEvent
{
    protected int eventId = 0;
    public BaseEvent() { eventId = EventHandler.currAvailableEventId++; }

    public override string ToString()
    {
        return "E| Event id: " + eventId;
    }
}

public class MenuStateChangedEvent : BaseEvent
{
    public MenuState state;

    public MenuStateChangedEvent(MenuState _state) 
    {
        state = _state;
    }
}