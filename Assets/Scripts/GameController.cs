using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameController : MonoBehaviour
{
    public static GameController instance;

    public static float initAttack = 1.0f;
    public static float initDefense = 1.0f;

    public static GamePhase currPhase;
    public static Player currPlayer;
    public static PlayerMode currPlayerMode;

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
            return GetPlayer(Player.L);
        } else
        {
            return GetPlayer(Player.R);
        }
    }
}
