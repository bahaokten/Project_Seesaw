using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mid_GreedyAttacker : BaseAI
{
    PlayerController opponentPc;
    bool canBuyUpgrade = false;
    WeaponType upgradableWeaponType;

    bool cardUsed = false;

    protected override void Initialize()
    {
        opponentPc = GameController.instance.GetOpponentPlayer(pc);
    }

    protected override void BuyPhase()
    {
        float bestUpgradePrice = 10000;
        foreach (WeaponType type in Enum.GetValues(typeof(WeaponType)))
        {
            if (pc.CanUpgradeWeapon(type, WeaponAttribute.Attack))
            {
                float upgradePrice = pc.weapons[type].GetUpgradePrice(WeaponAttribute.Attack);
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
            _EventBus.Publish<CardPurchased>(new CardPurchased(pc, CardType.SelfAttackIncreaseAdditiveCurrent1));
        }
    }

    protected override void ActionPhase()
    {
        if (canBuyUpgrade)
        {
            _EventBus.Publish<WeaponUpgraded>(new WeaponUpgraded(pc, upgradableWeaponType, WeaponAttribute.Attack));
        }
        else
        {
            if (pc.cards.Count != 0)
            {
                print("usingCard");
                _EventBus.Publish<CardUsed>(new CardUsed(pc, CardType.SelfAttackIncreaseAdditiveCurrent1));
                cardUsed = true;
            }
        }
    }

    protected override void AttackPhase()
    {
        float highestDamage = -1000;
        float cardDamageAdd = 0;
        WeaponType attackType = WeaponType.Scissor;

        if (cardUsed)
        {
            cardDamageAdd = 0.5f;
        }

        foreach (WeaponType type in Enum.GetValues(typeof(WeaponType)))
        {
            float currDamage = pc.GetWeapon(type).baseAttack + cardDamageAdd - opponentPc.GetWeapon(pc.GetWeapon(type).GetStrongType()).baseDefense;
            if (currDamage > highestDamage)
            {
                highestDamage = currDamage;
                attackType = type;
            }
        }
        print(highestDamage);
        _EventBus.Publish<AttackWeaponPicked>(new AttackWeaponPicked(pc, attackType));
    }

    protected override void PostAttackPhase(bool isWinner)
    {
        canBuyUpgrade = false;
        cardUsed = false;
    }
}
