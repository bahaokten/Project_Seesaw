using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class PlayerController : MonoBehaviour
{
    public Player playerType;
    //Weapon related
    public WeaponController scissorController;
    public WeaponController paperController;
    public WeaponController rockController;

    [HideInInspector]
    public WeaponType currentWeapon;

    [HideInInspector]
    public Dictionary<WeaponType, WeaponController> weapons;

    //Score related
    public float score;
    public float coins;

    //Card related
    public List<Cards.BaseCard> cards;

    public void Start()
    {
        score = 0;
        coins = 0;
        weapons = new Dictionary<WeaponType, WeaponController>();
        weapons.Add(WeaponType.Scissor, scissorController);
        weapons.Add(WeaponType.Paper, paperController);
        weapons.Add(WeaponType.Rock, rockController);
    }

    public WeaponController GetWeapon(WeaponType type)
    {
        if (type == WeaponType.Scissor)
        {
            return scissorController;
        } else if (type == WeaponType.Paper)
        {
            return paperController;
        } else if (type == WeaponType.Rock)
        {
            return rockController;
        }
        return null;
    }

    public WeaponController GetCurrentWeaponController()
    {
        return weapons[currentWeapon];
    }
}
