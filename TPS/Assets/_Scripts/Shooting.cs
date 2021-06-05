using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Shooting : MonoBehaviour
{
    public Gun[] allGuns;
    float shotTimer = 0;
    public float accumulatedTime = 0;

    public ParticleSystem muzzleFlash;
    public ParticleSystem hitEffect;
    public TrailRenderer tracerEffect;

    public Transform raycastOrigin;
    public Transform gunRaycastOrigin;
    public Transform notHittingPoint;

    public TMP_Text ammoCountText;
    int ammoCount;

    public Animator anim;
    public bool isReloading = false;

    #region constants
    private const string RELOADING_TRIGGER = "";
    #endregion


    Ray ray;
    RaycastHit hitInfo;

    private void Start()
    {
        ammoCount = allGuns[0].ammoCount;
        ammoCountText.text = ammoCount.ToString();
        //anim = GetComponent<Animator>();
    }

    private void Update()
    {
        

        if (Input.GetKey(KeyCode.Space) && !isReloading) {
            shotTimer = allGuns[0].rateOfFire;
            RateOfFire();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            anim.SetTrigger("isReloading");
            isReloading = true;
        }
    }

    void RateOfFire()
    {

        accumulatedTime += Time.deltaTime;
        float fireInterval = 1.0f / shotTimer;
        while (accumulatedTime >= 0.0f)
        {
            if(ammoCount > 0)
            {
                Shoot();
            }
            else
            {
                anim.SetTrigger("isReloading");
                isReloading = true;
            }
            accumulatedTime -= fireInterval;

        }
        //if(Time.time - shotTimer >= rateOfFire)
        //{
        //    Shoot();
        //    shotTimer = Time.time;
        //}
    }

    public void Reload()
    {
        Debug.Log("Reloading finished");
        ammoCount = allGuns[0].ammoCount;
        ammoCountText.text = ammoCount.ToString();
        this.isReloading = false;
    }

    void Shoot()
    {
        muzzleFlash.Emit(1);
        ray.origin = raycastOrigin.position;
        ray.direction = raycastOrigin.transform.forward;
        var tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        tracer.AddPosition(gunRaycastOrigin.position);
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
        ammoCount--;
        ammoCountText.text = ammoCount.ToString();
    }
}
