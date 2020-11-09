using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tries to buy a card 33% of the time
/// Tries to upgrade 50% of the time
/// Chooses each weapon with 33.3% chance
/// </summary>
public class Simp_Random: BaseAI
{
    protected override void Initialize()
    {
    }

    protected override void BuyPhase()
    {
        if (randObj.Next(0, 3) != 0)
        {
            _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));
        }
        else
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
    }

    protected override void ActionPhase()
    {
        if (pc.cards.Count != 0)
        {
            _EventBus.Publish<CardUsed>(new CardUsed(pc, pc.cards[0].type));
        } else if (randObj.Next(0, 2) == 0)
        {
            (WeaponType, WeaponAttribute, float)? upgrade = GetRandomAvailableUpgrade(this);
            if (upgrade != null)
            {
                _EventBus.Publish<WeaponUpgraded>(new WeaponUpgraded(pc, upgrade.Value.Item1, upgrade.Value.Item2));
            } else
            {
                _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));
            }
        } else
        {
            _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));
        }
    }

    protected override void AttackPhase()
    {
        //Has a 33% chance of choosing any weapon type
        int rand = randObj.Next(0, 3);
        if (rand == 0)
        {
            _EventBus.Publish<AttackWeaponPicked>(new AttackWeaponPicked(pc, WeaponType.Paper));
        }
        else if (rand == 1)
        {
            _EventBus.Publish<AttackWeaponPicked>(new AttackWeaponPicked(pc, WeaponType.Rock));
        }
        else
        {
            _EventBus.Publish<AttackWeaponPicked>(new AttackWeaponPicked(pc, WeaponType.Scissor));
        }
    }

    protected override void PostAttackPhase(bool isWinner, WeaponType opponentWeapon)
    {
    }
}
