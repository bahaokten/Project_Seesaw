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
                GameController.instance.PlayerTurnOver(WeaponType.Scissor);
                break;
            case "paper":
                GameController.instance.PlayerTurnOver(WeaponType.Paper);
                break;
            case "rock":
                GameController.instance.PlayerTurnOver(WeaponType.Rock);
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
    static float weaponPreAttack = 0.5f;

    public IEnumerator AnimateAttack()
    {
        MenuController.instance.DoMenuStateChange("animateAttack");
        yield return new WaitForSeconds(swapDuration/2 + 0.1f);

        // === MOVE PHASE ===
        Transform tL = GameController.instance.playerControllerL.GetCurrentWeaponController().weaponVisualObj.transform;
        LeanTween.move(tL.gameObject, new Vector3(-5, tL.position.y, 0), weaponFlyTime).setEase(LeanTweenType.easeInOutExpo);
        Transform tR = GameController.instance.playerControllerR.GetCurrentWeaponController().weaponVisualObj.transform;
        LeanTween.move(tR.gameObject, new Vector3(5, tR.position.y, 0), weaponFlyTime).setEase(LeanTweenType.easeInOutExpo);
        yield return new WaitForSeconds(weaponFlyTime + 0.1f);

        // === APPLY CARDS PHASE ===
        CardIterator cardIt = new CardIterator();
        BaseCard nextCard = cardIt.GetNextCard();
        while (nextCard != null)
        {
            //DO CARDS
            nextCard = cardIt.GetNextCard();
        }
        //yield return new WaitForSeconds(weaponPreAttack);
        //LeanTween.move(tL.gameObject, new Vector3(-0.5f, tL.position.y, 0), weaponFlyTime).setEase(LeanTweenType.easeInOutExpo);

        // === WIN PHASE ===
        Tuple<Player, float> winT = GameController.instance.DetermineWinner();
        print(winT.Item1);
    }
}
