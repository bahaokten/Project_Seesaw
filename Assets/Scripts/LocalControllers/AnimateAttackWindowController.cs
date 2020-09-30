using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnimateAttackWindowController : MonoBehaviour
{
    public GameObject PLDisplay;

    public GameObject PRDisplay;

    Subscription<CurrentWeaponAttrChanged> CurrentWeaponAttrChangedEventSubscription;

    void Awake()
    {
        CurrentWeaponAttrChangedEventSubscription = _EventBus.Subscribe<CurrentWeaponAttrChanged>(_OnCurrentWeaponAttrChange);
    }

    private void OnDisable()
    {
        _EventBus.Unsubscribe<CurrentWeaponAttrChanged>(CurrentWeaponAttrChangedEventSubscription);
    }

    private void OnEnable()
    {
        PlayerController pL = GameController.instance.GetPlayer(Player.L);
        WeaponController currentLWeapon = pL.GetWeapon(pL.currentWeapon);

        PLDisplay.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GlobalVars.ATTACK_PREFIX + currentLWeapon.baseAttack;
        PLDisplay.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GlobalVars.DEFENSE_PREFIX + currentLWeapon.baseDefense;

        PlayerController pR = GameController.instance.GetPlayer(Player.R);
        WeaponController currentRWeapon = pR.GetWeapon(pR.currentWeapon);

        PRDisplay.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GlobalVars.ATTACK_PREFIX + currentRWeapon.baseAttack;
        PRDisplay.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GlobalVars.DEFENSE_PREFIX + currentRWeapon.baseDefense;

        if (CurrentWeaponAttrChangedEventSubscription == null)
        {
            CurrentWeaponAttrChangedEventSubscription = _EventBus.Subscribe<CurrentWeaponAttrChanged>(_OnCurrentWeaponAttrChange);
        }
    }

    void _OnCurrentWeaponAttrChange(CurrentWeaponAttrChanged e)
    {
        //Assumes only displayed weapon is chaning value so no need to check for weapon type
        GameObject PDisplay;
        int attrIndex;
        if (e.player_t == Player.L)
        {
            PDisplay = PLDisplay;
        } else
        {
            PDisplay = PRDisplay;
        }
        if (e.weaponAttr == WeaponAttribute.Attack)
        {
            attrIndex = 0;
        }else
        {
            attrIndex = 1;
        }
        PDisplay.transform.GetChild(attrIndex).GetComponent<TextMeshProUGUI>().text = GlobalVars.ATTACK_PREFIX + e.newVal;
    }
}

