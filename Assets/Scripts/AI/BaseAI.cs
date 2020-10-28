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

    public bool active = false;

    public Subscription<CurrentPlayerChanged> CurrentPlayerChangedSubscription;
    public Subscription<PlayerWonRound> PlayerWonRoundSubscription;

    void Awake()
    {
        CurrentPlayerChangedSubscription = _EventBus.Subscribe<CurrentPlayerChanged>(_OnCurrentPlayerChanged);
        PlayerWonRoundSubscription = _EventBus.Subscribe<PlayerWonRound>(_OnPlayerWonRound);
    }

    private void OnDestroy()
    {
        _EventBus.Unsubscribe<CurrentPlayerChanged>(CurrentPlayerChangedSubscription);
        _EventBus.Unsubscribe<PlayerWonRound>(PlayerWonRoundSubscription);
    }

    public static Type GetAIType(PlayerType type)
    {
        switch (type)
        {
            case PlayerType.Simp_ScissorLover:
                return typeof(Simp_ScissorLover);
            case PlayerType.Simp_ScissorLover2:
                return typeof(Simp_ScissorLover2);
            case PlayerType.Simp_Random:
                return typeof(Simp_Random);
            case PlayerType.Simp_RandomUpgrade:
                return typeof(Simp_RandomUpgrade);
            case PlayerType.Simp_RandomCard:
                return typeof(Simp_RandomCard);
            case PlayerType.Mid_GreedyAttacker:
                return typeof(Mid_GreedyAttacker);
            case PlayerType.Mid_GreedyDefender:
                return typeof(Mid_GreedyDefender);
            case PlayerType.Mid_GreedyMixed:
                return typeof(Mid_GreedyMixed);
            case PlayerType.Mid_Tracker:
                return typeof(Mid_Tracker);
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
        if (!active)
        {
            return;
        }

        if (e.winner_t == player)
        {
            if (player == Player.L)
            {
                PostAttackPhase(true, e.playerRWeapon);
            } else
            {
                PostAttackPhase(true, e.playerLWeapon);
            }
        }
        else
        {
            if (player == Player.L)
            {
                PostAttackPhase(false, e.playerRWeapon);
            }
            else
            {
                PostAttackPhase(false, e.playerLWeapon);
            }
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

    // === VIRTUAL FUNCTIONS ===

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

    protected virtual void PostAttackPhase(bool isWinner, WeaponType opponentWeapon)
    {
        return;
    }

    // === STATIC HELPERS ===

    protected static List<(WeaponType, WeaponAttribute, float)> GetAvailableUpgrades(BaseAI instance)
    {
        List<(WeaponType, WeaponAttribute, float)> ret = new List<(WeaponType, WeaponAttribute, float)>();
        
        foreach (WeaponType type in Enum.GetValues(typeof(WeaponType)))
        {
            if (instance.pc.CanUpgradeWeapon(type, WeaponAttribute.Attack)) 
            {
                ret.Add((type, WeaponAttribute.Attack, instance.pc.GetWeapon(type).GetUpgradePrice(WeaponAttribute.Attack)));
            }
            if (instance.pc.CanUpgradeWeapon(type, WeaponAttribute.Defense))
            {
                ret.Add((type, WeaponAttribute.Defense, instance.pc.GetWeapon(type).GetUpgradePrice(WeaponAttribute.Defense)));
            }
        }
        return ret;
    }

    protected static List<(CardType, CardData)> GetAvailableCards(BaseAI instance)
    {
        List<(CardType, CardData)> ret = new List<(CardType, CardData)>();
        
        foreach (KeyValuePair<CardType, CardData> kv in GlobalVars.cardData)
        {
            if (kv.Value.cost <= instance.pc.coins)
            {
                ret.Add((kv.Key, kv.Value));
            }
        }

        return ret;
    }

    protected static (CardType, CardData)? GetRandomAvailableCard(BaseAI instance)
    {
        List<(CardType, CardData)> availCards = GetAvailableCards(instance);
        if (availCards.Count == 0)
        {
            return null;
        }
        return availCards[randObj.Next(0, availCards.Count)];
    }

    protected static (WeaponType, WeaponAttribute, float)? GetRandomAvailableUpgrade(BaseAI instance)
    {
        List<(WeaponType, WeaponAttribute, float)> availUpgrades = GetAvailableUpgrades(instance);
        if (availUpgrades.Count == 0)
        {
            return null;
        }
        return availUpgrades[randObj.Next(0, availUpgrades.Count)];
    }
}
