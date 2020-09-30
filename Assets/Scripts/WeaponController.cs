using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using TMPro;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    //Variables
    public GameObject weaponVisualObj;
    public Vector3 weaponVisualInitPos;

    public WeaponType weaponType;

    public WeaponTextController infoDisplay;

    [HideInInspector]
    public Player parentPlayer;

    public int attackLevel;
    public int defenseLevel;

    private float _baseAttack;
    private float _baseDefense;
    public float baseAttack
    {
        get
        {
            return _baseAttack;
        }
        set
        {
            _baseAttack = value;
            _EventBus.Publish<BaseWeaponAttrChanged>(new BaseWeaponAttrChanged(parentPlayer, weaponType, WeaponAttribute.Attack, value));
        }
    }
    public float baseDefense
    {
        get
        {
            return _baseDefense;
        }
        set
        {
            _baseDefense = value;
            _EventBus.Publish<BaseWeaponAttrChanged>(new BaseWeaponAttrChanged(parentPlayer, weaponType, WeaponAttribute.Defense, value));
        }
    }

    public float currentAttack;
    public float currentDefense;

    private void Awake()
    {
        infoDisplay.parent = this;
    }

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
        if (attr == WeaponAttribute.Attack && attackLevel != GlobalVars.maxWeaponLevel)
        {
            baseAttack += GlobalVars.WEAPON_LEVEL_UPGRADE_AMOUNT[attackLevel];
            attackLevel += 1;
            return true;
        }
        if (attr == WeaponAttribute.Defense && defenseLevel != GlobalVars.maxWeaponLevel)
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
        if (attr == WeaponAttribute.Attack && attackLevel != GlobalVars.maxWeaponLevel)
        {
            return GlobalVars.WEAPON_LEVEL_UPGRADE_PRICES[attackLevel];
        }
        if (attr == WeaponAttribute.Defense && defenseLevel != GlobalVars.maxWeaponLevel)
        {
            return GlobalVars.WEAPON_LEVEL_UPGRADE_PRICES[defenseLevel];
        }
        //Can't upgrade
        return -1;
    }
}
