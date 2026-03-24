using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.AI;

public class CollectorAgent : Agent
{
    [Header("References")]
    public NavMeshAgent navMeshAgent;     
    public Transform player;              
    public CoinManagerAgent coinManager;  
    public CollectorAgent[] otherCollectors; // <– Neue Referenz

    [Header("Settings")]
    public float moveSpeed = 3f;
    public float dangerDistance = 2f;
    public float safeDistance = 4f;
    public float minCollectorDistance = 2.5f; // <– Mindestabstand zu anderen Collectoren

    private GameObject targetCoin;

    public override void Initialize()
    {
        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public override void OnEpisodeBegin()
    {
        Vector3 spawn = coinManager.GetRandomSpawnPoint();
        navMeshAgent.Warp(spawn);  
        navMeshAgent.ResetPath();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Eigene Position
        sensor.AddObservation(transform.localPosition / 10f);

        // Distanz & Richtung zum Hunter
        Vector3 toPlayer = player.position - transform.position;
        sensor.AddObservation(toPlayer.normalized);
        sensor.AddObservation(toPlayer.magnitude / 10f);

        // Nächster Coin
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

        // Abstand zu anderen Collectoren (max. 2 berücksichtigen)
        int count = 0;
        foreach (var other in otherCollectors)
        {
            if (other == null || other == this) continue;
            Vector3 toOther = other.transform.position - transform.position;
            sensor.AddObservation(toOther.normalized);
            sensor.AddObservation(toOther.magnitude / 10f);
            count++;
            if (count >= 2) break;
        }

        // Falls weniger als 2 -> Dummy
        for (; count < 2; count++)
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
        navMeshAgent.Move(move * moveSpeed * Time.deltaTime);

        // --- Reward Shaping ---
        float distToHunter = Vector3.Distance(transform.position, player.position);

        // Abstand zum Hunter
        if (distToHunter > safeDistance)
            AddReward(+0.01f);
        else if (distToHunter < dangerDistance)
            AddReward(-0.005f);

        // Abstand zu anderen Collectoren
        foreach (var other in otherCollectors)
        {
            if (other == null || other == this) continue;
            float dist = Vector3.Distance(transform.position, other.transform.position);
            if (dist < minCollectorDistance)
                AddReward(-0.002f);  // zu nah -> Malus
            else
                AddReward(+0.001f);  // schön verteilt -> Bonus
        }

        // Kleine Living Penalty
        AddReward(-0.0005f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var c = actionsOut.ContinuousActions;
        c[0] = Input.GetAxis("Horizontal");
        c[1] = Input.GetAxis("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            AddReward(-5f);
            EndEpisode();
        }

        if (other.CompareTag("Coin"))
        {
            float distHunter = Vector3.Distance(transform.position, player.position);
            if (distHunter < safeDistance)
                AddReward(+1f);
            AddReward(+3f);
            coinManager.RespawnCoin(other.gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
            AddReward(-0.002f);
    }
}
