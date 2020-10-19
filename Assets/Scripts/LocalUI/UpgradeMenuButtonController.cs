using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuButtonController : MonoBehaviour
{
    public WeaponType weaponType;
    public WeaponAttribute weaponAttr;
    PlayerController currentPlayer;

    private void OnEnable()
    {
        currentPlayer = GameController.instance.GetCurrentPlayer();

        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = currentPlayer.GetWeapon(weaponType).GetUpgradePrice(weaponAttr) + GlobalVars.CURRENCY_SUFFIX;

        if (currentPlayer.CanUpgradeWeapon(weaponType, weaponAttr))
        {
            GetComponentInChildren<Button>().interactable = true;
        }
        else
        {
            GetComponentInChildren<Button>().interactable = false;
        }
    }

    public void DoWeaponUpgrade()
    {
        if (GameController.instance.currTurnPhase != TurnPhase.ActionPhase)
        {
            return;
        }
        print(currentPlayer);
        switch (weaponType)
        {
            case WeaponType.Scissor:
                if (weaponAttr == WeaponAttribute.Attack)
                {
                    _EventBus.Publish<WeaponUpgraded>(new WeaponUpgraded(currentPlayer, WeaponType.Scissor, WeaponAttribute.Attack));
                }
                else
                {
                    _EventBus.Publish<WeaponUpgraded>(new WeaponUpgraded(currentPlayer, WeaponType.Scissor, WeaponAttribute.Defense));
                }
                break;
            case WeaponType.Paper:
                if (weaponAttr == WeaponAttribute.Attack)
                {
                    _EventBus.Publish<WeaponUpgraded>(new WeaponUpgraded(currentPlayer, WeaponType.Paper, WeaponAttribute.Attack));
                }
                else
                {
                    _EventBus.Publish<WeaponUpgraded>(new WeaponUpgraded(currentPlayer, WeaponType.Paper, WeaponAttribute.Defense));
                }
                break;
            case WeaponType.Rock:
                if (weaponAttr == WeaponAttribute.Attack)
                {
                    _EventBus.Publish<WeaponUpgraded>(new WeaponUpgraded(currentPlayer, WeaponType.Rock, WeaponAttribute.Attack));
                }
                else
                {
                    _EventBus.Publish<WeaponUpgraded>(new WeaponUpgraded(currentPlayer, WeaponType.Rock, WeaponAttribute.Defense));
                }
                break;
        }
        _EventBus.Publish<MenuStateChanged>(new MenuStateChanged(MenuState.AttackPhaseMenu));
    }
}
