using UnityEngine;

public class TankCombatSystem : MonoBehaviour
{
    public float fireInterval = 2f;
    public float detectionRange = 25f;
    public float rotateSpeed = 3f;
    public Transform turret;
    public Transform firePoint;
    public GameObject shellPrefab;

    private float fireTimer = 0f;
    private Faction myFaction;
    private Transform target;

    void Start()
    {
        myFaction = GetComponent<Faction>();
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;

        FindTarget();
        AimAndFire();
    }

    void FindTarget()
    {
        if (target != null)
        {
            TankHealth th = target.GetComponentInParent<TankHealth>();
            Faction f = target.GetComponentInParent<Faction>();

            if (th != null && !th.isDead && f != null && f.factionType != myFaction.factionType)
            {
                float d = Vector3.Distance(transform.position, target.position);
                if (d < detectionRange) return;
            }
        }

        target = null;
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange);

        foreach (var h in hits)
        {
            Faction f = h.GetComponentInParent<Faction>();
            TankHealth th = h.GetComponentInParent<TankHealth>();

            if (f != null && th != null && !th.isDead &&
                f.factionType != myFaction.factionType)
            {
                target = h.GetComponentInParent<Transform>();
                return;
            }
        }
    }

    void AimAndFire()
    {
        if (target == null) return;

        Vector3 dir = target.position - turret.position;
        dir.y = 0;
        Quaternion desired = Quaternion.LookRotation(dir);
        turret.rotation = Quaternion.Slerp(turret.rotation, desired, rotateSpeed * Time.deltaTime);

        float angle = Quaternion.Angle(turret.rotation, desired);

        if (angle < 4f && fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireInterval;
        }
    }

    void Shoot()
    {
        GameObject shell = Instantiate(shellPrefab, firePoint.position, firePoint.rotation);

        Shell s = shell.GetComponent<Shell>();
        if (s != null)
            s.ownerFaction = myFaction;
    }
}
