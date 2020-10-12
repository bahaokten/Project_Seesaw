using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UseItemController : MonoBehaviour
{
    public CardType type;
    public Image image;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public Button button;


    public void Initialize(CardType _type)
    {
        type = _type;
        Card cardUIVars = GlobalVars.instance.cardUIData[type].GetComponent<Card>();
        image.sprite = cardUIVars.image;
        title.text = cardUIVars.title;
        description.text = cardUIVars.description;
    }

    public void DoCardUsed()
    {
        _EventBus.Publish<CardUsed>(new CardUsed(GameController.instance.GetCurrentPlayer(), type));
        button.interactable = false;
        _EventBus.Publish<MenuStateChanged>(new MenuStateChanged(MenuState.AttackPhaseMenu));
    }
}
