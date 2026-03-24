using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.AI;

public class DynamicAgent : Agent
{
    [Header("References")]
    public NavMeshAgent navMeshAgent;      
    public Transform player;               
    public CoinManagerAgent coinManager;        

    [Header("Settings")]
    public float moveSpeed = 3f;
    public float dangerDistance = 3f;      
    public float safeDistance = 5f;        

    [Header("Collector Settings")]
    public string collectorName = "Collector";
    public int coinsCollected = 0;
    public override void Initialize()
    {
        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // In DynamicVersion nicht mehr benötigt
    public override void OnEpisodeBegin()
    {
        // Respawn übernimmt GameManager
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition / 10f);

        // Distanz zum Player
        Vector3 toPlayer = player.position - transform.position;
        sensor.AddObservation(toPlayer.normalized);
        sensor.AddObservation(toPlayer.magnitude / 10f);

        // Distanz zum nächsten Coin
        GameObject nearestCoin = coinManager.GetNearestCoin(transform.position);
        if (nearestCoin != null)
        {
            Vector3 toCoin = nearestCoin.transform.position - transform.position;
            sensor.AddObservation(toCoin.normalized);
            sensor.AddObservation(toCoin.magnitude / 10f);
        }
        else
        {
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(1f);
        }

        // Nächstes Projektil
        ProjectileAgent[] projectiles = FindObjectsOfType<ProjectileAgent>();
        ProjectileAgent nearestProj = null;
        float bestDist = Mathf.Infinity;

        foreach (var p in projectiles)
        {
            float d = Vector3.Distance(transform.position, p.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                nearestProj = p;
            }
        }

        if (nearestProj != null)
        {
            Vector3 toProj = nearestProj.transform.position - transform.position;
            sensor.AddObservation(toProj.normalized);
            sensor.AddObservation(toProj.magnitude / 10f);
        }
        else
        {
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(1f);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveZ = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        Vector3 move = new Vector3(moveX, 0, moveZ).normalized;
        
        if (move.sqrMagnitude > 0.01f) // nur wenn wirklich eine Richtung da ist
        {
            // Zielpunkt etwas vor dem Agenten
            Vector3 target = transform.position + move * 2f; 
            navMeshAgent.SetDestination(target);
        }
        else
        {
            // Kein Input → Agent stehen lassen
            navMeshAgent.ResetPath();
        }
       // navMeshAgent.Move(move * moveSpeed * Time.deltaTime);

        // Rewards im Inference-Modus deaktivieren:
        /*
        float distToHunter = Vector3.Distance(transform.position, player.position);
        if (distToHunter > safeDistance) AddReward(+0.002f);
        else if (distToHunter < dangerDistance) AddReward(-0.002f);
        AddReward(-0.0005f);
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            // kein EndEpisode() mehr!
            
            gameObject.SetActive(false);
        }

        if (other.CompareTag("Coin"))
        {
            // im Spiel Coin despawnen + neuen spawnen
            coinsCollected++;
            coinManager.RespawnCoin(other.gameObject);
        }
    }
}
