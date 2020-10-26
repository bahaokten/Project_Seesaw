using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Simp_Tracker : BaseAI
{
    Queue<WeaponType> opponentPicks;
    int trackSize = 10;

    protected Dictionary<WeaponType, float> GetOpponentWeaponRatios()
    {
        Dictionary<WeaponType, float> ret = new Dictionary<WeaponType, float>() 
        {
            {WeaponType.Scissor, 0 },
            {WeaponType.Paper, 0 },
            {WeaponType.Rock, 0 }
        };

        foreach (WeaponType t in opponentPicks)
        {
            ret[t] += 1;
        }

        ret[WeaponType.Scissor] /= opponentPicks.Count;
        ret[WeaponType.Paper] /= opponentPicks.Count;
        ret[WeaponType.Rock] /= opponentPicks.Count;

        return ret;
    }

    protected override void Initialize()
    {
        opponentPicks = new Queue<WeaponType>();
    }

    protected override void BuyPhase()
    {
    }

    protected override void ActionPhase()
    {
    }

    protected override void AttackPhase()
    {
    }

    protected override void PostAttackPhase(bool isWinner, WeaponType opponentWeapon)
    {
        if (opponentPicks.Count == trackSize)
        {
            opponentPicks.Dequeue();
        }

        opponentPicks.Enqueue(opponentWeapon);
    }
}
