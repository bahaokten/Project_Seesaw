using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardIterator = Cards.CardIterator;
using BaseCard = Cards.BaseCard;
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
    public PlayerController playerControllerL;
    public PlayerController playerControllerR;

    public Dictionary<Player, List<Cards.BaseCard>> activeCards;

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
        playerControllerL = playerL.GetComponent<PlayerController>();
        playerControllerR = playerR.GetComponent<PlayerController>();
        currPlayer = Player.L;
        activeCards = new Dictionary<Player, List<Cards.BaseCard>>();
        activeCards.Add(Player.L, new List<Cards.BaseCard>());
        activeCards.Add(Player.R, new List<Cards.BaseCard>());
    }

    void Update()
    {
        
    }

    void OnDestroy()
    {
    }

    public void PlayerTurnOver(WeaponType weapon_t)
    {
        GetPlayer(currPlayer).currentWeapon = weapon_t;

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
                    nextCard = cardIt.GetNextCard();
                }
                DetermineWinner();
            }
        } else
        {
            currPlayer = GetOpponentPlayerType();
            currTurnPhase = TurnPhase.BuyPhase;
            
            if (currPlayerMode == PlayerMode.UI)
            {
                MenuController.instance.DoMenuStateChange("buyPhaseMenu");
            } else //API player, does nothing if both players are API players
            {
                MenuController.instance.DoMenuStateChange("nonUIPlayerPlaying");
            }
        }
    }

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
