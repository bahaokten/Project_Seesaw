using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventHandler : MonoBehaviour
{
    GameEventHandler instance;

    public Subscription<TurnPhaseChanged> TurnPhaseChangedSubscription;
    public Subscription<CardPurchased> CardPurchasedSubscription;
    public Subscription<CardUsed> CardUsedSubscription;
    public Subscription<WeaponUpgraded> WeaponUpgradedSubscription;
    public Subscription<AttackWeaponPicked> AttackWeaponPickedSubscription;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        TurnPhaseChangedSubscription = _EventBus.Subscribe<TurnPhaseChanged>(_OnTurnPhaseChanged);
        CardPurchasedSubscription = _EventBus.Subscribe<CardPurchased>(_OnCardPurchased);
        CardUsedSubscription = _EventBus.Subscribe<CardUsed>(_OnCardUsed);
        WeaponUpgradedSubscription = _EventBus.Subscribe<WeaponUpgraded>(_OnWeaponUpgraded);
        AttackWeaponPickedSubscription = _EventBus.Subscribe<AttackWeaponPicked>(_OnAttackWeaponPicked);
    }

    private void OnDisable()
    {
        _EventBus.Unsubscribe<TurnPhaseChanged>(TurnPhaseChangedSubscription);
        _EventBus.Unsubscribe<CardPurchased>(CardPurchasedSubscription);
        _EventBus.Unsubscribe<CardUsed>(CardUsedSubscription);
        _EventBus.Unsubscribe<WeaponUpgraded>(WeaponUpgradedSubscription);
        _EventBus.Unsubscribe<AttackWeaponPicked>(AttackWeaponPickedSubscription);
    }

    private void OnEnable()
    {
        if (TurnPhaseChangedSubscription == null)
        {
            TurnPhaseChangedSubscription = _EventBus.Subscribe<TurnPhaseChanged>(_OnTurnPhaseChanged);
        }
        if (CardPurchasedSubscription == null)
        {
            CardPurchasedSubscription = _EventBus.Subscribe<CardPurchased>(_OnCardPurchased);
        }
        if (CardUsedSubscription == null)
        {
            CardUsedSubscription = _EventBus.Subscribe<CardUsed>(_OnCardUsed);
        }
        if (WeaponUpgradedSubscription == null)
        {
            WeaponUpgradedSubscription = _EventBus.Subscribe<WeaponUpgraded>(_OnWeaponUpgraded);
        }
        if (AttackWeaponPickedSubscription == null)
        {
            AttackWeaponPickedSubscription = _EventBus.Subscribe<AttackWeaponPicked>(_OnAttackWeaponPicked);
        }
    }

    void _OnTurnPhaseChanged(TurnPhaseChanged e)
    {
        GameController.instance.currTurnPhase = e.phase;
    }
    
    void _OnCardPurchased(CardPurchased e)
    {
        BaseCard card = CardFactory.GetCard(e.type);
        e.player.coins -= card.price;
        e.player.cards.Add(card);
    }  
    
    void _OnCardUsed(CardUsed e)
    {
        foreach (BaseCard card in e.player.cards)
        {
            if (card.type == e.type)
            {
                CardController.UseCard(e.player, card);
            }
        }
    }

    void _OnWeaponUpgraded(WeaponUpgraded e)
    {
        e.player.UpgradeWeapon(e.type, e.attr);
    }

    void _OnAttackWeaponPicked(AttackWeaponPicked e)
    {

    }


}
