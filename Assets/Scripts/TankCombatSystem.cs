using UnityEngine;

public class TankCombatSystem : MonoBehaviour
{
    public float fireInterval = 2f;
    public float detectionRange = 25f;
    public float rotateSpeed = 3f;
    public Transform turret;
    public Transform firePoint;
    public GameObject shellPrefab;
    public ParticleSystem muzzleFlash;
    public LayerMask targetMask;


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
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            detectionRange,
            targetMask
        );


        foreach (var h in hits)
        {
            Faction f = h.GetComponentInParent<Faction>();
            TankHealth th = h.GetComponentInParent<TankHealth>();

            if (f != null && th != null && !th.isDead &&
                f.factionType != myFaction.factionType)
            {
                // 计算目标方向与坦克正前方的夹角
                Vector3 directionToTarget = (h.transform.position - transform.position).normalized;
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

                // 获取炮塔脚本中的最大偏移角度
                InMoving_TurretOffset offsetScript = turret.GetComponent<InMoving_TurretOffset>();
                float maxLockAngle = offsetScript != null ? offsetScript.maxOffsetAngle : 30f; // 默认30度

                // 如果目标在允许的角度范围内，则锁定目标
                if (angleToTarget <= maxLockAngle)
                {
                    target = h.GetComponentInParent<Transform>();
                    return;
                }
            }
        }
    }

    void AimAndFire()
    {
        if (target == null) return;

        Vector3 dir = target.position - turret.position;
        dir.y = 0;
        Quaternion desired = Quaternion.LookRotation(dir);

        // 获取偏移角度
        InMoving_TurretOffset offsetScript = turret.GetComponent<InMoving_TurretOffset>();
        float offsetAngle = offsetScript != null ? offsetScript.CurrentOffsetAngle : 0f;

        // 应用偏移
        desired = Quaternion.Euler(0f, desired.eulerAngles.y + offsetAngle, 0f);

        float angle = Quaternion.Angle(turret.rotation, desired);
        if (angle > 0.1f)
        {
            turret.rotation = Quaternion.Slerp(turret.rotation, desired, rotateSpeed * Time.deltaTime);
        }

        if (angle < 15f && fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireInterval;
        }
    }
    void Shoot()
    {
        // 1. 播放炮口火焰特效
        if (muzzleFlash != null)
        {
            muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            muzzleFlash.Play();
        }

        // 2. 发射炮弹
        GameObject shell = Instantiate(shellPrefab, firePoint.position, firePoint.rotation);

        Shell s = shell.GetComponent<Shell>();
        if (s != null)
            s.ownerFaction = myFaction;
    }
}