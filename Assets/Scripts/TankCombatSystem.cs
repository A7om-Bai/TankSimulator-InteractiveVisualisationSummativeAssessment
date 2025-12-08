using UnityEngine;

public class TankCombatSystem : MonoBehaviour
{
    [Header("Turret Recentering")]
    public float recenterSpeed = 0.8f;

    public AudioSource fireAudioSource;
    public AudioClip fireClip;

    public float fireInterval = 2f;
    public float detectionRange = 25f;
    public float rotateSpeed = 3f;
    public Transform turret;
    public Transform firePoint;
    public GameObject shellPrefab;
    public ParticleSystem muzzleFlash;
    public LayerMask targetMask;
    public bool IsInCombat { get; private set; }
    public bool IsAiming { get; private set; }

    private float fireTimer = 0f;
    private Faction myFaction;
    private Transform target;

    void Start()
    {
        // Initialize the faction of this tank
        myFaction = GetComponent<Faction>();
    }

    void Update()
    {
        // Decrease the fire timer over time
        fireTimer -= Time.deltaTime;

        // Continuously find a target and aim/fire at it
        FindTarget();
        AimAndFire();
    }

    /// <summary>
    /// Finds a valid target within the detection range.
    /// The target must belong to an enemy faction and be alive.
    /// </summary>
    void FindTarget()
    {
        // If a target is already locked, validate it
        if (target != null)
        {
            TankHealth th = target.GetComponentInParent<TankHealth>();
            Faction f = target.GetComponentInParent<Faction>();

            // Check if the target is alive and belongs to an enemy faction
            if (th != null && !th.isDead && f != null && f.factionType != myFaction.factionType)
            {
                float d = Vector3.Distance(transform.position, target.position);
                if (d < detectionRange) return; // Keep the current target if it's valid
            }
        }

        // Reset the target if the current one is invalid
        target = null;

        // Detect all potential targets within the detection range
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            detectionRange,
            targetMask
        );

        // Iterate through detected objects to find a valid target
        foreach (var h in hits)
        {
            Faction f = h.GetComponentInParent<Faction>();
            TankHealth th = h.GetComponentInParent<TankHealth>();

            // Check if the object is alive and belongs to an enemy faction
            if (f != null && th != null && !th.isDead &&
                f.factionType != myFaction.factionType)
            {
                // Calculate the angle between the tank's forward direction and the target
                Vector3 directionToTarget = (h.transform.position - transform.position).normalized;
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

                // Get the maximum allowable turret offset angle
                InMoving_TurretOffset offsetScript = turret.GetComponent<InMoving_TurretOffset>();
                float maxLockAngle = offsetScript != null ? offsetScript.maxOffsetAngle : 30f; // Default to 30 degrees

                // Lock onto the target if it's within the allowable angle
                if (angleToTarget <= maxLockAngle)
                {
                    target = h.GetComponentInParent<Transform>();
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Aims the turret at the target and fires if conditions are met.
    /// If no target is available, recenters the turret to align with the tank's forward direction.
    /// </summary>
    void AimAndFire()
    {
        // ================== NO TARGET ¡ú RECENTER ==================
        if (target == null)
        {
            IsAiming = false;

            // Recenter the turret to align with the tank's forward direction
            Vector3 forward = tankBodyForward();
            Quaternion recenterRotation = Quaternion.LookRotation(forward);

            turret.rotation = Quaternion.Slerp(
                turret.rotation,
                recenterRotation,
                recenterSpeed * Time.deltaTime
            );

            return;
        }

        // ================== COMBAT ==================
        // Calculate the direction to the target
        Vector3 dir = target.position - turret.position;
        dir.y = 0;

        Quaternion desiredLook = Quaternion.LookRotation(dir);

        // Apply turret offset angle if available
        InMoving_TurretOffset offsetScript = turret.GetComponent<InMoving_TurretOffset>();
        float offsetAngle = offsetScript != null ? offsetScript.CurrentOffsetAngle : 0f;

        Quaternion desired = Quaternion.Euler(
            0f,
            desiredLook.eulerAngles.y + offsetAngle,
            0f
        );

        // Calculate the angle difference between the turret's current and desired rotations
        float angle = Quaternion.Angle(turret.rotation, desired);

        // Determine if the turret is still aiming
        IsAiming = angle > 1.5f;

        // Rotate the turret towards the target
        if (IsAiming)
        {
            turret.rotation = Quaternion.Slerp(
                turret.rotation,
                desired,
                rotateSpeed * Time.deltaTime
            );
        }

        // Fire if the turret is sufficiently aligned with the target
        if (angle < 15f && fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireInterval; // Reset the fire timer
        }
    }

    /// <summary>
    /// Fires a shell from the turret, plays muzzle flash and firing sound.
    /// </summary>
    void Shoot()
    {
        // 1. Play muzzle flash effect
        if (muzzleFlash != null)
        {
            muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            muzzleFlash.Play();
        }

        // 2. Play firing sound
        if (fireAudioSource && fireClip)
        {
            fireAudioSource.PlayOneShot(fireClip);
        }

        // 3. Instantiate and fire the shell
        GameObject shell = Instantiate(shellPrefab, firePoint.position, firePoint.rotation);
        Shell s = shell.GetComponent<Shell>();
        if (s != null)
            s.ownerFaction = myFaction; // Assign the shell's faction to the tank's faction
    }

    /// <summary>
    /// Returns the forward direction of the tank body, ignoring the y-axis.
    /// </summary>
    /// <returns>Normalized forward direction of the tank body.</returns>
    Vector3 tankBodyForward()
    {
        Vector3 forward = transform.forward;
        forward.y = 0;
        return forward.normalized;
    }
}