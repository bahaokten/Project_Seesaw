using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using TMPro;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public WeaponType weaponType;

    public TextMeshProUGUI attackDisplay;
    public TextMeshProUGUI defenseDisplay;

    public float baseAttack;
    public float baseDefense;

    public float currentAttack;
    public float currentDefense;

    void Start()
    {
        baseAttack = GlobalVars.initAttack;
        baseDefense = GlobalVars.initDefense;
    }


    void Update()
    {
        
    }
}
