using System;

public static class PlayerAPI
{
    //=========== BUY PHASE ===========
    public static bool BuyCard(PlayerController player, Type card_t)
    {
        Cards.BaseCard card = (Cards.BaseCard)Activator.CreateInstance(card_t);

        if (GameController.instance.currTurnPhase != TurnPhase.BuyPhase || card.price > player.coins)
        {
            return false;
        }

        player.coins -= card.price;
        player.cards.Add(card);
        return true;
    }

    public static bool EndBuyPhase()
    {
        if (GameController.instance.currTurnPhase == TurnPhase.BuyPhase)
        {
            GameController.instance.currTurnPhase = TurnPhase.ActionPhase;
            return true;
        }

        return false;
    }

    //=========== ACTION PHASE ===========
    public static bool UseCard(PlayerController player, Type card_t)
    {
        if (GameController.instance.currTurnPhase != TurnPhase.ActionPhase)
        {
            return false;
        }

        if (UseCard(player, card_t))
        {
            return EndActionPhase();
        }
        //Use Card failed
        return false;
    }

    private static bool _UseCard(PlayerController player, Type card_t)
    {
        foreach (Cards.BaseCard card in player.cards)
        {
            if (card.GetType() == card_t)
            {
                Cards.UseCard(player, card);
                return true;
            }
        }

        return false;
    }

    public static bool UpgradeWeapon(PlayerController player, WeaponType weapon_t, WeaponAttribute attr)
    {
        if (GameController.instance.currTurnPhase != TurnPhase.ActionPhase)
        {
            return false;
        }

        if (_UpgradeWeapon(player, weapon_t, attr))
        {
            return EndActionPhase();
        }
        //Upgrade Weapon failed
        return false;
    }

    private static bool _UpgradeWeapon(PlayerController player, WeaponType weapon_t, WeaponAttribute attr)
    {
        WeaponController weapon = player.GetWeapon(weapon_t);
        int upgradePrice = weapon.GetUpgradePrice(attr);
        if (upgradePrice >= 0 && player.coins >= upgradePrice)
        {
            return weapon.Upgrade(attr);
        }
        return false;
    }

    public static bool EndActionPhase()
    {
        if (GameController.instance.currTurnPhase == TurnPhase.ActionPhase)
        {
            GameController.instance.currTurnPhase = TurnPhase.AttackPhase;
            return true;
        }

        return false;
    }

    //=========== ATTACK PHASE ===========

    public static bool PickAttack(WeaponType weapon_t)
    {
        if (GameController.instance.currTurnPhase != TurnPhase.AttackPhase)
        {
            return false;
        }

        GameController.instance.PlayerTurnOver(weapon_t);

        return false;
    }
}
