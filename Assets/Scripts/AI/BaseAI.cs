using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAI : MonoBehaviour
{
    protected Player player = Player.R;
    protected PlayerController pc;
    protected float waitDurBetweenPhases = 0;

    protected System.Random randObj = new System.Random();

    public Subscription<CurrentPlayerChanged> CurrentPlayerChangedSubscription;

    void Awake()
    {
        CurrentPlayerChangedSubscription = _EventBus.Subscribe<CurrentPlayerChanged>(_OnCurrentPlayerChanged);
    }

    private void Start()
    {
        pc = GameController.instance.GetPlayer(player);
    }

    void _OnCurrentPlayerChanged(CurrentPlayerChanged e)
    {
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
