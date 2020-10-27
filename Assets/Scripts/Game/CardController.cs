using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.UIElements;

public struct CardData
{
    public float cost;
    public int priority;
    public Dictionary<CardModificationType, float> modifications;

    public CardData(float _cost, int _priority = 5, Dictionary<CardModificationType, float>  _modifications = default)
    {
        cost = _cost;
        priority = _priority;
        if (_modifications == default)
        {
            modifications = null;
        } else
        {
            modifications = _modifications;
        }
    }
}

public class CardController
{
    private static int _currAvailableCardId = 0;
    public static int currAvailableCardId
    {
        get
        {
            return _currAvailableCardId++;
        }
    }

    public static void UseCard(PlayerController player, BaseCard card)
    {
        player.cards.Remove(card);
        GameController.instance.currentRoundCards[player.player].Add(card);
    }

    public static void DestroyCard(PlayerController player, BaseCard card)
    {
        GameController.instance.activeCards[player.player].Remove(card);
    }

    public class CardIterator
    {
        Player player_t;
        int currCardIndex;

        public CardIterator()
        {
            ResetIterator();
        }

        public void ResetIterator()
        {
            player_t = Player.L;
            currCardIndex = 0;
        }

        public BaseCard GetNextCard()
        {
            if (currCardIndex == GameController.instance.activeCards[player_t].Count)
            {
                currCardIndex = 0;
                if (player_t == Player.L)
                {
                    player_t = Player.R;
                    return GetNextCard();
                }
                else
                {
                    return null;
                }
            }
            return GameController.instance.activeCards[player_t][currCardIndex++];
        }
    }

    //Will grow in size as cards have more abilities
    //Currently describes extraAttack, extraDamage
    public static (float, float) SimulateCardEffect(CardData data)
    {
        (float, float) ret = (0, 0);
        foreach (KeyValuePair<CardModificationType, float> kv in data.modifications)
        {
            if (kv.Key == CardModificationType.IncrementAttack)
            {
                ret.Item1 += kv.Value;
            }
            else if (kv.Key == CardModificationType.IncrementDefense)
            {
                ret.Item2 += kv.Value;
            }
        }
        return ret;
    }
}

public static class CardFactory
{
    public static BaseCard GetCard(PlayerController owner, CardType type)
    {
        switch (type)
        {
            case CardType.SelfAttackIncreaseAdditiveCurrent1:
                return new SelfAttackIncreaseAdditiveCurrent1(owner, GlobalVars.cardData[CardType.SelfAttackIncreaseAdditiveCurrent1]);
            case CardType.SelfDefenseIncreaseAdditiveCurrent1:
                return new SelfDefenseIncreaseAdditiveCurrent1(owner, GlobalVars.cardData[CardType.SelfDefenseIncreaseAdditiveCurrent1]);
            case CardType.SelfDefenseIncreaseAdditiveScissor1:
                return new SelfDefenseIncreaseAdditiveScissor1(owner, GlobalVars.cardData[CardType.SelfDefenseIncreaseAdditiveScissor1]);
            case CardType.OpponentDefenseDecreaseAdditiveScissor1:
                return new OpponentDefenseDecreaseAdditiveScissor1(owner, GlobalVars.cardData[CardType.OpponentDefenseDecreaseAdditiveScissor1]);
            case CardType.OpponentDefenseDecreaseMultScissor1:
                return new OpponentDefenseDecreaseMultScissor1(owner, GlobalVars.cardData[CardType.OpponentDefenseDecreaseMultScissor1]);
        }
        return null;
    }
}

public abstract class BaseCard
{
    public int cardId;

    public int priority = 5; //Lower priority card is issued first

    public float price;

    public CardType type;

    public CardData data;

    public PlayerController owner;

    private int _lifeSpan;
    protected int lifeSpan
    {
        get
        {
            return _lifeSpan;
        }
        set
        {
            _lifeSpan = value;
            if (_lifeSpan <= 0)
            {
                DestroyCard();
            }
        }
    }

    public Subscription<GameStateOver> GameStateOverSubscription;

