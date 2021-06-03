using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Gun[] allGuns;
    float shotTimer = 0;

    public ParticleSystem muzzleFlash;
    public ParticleSystem hitEffect;
    public TrailRenderer tracerEffect;

    public Transform raycastOrigin;
    public Transform notHittingPoint;

    Ray ray;
    RaycastHit hitInfo;

    private void Start()
    {
        shotTimer = allGuns[0].rateOfFire;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space)) {
            RateOfFire(allGuns[0].rateOfFire);
        }
    }

    void RateOfFire(float rateOfFire)
    {
        //Debug.Log("Shot timer " + shotTimer);
        if(Time.time - shotTimer >= rateOfFire)
        {
            Shoot();
            shotTimer = Time.time;
        }
    }

    void Shoot()
    {
        muzzleFlash.Emit(1);
        ray.origin = raycastOrigin.position;
        ray.direction = raycastOrigin.transform.forward;
        var tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        tracer.AddPosition(ray.origin);
        if (Physics.Raycast(ray, out hitInfo, 1000f))
        {
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);
            tracer.transform.position = hitInfo.point;
        }
        else
        {
            tracer.transform.position = notHittingPoint.position;
        }
        Destroy(tracer.gameObject, 0.25f);
    }
}
