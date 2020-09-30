using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuButtonController : MonoBehaviour
{
    public WeaponType weaponType;
    public WeaponAttribute weaponAttr;

    private void OnEnable()
    {
        PlayerController currentPlayer = GameController.instance.GetCurrentPlayer();

        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = currentPlayer.GetWeapon(weaponType).GetUpgradePrice(weaponAttr) + GlobalVars.UPGRADE_COST_SUFFIX;

        if (currentPlayer.CanUpgradeWeapon(weaponType, weaponAttr))
        {
            GetComponentInChildren<Button>().interactable = true;
        }
        else
        {
            GetComponentInChildren<Button>().interactable = false;
        }
    }
}
