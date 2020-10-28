using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tries to buy a card every round and use it
/// Never tries to upgrade
/// Chooses each weapon with 33.3% chance
/// </summary>
public class Simp_RandomCard: Simp_Random
{
    protected override void Initialize()
    {
    }

    protected override void BuyPhase()
    {
        (CardType, CardData)?  card = GetRandomAvailableCard(this);
        if (card != null)
        {
            _EventBus.Publish<CardPurchased>(new CardPurchased(pc, card.Value.Item1));
        } else
        {
            _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));
        }
    }

    protected override void ActionPhase()
    {
        if (pc.cards.Count != 0)
        {
            _EventBus.Publish<CardUsed>(new CardUsed(pc, pc.cards[0].type));
        } 
        else
        {
            _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));
        }
    }
}
