using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Tracks opponent's pick habits, and tries to guess what they are going to choose
/// Picks the weapon with winning matchup with bias to choose 1 specific weapon if winning ratios are too close
/// Prioritizes Upgrading picked weapon's random attribute
/// If current weapon cannot be upgraded, tries to buy and use a card
/// </summary>
public class Mid_Tracker2 : Mid_Tracker
{
    WeaponType biasedWeapon = WeaponType.Paper;
    (int, int) biasAdditionPercentageRange = (30, 37); //Slightly below and above random margin
    protected override void BuyPhase()
    {
        List<(WeaponType, float)> opponentWeaponPrediction = new List<(WeaponType, float)>() 
        { 
            (WeaponController.GetStrongType(biasedWeapon), randObj.Next(biasAdditionPercentageRange.Item1, biasAdditionPercentageRange.Item2) / 100) 
        };

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

}
