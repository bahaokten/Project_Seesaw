using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Has 33% chance each round to attempt buying a random card IF no upgrades can be bought
/// Always attempts to upgrade scissor attack and defense
/// Has a 100% chance of choosing scissor
/// </summary>
public class Simp_ScissorLover2 : Simp_ScissorLover
{
    protected override void Initialize()
    {
    }

    protected override void BuyPhase()
    {
        if (pc.CanUpgradeWeapon(WeaponType.Scissor, WeaponAttribute.Attack) || pc.CanUpgradeWeapon(WeaponType.Scissor, WeaponAttribute.Defense))
        {
            _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));
            return;
        }
        if (randObj.Next(0, 3) > 0)
        {
            _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));
        }
        else
        {
            (CardType, CardData)? card = GetRandomAvailableCard(this);
            if (card != null)
            {
                _EventBus.Publish<CardPurchased>(new CardPurchased(pc, card.Value.Item1));
            }
            else
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
        }
        else if (pc.CanUpgradeWeapon(WeaponType.Scissor, WeaponAttribute.Attack))
        {
            _EventBus.Publish<WeaponUpgraded>(new WeaponUpgraded(pc, WeaponType.Scissor, WeaponAttribute.Attack));
        } 
        else if (pc.CanUpgradeWeapon(WeaponType.Scissor, WeaponAttribute.Defense))
        {
            _EventBus.Publish<WeaponUpgraded>(new WeaponUpgraded(pc, WeaponType.Scissor, WeaponAttribute.Defense));
        } 
        else
        {
            _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));
        }
    }

    protected override void AttackPhase()
    {
        _EventBus.Publish<AttackWeaponPicked>(new AttackWeaponPicked(pc, WeaponType.Scissor));
    }

    protected override void PostAttackPhase(bool isWinner, WeaponType opponentWeapon)
    {
    }
}
