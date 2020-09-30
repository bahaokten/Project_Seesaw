using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnimateAttackWindowController : MonoBehaviour
{
    public GameObject PLDisplay;

    public GameObject PRDisplay;

    public GameObject WinnerText;

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
        WinnerText.SetActive(false);
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

    public void EnableWinnerText(Player winner, float coinAmount)
    {
        if (winner == Player.L)
        {
            WinnerText.GetComponent<TextMeshProUGUI>().text = GlobalVars.ROUND_WIN_DISPLAY_TEXT[0] + "L" + GlobalVars.ROUND_WIN_DISPLAY_TEXT[1] + coinAmount + GlobalVars.ROUND_WIN_DISPLAY_TEXT[2];
        }
        else if (winner == Player.R)
        {
            WinnerText.GetComponent<TextMeshProUGUI>().text = GlobalVars.ROUND_WIN_DISPLAY_TEXT[0] + "R" + GlobalVars.ROUND_WIN_DISPLAY_TEXT[1] + coinAmount + GlobalVars.ROUND_WIN_DISPLAY_TEXT[2];
        } else
        {
            WinnerText.GetComponent<TextMeshProUGUI>().text = GlobalVars.ROUND_STALEMATE_DISPLAY_TEXT;
        }

        WinnerText.SetActive(true);
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

