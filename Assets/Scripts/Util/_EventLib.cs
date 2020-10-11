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

    public string GetTypeString()
    {
        return GetType().ToString();
    }
}

public abstract class PlayerBaseEvent : BaseEvent
{
    public PlayerController player;

    public PlayerBaseEvent(PlayerController _player) 
    {
        player = _player;
    } 
}

//Game

public class TurnPhaseChanged : PlayerBaseEvent
{
    public TurnPhase phase;

    public TurnPhaseChanged(PlayerController _player, TurnPhase _phase) : base(_player)
    {
        phase = _phase;
    }
}

public class CardPurchased : PlayerBaseEvent
{
    public CardType type;

    public CardPurchased(PlayerController _player, CardType _type) : base(_player)
    {
        type = _type;
    }
}

public class CardUsed : PlayerBaseEvent
{
    public CardType type;

    public CardUsed(PlayerController _player, CardType _type) : base(_player)
    {
        type = _type;
    }
}

public class WeaponUpgraded : PlayerBaseEvent
{
    public WeaponType type;
    public WeaponAttribute attr;

    public WeaponUpgraded(PlayerController _player, WeaponType _type, WeaponAttribute _attr) : base(_player)
    {
        type = _type;
        attr = _attr;
    }
}

public class AttackWeaponPicked : PlayerBaseEvent
{
    public WeaponType type;

    public AttackWeaponPicked(PlayerController _player, WeaponType _type) : base(_player)
    {
        type = _type;
    }
}

//Internal

public class GameStateOver
{
    public GameStateOver()
    {

    }
}

//UI

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

public class CurrentWeaponAttrChanged : BaseEvent
{
    public Player player_t;
    public WeaponType weapon_t;
    public WeaponAttribute weaponAttr;
    public float newVal;

    public CurrentWeaponAttrChanged(Player _player_t, WeaponType _weapon_t, WeaponAttribute _weaponAttr, float _newVal)
    {
        player_t = _player_t;
        weapon_t = _weapon_t;
        weaponAttr = _weaponAttr;
        newVal = _newVal;
    }
}