using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.AI;

public class HunterAgent : Agent
{
    [Header("References")]
    public NavMeshAgent navMeshAgent;        // Für Bewegung
    public Transform projectileSpawnPoint;   // Mündung für Schüsse
    public GameObject projectilePrefab;      // Projektil Prefab
    public CoinManagerAgent coinManager;          // Coins im Spielfeld
    public CollectorAgent[] collectors;      // Referenz zu allen Collectors

    [Header("Settings")]
    public float moveSpeed = 4f;
    public float shootForce = 10f;
    public float shootCooldown = 1f;
    public float shootRange = 5f;

    private float lastShootTime = -999f;

    public override void Initialize()
    {
        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();

        if (coinManager == null)
            coinManager = FindObjectOfType<CoinManagerAgent>();
    }

    public override void OnEpisodeBegin()
    {
        // Hunter zurücksetzen
        transform.localPosition = coinManager.GetRandomSpawnPoint();
        navMeshAgent.ResetPath();

        // Reset Cooldowns
        lastShootTime = -999f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Hunter Position
        sensor.AddObservation(transform.localPosition / 10f);
        
        // Eigene Blickrichtung Iteration 2
        sensor.AddObservation(transform.forward);

        // Beobachte Collector-Positionen
        foreach (var c in collectors)
        {
            if (c != null)
            {
                Vector3 toCollector = c.transform.position - transform.position;
                sensor.AddObservation(toCollector.normalized);
                sensor.AddObservation(toCollector.magnitude / 10f);
            }
            else
            {
                // Falls kein Collector → Dummy
                sensor.AddObservation(Vector3.zero);
                sensor.AddObservation(1f);
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Bewegung (Continuous)
        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveZ = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
        Vector3 move = new Vector3(moveX, 0, moveZ).normalized;

        navMeshAgent.Move(move * moveSpeed * Time.deltaTime);




        // Schießen (Discrete: 0 = nichts, 1 = schießen)
        if (actions.DiscreteActions[0] == 1 && Time.time - lastShootTime > shootCooldown)
        {
            Shoot();
        }

        // Reward Shaping
        // In Schussnähe von Collectors bleiben
        foreach (var c in collectors)
        {
            if (c == null) continue;
            float dist = Vector3.Distance(transform.position, c.transform.position);

            // Nähe belohnen
            //if (dist < shootRange * 1.5f) AddReward(+0.002f);
            // Iteration 3
            if (dist < shootRange * 2f) AddReward(+0.002f);
            if (dist < shootRange * 1.2f) AddReward(+0.005f);

            // Im Zielkorridor (z. B. Hunter schaut in Collector-Richtung)
            Vector3 dirToCollector = (c.transform.position - transform.position).normalized;
            float aimDot = Vector3.Dot(transform.forward, dirToCollector);
            //  if (aimDot > 0.95f && dist < shootRange * 1.2f) AddReward(+0.01f); // zielt "fast direkt" auf Collector
            
            // Iteration 3
            if (aimDot > 0.9f && dist < shootRange * 1.5f) AddReward(+0.005f);
            if (aimDot > 0.98f && dist < shootRange * 1.2f) AddReward(+0.02f);
            
        }
        // Kleine Living Penalty
        AddReward(-0.0005f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Für Tastatur-Test
        var c = actionsOut.ContinuousActions;
        c[0] = Input.GetAxis("Horizontal");
        c[1] = Input.GetAxis("Vertical");

        var d = actionsOut.DiscreteActions;
        d[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    /*  // Iteration 1
    private void Shoot()
    {
        lastShootTime = Time.time;

        GameObject proj = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        rb.AddForce(projectileSpawnPoint.forward * shootForce, ForceMode.VelocityChange);

        // Owner setzen (ProjectileAgent!)
        ProjectileAgent p = proj.GetComponent<ProjectileAgent>();
        if (p != null) p.SetOwner(this);

        Destroy(proj, 1f); // Projektil nach 3s löschen
        
        AddReward(-0.01f);
    }
    */
    
    private void Shoot()
    {
        lastShootTime = Time.time;

        GameObject proj = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        rb.AddForce(projectileSpawnPoint.forward * shootForce, ForceMode.VelocityChange);

        ProjectileAgent p = proj.GetComponent<ProjectileAgent>();
        if (p != null) p.SetOwner(this);

        Destroy(proj, 1f);

        // Strafe fürs Ballern ins Leere (deutlich spürbar machen)
        AddReward(-0.02f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            // Strafe: Hunter sollte Coins nicht einsammeln , Camping verhindern
            AddReward(-0.5f);
        }
    }

    public void OnCollectorHit()
    {
        // Wird von Collector aufgerufen, wenn er getroffen wurde
        AddReward(+10f);
    }
}
