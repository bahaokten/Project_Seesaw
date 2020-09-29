using System;

public class Cards
{
    private static int _currAvailableCardId = 0;
    public static int currAvailableCardId{
        get
        {
            _currAvailableCardId += 1;
            return _currAvailableCardId - 1;
        }
    }

    public static void UseCard(PlayerController player, BaseCard card)
    {
        player.cards.Remove(card);
        GameController.instance.activeCards[player.playerType].Add(card);
        card.DoPreAttackAction();
    }

    public abstract class BaseCard
    {
        public int cardId;

        public int priority = 5; //Lower priority card is issues first

        public int price;

        private int _lifeSpan;
        protected int lifeSpan
        {
            get
            {
                return _lifeSpan;
            }
            set
            {
                lifeSpan = value;
                if (lifeSpan <= 0)
                {
                    DestroyCard();
                }
            }
        }

        public BaseCard()
        {
            cardId = currAvailableCardId;
        }

        public override bool Equals(Object obj)
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

        virtual public void DoPostAttackAction()
        {
            lifeSpan -= 1;
        }

        public void DestroyCard()
        {

        }
    }

    public class SelfAttackIncreaseAdditiveCurrent1 : BaseCard
    {
        public SelfAttackIncreaseAdditiveCurrent1()
        {
            price = 2;
        }

        public override void DoPreAttackAction()
        {
            GameController.instance.GetPlayer(GameController.currPlayer).GetCurrentWeaponController().currentAttack += 0.5f;
            base.DoPreAttackAction();
        }

        public override void DoPostAttackAction()
        {
            base.DoPostAttackAction();
        }
    }

    public class SelfDefenseIncreaseAdditiveScissor1 : BaseCard
    {
        public SelfDefenseIncreaseAdditiveScissor1()
        {
            price = 2;
        }

        public override void DoPreAttackAction()
        {
            GameController.instance.GetPlayer(GameController.currPlayer).scissorController.currentDefense += 0.5f;
            base.DoPreAttackAction();
        }

        public override void DoPostAttackAction()
        {
            base.DoPostAttackAction();
        }
    }

    public class OpponentDefenseDecreaseAdditiveScissor1 : BaseCard
    {
        public OpponentDefenseDecreaseAdditiveScissor1()
        {
            price = 3;
        }

        public override void DoPreAttackAction()
        {
            GameController.instance.GetOpponentPlayer().scissorController.currentDefense -= 0.5f;
            base.DoPreAttackAction();
        }

        public override void DoPostAttackAction()
        {
            base.DoPostAttackAction();
        }
    }

    public class OpponentDefenseDecreaseMultScissor1 : BaseCard
    {
        public OpponentDefenseDecreaseMultScissor1()
        {
            priority = 3;
            price = 3;
        }

        public override void DoPreAttackAction()
        {
            WeaponController scissor = GameController.instance.GetOpponentPlayer().scissorController;
            scissor.currentDefense = scissor.baseDefense / 2f;
            base.DoPreAttackAction();
        }

        public override void DoPostAttackAction()
        {
            base.DoPostAttackAction();
        }
    }

}
