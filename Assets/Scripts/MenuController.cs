using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public static MenuController instance;

    public GameObject buyPhaseMenu;
    public GameObject buyMenu;
    public GameObject actionPhaseMenu;
    public GameObject useMenu;
    public GameObject upgradeMenu;
    public GameObject attackPhaseMenu;

    public Dictionary<string, Vector3> menuLocTable;

    [HideInInspector]
    public MenuState currMenu;
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
            { attackPhaseMenu.name, attackPhaseMenu.transform.position }
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
        }
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

    IEnumerator DoSwapAnimation(GameObject exitingObj, GameObject enteringObj, MenuState newState = MenuState.NaN)
    {
        AnimateExit(exitingObj, swapDuration / 2);
        yield return new WaitForSeconds(swapDuration / 2);

        DoSwap(exitingObj, enteringObj, newState);

        AnimateEntry(enteringObj, swapDuration / 2);
    }
}
