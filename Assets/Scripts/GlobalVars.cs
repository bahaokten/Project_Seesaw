using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Rock,
    Paper,
    Scissor
}

public enum Player
{
    R,
    L
}

public enum PlayerMode
{
    UI,
    API
}

public enum MenuState
{
    NaN,
    BuyPhaseMenu,
    BuyMenu,
    ActionPhaseMenu,
    UseMenu,
    UpgradeMenu,
    AttackPhaseMenu
}

public enum GamePhase
{
    BuyPhase,
    ActionPhase,
    AttackPhase,
    IdlePhase
}

public class GlobalVars : MonoBehaviour
{
    public static GlobalVars instance;

    public static float initAttack = 1.0f;
    public static float initDefense = 1.0f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
