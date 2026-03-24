using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManagerDynamic : MonoBehaviour
{
    [Header("Game Settings")]
    public float roundTime = 60f;
    public Vector3 arenaSize = new Vector3(10, 0, 10); // gleiche Größe wie im Training

    [Header("UI")]
    public TMP_Text timerText;
    public GameObject scoreboardPanel;
    public TMP_Text scoreboardTitle;
    public TMP_Text scoreboardText;
    
    private float timeRemaining;
    private bool roundActive = true;

    private DynamicAgent[] collectors;
    private CoinManagerAgent coinManager;

    void Start()
    {
        // Referenzen holen
        collectors = FindObjectsOfType<DynamicAgent>();
        coinManager = FindObjectOfType<CoinManagerAgent>();

        StartRound();
    }

    void Update()
    {
        if (!roundActive) return;

        timeRemaining -= Time.deltaTime;
        timerText.text = Mathf.Ceil(timeRemaining).ToString() + "s";

        if (timeRemaining <= 0 || AllCollectorsDown())
        {
            EndRound();
        }
    }

    void StartRound()
    {
        timeRemaining = roundTime;
        roundActive = true;
        scoreboardPanel.SetActive(false);

        // Coins resetten
        if (coinManager != null)
        {
            ResetAllCoins();
        }

        // Collectors neu spawnen
        foreach (var c in collectors)
        {
            Vector3 randomSpawn = new Vector3(
                Random.Range(-arenaSize.x / 2, arenaSize.x / 2),
                0.5f,
                Random.Range(-arenaSize.z / 2, arenaSize.z / 2)
            );
            c.navMeshAgent.Warp(randomSpawn);
            c.coinsCollected = 0;
            c.gameObject.SetActive(true);
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

    // Coins komplett neu spawnen
    void ResetAllCoins()
    {
        // Alle alten Coins zerstören
        var oldCoins = GameObject.FindGameObjectsWithTag("Coin");
        foreach (var coin in oldCoins)
        {
            Destroy(coin);
        }

        // Neue Coins spawnen
        for (int i = 0; i < coinManager.maxCoins; i++)
        {
            coinManager.SendMessage("SpawnCoin");
        }
    }

    // UI Buttons
    public void RestartRound()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
