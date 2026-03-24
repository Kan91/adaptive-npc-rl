using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CollectorController : MonoBehaviour
{
    [Header("Collector Settings")]
    public string collectorName = "Collector"; // frei im Inspector setzbar
   // public float moveSpeed = 3f;
    public int coinsCollected = 0;

    
    [Header("Behaviour Settings")]
    public float dangerDistance = 5f; // wenn Player so nah ist -> Ausweichmodus
    public float safeDistance = 8f;   // Coins in diesem Abstand bevorzugt
    
   // private Rigidbody rb;
   // private GameObject targetCoin;
   // private Transform player;
    
    private NavMeshAgent agent;
    private GameObject targetCoin;
    private Transform player;
    
    void Start()
    {
       // rb = GetComponent<Rigidbody>();
       // rb.freezeRotation = true;
        
       // player = GameObject.FindGameObjectWithTag("Player")?.transform;
       // FindNewTarget();
        
        
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        FindNewTarget();
    }

    void Update()
    {
        if ((object)targetCoin == null || !targetCoin.activeSelf)
        {
            FindNewTarget();
        }
        else
        {
            //MoveToCoin();
            agent.SetDestination(targetCoin.transform.position);
        }
    }


void FindNewTarget()
{
    GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
    GameObject[] collectors = GameObject.FindGameObjectsWithTag("Collector");

    if (coins.Length == 0)
    {
        targetCoin = null;
        return;
    }

    float playerDistance = player != null ? Vector3.Distance(transform.position, player.position) : Mathf.Infinity;

    GameObject bestCoin = null;
    float bestValue = Mathf.Infinity;

    foreach (GameObject coin in coins)
    {
        if (!coin.activeSelf) continue;

        float distToCollector = Vector3.Distance(transform.position, coin.transform.position);
        float distToPlayer = player != null ? Vector3.Distance(player.position, coin.transform.position) : Mathf.Infinity;

        // --- Abstand zu anderen Collectoren berechnen ---
        float minDistToOtherCollector = Mathf.Infinity;
        foreach (GameObject col in collectors)
        {
            if (col == this.gameObject) continue;
            float d = Vector3.Distance(col.transform.position, coin.transform.position);
            if (d < minDistToOtherCollector)
                minDistToOtherCollector = d;
        }

        // Coins, die weiter von anderen Collectoren entfernt sind, sind attraktiver
        float spacingBonus = minDistToOtherCollector * 0.3f;

        // Bewertungslogik abhängig von Spielersituation
        float value;
        if (playerDistance < dangerDistance)
        {
            // Spieler ist nah -> Coins bevorzugen, die weiter vom Spieler weg sind
            value = distToCollector - spacingBonus - distToPlayer * 0.5f;
        }
        else
        {
            // Normal -> Coins nehmen, die nah am Collector liegen, aber nicht zu nah am Spieler
            value = distToCollector - spacingBonus + (distToPlayer < safeDistance ? 5f : 0f);
        }

        if (value < bestValue)
        {
            bestValue = value;
            bestCoin = coin;
        }
    }

    // Fallback: wenn kein guter Coin gefunden -> irgendeinen nehmen
    if (bestCoin == null && coins.Length > 0)
    {
        bestCoin = coins[Random.Range(0, coins.Length)];
    }

    targetCoin = bestCoin;
}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            coinsCollected++;
            other.gameObject.SetActive(false); // Coin einsammeln
            FindNewTarget();
        }
    }
    
    
    //Für Editor
    private void OnDrawGizmos()
    {
        // DangerDistance (rot) → Kreis um den Collector
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dangerDistance);

        // SafeDistance (grün) → Kreis um den Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.transform.position, safeDistance);
        }

        // Linie zum aktuellen Coin (gelb)
        if (targetCoin != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetCoin.transform.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Dieser Code läuft IMMER im SceneView, sobald das GameObject ausgewählt ist

        // DangerDistance (rot)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dangerDistance);

        // SafeDistance (grün)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, safeDistance);
    }
}


