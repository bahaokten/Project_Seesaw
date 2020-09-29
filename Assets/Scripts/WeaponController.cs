using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using TMPro;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject weaponVisualObj;
    public Vector3 weaponVisualInitPos;

    public WeaponType weaponType;

    public TextMeshProUGUI attackDisplay;
    public TextMeshProUGUI defenseDisplay;

    public int attackLevel;
    public int defenseLevel;

    public float baseAttack;
    public float baseDefense;

    public float currentAttack;
    public float currentDefense;

    void Start()
    {
        weaponVisualInitPos = weaponVisualObj.transform.position;

        attackLevel = 0;
        defenseLevel = 0;
        baseAttack = GlobalVars.INIT_ATTACK;
        baseDefense = GlobalVars.INIT_DEFENSE;
    }

    public bool Upgrade(WeaponAttribute attr)
    {
        if (attr == WeaponAttribute.Attack && attackLevel != GlobalVars.instance.maxWeaponLevel)
        {
            baseAttack += GlobalVars.WEAPON_LEVEL_UPGRADE_AMOUNT[attackLevel];
            attackLevel += 1;
            return true;
        }
        if (attr == WeaponAttribute.Defense && defenseLevel != GlobalVars.instance.maxWeaponLevel)
        {
            baseDefense += GlobalVars.WEAPON_LEVEL_UPGRADE_AMOUNT[defenseLevel];
            defenseLevel += 1;
            return true;
        }
        //Can't upgrade
        return false;
    }

    public int GetUpgradePrice(WeaponAttribute attr)
    {
        if (attr == WeaponAttribute.Attack && attackLevel != GlobalVars.instance.maxWeaponLevel)
        {
            return GlobalVars.WEAPON_LEVEL_UPGRADE_PRICES[attackLevel];
        }
        if (attr == WeaponAttribute.Defense && defenseLevel != GlobalVars.instance.maxWeaponLevel)
        {
            return GlobalVars.WEAPON_LEVEL_UPGRADE_PRICES[defenseLevel];
        }
        //Can't upgrade
        return -1;
    }
}
