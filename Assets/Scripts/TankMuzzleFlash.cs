using UnityEngine;

public class TankMuzzleFlash : MonoBehaviour
{
    private ParticleSystem muzzleFx;

    void Awake()
    {
        muzzleFx = GetComponent<ParticleSystem>();
    }

    public void PlayFlash()
    {
        if (muzzleFx == null) return;

        muzzleFx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        muzzleFx.Play();
    }
}
