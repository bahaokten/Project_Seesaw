using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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

        currState = GetNextState();
        if (currState == GameState.IssuingAttack)
        {
            //Do attack logic
        } else
        {
            currPlayer = GetOpponentPlayerType();
            currTurnPhase = TurnPhase.BuyPhase;
            
            if (currPlayerMode == PlayerMode.UI)
            {
                MenuController.instance.DoMenuStateChange("buyPhaseMenu");
            } else //API player
            {
                MenuController.instance.DoMenuStateChange("nonUIPlayerPlaying");
            }
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
