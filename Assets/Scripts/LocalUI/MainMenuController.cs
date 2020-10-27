using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    
    public void ChangeLType(string t)
    {
        ChangePlayerType(true, t);
    }

    public void ChangeRType(string t)
    {
        ChangePlayerType(false, t);
    }

    public void Start()
    {
    }


    public void ChangePlayerType(bool isLPlayer, string t)
    {
        PlayerType newType;
        switch (t)
        {
            default:
                newType = PlayerType.Human;
                break;
            case "Human":
                newType = PlayerType.Human;
                break;
            case "Simp_ScissorLover":
                newType = PlayerType.Simp_ScissorLover;
                break;
            case "Simp_Random":
                newType = PlayerType.Simp_Random;
                break;
            case "Mid_GreedyAttacker":
                newType = PlayerType.Mid_GreedyAttacker;
                break;
            case "Mid_GreedyDefender":
                newType = PlayerType.Mid_GreedyDefender;
                break;
            case "Mid_GreedyMixed":
                newType = PlayerType.Mid_GreedyMixed;
                break;
            case "Mid_Tracker":
                newType = PlayerType.Mid_Tracker;
                break;
        }

        if (isLPlayer)
        {
            GlobalVars.instance.LType = newType;
        } else
        {
            GlobalVars.instance.RType = newType;
        }
    }

    public void AnimateChange(bool change)
    {
        GlobalVars.instance.animateAttack = change;
    }

    public void StartGame()
    {
        GlobalVars.instance.currGamesToPlay = GlobalVars.instance.numGamesToPlay;
        SceneManager.LoadScene("Game");
    }

}
