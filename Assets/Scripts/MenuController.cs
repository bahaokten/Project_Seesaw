using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using CardIterator = Cards.CardIterator;
using BaseCard = Cards.BaseCard;

public class MenuController : MonoBehaviour
{
    public static MenuController instance;

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
            _EventBus.Publish<MenuStateChanged>(new MenuStateChanged(_currMenu));
        }
    }
    [HideInInspector]
    public GameObject currMenuObj;

    public float swapDuration = 0.4f;

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
    }

    void Start()
    {
        currMenu = MenuState.BuyPhaseMenu;
        currMenuObj = buyPhaseMenu;
        menuLocTable = new Dictionary<string, Vector3>
        {
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
    }

    public void DoMenuStateChange(string action)
    {
        switch (action)
        {
            case "buyPhaseMenu":
                GameController.instance.currTurnPhase = TurnPhase.BuyPhase;
                StartCoroutine(DoSwapAnimation(currMenuObj, buyPhaseMenu, MenuState.BuyPhaseMenu));
                break;
            case "buyMenu":
                StartCoroutine(DoSwapAnimation(currMenuObj, buyMenu, MenuState.BuyMenu));
                break;
            case "actionPhaseMenu":
                GameController.instance.currTurnPhase = TurnPhase.ActionPhase;
                StartCoroutine(DoSwapAnimation(currMenuObj, actionPhaseMenu, MenuState.ActionPhaseMenu));
                break;
            case "useMenu":
                StartCoroutine(DoSwapAnimation(currMenuObj, useMenu, MenuState.UseMenu));
                break;
            case "upgradeMenu":
                StartCoroutine(DoSwapAnimation(currMenuObj, upgradeMenu, MenuState.UpgradeMenu));
                break;
            case "attackPhaseMenu":
                GameController.instance.currTurnPhase = TurnPhase.AttackPhase;
                StartCoroutine(DoSwapAnimation(currMenuObj, attackPhaseMenu, MenuState.AttackPhaseMenu));
                break;
            case "nonUIPlayerPlaying":
                break;
            case "animateAttack":
                StartCoroutine(DoSwapOutPopInAnimation(currMenuObj, animateAttackWindow, MenuState.AnimatingAttack));
                break;
        }
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
                GameController.instance.PlayerPickedWeapon(WeaponType.Scissor);
                break;
            case "paper":
                GameController.instance.PlayerPickedWeapon(WeaponType.Paper);
                break;
            case "rock":
                GameController.instance.PlayerPickedWeapon(WeaponType.Rock);
                break;
        }

    }

    public void DoWeaponUpgrade(string action)
    {
        if (GameController.instance.currTurnPhase != TurnPhase.ActionPhase)
        {
            return;
        }

        switch (action)
        {
            case "scissor_attack":
                GameController.instance.GetCurrentPlayer().UpgradeWeapon(WeaponType.Scissor, WeaponAttribute.Attack);
                break;
            case "scissor_defense":
                GameController.instance.GetCurrentPlayer().UpgradeWeapon(WeaponType.Scissor, WeaponAttribute.Defense);
                break;
            case "paper_attack":
                GameController.instance.GetCurrentPlayer().UpgradeWeapon(WeaponType.Paper, WeaponAttribute.Attack);
                break;
            case "paper_defense":
                GameController.instance.GetCurrentPlayer().UpgradeWeapon(WeaponType.Paper, WeaponAttribute.Defense);
                break;
            case "rock_attack":
                GameController.instance.GetCurrentPlayer().UpgradeWeapon(WeaponType.Rock, WeaponAttribute.Attack);
                break;
            case "rock_defense":
                GameController.instance.GetCurrentPlayer().UpgradeWeapon(WeaponType.Rock, WeaponAttribute.Defense);
                break;
        }

        DoMenuStateChange("attackPhaseMenu");
    }

    void AnimateExit(GameObject obj, float time = 2f)
    {
        LeanTween.move(obj, new Vector3(-1300, obj.transform.position.y, 0), time).setEase(LeanTweenType.clamp);
    }

    void AnimateEntry(GameObject obj, float time = 2f)
    {
        obj.transform.position = new Vector3(-1300, obj.transform.position.y, 0);
        LeanTween.move(obj, menuLocTable[obj.name], time).setEase(LeanTweenType.clamp);
    }
    private void DoSwap(GameObject exitingObj, GameObject enteringObj, MenuState newState = MenuState.NaN)
    {
        exitingObj.SetActive(false);

        if (newState != MenuState.NaN)
        {
            currMenu = newState;
        }
        currMenuObj = enteringObj;

        enteringObj.SetActive(true);
    }

    IEnumerator DoSwapOutPopInAnimation(GameObject exitingObj, GameObject enteringObj, MenuState newState = MenuState.NaN)
    {
        if (exitingObj == enteringObj)
        {
            yield break;
        }
        AnimateExit(exitingObj, swapDuration / 2);
        yield return new WaitForSeconds(swapDuration / 2);

        enteringObj.transform.position = menuLocTable[enteringObj.name];
        DoSwap(exitingObj, enteringObj, newState);
    }

    IEnumerator DoSwapAnimation(GameObject exitingObj, GameObject enteringObj, MenuState newState = MenuState.NaN)
    {
        if (exitingObj == enteringObj)
        {
            yield break;
        }
        AnimateExit(exitingObj, swapDuration / 2);
        yield return new WaitForSeconds(swapDuration / 2);

        DoSwap(exitingObj, enteringObj, newState);

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
            //TODO: Card logic
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
        GameController.instance.GamePhaseOver();
    }
}
