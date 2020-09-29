using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAPI : MonoBehaviour
{
    //BUY PHASE
    public static bool BuyCard(PlayerController player, Type card_t)
    {
        Cards.BaseCard card = (Cards.BaseCard)Activator.CreateInstance(card_t);

        if (GameController.currPhase != GamePhase.BuyPhase || card.price > player.coins)
        {
            return false;
        }

        player.coins -= card.price;
        player.cards.Add(card);
        return true;
    }

    public static bool EndBuyPhase()
    {
        if (GameController.currPhase == GamePhase.BuyPhase)
        {
            GameController.currPhase = GamePhase.ActionPhase;
            return true;
        }

        return false;
    }

    //ACTION PHASE
    public static bool UseCard(PlayerController player, Type card_t)
    {
        if (GameController.currPhase != GamePhase.ActionPhase)
        {
            return false;
        }

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

    public static bool UpgradeWeapon(PlayerController player, WeaponType weapon)
    {
        return false;
    }
}
