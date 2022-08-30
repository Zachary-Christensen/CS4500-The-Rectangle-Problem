using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public GameSquareInput gameSquareInput;

    public GameObject gameUI;
    public GameObject gameRules;
    public GameObject previousSolutions;
    public Solutions solutions;
    public SkyBoxController skyBoxController;

    public GameObject returnToMainMenu;
    public GameObject nPlus3;
    public GameObject nPlus4;
    public GameObject allNumbers;
    public GameObject winPanel;
    public GameObject endGamePanel;
    public GameObject hintPanel;

    private bool nPlus3Enabled = false;
    public GameObject txtNPlus3;
    private bool nPlus4Enabled = false;
    public GameObject txtNPlus4;
    private bool allNumbersEnabled = false;
    public GameObject txtAllNumbers;
    private bool hintEnabled = true;
    public GameObject txtHint;
    public void EnableNPlus3()
    {
        nPlus3Enabled = true;
        txtNPlus3.SetActive(true);
    }
    public void EnableNPlus4()
    {
        nPlus4Enabled = true;
        txtNPlus4.SetActive(true);
    }

    public void EnableAllNumbers()
    {
        allNumbersEnabled = true;
        txtAllNumbers.SetActive(true);
    }

    private void ShowReturnToMainMenu()
    {
        returnToMainMenu.SetActive(true);
    }

    private void HideReturnToMainMenu()
    {
        returnToMainMenu.SetActive(false);
    }

    private void ShowPreviousSolutions()
    {
        previousSolutions.SetActive(true);
    }

    private void HidePreviousSolutions()
    {
        previousSolutions.SetActive(false);
        solutions.HideSolutions();
    }

    private void ShowEndGamePanel()
    {
        endGamePanel.SetActive(true);
    }

    private void HideEndGamePanel()
    {
        endGamePanel.SetActive(false);
    }

    private void ShowWinPanel()
    {
        winPanel.SetActive(true);
    }

    private void HideWinPanel()
    {
        winPanel.SetActive(false);
    }

    private void ShowNPlus3()
    {
        nPlus3.SetActive(true);
    }

    private void HideNPlus3()
    {
        nPlus3.SetActive(false);
    }

    private void ShowNPlus4()
    {
        nPlus4.SetActive(true);
    }

    private void HideNPlus4()
    {
        nPlus4.SetActive(false);
    }

    private void ShowAllNumbers()
    {
        allNumbers.SetActive(true);
    }

    private void HideAllNumbers()
    {
        allNumbers.SetActive(false);
    }

    private void ShowHint()
    {
        hintPanel.SetActive(true);
    }

    private void HideHint()
    {
        hintPanel.SetActive(false);
    }

    private void ShowGameUI()
    {
        gameUI.SetActive(true);
    }

    private void HideGameUI()
    {
        gameUI.SetActive(false);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void ShowRules()
    {
        gameRules.SetActive(true);
    }

    private void HideRules()
    {
        gameRules.SetActive(false);
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("Credits");
    }


    public void OpenRules()
    {
        HideGameUI();
        ShowRules();
    }

    public void OpenReturnToMainMenu()
    {
        HideGameUI();
        ShowReturnToMainMenu();
    }
    public void OpenGame()
    {
        HideRules();
        HideNPlus3();
        HideNPlus4();
        HideAllNumbers();
        HideWinPanel();
        HideEndGamePanel();
        HidePreviousSolutions();
        HideHint();
        HideReturnToMainMenu();

        ShowGameUI();
        
        skyBoxController.SetSkyBox();
    }

    public void OpenHint()
    {
        if (hintEnabled)
        {
            string hint = "";
            switch (gameSquareInput.IdxN)
            {
                case 0:
                    hint = "This one is easy";
                    break;
                case 1:
                    hint = "Use one 1/1 scale rectangle";
                    break;
                case 2:
                    hint = "Use the N + 3 rule on one of the rectangles";
                    break;
                case 3:
                    hint = "Use four 2/3's rectangles";
                    break;
                case 4:
                    hint = "Use the N + 4 rule with the N = 6 solution";
                    break;
                case 5:
                    hint = "Use the N + 3 rule with the N = 6 solution";
                    break;
                case 6:
                    hint = "Use five 1/3 rectangles";
                    break;
            }
            txtHint.GetComponent<Text>().text = hint;
            ShowHint();
            HideGameUI();
        }
    }

    public void OpenNPlus3()
    {
        if (nPlus3Enabled)
        {
            ShowNPlus3();
            HideGameUI(); 
        }
    }

    public void OpenNPlus4()
    {
        if (nPlus4Enabled)
        {
            ShowNPlus4();
            HideGameUI(); 
        }
    }

    public void OpenAllNumbers()
    {
        if (allNumbersEnabled)
        {
            ShowAllNumbers();
            HideGameUI(); 
        }
    }

    public void OpenWin()
    {
        ShowWinPanel();
        HideGameUI();
    }

    public void OpenEndGamePanel()
    {
        ShowEndGamePanel();
        HideGameUI();
    }

    public void OpenPreviousSolutions()
    {
        ShowPreviousSolutions();
        HideGameUI();
    }

}
