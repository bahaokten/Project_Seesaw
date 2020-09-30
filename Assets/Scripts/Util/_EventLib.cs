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

public class MenuStateChanged : BaseEvent
{
    public MenuState state;

    public MenuStateChanged(MenuState _state) 
    {
        state = _state;
    }
}

public class BaseWeaponAttrChanged : BaseEvent
{
    public Player player_t;
    public WeaponType weapon_t;
    public WeaponAttribute weaponAttr;
    public float newVal;

    public BaseWeaponAttrChanged(Player _player_t, WeaponType _weapon_t, WeaponAttribute _weaponAttr, float _newVal)
    {
        player_t = _player_t;
        weapon_t = _weapon_t;
        weaponAttr = _weaponAttr;
        newVal = _newVal;
    }

}