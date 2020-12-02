using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public TMP_InputField scoreToWin;
    public TMP_InputField gamesToPlay;
    
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
        scoreToWin.text = GlobalVars.SCORE_TO_WIN.ToString();
        gamesToPlay.text = GlobalVars.GAMES_TO_PLAY.ToString();
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
            case "Simp_ScissorLover2":
                newType = PlayerType.Simp_ScissorLover2;
                break;
            case "Simp_Random":
                newType = PlayerType.Simp_Random;
                break;
            case "Simp_RandomUpgrade":
                newType = PlayerType.Simp_RandomUpgrade;
                break;
            case "Simp_RandomCard":
                newType = PlayerType.Simp_RandomCard;
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
            case "Mid_Tracker2":
                newType = PlayerType.Mid_Tracker2;
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
        int currScore = 0;
        int.TryParse(scoreToWin.text, out currScore);
        int currGamesToPlay = 1;
        int.TryParse(gamesToPlay.text, out currGamesToPlay);
        GlobalVars.SCORE_TO_WIN =  currScore;
        GlobalVars.GAMES_TO_PLAY = currGamesToPlay;
        print(GlobalVars.SCORE_TO_WIN);
        SceneManager.LoadScene("Game");
    }

}
