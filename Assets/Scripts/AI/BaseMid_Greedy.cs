using System;
using System.Collections.Generic;
using UnityEngine.XR;

/// <summary>
/// Chooses weapon, upgrades, and cards that will yield the most coins if won.
/// </summary>
public abstract class BaseMid_Greedy : BaseAI
{
    protected PlayerController opponentPc;
    protected bool canBuyUpgrade = false;
    protected WeaponType upgradableWeaponType;

    protected List<CardData> currRoundCards;

    protected WeaponAttribute attrOfInterest;
    protected CardType cardOfInterest;

    protected virtual void SetInterest()
    {

    }

    protected override void Initialize()
    {
        opponentPc = GameController.instance.GetOpponentPlayer(pc);
        currRoundCards = new List<CardData>();
        SetInterest();
    }

    protected override void BuyPhase()
    {
        float bestUpgradePrice = 10000;
        foreach (WeaponType type in Enum.GetValues(typeof(WeaponType)))
        {
            if (pc.CanUpgradeWeapon(type, attrOfInterest))
            {
                float upgradePrice = pc.weapons[type].GetUpgradePrice(attrOfInterest);
                if (upgradePrice < bestUpgradePrice)
                {
                    bestUpgradePrice = upgradePrice;
                    canBuyUpgrade = true;
                    upgradableWeaponType = type;
                }
            }
        }
        if (!canBuyUpgrade && pc.coins > 4)
        {
            _EventBus.Publish<CardPurchased>(new CardPurchased(pc, cardOfInterest));
        } else
        {
            _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));
        }
    }

    protected override void ActionPhase()
    {
        if (canBuyUpgrade)
        {
            _EventBus.Publish<WeaponUpgraded>(new WeaponUpgraded(pc, upgradableWeaponType, attrOfInterest));
        }
        else
        {
            if (pc.cards.Count != 0)
            {
                _EventBus.Publish<CardUsed>(new CardUsed(pc, cardOfInterest));
                currRoundCards.Add(GlobalVars.cardData[cardOfInterest]);
            } else
            {
                _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));
            }
        }
    }

    protected override void AttackPhase()
    {
        float highestDamage = -1000;
        WeaponType attackType = WeaponType.Scissor;

        Dictionary<Player, List<CardData>> cards = new Dictionary<Player, List<CardData>>() { { pc.player, currRoundCards } };

        foreach (WeaponType type in Enum.GetValues(typeof(WeaponType)))
        {
            Dictionary<Player, WeaponType> pWeapon = new Dictionary<Player, WeaponType>();
            pWeapon[pc.player] = type;

            //Same weapon
            pWeapon[GameController.instance.GetOpponentPlayerType()] = type;
            WinnerData data1 = GameController.instance.SimulateRound(pWeapon, cards);
            if (data1.winner != pc.player)
            {
                data1.winnerAttDefDiff = 0;
            }

            //Opponent picks strong type
            pWeapon[GameController.instance.GetOpponentPlayerType()] = WeaponController.GetStrongType(type);
            WinnerData data2 = GameController.instance.SimulateRound(pWeapon, cards);

            //Average both scenarios
            if ((data1.winnerAttDefDiff + data2.winnerAttDefDiff ) /2  > highestDamage)
            {
                highestDamage = (data1.winnerAttDefDiff + data2.winnerAttDefDiff) / 2;
                attackType = type;
            }
        }
        _EventBus.Publish<AttackWeaponPicked>(new AttackWeaponPicked(pc, attackType));
    }

    protected override void PostAttackPhase(bool isWinner, WeaponType opponentWeapon)
    {
        canBuyUpgrade = false;
        currRoundCards.Clear();
    }
}
