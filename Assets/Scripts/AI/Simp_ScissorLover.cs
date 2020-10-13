using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simp_ScissorLover : BaseAI
{
    protected override void Initialize()
    {
    }

    protected override void BuyPhase()
    {
        //Buys Nothing
        _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));
    }

    protected override void ActionPhase()
    {
        //If possible, always upgrades scissor's attack
        if (pc.CanUpgradeWeapon(WeaponType.Scissor, WeaponAttribute.Attack))
        {
            _EventBus.Publish<WeaponUpgraded>(new WeaponUpgraded(pc, WeaponType.Scissor, WeaponAttribute.Attack));
        }
    }

    protected override void AttackPhase()
    {
        //Has a 10% change of choosing rock and 10% chance of choosing paper, otherwise chooses scissor 
        int rand = randObj.Next(0, 100);
        if (rand < 10)
        {
            _EventBus.Publish<AttackWeaponPicked>(new AttackWeaponPicked(pc, WeaponType.Paper));
        }
        else if (10 <= rand && rand < 20)
        {
            _EventBus.Publish<AttackWeaponPicked>(new AttackWeaponPicked(pc, WeaponType.Rock));
        }
        else
        {
            _EventBus.Publish<AttackWeaponPicked>(new AttackWeaponPicked(pc, WeaponType.Scissor));
        }
    }

    protected override void PostAttackPhase(bool isWinner)
    {
    }
}
