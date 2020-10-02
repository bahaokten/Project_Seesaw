using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventHandler : MonoBehaviour
{
    GameEventHandler instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        MenuStateChangedEventSubscription = _EventBus.Subscribe<MenuStateChanged>(_OnMenuStateChange);
        BaseWeaponAttrChangedEventSubscription = _EventBus.Subscribe<BaseWeaponAttrChanged>(_OnBaseWeaponAttrChange);
    }

    private void OnDisable()
    {
        _EventBus.Unsubscribe<MenuStateChanged>(MenuStateChangedEventSubscription);
        _EventBus.Unsubscribe<BaseWeaponAttrChanged>(BaseWeaponAttrChangedEventSubscription);
    }

    private void OnEnable()
    {
        if (MenuStateChangedEventSubscription == null)
        {
            MenuStateChangedEventSubscription = _EventBus.Subscribe<MenuStateChanged>(_OnMenuStateChange);
        }
        if (BaseWeaponAttrChangedEventSubscription == null)
        {
            BaseWeaponAttrChangedEventSubscription = _EventBus.Subscribe<BaseWeaponAttrChanged>(_OnBaseWeaponAttrChange);
        }
    }
}
