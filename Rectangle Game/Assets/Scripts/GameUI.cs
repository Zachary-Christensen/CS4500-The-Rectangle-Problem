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

    public void EnableRules()
    {
        HideGameUI();
        ShowRules();
    }

    public void ReturnToGame()
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

    public void EnableNPlus3()
    {
        ShowNPlus3();
        HideGameUI();
    }

    public void EnableNPlus4()
    {
        ShowNPlus4();
        HideGameUI();
    }

    public void EnableAllNumbers()
    {
        ShowAllNumbers();
        HideGameUI();
    }

    public void EnableWin()
    {
        ShowWinPanel();
        HideGameUI();
    }

    public void EnableEndGamePanel()
    {
        ShowEndGamePanel();
        HideGameUI();
    }

    public void EnablePreviousSolutions()
    {
        ShowPreviousSolutions();
        HideGameUI();
    }
}
