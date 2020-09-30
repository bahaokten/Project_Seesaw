using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public TextMeshProUGUI scoreDisplay;
    public TextMeshProUGUI coinDisplay;

    public Player playerType;
    public PlayerMode playerMode;

    //Weapon related
    public WeaponController scissorController;
    public WeaponController paperController;
    public WeaponController rockController;

    [HideInInspector]
    public WeaponType currentWeapon;

    [HideInInspector]
    public Dictionary<WeaponType, WeaponController> weapons;

    //Score related
    private float _score;
    private float _coins;
    public float coins
    {
        get
        {
            return _coins;
        }
        set
        {
            _coins = value;
            coinDisplay.text = GlobalVars.COINS_PREFIX + coins;
        }
    }
    public float score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
            scoreDisplay.text = GlobalVars.SCORE_PREFIX + score;
        }
    }

    //Card related
    public List<Cards.BaseCard> cards;

    public void Awake()
    {
        scissorController.parentPlayer = playerType;
        paperController.parentPlayer = playerType;
        rockController.parentPlayer = playerType;
    }

    public void Start()
    {
        score = 0;
        coins = 5;
        weapons = new Dictionary<WeaponType, WeaponController>();
        weapons.Add(WeaponType.Scissor, scissorController);
        weapons.Add(WeaponType.Paper, paperController);
        weapons.Add(WeaponType.Rock, rockController);
    }

    public void ResetAllCurrentWeaponStats()
    {
        scissorController.ResetCurrentStats();
        paperController.ResetCurrentStats();
        rockController.ResetCurrentStats();
    }

    public WeaponController GetWeapon(WeaponType weapon_t)
    {
        if (weapon_t == WeaponType.Scissor)
        {
            return scissorController;
        } else if (weapon_t == WeaponType.Paper)
        {
            return paperController;
        } else if (weapon_t == WeaponType.Rock)
        {
            return rockController;
        }
        return null;
    }

    public bool CanUpgradeWeapon(WeaponType weapon_t, WeaponAttribute attr)
    {
        WeaponController weapon = GetWeapon(weapon_t);
        int upgradePrice = weapon.GetUpgradePrice(attr);
        if (upgradePrice >= 0 && coins >= upgradePrice)
        {
            return true;
        }
        return false;
    }

    public bool UpgradeWeapon(WeaponType weapon_t, WeaponAttribute attr)
    {
        WeaponController weapon = GetWeapon(weapon_t);
        int upgradePrice = weapon.GetUpgradePrice(attr);
        if (weapon.Upgrade(attr))
        {
            coins -= upgradePrice;
            return true;
        }
        return false;
    }

    public WeaponController GetCurrentWeaponController()
    {
        return weapons[currentWeapon];
    }
}
