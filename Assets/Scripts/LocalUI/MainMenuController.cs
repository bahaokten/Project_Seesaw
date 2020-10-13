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
        SceneManager.LoadScene("Game");
    }

}
