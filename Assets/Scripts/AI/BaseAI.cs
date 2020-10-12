using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAI : MonoBehaviour
{
    protected Player player = Player.R;
    protected PlayerController pc;

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

    protected virtual IEnumerator DoTurn()
    {
        print("NO");
        //=== BUY PHASE ===
        _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));

        //=== ACTION PHASE ===
        _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));

        //=== ATTACK PHASE ===
        _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));

        yield break;
    }
}
