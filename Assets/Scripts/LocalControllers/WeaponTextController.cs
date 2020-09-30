using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponTextController : MonoBehaviour
{
    [HideInInspector]
    public WeaponController parent;

    Subscription<MenuStateChanged> MenuStateChangedEventSubscription;
    Subscription<BaseWeaponAttrChanged> BaseWeaponAttrChangedEventSubscription;

    void Awake()
    {
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

    void _OnMenuStateChange(MenuStateChanged e)
    {
        if (e.state == MenuState.AnimatingAttack)
        {
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
        }
    }

    void _OnBaseWeaponAttrChange(BaseWeaponAttrChanged e)
    {
        if (e.player_t == parent.parentPlayer && e.weapon_t == parent.weaponType)
        {
            if (e.weaponAttr == WeaponAttribute.Attack)
            {
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GlobalVars.ATTACK_PREFIX + e.newVal;
            } else
            {
                transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GlobalVars.DEFENSE_PREFIX + e.newVal;
            }
        }
    }
}
