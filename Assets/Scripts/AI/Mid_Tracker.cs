using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Tracks opponent's pick habits, and tries to guess what they are going to choose
/// Picks the weapon with winning matchup
/// Prioritizes Upgrading picked weapon's random attribute
/// If current weapon cannot be upgraded, tries to buy and use a card
/// </summary>
public class Mid_Tracker : BaseAI
{
    protected Queue<WeaponType> opponentPicks;
    protected int trackSize = 10;

    protected WeaponType currentPick;
    protected (WeaponType, WeaponAttribute, float)? upgradePick = null;

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
        List<(WeaponType, float)> opponentWeaponPrediction = new List<(WeaponType, float)>() { (WeaponType.Scissor, -1), (WeaponType.Paper, -1), (WeaponType.Rock, -1) };

        foreach (KeyValuePair<WeaponType, float> kv in GetOpponentWeaponRatios())
        {
            if (kv.Value > opponentWeaponPrediction[0].Item2)
            {
                opponentWeaponPrediction.Clear();
                opponentWeaponPrediction.Add((kv.Key, kv.Value));
            } else if (kv.Value == opponentWeaponPrediction[0].Item2)
            {
                opponentWeaponPrediction.Add((kv.Key, kv.Value));
            }
        }
        //Pick what weapon to play
        currentPick = WeaponController.GetWeakType(opponentWeaponPrediction[randObj.Next(0, opponentWeaponPrediction.Count())].Item1);

        (CardType, CardData)? availCard = GetRandomAvailableCard(this);
        List<(WeaponType, WeaponAttribute, float)> availUpgrades = GetAvailableUpgrades(this).Where(x => x.Item1 == currentPick).ToList();

        if (availUpgrades.Count != 0)
        {
            upgradePick = availUpgrades[randObj.Next(0, availUpgrades.Count)];
            _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));
        } 
        else if (availCard != null)
        {
            _EventBus.Publish<CardPurchased>(new CardPurchased(pc, availCard.Value.Item1));
        } 
        else
        {
            _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));
        }
    }

    protected override void ActionPhase()
    {
        if (pc.cards.Count != 0)
        {
            _EventBus.Publish<CardUsed>(new CardUsed(pc, pc.cards[0].type));
        } else if (upgradePick != null)
        {
            _EventBus.Publish<WeaponUpgraded>(new WeaponUpgraded(pc, upgradePick.Value.Item1, upgradePick.Value.Item2));
        } else
        {
            _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));
        }
    }

    protected override void AttackPhase()
    {
        _EventBus.Publish<AttackWeaponPicked>(new AttackWeaponPicked(pc, currentPick));
    }

    protected override void PostAttackPhase(bool isWinner, WeaponType opponentWeapon)
    {
        if (opponentPicks.Count == trackSize)
        {
            opponentPicks.Dequeue();
        }

        opponentPicks.Enqueue(opponentWeapon);

        upgradePick = null;
    }
}
