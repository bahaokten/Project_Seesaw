using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Never tries to buy a card
/// Tries to buy an upgrade 33% of the time if the player has less than 5 coins
/// Tries to buy an upgrade 100% of the time (prefers attack upgrade) if player has 5 or more coins
/// Chooses each weapon with 33.3% chance
/// </summary>
public class Simp_RandomUpgrade : Simp_Random
{

    protected override void BuyPhase()
    {
        _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));
    }

    protected override void ActionPhase()
    {
        if (pc.coins < 5)
        {
            if (randObj.Next(0, 3) == 0)
            {
                (WeaponType, WeaponAttribute, float)? upgrade = GetRandomAvailableUpgrade(this);
                if (upgrade != null)
                {
                    _EventBus.Publish<WeaponUpgraded>(new WeaponUpgraded(pc, upgrade.Value.Item1, upgrade.Value.Item2));
                }
                else
                {
                    _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));
                }
            }
            else
            {
                _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));
            }
        }
        else
        {
            List<(WeaponType, WeaponAttribute, float)> upgrades = GetAvailableUpgrades(this);
            if (upgrades.Count != 0)
            {
                foreach ((WeaponType, WeaponAttribute, float) w in upgrades) 
                {
                    if (w.Item2 == WeaponAttribute.Attack)
                    {
                        _EventBus.Publish<WeaponUpgraded>(new WeaponUpgraded(pc, w.Item1, w.Item2));
                        return;
                    }
                }

                (WeaponType, WeaponAttribute, float) randw = upgrades[randObj.Next(0, upgrades.Count)];
                _EventBus.Publish<WeaponUpgraded>(new WeaponUpgraded(pc, randw.Item1, randw.Item2));
            }
            else
            {
                _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));
            }
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
