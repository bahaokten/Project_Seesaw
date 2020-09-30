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

public enum WeaponAttribute
{
    Attack,
    Defense
}

public enum MenuState
{
    NaN,
    BuyPhaseMenu,
    BuyMenu,
    ActionPhaseMenu,
    UseMenu,
    UpgradeMenu,
    AttackPhaseMenu,
    AnimatingAttack,
    NonUIPlayerPlaying
}

public enum GameState
{
    LeftActive,
    RightActive,
    IssuingAttack
}

public enum TurnPhase
{
    BuyPhase,
    ActionPhase,
    AttackPhase
}

public class GlobalVars : MonoBehaviour
{
    public static GlobalVars instance;

    //CONFIG
    public static readonly bool ANIMATE_ATTACK = true;

    //CONSTANTS
    public static readonly int SCORE_TO_WIN = 15;

    public static readonly float INIT_ATTACK = 1.0f;
    public static readonly float INIT_DEFENSE = 1.0f;

    public static readonly List<int> WEAPON_LEVEL_UPGRADE_PRICES = new List<int> { 5, 10, 20, 40 };
    public static readonly List<float> WEAPON_LEVEL_UPGRADE_AMOUNT = new List<float> { 0.5f, 0.5f, 0.5f, 0.5f };
    public static int maxWeaponLevel = WEAPON_LEVEL_UPGRADE_PRICES.Count;

    //SPRING CONSTANTS
    public static readonly string ATTACK_PREFIX = "A: ";
    public static readonly string DEFENSE_PREFIX = "D: ";
    public static readonly string SCORE_PREFIX = "Score: ";
    public static readonly string COINS_PREFIX = "Coins: ";
    public static readonly string UPGRADE_COST_SUFFIX = " Coins";

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
}
