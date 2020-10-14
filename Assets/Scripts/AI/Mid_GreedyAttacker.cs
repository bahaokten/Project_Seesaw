using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mid_GreedyAttacker : BaseMid_Greedy
{
    protected override void SetInterest()
    {
        attrOfInterest = WeaponAttribute.Attack;
        cardOfInterest = CardType.SelfAttackIncreaseAdditiveCurrent1;
    }
}
