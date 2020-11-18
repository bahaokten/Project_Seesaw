using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public TextMeshProUGUI scoreDisplay;
    public TextMeshProUGUI coinDisplay;

    public Player player;
    public PlayerMode playerMode;

    //Weapon related
    public WeaponController scissorController;
    public WeaponController paperController;
    public WeaponController rockController;

    [HideInInspector]
    public WeaponType currentWeapon;

    [HideInInspector]
    public Dictionary<WeaponType, WeaponController> weapons;

    //Card related
    public List<BaseCard> cards;

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

    public void Awake()
    {
        scissorController.parentPlayer = player;
        paperController.parentPlayer = player;
        rockController.parentPlayer = player;
    }

    public void Start()
    {
        score = 0;
        coins = GlobalVars.INITIAL_COINS;

        cards = new List<BaseCard>();

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

    public float GetTotalWeaponStats()
    {
        float totStats = 0;
        foreach (KeyValuePair<WeaponType, WeaponController> kv in weapons)
        {
            totStats += kv.Value.currentAttack + kv.Value.currentDefense;
        }
        return totStats;
    }

    public int GetTotalUpgrades()
    {
        int totUpgrades = 0;
        foreach (KeyValuePair<WeaponType, WeaponController> kv in weapons)
        {
            totUpgrades = kv.Value.attackLevel + kv.Value.defenseLevel;
        }
        return totUpgrades;
    } 
    
    public int GetTotalCards()
    {
        return cards.Count;
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
