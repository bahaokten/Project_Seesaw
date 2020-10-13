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
    L,
    NaN
}

public enum PlayerMode
{
    UI,
    API
}

public enum PlayerType
{
    Human,
    Simp_ScissorLover,
    Mid_GreedyAttacker
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

public enum CardType
{
    BaseCard,
    SelfAttackIncreaseAdditiveCurrent1,
    SelfDefenseIncreaseAdditiveCurrent1,
    SelfDefenseIncreaseAdditiveScissor1,
    OpponentDefenseDecreaseAdditiveScissor1,
    OpponentDefenseDecreaseMultScissor1,
}

public enum CardModificationType
{
    IncrementAttack,
    IncrementDefense,
    MultiplyAttack,
    MultiplyDefense
}

public class GlobalVars : MonoBehaviour
{
    public static GlobalVars instance;


    //CONSTANTS
    public static readonly int SCORE_TO_WIN = 20;

    public static readonly float WINNING_ROUND_BASE_COINS = 1;

    public static readonly float INIT_ATTACK = 1.0f;
    public static readonly float INIT_DEFENSE = 1.0f;

    public static readonly List<int> WEAPON_LEVEL_UPGRADE_PRICES_ATTACK = new List<int> { 5, 10, 20, 40 };
    public static readonly List<int> WEAPON_LEVEL_UPGRADE_PRICES_DEFENSE = new List<int> { 2, 4, 8, 16 };
    public static readonly List<float> WEAPON_LEVEL_UPGRADE_AMOUNT = new List<float> { 0.5f, 0.5f, 0.5f, 0.5f };
    public static int maxWeaponLevel = WEAPON_LEVEL_UPGRADE_AMOUNT.Count;

    public static readonly Dictionary<CardType, CardData> cardData = new Dictionary<CardType, CardData>()
    {
        { CardType.BaseCard, new CardData(0) },
        { CardType.SelfAttackIncreaseAdditiveCurrent1, new CardData(2, _modifications : new Dictionary<CardModificationType, float>() { { CardModificationType.IncrementAttack, 0.5f } }) },
        { CardType.SelfDefenseIncreaseAdditiveCurrent1, new CardData(1) },
        { CardType.SelfDefenseIncreaseAdditiveScissor1, new CardData(2) },
        { CardType.OpponentDefenseDecreaseAdditiveScissor1, new CardData(3) },
        { CardType.OpponentDefenseDecreaseMultScissor1, new CardData(6, 3) }
    };

    //SPRING CONSTANTS
    public static readonly string ATTACK_PREFIX = "A: ";
    public static readonly string DEFENSE_PREFIX = "D: ";
    public static readonly string SCORE_PREFIX = "Score: ";
    public static readonly string COINS_PREFIX = "Coins: ";
    public static readonly string CURRENCY_SUFFIX = " Coins";
    public static readonly string ROUND_STALEMATE_DISPLAY_TEXT = "Stalemate!";
    public static readonly List<string> ROUND_WIN_DISPLAY_TEXT = new List<string> { "Player ", " Wins The Round Earning ", " Coins" };
    public static readonly List<string> GAME_WIN_DISPLAY_TEXT = new List<string> { "Player ", " Wins The Game!!!!."};

    //RUNTIME

    public Dictionary<CardType, GameObject> cardUIData;

    //RUNTIME CONFIG
    public bool animateAttack = true;
    public PlayerType LType;
    public PlayerType RType;

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

        cardUIData = new Dictionary<CardType, GameObject>();
        foreach (CardType type in System.Enum.GetValues(typeof(CardType)))
        {
            cardUIData[type] = Resources.Load<GameObject>("Cards/" + type.ToString());
        }
    }

    public void InitGame(PlayerType L, PlayerType R, bool isAnimate)
    {
        animateAttack = isAnimate;
        LType = L;
        RType = R;
    }
}
