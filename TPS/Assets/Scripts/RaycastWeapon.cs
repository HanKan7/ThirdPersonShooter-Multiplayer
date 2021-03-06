using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{

    class Bullet
    {
        public float time;
        public Vector3 initialPosition;
        public Vector3 initialVelocity;
        public TrailRenderer tracer;
        public  float bounce;
    }

    public bool isAutomatic = false;
    public bool isFiring = false;
    public float fireRate = 25;
    public float bulletSpeed = 1000f;
    public float bulletDrop = 0.0f;
    public float maxBounces = 0;
    public ActiveWeapon.WeaponSlot weaponSlot;
    public int thisWeaponIsEquipped = -1;

    public ParticleSystem muzzleFlash;
    public ParticleSystem hitEffect;
    public TrailRenderer tracerEffect;
    public Transform raycastOrigin;
    public Transform raycastDestiation;
    public WeaponRecoil recoil;
    public string weaponName;

    public GameObject magazine;

    Ray ray;
    RaycastHit hitInfo;
    float accumulatedTime;
    List<Bullet> bullets = new List<Bullet>();
    float maxLifeTime = 3f;

    public int ammoCount;
    public int clipSize;

    public void Awake()
    {
        recoil = GetComponent<WeaponRecoil>();
    }

    Vector3 GetPosition(Bullet bullet)
    {
        //initialPosition + velocity * time + 0.5 * gravity * time * time;
        Vector3 gravity = Vector3.down * bulletDrop;
        return bullet.initialPosition + bullet.initialVelocity * bullet.time + 0.5f * gravity * bullet.time * bullet.time;
    }

    Bullet CreateBullet(Vector3 position, Vector3 velocity)
    {
        Bullet bullet = new Bullet();
        bullet.initialPosition = position;
        bullet.initialVelocity = velocity;
        bullet.time = 0.0f;
        bullet.bounce = maxBounces;
        bullet.tracer = Instantiate(tracerEffect, position, Quaternion.identity);   //tracer shit
        bullet.tracer.AddPosition(position);
        return bullet;
    }

    public void StartFiring()
    {
        Debug.Log("Started Firing " + this.gameObject.name);
        isFiring = true;
        accumulatedTime = 0.0f;
        FireBullet();
        recoil.Reset();
    }

    public void UpdateWeapon(float deltaTime)
    {
        if (Input.GetButtonDown("Fire1"))
        {
            StartFiring();
        }
        if (isFiring)
        {
            UpdateFiring(Time.deltaTime);
            //UpdateBullets(Time.deltaTime);
        }

        if (Input.GetButtonUp("Fire1"))
        {
            StopFiring();
        }
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

    public void UpdateBullets(float deltaTime)
    {
        SimulateBullets(deltaTime);
        DestroyBullets();
    }

    void SimulateBullets(float deltaTime)
    {
        bullets.ForEach(bullet =>
        {
            Vector3 p0 = GetPosition(bullet);
            bullet.time += deltaTime;
            Vector3 p1 = GetPosition(bullet);
            RaycastSegment(p0, p1, bullet);
        });
    }

    void DestroyBullets()
    {
        bullets.RemoveAll(bullet => bullet.time >= maxLifeTime);
    }

    void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    {
        Vector3 direction = end - start;
        float distance = (end - start).magnitude;
        ray.origin = start;
        ray.direction = direction;
        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            //Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1.0f);.
            Debug.Log("Hitting Something New");
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);
            bullet.tracer.transform.position = hitInfo.point;
            bullet.time = maxLifeTime;

            //Collision Impulse
            var rb2d = hitInfo.collider.GetComponent<Rigidbody>();
            if (rb2d)
            {
                rb2d.AddForceAtPosition(ray.direction * 4, hitInfo.point, ForceMode.Impulse);
            }

            //Bullet Ricochet
            if(bullet.bounce > 0)
            {
                bullet.time = 0;
                bullet.initialPosition = hitInfo.point;
                bullet.initialVelocity = Vector3.Reflect(bullet.initialVelocity, hitInfo.normal);
                bullet.bounce--;
            }
        }
        else
        {
            bullet.tracer.transform.position = end;
        }
    }

    private void FireBullet()
    {
        Debug.Log("Fire Bullet " + this.gameObject.name);
        if (ammoCount <= 0)
        {
            return;
        }
        ammoCount--;
        muzzleFlash.Emit(1);
        //Vector3 veloctiy = (raycastDestiation.position - raycastOrigin.position).normalized * bulletSpeed;
        //var bullet = CreateBullet(raycastOrigin.position, veloctiy);
        //bullets.Add(bullet);


        ray.origin = raycastOrigin.position;
        ray.direction = raycastDestiation.position - raycastOrigin.position;
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        var tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        tracer.AddPosition(ray.origin);
        if (Physics.Raycast(raycastOrigin.position, raycastDestiation.position - raycastOrigin.position, out hitInfo, 1000f))
        {
            //Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1.0f);
            //Debug.Log("Hitting Something" + " " + transform.parent.name);
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);
            tracer.transform.position = hitInfo.point;



            //Collision Impulse
            var rb2d = hitInfo.collider.GetComponent<Rigidbody>();
            if (rb2d)
            {
                rb2d.AddForceAtPosition(ray.direction * 4, hitInfo.point, ForceMode.Impulse);
            }

        }
        else
        {

            //Debug.Log("Didnt hit anything");
        }
        recoil.GenerateRecoil(weaponName);
        Destroy(tracer.gameObject, 0.25f);

    }

    public void StopFiring()
    {
        isFiring = false;
    }

}
