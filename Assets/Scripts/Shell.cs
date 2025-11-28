using UnityEngine;

public class Shell : MonoBehaviour
{
    public float speed = 25f;
    public float minDamage = 20f;
    public float maxDamage = 40f;
    public float lifeTime = 5f;

    public Faction ownerFaction;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        TankHealth health = collision.collider.GetComponentInParent<TankHealth>();
        if (health != null)
        {
            Faction f = collision.collider.GetComponentInParent<Faction>();

            if (f != null && ownerFaction != null &&
                f.factionType == ownerFaction.factionType)
            {
                Destroy(gameObject);
                return;
            }

            float dmg = Random.Range(minDamage, maxDamage);
            health.TakeDamage(dmg);
        }

        Destroy(gameObject);
    }
}
