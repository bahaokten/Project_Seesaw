using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simp_ScissorLover : BaseAI
{

    protected override IEnumerator DoTurn()
    {
        yield return 0;

        //=== BUY PHASE ===
        _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));

        //=== ACTION PHASE ===
        if (pc.CanUpgradeWeapon(WeaponType.Scissor, WeaponAttribute.Attack))
        {
            _EventBus.Publish<WeaponUpgraded>(new WeaponUpgraded(pc, WeaponType.Scissor, WeaponAttribute.Attack));
        }

        //=== ATTACK PHASE ===
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
}
