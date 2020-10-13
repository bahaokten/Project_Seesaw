using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAI : MonoBehaviour
{
    protected Player player;
    protected PlayerController pc;
    protected float waitDurBetweenPhases = 0;

    protected static System.Random randObj = new System.Random();

    private bool active = false;

    public Subscription<CurrentPlayerChanged> CurrentPlayerChangedSubscription;
    public Subscription<PlayerWonRound> PlayerWonRoundSubscription;

    void Awake()
    {
        CurrentPlayerChangedSubscription = _EventBus.Subscribe<CurrentPlayerChanged>(_OnCurrentPlayerChanged);
        PlayerWonRoundSubscription = _EventBus.Subscribe<PlayerWonRound>(_OnPlayerWonRound);
    }

    public static Type GetAIType(PlayerType type)
    {
        switch (type)
        {
            case PlayerType.Simp_ScissorLover:
                return typeof(Simp_ScissorLover);
            case PlayerType.Mid_GreedyAttacker:
                return typeof(Mid_GreedyAttacker);
        }
        //Human
        return null;
    }

    public void InitializeBase(Player _player)
    {
        pc = GameController.instance.GetPlayer(_player);
        player = _player;
        active = true;
        Initialize();
    }

    void _OnCurrentPlayerChanged(CurrentPlayerChanged e)
    {
        if (!active)
        {
            return;
        }
        if (e.newCurr == player)
        {
            StartCoroutine(DoTurn());
        }
    }

    void _OnPlayerWonRound(PlayerWonRound e)
    {
        if (e.player_t == player)
        {
            PostAttackPhase(true);
        }
        else
        {
            PostAttackPhase(false);
        }
    }

    protected IEnumerator DoTurn()
    {
        yield return 0;

        //=== BUY PHASE ===
        BuyPhase();
        if (waitDurBetweenPhases != 0)
        {
            yield return new WaitForSeconds(waitDurBetweenPhases);
        }

        //=== ACTION PHASE ===
        ActionPhase();
        if (waitDurBetweenPhases != 0)
        {
            yield return new WaitForSeconds(waitDurBetweenPhases);
        }

        //=== ATTACK PHASE ===
        AttackPhase();
        if (waitDurBetweenPhases != 0)
        {
            yield return new WaitForSeconds(waitDurBetweenPhases);
        }

        yield break;
    }

    protected virtual void Initialize()
    {
        return;
    }

    protected virtual void BuyPhase()
    {
        _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));
    }

    protected virtual void ActionPhase()
    {
        _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));
    }

    protected virtual void AttackPhase()
    {
        _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));
    }

    protected virtual void PostAttackPhase(bool isWinner)
    {
        return;
    }
}
