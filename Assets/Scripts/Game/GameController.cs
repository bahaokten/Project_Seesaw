﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardIterator = CardController.CardIterator;
using System;
using System.Xml;
using UnityEngine.SceneManagement;
using System.Linq;

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
    public Dictionary<Player, List<BaseCard>> currentRoundCards;

    public Subscription<EndTurnPhase> EndTurnPhaseSubscription;
    public Subscription<TurnPhaseChanged> TurnPhaseChangedSubscription;
    public Subscription<CardPurchased> CardPurchasedSubscription;
    public Subscription<CardUsed> CardUsedSubscription;
    public Subscription<CardDestroyed> CardDestroyedSubscription;
    public Subscription<WeaponUpgraded> WeaponUpgradedSubscription;
    public Subscription<AttackWeaponPicked> AttackWeaponPickedSubscription;
    public Subscription<GameStateOver> GameStateOverSubscription;
    public Subscription<CurrentPlayerChanged> CurrentPlayerChangedSubscription;
    public Subscription<GameOver> GameOverSubscription;

    public System.Random randObj;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(instance);
            instance = this;
        }

        EndTurnPhaseSubscription = _EventBus.Subscribe<EndTurnPhase>(_OnEndTurnPhase);
        TurnPhaseChangedSubscription = _EventBus.Subscribe<TurnPhaseChanged>(_OnTurnPhaseChanged);
        CardPurchasedSubscription = _EventBus.Subscribe<CardPurchased>(_OnCardPurchased);
        CardUsedSubscription = _EventBus.Subscribe<CardUsed>(_OnCardUsed);
        CardDestroyedSubscription = _EventBus.Subscribe<CardDestroyed>(_OnCardDestroyed);
        WeaponUpgradedSubscription = _EventBus.Subscribe<WeaponUpgraded>(_OnWeaponUpgraded);
        AttackWeaponPickedSubscription = _EventBus.Subscribe<AttackWeaponPicked>(_OnAttackWeaponPicked);
        GameStateOverSubscription = _EventBus.Subscribe<GameStateOver>(_OnGameStateOver);
        CurrentPlayerChangedSubscription = _EventBus.Subscribe<CurrentPlayerChanged>(_OnCurrentPlayerChanged);
        GameOverSubscription = _EventBus.Subscribe<GameOver>(_OnGameOver);

        playerControllerL = playerL.GetComponent<PlayerController>();
        playerControllerR = playerR.GetComponent<PlayerController>();

        randObj = new System.Random();
    }

    void Start()
    {
        StartGame();
    }

    private void OnDisable()
    {
        _EventBus.Unsubscribe<EndTurnPhase>(EndTurnPhaseSubscription);
        _EventBus.Unsubscribe<TurnPhaseChanged>(TurnPhaseChangedSubscription);
        _EventBus.Unsubscribe<CardPurchased>(CardPurchasedSubscription);
        _EventBus.Unsubscribe<CardUsed>(CardUsedSubscription);
        _EventBus.Unsubscribe<CardDestroyed>(CardDestroyedSubscription);
        _EventBus.Unsubscribe<WeaponUpgraded>(WeaponUpgradedSubscription);
        _EventBus.Unsubscribe<AttackWeaponPicked>(AttackWeaponPickedSubscription);
        _EventBus.Unsubscribe<GameStateOver>(GameStateOverSubscription);
        _EventBus.Unsubscribe<CurrentPlayerChanged>(CurrentPlayerChangedSubscription);
        _EventBus.Unsubscribe<GameOver>(GameOverSubscription);
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
        if (GameOverSubscription == null)
        {
            GameOverSubscription = _EventBus.Subscribe<GameOver>(_OnGameOver);
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

    void _OnCardDestroyed(CardDestroyed e)
    {
        CardController.DestroyCard(e.player, e.card);
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

    void _OnGameOver(GameOver e)
    {
        GlobalVars.GAMES_TO_PLAY--;
        print(GlobalVars.GAMES_TO_PLAY + " Games Left To Play");
        if (GlobalVars.GAMES_TO_PLAY > 0)
        {
            //Reload Game
            SceneManager.LoadScene("Game");
        } else
        {
            //Load Menu
            Logger.instance.SaveLogs();
            SceneManager.LoadScene(0);
        }
    }

    public IEnumerator LoadMenu() 
    {
        
        yield return 0;
        
    }

    //======= Functions =======

    public void StartGame()
    {
        _EventBus.Publish<GameStarted>(new GameStarted());

        activeCards = new Dictionary<Player, List<BaseCard>>() 
        { 
            { Player.L, new List<BaseCard>() },
            {Player.R, new List<BaseCard>() }
        };

        currentRoundCards = new Dictionary<Player, List<BaseCard>>()
        {
            { Player.L, new List<BaseCard>() },
            { Player.R, new List<BaseCard>() }
        };

        if (GlobalVars.instance.LType != PlayerType.Human)
        {
            playerControllerL.playerMode = PlayerMode.API;
            BaseAI aiL = gameObject.AddComponent(BaseAI.GetAIType(GlobalVars.instance.LType)) as BaseAI;
            aiL.InitializeBase(Player.L);
        }
        else
        {
            playerControllerL.playerMode = PlayerMode.UI;
        }
        if (GlobalVars.instance.RType != PlayerType.Human)
        {
            playerControllerR.playerMode = PlayerMode.API;
            BaseAI aiR = gameObject.AddComponent(BaseAI.GetAIType(GlobalVars.instance.RType)) as BaseAI;
            aiR.InitializeBase(Player.R);
        }
        else
        {
            playerControllerR.playerMode = PlayerMode.UI;
        }

        _EventBus.Publish<CurrentPlayerChanged>(new CurrentPlayerChanged(Player.L));
    }

    public void PlayerPickedWeapon(WeaponType weapon_t)
    {
        GetPlayer(currPlayer).currentWeapon = weapon_t;
        _EventBus.Publish<GameStateOver>(new GameStateOver(GameController.instance.currState));
    }

    public void GameStateOver()
    {
        currState = GetNextState();
        if (currState == GameState.IssuingAttack)
        {
            playerControllerL.ResetAllCurrentWeaponStats();
            playerControllerR.ResetAllCurrentWeaponStats();

            //Issue this round's cards
            foreach (KeyValuePair<Player, List<BaseCard>> kv in currentRoundCards)
            {
                foreach (BaseCard c in kv.Value.ToList<BaseCard>())
                {
                    activeCards[kv.Key].Add(c);
                }
            }
            currentRoundCards[Player.L].Clear();
            currentRoundCards[Player.R].Clear();

            //Issue Attack
            if (GlobalVars.instance.animateAttack)
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
                    _EventBus.Publish<GameStateOver>(new GameStateOver(GameController.instance.currState));
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
    public WinnerData DetermineWinner()
    {
        WeaponController LWeapon = playerControllerL.GetCurrentWeaponController();
        WeaponController RWeapon = playerControllerR.GetCurrentWeaponController();

        if (LWeapon.weaponType == RWeapon.weaponType)
        {
            float LStats = LWeapon.currentAttack + LWeapon.currentDefense;
            float RStats = RWeapon.currentAttack + RWeapon.currentDefense;
            if (LStats > RStats)
            {
                //player L wins
                return new WinnerData(Player.L, LStats - RStats, LWeapon.weaponType, RWeapon.weaponType);
            }
            else if(RStats > LStats)
            {
                //player R wins
                return new WinnerData(Player.R, RStats - LStats, LWeapon.weaponType, RWeapon.weaponType);
            }
            else
            {   //stalemate
                //stalemate streak reached max value, break the stalemate
                if (GlobalVars.instance.currentStalemateStreak == GlobalVars.MAX_STALEMATE_STREAK)
                {
                    return StalemateStreakBreaker(LWeapon, RWeapon);
                }
                return new WinnerData(Player.NaN, 0, LWeapon.weaponType, RWeapon.weaponType);
            }
        } else if (LWeapon.GetWeakType() == RWeapon.weaponType)
        {
            //player R wins
            return new WinnerData(Player.R, RWeapon.currentAttack - LWeapon.currentDefense, LWeapon.weaponType, RWeapon.weaponType);
        }
        {
            //player L wins
            return new WinnerData(Player.L, LWeapon.currentAttack - RWeapon.currentDefense, LWeapon.weaponType, RWeapon.weaponType);
        }
    }

    public WinnerData StalemateStreakBreaker(WeaponController LWeapon, WeaponController RWeapon)
    {
        float LTotWeaponStats = playerControllerL.GetTotalWeaponStats();
        float RTotWeaponStats = playerControllerR.GetTotalWeaponStats();
        //The player with highest total stats win
        if (LTotWeaponStats > RTotWeaponStats)
        {
            return new WinnerData(Player.L, -1, LWeapon.weaponType, RWeapon.weaponType);
        }
        else if (RTotWeaponStats > LTotWeaponStats)
        {
            return new WinnerData(Player.R, -1, LWeapon.weaponType, RWeapon.weaponType);
        }
        else
        {
            //The player with most coins win
            if (playerControllerL.coins > playerControllerR.coins)
            {
                return new WinnerData(Player.L, -1, LWeapon.weaponType, RWeapon.weaponType);
            }
            else if (playerControllerR.coins > playerControllerL.coins)
            {
                return new WinnerData(Player.R, -1, LWeapon.weaponType, RWeapon.weaponType);
            }
            else
            {
                //The player with most upgrades and cards win
                int totLCardUpgrades = playerControllerL.GetTotalCards() + playerControllerL.GetTotalUpgrades();
                int totRCardUpgrades = playerControllerR.GetTotalCards() + playerControllerR.GetTotalUpgrades();
                if (totLCardUpgrades > totRCardUpgrades)
                {
                    return new WinnerData(Player.L, -1, LWeapon.weaponType, RWeapon.weaponType);
                }
                else if (totRCardUpgrades > totLCardUpgrades)
                {
                    return new WinnerData(Player.R, -1, LWeapon.weaponType, RWeapon.weaponType);
                }
                else
                {
                    //The player with higher score wins
                    if (playerControllerL.score > playerControllerR.score)
                    {
                        return new WinnerData(Player.L, -1, LWeapon.weaponType, RWeapon.weaponType);
                    } 
                    else if (playerControllerR.score > playerControllerL.score)
                    {
                        return new WinnerData(Player.R, -1, LWeapon.weaponType, RWeapon.weaponType);
                    } else
                    {
                        //Human player wins
                        if (GlobalVars.instance.LType == PlayerType.Human ^ GlobalVars.instance.RType == PlayerType.Human)
                        {
                            if (GlobalVars.instance.LType == PlayerType.Human)
                            {
                                return new WinnerData(Player.L, -1, LWeapon.weaponType, RWeapon.weaponType);
                            }
                            else
                            {
                                return new WinnerData(Player.R, -1, LWeapon.weaponType, RWeapon.weaponType);
                            }
                        }
                        else
                        {
                            //Random player wins
                            if (randObj.Next(0, 2) == 0)
                            {
                                return new WinnerData(Player.L, -1, LWeapon.weaponType, RWeapon.weaponType);
                            } else
                            {
                                return new WinnerData(Player.R, -1, LWeapon.weaponType, RWeapon.weaponType);
                            }
                        }
                    }
                }
            }
        }
    }

    public Player RegisterWinner(WinnerData winnerData)
    {
        float coins = CalculateCoins(winnerData.winnerAttDefDiff);
        if (winnerData.winner == Player.L)
        {
            GlobalVars.instance.currentStalemateStreak = 0;

            playerControllerL.score++;
            playerControllerL.coins += coins;
            _EventBus.Publish<PlayerWonRound>(new PlayerWonRound(Player.L, coins, winnerData.weaponUsed[Player.L], winnerData.weaponUsed[Player.R]));
            if (playerControllerL.score >= GlobalVars.SCORE_TO_WIN)
            {
                return Player.L;
            }
        } else if (winnerData.winner == Player.R)
        {
            GlobalVars.instance.currentStalemateStreak = 0;

            playerControllerR.score++;
            playerControllerR.coins += coins;
            _EventBus.Publish<PlayerWonRound>(new PlayerWonRound(Player.R, coins, winnerData.weaponUsed[Player.L], winnerData.weaponUsed[Player.R]));
            if (playerControllerR.score >= GlobalVars.SCORE_TO_WIN)
            {
                return Player.R;
            }
        }
        else
        {
            GlobalVars.instance.currentStalemateStreak++;

            _EventBus.Publish<PlayerWonRound>(new PlayerWonRound(Player.NaN, 0, 0, 0));
        }
        return Player.NaN;
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

    public Player GetOpponentPlayerType(Player p)
    {
        if (p == Player.L)
        {
            return Player.R;
        }
        else
        {
            return Player.L;
        }
    }

    //Used by AI, remembers previous rounds cards
    public WinnerData SimulateRound(Dictionary<Player, WeaponType> pWeapon, Dictionary<Player, List<CardData>> playerCards)
    {
        WeaponController LWeapon = playerControllerL.GetWeapon(pWeapon[Player.L]);
        WeaponController RWeapon = playerControllerR.GetWeapon(pWeapon[Player.L]);

        Dictionary<Player, List<float>> attackVals = new Dictionary<Player, List<float>>()
        {
            {Player.L, new List<float>(){LWeapon.baseAttack, LWeapon.baseDefense} },
            {Player.R, new List<float>(){RWeapon.baseAttack, RWeapon.baseDefense} }
        };

        foreach (KeyValuePair<Player, List<BaseCard>> kv in activeCards)
        {
            foreach (BaseCard c in kv.Value.ToList<BaseCard>())
            {
                (float, float) modifier = CardController.SimulateCardEffect(c.data);
                attackVals[kv.Key][0] += modifier.Item1; //Attack increase
                attackVals[kv.Key][1] += modifier.Item2; //Defense increase
            }
        }

        foreach (KeyValuePair<Player, List<CardData>> kv in playerCards)
        {
            foreach (CardData cd in playerCards[kv.Key])
            {
                (float, float) modifier = CardController.SimulateCardEffect(cd);
                attackVals[Player.L][0] += modifier.Item1; //Attack increase
                attackVals[Player.L][1] += modifier.Item2; //Defense increase
            }
        }
        
        if (LWeapon.weaponType == RWeapon.weaponType)
        {
            float LStats = attackVals[Player.L][0] + attackVals[Player.L][1];
            float RStats = attackVals[Player.R][0] + attackVals[Player.R][1];
            if (LStats > RStats)
            {
                //player L wins
                return new WinnerData(Player.L, LStats - RStats, LWeapon.weaponType, RWeapon.weaponType);
            }
            else if (RStats < LStats)
            {
                //player R wins
                return new WinnerData(Player.R, RStats - LStats, LWeapon.weaponType, RWeapon.weaponType);
            }
            else
            {
                //stalemate
                return new WinnerData(Player.NaN, 0, LWeapon.weaponType, RWeapon.weaponType);
            }
        }
        else if (LWeapon.GetWeakType() == RWeapon.weaponType)
        {
            //player R wins
            return new WinnerData(Player.R, attackVals[Player.R][0] - attackVals[Player.L][1], LWeapon.weaponType, RWeapon.weaponType);
        }
        {
            //player L wins
            return new WinnerData(Player.L, attackVals[Player.L][0] - attackVals[Player.R][1], LWeapon.weaponType, RWeapon.weaponType);
        }
    }
}

public struct WinnerData
{
    public Player winner;
    public float winnerAttDefDiff;
    public Dictionary<Player, WeaponType> weaponUsed;

    public WinnerData(Player _winner, float _winnerAttDefDiff, WeaponType playerLWeapon, WeaponType playerRWeapon)
    {
        winner = _winner;

        if (_winnerAttDefDiff < 0)
        {
            winnerAttDefDiff = 0;
        }
        else
        {
            winnerAttDefDiff = _winnerAttDefDiff;
        }

        weaponUsed = new Dictionary<Player, WeaponType>();
        weaponUsed[Player.L] = playerLWeapon;
        weaponUsed[Player.R] = playerRWeapon;
    }
}