using System;

public abstract class BaseMid_Greedy : BaseAI
{
    protected PlayerController opponentPc;
    protected bool canBuyUpgrade = false;
    protected WeaponType upgradableWeaponType;

    protected bool cardUsed = false;

    protected WeaponAttribute attrOfInterest;
    protected CardType cardOfInterest;

    protected virtual void SetInterest()
    {

    }

    protected override void Initialize()
    {
        opponentPc = GameController.instance.GetOpponentPlayer(pc);
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
                cardUsed = true;
            } else
            {
                _EventBus.Publish<EndTurnPhase>(new EndTurnPhase(pc));
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
        _EventBus.Publish<AttackWeaponPicked>(new AttackWeaponPicked(pc, attackType));
    }

    protected override void PostAttackPhase(bool isWinner, WeaponType opponentWeapon)
    {
        canBuyUpgrade = false;
        cardUsed = false;
    }
}
