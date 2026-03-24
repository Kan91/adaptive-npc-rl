using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [Header("Coin Settings")]
    public GameObject coinPrefab;
    public int maxCoins = 3;
    public Vector3 arenaSize = new Vector3(10, 0, 10); // Größe des Spielfelds

    private List<GameObject> activeCoins = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < maxCoins; i++)
        {
            SpawnCoin();
        }
    }

    void Update()
    {
        // Überprüfen, ob ein Coin eingesammelt wurde
        for (int i = activeCoins.Count - 1; i >= 0; i--)
        {
            Coin coin = activeCoins[i].GetComponent<Coin>();
            if (coin.collected)
            {
                activeCoins.RemoveAt(i);
                SpawnCoin();
            }
        }
    }

    void SpawnCoin()
    {
        Vector3 randomPos = new Vector3(
            Random.Range(-arenaSize.x / 2, arenaSize.x / 2),
            0.1f, // leicht über Boden
            Random.Range(-arenaSize.z / 2, arenaSize.z / 2)
        );

        GameObject coin = Instantiate(coinPrefab, randomPos, Quaternion.identity);
        activeCoins.Add(coin);
    }
}
