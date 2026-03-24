using UnityEngine;

public class ProjectileAgent : MonoBehaviour
{
    private HunterAgent owner;

    public void SetOwner(HunterAgent hunter)
    {
        owner = hunter;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collector"))
        {
            // Collector bestraft
            CollectorAgent collector = other.GetComponent<CollectorAgent>();
            if (collector != null)
            {
               // collector.EndEpisode(); // Collector macht Strafe in seinem eigenen OnTriggerEnter
                
                // WICHTIG: Collector sofort wieder aktivieren
                //collector.gameObject.SetActive(true);
                
                // Hunter belohnen

                if (owner != null)
                    owner.OnCollectorHit();
            }

            Destroy(gameObject); // Projektil entfernen
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject); // Projektil verschwindet bei Wandkontakt
        }
    }
}