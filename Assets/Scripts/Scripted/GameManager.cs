using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public float roundTime = 60f;

    [Header("UI")]
    public TMP_Text timerText;
    public GameObject scoreboardPanel;
    public TMP_Text scoreboardTitle;
    public TMP_Text scoreboardText;
    
    private float timeRemaining;
    private bool roundActive = true;
    private CollectorController[] collectors;

    void Start()
    {
        timeRemaining = roundTime;
        roundActive = true;
        scoreboardPanel.SetActive(false);

        // Alle Collector in der Szene suchen
        collectors = FindObjectsOfType<CollectorController>();
    }

    void Update()
    {
        if (!roundActive) return;

        // Timer runterzählen
        timeRemaining -= Time.deltaTime;
        timerText.text = Mathf.Ceil(timeRemaining).ToString() + "s";

        // Bedingungen für Rundenende
        if (timeRemaining <= 0 || AllCollectorsDown())
        {
            EndRound();
        }
    }

    bool AllCollectorsDown()
    {
        foreach (var c in collectors)
        {
            if (c.gameObject.activeSelf) return false;
        }
        return true;
    }

    void EndRound()
    {
        roundActive = false;
        scoreboardPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        float roundDuration = roundTime - timeRemaining;
        scoreboardTitle.text = "Scoreboard";
        string results = "Runde beendet nach " + roundDuration.ToString("F1") + " Sekunden\n\n";

        foreach (var c in collectors)
        {
            results += c.collectorName + ": " + c.coinsCollected + " Coins\n";
        }

        scoreboardText.text = results;
    }
    //Buttons
    public void RestartRound()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
}
