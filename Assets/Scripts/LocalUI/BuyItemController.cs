using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyItemController : MonoBehaviour
{
    public CardType type;
    public Image image;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public TextMeshProUGUI cost;
    public Button button;

    public Subscription<CardPurchased> CardPurchasedSubscription;

    void Start()
    {
        Card cardUIVars = GlobalVars.instance.cardUIData[type].GetComponent<Card>();
        image.sprite = cardUIVars.image;
        title.text = cardUIVars.title;
        description.text = cardUIVars.description;
        cost.text = GlobalVars.cardData[type].cost + GlobalVars.CURRENCY_SUFFIX;

        CheckAvailibility();
    }

    private void OnEnable()
    {
        CardPurchasedSubscription = _EventBus.Subscribe<CardPurchased>(_OnOtherCardPurchased);
    }

    private void OnDisable()
    {
        _EventBus.Unsubscribe<CardPurchased>(CardPurchasedSubscription);
    }

    void CheckAvailibility()
    {
        if (GameController.instance.GetCurrentPlayer().coins < GlobalVars.cardData[type].cost)
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }

    public void _OnOtherCardPurchased(CardPurchased e)
    {
        CheckAvailibility();
    }

    public void DoCardPurchased()
    {
        _EventBus.Publish<CardPurchased>(new CardPurchased(GameController.instance.GetCurrentPlayer(), type));
    }
}
