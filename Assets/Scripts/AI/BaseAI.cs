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

    void Awake()
    {
        CurrentPlayerChangedSubscription = _EventBus.Subscribe<CurrentPlayerChanged>(_OnCurrentPlayerChanged);
    }

    public static Type GetAIType(PlayerType type)
    {
        switch (type)
        {
            case PlayerType.Simp_ScissorLover:
                return typeof(Simp_ScissorLover);
        }
        //Human
        return null;
    }

    public void Initialize(Player _player)
    {
        pc = GameController.instance.GetPlayer(_player);
        player = _player;
        active = true;
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
}
