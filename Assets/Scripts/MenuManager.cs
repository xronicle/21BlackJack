using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public GameObject menuCanvas;
    public GameObject gameCanvas;
    public GameObject helpCanvas;
    public GameObject recordCanvas;

    public GameManager gameManager;
    public TMP_InputField nameInputField;

    public LeaderboardUI recordTableUI; // Переименовано с RecordTableUI

    void Start()
    {

        menuCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        helpCanvas.SetActive(false);
        recordCanvas.SetActive(false);

        if (PlayerProfileManager.HasNameSaved())
            nameInputField.text = PlayerProfileManager.GetPlayerName();
    }

    public void OnStartGame()
    {
        menuCanvas.SetActive(false);
        helpCanvas.SetActive(false);
        recordCanvas.SetActive(false);
        gameCanvas.SetActive(true);

        gameManager.StartNewGame();
    }

    public void OnExit() => Application.Quit();

    public void ShowHelp()
    {
        menuCanvas.SetActive(false);
        helpCanvas.SetActive(true);
    }

    public void CloseHelp()
    {
        helpCanvas.SetActive(false);
        menuCanvas.SetActive(true);
    }

    public void ShowRecords()
    {
        menuCanvas.SetActive(false);
        helpCanvas.SetActive(false);
        gameCanvas.SetActive(false);
        recordCanvas.SetActive(true);
        recordTableUI.PopulateLeaderboard();
    }

    public void CloseRecords()
    {
        recordCanvas.SetActive(false);
        menuCanvas.SetActive(true);
    }

    public void ReturnToMenu()
    {
        gameCanvas.SetActive(false);
        helpCanvas.SetActive(false);
        recordCanvas.SetActive(false);
        menuCanvas.SetActive(true);
    }

    public void OnNameChanged()
    {
        PlayerProfileManager.SavePlayerName(nameInputField.text);
    }
}
