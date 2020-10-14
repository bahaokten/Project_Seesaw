using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mid_GreedyMixed : BaseMid_Greedy
{
    protected override void SetInterest()
    {
        return;
    }

    protected override void BuyPhase()
    {
        RandomizeInterest();
        base.BuyPhase();
    }

    private void RandomizeInterest()
    {
        int type = randObj.Next(0, 2);
        if (type == 0)
        {
            attrOfInterest = WeaponAttribute.Defense;
            cardOfInterest = CardType.SelfDefenseIncreaseAdditiveCurrent1;
        } else
        {
            attrOfInterest = WeaponAttribute.Attack;
            cardOfInterest = CardType.SelfAttackIncreaseAdditiveCurrent1;
        }
    }
}
