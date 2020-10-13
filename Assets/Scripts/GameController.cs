using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardIterator = CardController.CardIterator;
using System;
using System.Xml;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    //Left Player, Right Player, or animating attack
    public GameState currState;

    //BuyPhase, ActionPhase, AttackPhase, IdlePhase
    public TurnPhase currTurnPhase;

    //L or R player
    private Player _currPlayer;
    public Player currPlayer
    {
        get
        {
            return _currPlayer;
        }
        set
        {
            _currPlayer = value;
            currPlayerMode = GetPlayer(_currPlayer).playerMode;
        }
    }

    //UI or API player
    public PlayerMode currPlayerMode;

    public GameObject playerL;
    public GameObject playerR;
    [HideInInspector]
    public PlayerController playerControllerL;
    [HideInInspector]
    public PlayerController playerControllerR;

    public Dictionary<Player, List<BaseCard>> activeCards;

    public Subscription<EndTurnPhase> EndTurnPhaseSubscription;
    public Subscription<TurnPhaseChanged> TurnPhaseChangedSubscription;
    public Subscription<CardPurchased> CardPurchasedSubscription;
    public Subscription<CardUsed> CardUsedSubscription;
    public Subscription<WeaponUpgraded> WeaponUpgradedSubscription;
    public Subscription<AttackWeaponPicked> AttackWeaponPickedSubscription;
    public Subscription<GameStateOver> GameStateOverSubscription;
    public Subscription<CurrentPlayerChanged> CurrentPlayerChangedSubscription;

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

        EndTurnPhaseSubscription = _EventBus.Subscribe<EndTurnPhase>(_OnEndTurnPhase);
        TurnPhaseChangedSubscription = _EventBus.Subscribe<TurnPhaseChanged>(_OnTurnPhaseChanged);
        CardPurchasedSubscription = _EventBus.Subscribe<CardPurchased>(_OnCardPurchased);
        CardUsedSubscription = _EventBus.Subscribe<CardUsed>(_OnCardUsed);
        WeaponUpgradedSubscription = _EventBus.Subscribe<WeaponUpgraded>(_OnWeaponUpgraded);
        AttackWeaponPickedSubscription = _EventBus.Subscribe<AttackWeaponPicked>(_OnAttackWeaponPicked);
        GameStateOverSubscription = _EventBus.Subscribe<GameStateOver>(_OnGameStateOver);
        CurrentPlayerChangedSubscription = _EventBus.Subscribe<CurrentPlayerChanged>(_OnCurrentPlayerChanged);

        playerControllerL = playerL.GetComponent<PlayerController>();
        playerControllerR = playerR.GetComponent<PlayerController>();
    }

    void Start()
    {
        _EventBus.Publish<CurrentPlayerChanged>(new CurrentPlayerChanged(Player.L));
        activeCards = new Dictionary<Player, List<BaseCard>>();
        activeCards.Add(Player.L, new List<BaseCard>());
        activeCards.Add(Player.R, new List<BaseCard>());
    }


    private void OnDisable()
    {
        _EventBus.Unsubscribe<TurnPhaseChanged>(TurnPhaseChangedSubscription);
        _EventBus.Unsubscribe<CardPurchased>(CardPurchasedSubscription);
        _EventBus.Unsubscribe<CardUsed>(CardUsedSubscription);
        _EventBus.Unsubscribe<WeaponUpgraded>(WeaponUpgradedSubscription);
        _EventBus.Unsubscribe<AttackWeaponPicked>(AttackWeaponPickedSubscription);
        _EventBus.Unsubscribe<GameStateOver>(GameStateOverSubscription);
        _EventBus.Unsubscribe<CurrentPlayerChanged>(CurrentPlayerChangedSubscription);
    }

    private void OnEnable()
    {
        if (TurnPhaseChangedSubscription == null)
        {
            TurnPhaseChangedSubscription = _EventBus.Subscribe<TurnPhaseChanged>(_OnTurnPhaseChanged);
        }
        if (CardPurchasedSubscription == null)
        {
            CardPurchasedSubscription = _EventBus.Subscribe<CardPurchased>(_OnCardPurchased);
        }
        if (CardUsedSubscription == null)
        {
            CardUsedSubscription = _EventBus.Subscribe<CardUsed>(_OnCardUsed);
        }
        if (WeaponUpgradedSubscription == null)
        {
            WeaponUpgradedSubscription = _EventBus.Subscribe<WeaponUpgraded>(_OnWeaponUpgraded);
        }
        if (AttackWeaponPickedSubscription == null)
        {
            AttackWeaponPickedSubscription = _EventBus.Subscribe<AttackWeaponPicked>(_OnAttackWeaponPicked);
        }
        if (GameStateOverSubscription == null)
        {
            GameStateOverSubscription = _EventBus.Subscribe<GameStateOver>(_OnGameStateOver);
        }
        if (CurrentPlayerChangedSubscription == null)
        {
            CurrentPlayerChangedSubscription = _EventBus.Subscribe<CurrentPlayerChanged>(_OnCurrentPlayerChanged);
        }

    }

    //======= Event Listeners =======

    void _OnEndTurnPhase(EndTurnPhase e)
    {
        if (currTurnPhase == TurnPhase.BuyPhase)
        {
            _EventBus.Publish<TurnPhaseChanged>(new TurnPhaseChanged(e.player, TurnPhase.ActionPhase));
        }
        else if (currTurnPhase == TurnPhase.ActionPhase)
        {
            _EventBus.Publish<TurnPhaseChanged>(new TurnPhaseChanged(e.player, TurnPhase.AttackPhase));
        }
        else if (currTurnPhase == TurnPhase.AttackPhase)
        {
            //Chooses weapon by default which ends the turn
            _EventBus.Publish<AttackWeaponPicked>(new AttackWeaponPicked(e.player, WeaponType.Scissor));
        }
    }

    void _OnTurnPhaseChanged(TurnPhaseChanged e)
    {
        GameController.instance.currTurnPhase = e.phase;
    }

    void _OnCardPurchased(CardPurchased e)
    {
        BaseCard card = CardFactory.GetCard(e.player, e.type);
        e.player.coins -= card.price;
        e.player.cards.Add(card);
    }

    void _OnCardUsed(CardUsed e)
    {
        BaseCard useCard = null;
        foreach (BaseCard card in e.player.cards)
        {
            if (card.type == e.type)
            {
                useCard = card;
            }
        }
        if (useCard != null)
        {
            CardController.UseCard(e.player, useCard);
        }
        _EventBus.Publish<TurnPhaseChanged>(new TurnPhaseChanged(null, TurnPhase.AttackPhase));
    }

    void _OnWeaponUpgraded(WeaponUpgraded e)
    {
        PlayerController p = e.player;
        if (p == null)
        {
            p = GameController.instance.GetCurrentPlayer();
        }
        p.UpgradeWeapon(e.type, e.attr);
        _EventBus.Publish<TurnPhaseChanged>(new TurnPhaseChanged(null, TurnPhase.AttackPhase));
    }

    void _OnAttackWeaponPicked(AttackWeaponPicked e)
    {
        GameController.instance.PlayerPickedWeapon(e.type);
    }

    void _OnGameStateOver(GameStateOver e)
    {
        GameStateOver();
    }

    void _OnCurrentPlayerChanged(CurrentPlayerChanged e)
    {
        currPlayer = e.newCurr;
    }

    //======= Functions =======

    public void PlayerPickedWeapon(WeaponType weapon_t)
    {
        GetPlayer(currPlayer).currentWeapon = weapon_t;
        _EventBus.Publish<GameStateOver>(new GameStateOver());
    }

    public void GameStateOver()
    {
        currState = GetNextState();
        if (currState == GameState.IssuingAttack)
        {
            playerControllerL.ResetAllCurrentWeaponStats();
            playerControllerR.ResetAllCurrentWeaponStats();

            if (GlobalVars.ANIMATE_ATTACK)
            {
                StartCoroutine(MenuController.instance.AnimateAttack());
            } else
            {
                CardIterator cardIt = new CardIterator();
                BaseCard nextCard = cardIt.GetNextCard();
                while (nextCard != null)
                {
                    //DO CARDS
                    nextCard.DoPreAttackAction();
                    nextCard = cardIt.GetNextCard();
                }
                Player isGameWinner = RegisterWinner(DetermineWinner());
                if (isGameWinner != Player.NaN)
                {
                    _EventBus.Publish<GameOver>(new GameOver(isGameWinner));
                } else
                {
                    _EventBus.Publish<GameStateOver>(new GameStateOver());
                }
            }
        } else
        {
            _EventBus.Publish<TurnPhaseChanged>(new TurnPhaseChanged(null, TurnPhase.BuyPhase));
            
            _EventBus.Publish<CurrentPlayerChanged>(new CurrentPlayerChanged(GetOpponentPlayerType()));

            if (currPlayerMode == PlayerMode.UI)
            {
                _EventBus.Publish<MenuStateChanged>(new MenuStateChanged(MenuState.BuyPhaseMenu));
            }
            else //API player, does nothing if both players are API players
            {
                _EventBus.Publish<MenuStateChanged>(new MenuStateChanged(MenuState.NonUIPlayerPlaying));
            }
        }
    }

    public static float CalculateCoins(float attackDamage)
    {
        if (attackDamage <= 0)
        {
            return GlobalVars.WINNING_ROUND_BASE_COINS;
        }else
        {
            return attackDamage + GlobalVars.WINNING_ROUND_BASE_COINS;
        }
    }

    //<player, damage dealt>
    public Tuple<Player, float> DetermineWinner()
    {
        WeaponController LWeapon = playerControllerL.GetCurrentWeaponController();
        WeaponController RWeapon = playerControllerR.GetCurrentWeaponController();

        if (LWeapon.weaponType == RWeapon.weaponType)
        {
            if (LWeapon.currentAttack > RWeapon.currentAttack)
            {
                //player L wins
                return new Tuple<Player, float>(Player.L, LWeapon.currentAttack - RWeapon.currentDefense);
            }
            else if(LWeapon.currentAttack < RWeapon.currentAttack)
            {
                //player R wins
                return new Tuple<Player, float>(Player.R, RWeapon.currentAttack - LWeapon.currentDefense);
            }
            else
            {
                //stalemate
                return new Tuple<Player, float>(Player.NaN, 0f);
            }
        } else if (LWeapon.GetWeakType() == RWeapon.weaponType)
        {
            //player R wins
            return new Tuple<Player, float>(Player.R, RWeapon.currentAttack - LWeapon.currentDefense);
        }
        {
            //player L wins
            return new Tuple<Player, float>(Player.L, LWeapon.currentAttack - RWeapon.currentDefense);
        }
    }

    public Player RegisterWinner(Tuple<Player, float> winnerData)
    {
        if (winnerData.Item1 == Player.L)
        {
            playerControllerL.score++;
            playerControllerL.coins += CalculateCoins(winnerData.Item2);
            if (playerControllerL.score >= GlobalVars.SCORE_TO_WIN)
            {
                return Player.L;
            }
        } else if (winnerData.Item1 == Player.R)
        {
            playerControllerR.score++;
            playerControllerR.coins += CalculateCoins(winnerData.Item2);
            if (playerControllerR.score >= GlobalVars.SCORE_TO_WIN)
            {
                return Player.R;
            }
        }
        return Player.NaN;
    }

    public void EndGame(Player winner)
    {

    }

    public GameState GetNextState()
    {
        if (currState == GameState.LeftActive)
        {
            return GameState.RightActive;
        } else if (currState == GameState.RightActive)
        {
            return GameState.IssuingAttack;
        } else //IssuingAttack
        {
            return GameState.LeftActive;
        }
    }

    public PlayerController GetPlayer(Player _player)
    {
        if (_player == Player.L)
        {
            return playerControllerL;
        } else
        {
            return playerControllerR;
        }
    }

    public PlayerController GetCurrentPlayer()
    {
        return GetPlayer(currPlayer);
    }

    public PlayerController GetOpponentPlayer()
    {
        if (currPlayer == Player.L)
        {
            return GetPlayer(Player.R);
        } else
        {
            return GetPlayer(Player.L);
        }
    }

    public PlayerController GetOpponentPlayer(PlayerController p)
    {
        if (p == playerControllerL)
        {
            return playerControllerR;
        }
        else
        {
            return playerControllerL;
        }
    }

    public Player GetOpponentPlayerType()
    {
        if (currPlayer == Player.L)
        {
            return Player.R;
        }
        else
        {
            return Player.L;
        }
    }
}
