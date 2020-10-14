using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mid_GreedyDefender : BaseMid_Greedy
{
    protected override void SetInterest()
    {
        attrOfInterest = WeaponAttribute.Defense;
        cardOfInterest = CardType.SelfDefenseIncreaseAdditiveCurrent1;
    }
}
