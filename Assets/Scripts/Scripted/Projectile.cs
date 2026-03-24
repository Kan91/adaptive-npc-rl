using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Collector"))
        {
            // Collector „ausschalten“
            collision.gameObject.SetActive(false);
            
            Destroy(gameObject);
        }

        
    }
}
