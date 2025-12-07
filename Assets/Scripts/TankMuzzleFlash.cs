using UnityEngine;

public class TankMuzzleFlash : MonoBehaviour
{
    private ParticleSystem muzzleFx;

    void Awake()
    {
        // 就挂在同一个物体上，所以直接拿
        muzzleFx = GetComponent<ParticleSystem>();
    }

    public void PlayFlash()
    {
        if (muzzleFx == null) return;

        // 保险起见，先停再播，防止连开火时卡住
        muzzleFx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        muzzleFx.Play();
    }
}
