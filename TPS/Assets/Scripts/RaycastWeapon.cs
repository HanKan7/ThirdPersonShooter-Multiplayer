using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    public bool isFiring = false;
    public int fireRate = 25;
    public ParticleSystem muzzleFlash;
    public ParticleSystem hitEffect;
    public TrailRenderer tracerEffect;
    public Transform raycastOrigin;
    public Transform raycastDestiation;

    Ray ray;
    RaycastHit hitInfo;
    float accumulatedTime;

    public void StartFiring()
    {
        isFiring = true;
        accumulatedTime = 0.0f;
        FireBullet();
    }

    public void UpdateFiring(float deltaTime)
    {
        accumulatedTime += deltaTime;
        float fireInterval = 1.0f / fireRate;
        while(accumulatedTime >= 0.0f)
        {
            FireBullet();
            accumulatedTime -= fireInterval;
        }
    }

    private void FireBullet()
    {
        muzzleFlash.Emit(1);

        ray.origin = raycastOrigin.position;
        ray.direction = raycastDestiation.position - raycastOrigin.position;
        var tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        tracer.AddPosition(ray.origin);
        if (Physics.Raycast(ray, out hitInfo))
        {
            //Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1.0f);
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);
            tracer.transform.position = hitInfo.point;
        }
    }

    public void StopFiring()
    {
        isFiring = false;
    }
}
