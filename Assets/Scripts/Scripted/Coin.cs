using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collector"))
        {
            collected = true;
            gameObject.SetActive(false); // Coin verschwindet
        }
    }
    
    public void ResetCoin()
    {
        collected = false;
        gameObject.SetActive(true);
    }
}
