using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject gameUI;
    public GameObject gameRules;
    public GameObject previousSolutions;
    public Solutions solutions;

    public GameObject nPlus3;
    public GameObject nPlus4;
    public GameObject allNumbers;
    public GameObject winPanel;
    public GameObject endGamePanel;

    private bool nPlus3Enabled = false;
    public GameObject txtNPlus3;
    private bool nPlus4Enabled = false;
    public GameObject txtNPlus4;
    private bool allNumbersEnabled = false;
    public GameObject txtAllNumbers;

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

    public void OpenRules()
    {
        HideGameUI();
        ShowRules();
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

        ShowGameUI();
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