    public BaseCard(PlayerController _owner, CardData _data)
    {
        owner = _owner;
        cardId = CardController.currAvailableCardId;
        type = CardType.BaseCard;
        lifeSpan = 1; //unused feature at the moment so set to 1
        data = _data;
        GameStateOverSubscription = _EventBus.Subscribe<GameStateOver>(_OnGameStateOver);
    }

    public void _OnGameStateOver(GameStateOver e)
    {
        if (e.prevState == GameState.IssuingAttack)
        {
            DoPostAttackAction();
        }
    }

    public override bool Equals(object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            return obj.GetHashCode() == cardId;
        }
    }

    public override int GetHashCode()
    {
        return cardId;
    }

    virtual public void DoPreAttackAction()
    {
    }

    virtual protected void DoPostAttackAction()
    {
        lifeSpan -= 1;
    }

    protected void DestroyCard()
    {
        _EventBus.Publish<CardDestroyed>(new CardDestroyed(owner, this));
    }
}

public class SelfAttackIncreaseAdditiveCurrent1 : BaseCard
{
    protected float attackIncrease;

    public SelfAttackIncreaseAdditiveCurrent1(PlayerController _owner, CardData _data) : base(_owner, _data)
    {
        type = CardType.SelfAttackIncreaseAdditiveCurrent1;
        price = data.cost;
        priority = data.priority;
        attackIncrease = data.modifications[CardModificationType.IncrementAttack];
    }

    public override void DoPreAttackAction()
    {
        owner.GetCurrentWeaponController().currentAttack += 0.5f;
        base.DoPreAttackAction();
    }

    protected override void DoPostAttackAction()
    {
        base.DoPostAttackAction();
    }
}

public class SelfDefenseIncreaseAdditiveCurrent1 : BaseCard
{
    protected float attackIncrease;

    public SelfDefenseIncreaseAdditiveCurrent1(PlayerController _owner, CardData _data) : base(_owner, _data)
    {
        type = CardType.SelfDefenseIncreaseAdditiveCurrent1;
        price = data.cost;
        priority = data.priority;
    }

    public override void DoPreAttackAction()
    {
        owner.GetCurrentWeaponController().currentDefense += 1f;
        base.DoPreAttackAction();
    }

    protected override void DoPostAttackAction()
    {
        base.DoPostAttackAction();
    }
}

public class SelfDefenseIncreaseAdditiveScissor1 : BaseCard
{
    public SelfDefenseIncreaseAdditiveScissor1(PlayerController _owner, CardData _data) : base(_owner, _data)
    {
        type = CardType.SelfDefenseIncreaseAdditiveScissor1;
        price = data.cost;
        priority = data.priority;
    }

    public override void DoPreAttackAction()
    {
        owner.scissorController.currentDefense += 0.5f;
        base.DoPreAttackAction();
    }

    protected override void DoPostAttackAction()
    {
        base.DoPostAttackAction();
    }
}

public class OpponentDefenseDecreaseAdditiveScissor1 : BaseCard
{
    public OpponentDefenseDecreaseAdditiveScissor1(PlayerController _owner, CardData _data) : base(_owner, _data)
    {
        type = CardType.OpponentDefenseDecreaseAdditiveScissor1;
        price = data.cost;
        priority = data.priority;
    }

    public override void DoPreAttackAction()
    {
        GameController.instance.GetOpponentPlayer(owner).scissorController.currentDefense -= 0.5f;
        base.DoPreAttackAction();
    }

    protected override void DoPostAttackAction()
    {
        base.DoPostAttackAction();
    }
}

public class OpponentDefenseDecreaseMultScissor1 : BaseCard
{
    public OpponentDefenseDecreaseMultScissor1(PlayerController _owner, CardData _data) : base(_owner, _data)
    {
        type = CardType.OpponentDefenseDecreaseMultScissor1;
        price = data.cost;
        priority = data.priority;
    }

    public override void DoPreAttackAction()
    {
        WeaponController scissor = GameController.instance.GetOpponentPlayer(owner).scissorController;
        scissor.currentDefense = scissor.baseDefense / 2f;
        base.DoPreAttackAction();
    }

    protected override void DoPostAttackAction()
    {
        base.DoPostAttackAction();
    }
}
