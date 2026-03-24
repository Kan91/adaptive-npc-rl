using System.Collections.Generic;
using UnityEngine;

public class CoinManagerAgent : MonoBehaviour
{
    [Header("Coin Settings")]
    public GameObject coinPrefab;           // Prefab für Coins
    public int maxCoins = 3;                // Anzahl aktiver Coins
    public Vector3 arenaSize = new Vector3(10, 0, 10); // Spielfeldgröße

    private List<GameObject> activeCoins = new List<GameObject>();

    
    void Start()
    {
        ResetCoins(); // Coins einmal beim Start platzieren
    }
    
    
    // Coins beim Start/Reset neu spawnen
    public void ResetCoins()
    {
        // Vorhandene Coins zerstören
        foreach (var coin in activeCoins)
        {
            Destroy(coin);
        }
        activeCoins.Clear();

        // Neue Coins erzeugen
        for (int i = 0; i < maxCoins; i++)
        {
            SpawnCoin();
        }
    }

    // Zufällige Coin-Position innerhalb der Arena
    public Vector3 GetRandomSpawnPoint()
    {
        return new Vector3(
            Random.Range(-arenaSize.x / 2, arenaSize.x / 2),
            0.1f, // leicht über Boden
            Random.Range(-arenaSize.z / 2, arenaSize.z / 2)
        );
    }

    // Coin instanziieren
    private void SpawnCoin()
    {
        Vector3 pos = GetRandomSpawnPoint();
        GameObject coin = Instantiate(coinPrefab, pos, Quaternion.identity);
        coin.tag = "Coin"; // wichtig für Agent-Erkennung
        activeCoins.Add(coin);
    }

    // Coin neu platzieren (wenn eingesammelt)
    public void RespawnCoin(GameObject coin)
    {
        coin.transform.position = GetRandomSpawnPoint();
        coin.SetActive(true);
    }

    // Nächstgelegenen Coin für Observations finden
    public GameObject GetNearestCoin(Vector3 fromPosition)
    {
        GameObject best = null;
        float bestDist = Mathf.Infinity;

        foreach (var coin in activeCoins)
        {
            if (!coin.activeSelf) continue;
            float d = Vector3.Distance(fromPosition, coin.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = coin;
            }
        }
        return best;
    }
}
