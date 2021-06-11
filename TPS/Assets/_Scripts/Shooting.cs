using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Shooting : MonoBehaviourPunCallbacks
{
    public Gun[] allGuns;
    float shotTimer = 0;
    public float accumulatedTime = 0;

    public ParticleSystem muzzleFlash;
    public ParticleSystem hitEffect;
    public TrailRenderer tracerEffect;

    public Transform raycastOriginOfBullet;
    public Transform tracerGunRaycastOrigin;
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
        if (photonView.IsMine)
        {
            ammoCount = allGuns[0].ammoCount;
            ammoCountText = GameObject.Find("AmmoAmount").GetComponent<TMP_Text>();
            ammoCountText.text = ammoCount.ToString();
            //anim = GetComponent<Animator>();
        }

    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetMouseButton(0) && !isReloading)
            {
                shotTimer = allGuns[0].rateOfFire;
                RateOfFire();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                anim.SetTrigger("isReloading");
                isReloading = true;
                //anim.SetBool("reloading", isReloading);
            }
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
                //anim.SetBool("reloading", isReloading);
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
        if (photonView.IsMine)
        {
            Debug.Log("Reloading finished");
            ammoCount = allGuns[0].ammoCount;
            ammoCountText.text = ammoCount.ToString();
            this.isReloading = false;
        }

    }

    void Shoot()
    {
        if (photonView.IsMine)
        {
            muzzleFlash.Emit(1);
            ray.origin = raycastOriginOfBullet.position;
            ray.direction = raycastOriginOfBullet.transform.forward;
            var tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
            tracer.GetComponent<TrailRenderer>().AddPosition(tracerGunRaycastOrigin.position);
            if (Physics.Raycast(ray, out hitInfo, 1000f))
            {
                photonView.RPC("ShowParticles", RpcTarget.All);
                //ShowParticles();
                tracer.transform.position = hitInfo.point;
                if (hitInfo.collider.CompareTag("Player"))
                {
                    GiveDamage(allGuns[0].shotDamage);
                }
            }
            else
            {
                tracer.transform.position = notHittingPoint.position;
            }
            Destroy(tracer.gameObject, 0.25f);
            //photonView.RPC("DestroyTracerEffectRPC", RpcTarget.All, tracer);
            ammoCount--;
            ammoCountText.text = ammoCount.ToString();
        }
    }

    [PunRPC]
    void ShowParticles()
    {
        hitEffect.transform.position = hitInfo.point;
        hitEffect.transform.forward = hitInfo.normal;
        hitEffect.Emit(1);
    }

    [PunRPC]
    void DestroyTracerEffectRPC(GameObject tracer)
    {
        StartCoroutine(DestroyTracerEffectCoroutine(tracer.gameObject));
    }

    IEnumerator DestroyTracerEffectCoroutine(GameObject tracer)
    {
        yield return new WaitForSeconds(0.25f);
        PhotonNetwork.Destroy(tracer);
    }

    void GiveDamage(float damageAmt)
    {

    }
}
