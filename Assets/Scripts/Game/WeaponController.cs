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

    private float _currentAttack;
    public float currentAttack
    {
        get
        {
            return _currentAttack;
        }
        set
        {
            _currentAttack = value;
            //print(parentPlayer + " w" + weaponType + " attack changed to" + value);
            _EventBus.Publish<CurrentWeaponAttrChanged>(new CurrentWeaponAttrChanged(parentPlayer, weaponType, WeaponAttribute.Attack, value));
        }
    }

    private float _currentDefense;
    public float currentDefense
    {
        get
        {
            return _currentDefense;
        }
        set
        {
            _currentDefense = value;
            //print(parentPlayer + " w" + weaponType + " defense changed to" + value);
            _EventBus.Publish<CurrentWeaponAttrChanged>(new CurrentWeaponAttrChanged(parentPlayer, weaponType, WeaponAttribute.Defense, value));
        }
    }

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

        ResetCurrentStats();
    }

    public void ResetCurrentStats()
    {
        currentAttack = baseAttack;
        currentDefense = baseDefense;
    }

    public static WeaponType GetWeakType(WeaponType type)
    {
        if (type == WeaponType.Scissor)
        {
            return WeaponType.Rock;
        }
        else if (type == WeaponType.Paper)
        {
            return WeaponType.Scissor;
        }
        else
        {
            return WeaponType.Paper;
        }
    }

    public static WeaponType GetStrongType(WeaponType type)
    {
        if (type == WeaponType.Scissor)
        {
            return WeaponType.Paper;
        }
        else if (type == WeaponType.Paper)
        {
            return WeaponType.Rock;
        }
        else
        {
            return WeaponType.Scissor;
        }
    }

    public WeaponType GetWeakType()
    {
        if (weaponType == WeaponType.Scissor)
        {
            return WeaponType.Rock;
        }
        else if (weaponType == WeaponType.Paper)
        {
            return WeaponType.Scissor;
        }
        else
        {
            return WeaponType.Paper;
        }
    }

    public WeaponType GetStrongType()
    {
        if (weaponType == WeaponType.Scissor)
        {
            return WeaponType.Paper;
        }
        else if (weaponType == WeaponType.Paper)
        {
            return WeaponType.Rock;
        }
        else
        {
            return WeaponType.Scissor;
        }
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
        print("cant upgrade man");
        //Can't upgrade
        return false;
    }

    public int GetUpgradePrice(WeaponAttribute attr)
    {
        if (attr == WeaponAttribute.Attack && attackLevel != GlobalVars.maxWeaponLevel)
        {
            return GlobalVars.WEAPON_LEVEL_UPGRADE_PRICES_ATTACK[attackLevel];
        }
        if (attr == WeaponAttribute.Defense && defenseLevel != GlobalVars.maxWeaponLevel)
        {
            return GlobalVars.WEAPON_LEVEL_UPGRADE_PRICES_DEFENSE[defenseLevel];
        }
        //Can't upgrade
        return -1;
    }
}
