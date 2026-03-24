using UnityEngine;

public class CoinCollectorArea : MonoBehaviour
{
    public GameObject[] coins;

    public void ResetCoins()
    {
        foreach (var coin in coins)
        {
            coin.SetActive(true);
            coin.transform.localPosition = new Vector3(
                transform.localPosition.x + Random.Range(-4f, 4f),
                transform.localPosition.y + 0.05f,
                transform.localPosition.z + Random.Range(-4f, 4f)
            );
        }
    }
}