using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardIterator = CardController.CardIterator;

public class MenuController : MonoBehaviour
{
    public static MenuController instance;

    public GameObject nonUIPlayerPlaying;
    public GameObject buyPhaseMenu;
    public GameObject buyMenu;
    public GameObject actionPhaseMenu;
    public GameObject useMenu;
    public GameObject upgradeMenu;
    public GameObject attackPhaseMenu;
    public GameObject animateAttackWindow;

    public Dictionary<string, Vector3> menuLocTable;

    private MenuState _currMenu;
    public MenuState currMenu
    {
        get
        {
            return _currMenu;
        }
        set
        {
            if (value == _currMenu)
            {
                return;
            }
            _currMenu = value;
        }
    }
    [HideInInspector]
    public GameObject currMenuObj;

    public float swapDuration = 0.4f;

    Subscription<MenuStateChanged> MenuStateChangedSubscription;

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

        MenuStateChangedSubscription = _EventBus.Subscribe<MenuStateChanged>(_OnMenuStateChange);
    }

    void Start()
    {
        currMenu = MenuState.BuyPhaseMenu;
        currMenuObj = buyPhaseMenu;
        menuLocTable = new Dictionary<string, Vector3>
        {
            { nonUIPlayerPlaying.name, nonUIPlayerPlaying.transform.position },
            { buyPhaseMenu.name, buyPhaseMenu.transform.position },
            { buyMenu.name, buyMenu.transform.position },
            { actionPhaseMenu.name, actionPhaseMenu.transform.position },
            { useMenu.name, useMenu.transform.position },
            { upgradeMenu.name, upgradeMenu.transform.position },
            { attackPhaseMenu.name, attackPhaseMenu.transform.position },
            { animateAttackWindow.name, animateAttackWindow.transform.position }
        };
    }

    void Update()
    {

    }

    void OnDestroy()
    {
        _EventBus.Unsubscribe<MenuStateChanged>(MenuStateChangedSubscription);
    }

    private MenuState StringToMenuState(string s)
    {
        switch (s)
        {
            default:
                return MenuState.BuyPhaseMenu;
            case "buyPhaseMenu":
                return MenuState.BuyPhaseMenu;
            case "buyMenu":
                return MenuState.BuyMenu;    
            case "actionPhaseMenu":
                return MenuState.ActionPhaseMenu;
            case "useMenu":
                return MenuState.UseMenu;
            case "upgradeMenu":
                return MenuState.UpgradeMenu;
            case "attackPhaseMenu":
                return MenuState.AttackPhaseMenu;
            case "nonUIPlayerPlaying":
                return MenuState.NonUIPlayerPlaying;
            case "animateAttack":
                return MenuState.AnimatingAttack;
        }
    }

    public void DoMenuStateChange(string action)
    {
        MenuState newState = StringToMenuState(action);
        if (newState == currMenu)
        {
            return;
        }
        if (newState == MenuState.BuyPhaseMenu)
        {
            _EventBus.Publish<TurnPhaseChanged>(new TurnPhaseChanged(null, TurnPhase.BuyPhase));
        } else if (newState == MenuState.ActionPhaseMenu)
        {
            _EventBus.Publish<TurnPhaseChanged>(new TurnPhaseChanged(null, TurnPhase.ActionPhase));
        }
        else if (newState == MenuState.AttackPhaseMenu)
        {
            _EventBus.Publish<TurnPhaseChanged>(new TurnPhaseChanged(null, TurnPhase.AttackPhase));
        }
        _EventBus.Publish<MenuStateChanged>(new MenuStateChanged(newState));
    }

    public void DoAttackChoice(string action)
    {
        if (GameController.instance.currTurnPhase != TurnPhase.AttackPhase)
        {
            return;
        }

        switch (action)
        {
            case "scissor":
                _EventBus.Publish<AttackWeaponPicked>(new AttackWeaponPicked(null, WeaponType.Scissor));
                break;
            case "paper":
                _EventBus.Publish<AttackWeaponPicked>(new AttackWeaponPicked(null, WeaponType.Paper));
                break;
            case "rock":
                _EventBus.Publish<AttackWeaponPicked>(new AttackWeaponPicked(null, WeaponType.Rock));
                break;
        }

    }

    // EVENT LISTENERS

    public void _OnMenuStateChange(MenuStateChanged e)
    {
        if (e.state == currMenu)
        {
            return;
        }

        switch (e.state)
        {
            case MenuState.NonUIPlayerPlaying:
                currMenu = MenuState.NonUIPlayerPlaying;
                DoSwap(currMenuObj, nonUIPlayerPlaying);
                break;
            case MenuState.BuyPhaseMenu:
                currMenu = MenuState.BuyPhaseMenu;
                StartCoroutine(DoSwapAnimation(currMenuObj, buyPhaseMenu));
                break;
            case MenuState.BuyMenu:
                currMenu = MenuState.BuyMenu;
                StartCoroutine(DoSwapAnimation(currMenuObj, buyMenu));
                break;
            case MenuState.ActionPhaseMenu:
                currMenu = MenuState.ActionPhaseMenu;
                StartCoroutine(DoSwapAnimation(currMenuObj, actionPhaseMenu));
                break;
            case MenuState.UseMenu:
                currMenu = MenuState.UseMenu;
                StartCoroutine(DoSwapAnimation(currMenuObj, useMenu));
                break;
            case MenuState.UpgradeMenu:
                currMenu = MenuState.UpgradeMenu;
                StartCoroutine(DoSwapAnimation(currMenuObj, upgradeMenu));
                break;
            case MenuState.AttackPhaseMenu:
                currMenu = MenuState.AttackPhaseMenu;
                StartCoroutine(DoSwapAnimation(currMenuObj, attackPhaseMenu));
                break;
            case MenuState.AnimatingAttack:
                currMenu = MenuState.AnimatingAttack;
                StartCoroutine(DoSwapOutPopInAnimation(currMenuObj, animateAttackWindow));
                break;
        }
    }

    //ANIMATORS

    void AnimateExit(GameObject obj, float time = 2f)
    {
        LeanTween.move(obj, new Vector3(-1300, obj.transform.position.y, 0), time).setEase(LeanTweenType.clamp);
    }

    void AnimateEntry(GameObject obj, float time = 2f)
    {
        obj.transform.position = new Vector3(-1300, obj.transform.position.y, 0);
        LeanTween.move(obj, menuLocTable[obj.name], time).setEase(LeanTweenType.clamp);
    }
    private void DoSwap(GameObject exitingObj, GameObject enteringObj)
    {
        exitingObj.SetActive(false);

        currMenuObj = enteringObj;

        enteringObj.SetActive(true);
    }

    IEnumerator DoSwapOutPopInAnimation(GameObject exitingObj, GameObject enteringObj)
    {
        if (exitingObj == enteringObj)
        {
            yield break;
        }
        AnimateExit(exitingObj, swapDuration / 2);
        yield return new WaitForSeconds(swapDuration / 2);

        enteringObj.transform.position = menuLocTable[enteringObj.name];
        DoSwap(exitingObj, enteringObj);
    }

    IEnumerator DoSwapAnimation(GameObject exitingObj, GameObject enteringObj)
    {
        if (exitingObj == enteringObj)
        {
            yield break;
        }
        AnimateExit(exitingObj, swapDuration / 2);
        yield return new WaitForSeconds(swapDuration / 2);

        DoSwap(exitingObj, enteringObj);

        AnimateEntry(enteringObj, swapDuration / 2);
    }

    static float weaponFlyTime = 1f;
    static float weaponSizeChangeTime = 0.2f;
    static float weaponFlyBounceBackTime = 0.5f;
    static float weaponPreAttack = 0.5f;
    static float winningTextDisplayTime = 3f;
    static float winnerScale = 1.2f;
    static float loserScale = 0.8f;

    public IEnumerator AnimateAttack()
    {
        MenuController.instance.DoMenuStateChange("animateAttack");
        yield return new WaitForSeconds(swapDuration/2 + 0.1f);

        // === MOVE PHASE ===
        WeaponController weaponL = GameController.instance.playerControllerL.GetCurrentWeaponController();
        Transform tL = weaponL.weaponVisualObj.transform;
        LeanTween.move(tL.gameObject, new Vector3(-5, tL.position.y, 0), weaponFlyTime).setEase(LeanTweenType.easeInOutExpo);

        WeaponController weaponR = GameController.instance.playerControllerR.GetCurrentWeaponController();
        Transform tR = weaponR.weaponVisualObj.transform;
        LeanTween.move(tR.gameObject, new Vector3(5, tR.position.y, 0), weaponFlyTime).setEase(LeanTweenType.easeInOutExpo);
        yield return new WaitForSeconds(weaponFlyTime + 0.1f);

        // === APPLY CARDS PHASE ===
        CardIterator cardIt = new CardIterator();
        BaseCard nextCard = cardIt.GetNextCard();
        while (nextCard != null)
        {
            //DO CARDS
            nextCard.DoPreAttackAction();
            nextCard = cardIt.GetNextCard();
        }

        // === WIN PHASE ===
        Tuple<Player, float> winnerData = GameController.instance.DetermineWinner();
        float coins = GameController.CalculateCoins(winnerData.Item2);
        yield return new WaitForSeconds(weaponPreAttack);

        Vector3 tlScale = tL.localScale;
        Vector3 tRScale = tR.localScale;
        if (winnerData.Item1 == Player.L)
        {
            LeanTween.move(tL.gameObject, new Vector3(0.2f, tL.position.y, 0), weaponFlyTime).setEase(LeanTweenType.easeInOutExpo);
            LeanTween.move(tR.gameObject, new Vector3(0.1f, tR.position.y, 0), weaponFlyTime).setEase(LeanTweenType.easeInOutExpo);
            yield return new WaitForSeconds(weaponFlyTime - 0.3f);
            LeanTween.scale(tL.gameObject, tL.localScale * winnerScale, weaponSizeChangeTime);
            LeanTween.scale(tR.gameObject, tR.localScale * loserScale, weaponSizeChangeTime);
            LeanTween.move(tR.gameObject, new Vector3(4f, tR.position.y, 0), weaponFlyBounceBackTime).setEase(LeanTweenType.easeOutExpo);
            currMenuObj.GetComponent<AnimateAttackWindowController>().EnableWinnerText(Player.L, coins);
        } else if (winnerData.Item1 == Player.R)
        {
            LeanTween.move(tR.gameObject, new Vector3(-0.2f, tR.position.y, 0), weaponFlyTime).setEase(LeanTweenType.easeInOutExpo);
            LeanTween.move(tL.gameObject, new Vector3(0.1f, tL.position.y, 0), weaponFlyTime).setEase(LeanTweenType.easeInOutExpo);
            yield return new WaitForSeconds(weaponFlyTime - 0.3f);
            LeanTween.scale(tR.gameObject, tR.localScale * winnerScale, weaponSizeChangeTime);
            LeanTween.scale(tL.gameObject, tL.localScale * loserScale, weaponSizeChangeTime);
            LeanTween.move(tL.gameObject, new Vector3(-4f, tL.position.y, 0), weaponFlyBounceBackTime).setEase(LeanTweenType.easeOutExpo);
            currMenuObj.GetComponent<AnimateAttackWindowController>().EnableWinnerText(Player.R, coins);
        } else {
            LeanTween.move(tR.gameObject, new Vector3(-0.5f, tR.position.y, 0), weaponFlyTime).setEase(LeanTweenType.easeInOutExpo);
            LeanTween.move(tL.gameObject, new Vector3(0.5f, tL.position.y, 0), weaponFlyTime).setEase(LeanTweenType.easeInOutExpo);
            currMenuObj.GetComponent<AnimateAttackWindowController>().EnableWinnerText(Player.NaN, coins);
        }
        // === RESET PHASE ===
        yield return new WaitForSeconds(winningTextDisplayTime);
        LeanTween.scale(tR.gameObject, tRScale, weaponFlyTime);
        LeanTween.scale(tL.gameObject, tlScale, weaponFlyTime);
        LeanTween.move(tL.gameObject, weaponL.weaponVisualInitPos, weaponFlyTime).setEase(LeanTweenType.easeInOutExpo);
        LeanTween.move(tR.gameObject, weaponR.weaponVisualInitPos, weaponFlyTime).setEase(LeanTweenType.easeInOutExpo);
        GameController.instance.RegisterWinner(winnerData);
        _EventBus.Publish<GameStateOver>(new GameStateOver());
    }
}
