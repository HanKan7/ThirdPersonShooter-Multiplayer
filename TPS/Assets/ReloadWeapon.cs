using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadWeapon : MonoBehaviour
{
    public Animator rigController;
    public WeaponAnimationEvents animationEvents;

    public ActiveWeapon activeWeapon;
    public Transform leftHand;

    GameObject magHand;

    private void Start()
    {
        animationEvents.WeaponAnimationEvent.AddListener(OnAnimationEvent);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            rigController.SetTrigger("reload_weapon");
        }
    }

    void OnAnimationEvent(string eventName)
    {
        Debug.Log(eventName);
        switch (eventName)
        {
            case "detach_magazine":
                DetachMag();
                break;

            case "drop_magazine":
                DropMag();
                break;

            case "refill_magazine":
                RefillMag();
                break;

            case "attach_magazine":
                AttachMag();
                break;
        }
    }

    void DetachMag()
    {
        RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
        magHand = Instantiate(weapon.magazine, leftHand, true);
        weapon.magazine.SetActive(false);
    }

    void DropMag()
    {
        GameObject droppedMagazine = Instantiate(magHand, magHand.transform.position, magHand.transform.rotation);
        droppedMagazine.AddComponent<Rigidbody>();
        droppedMagazine.AddComponent<BoxCollider>();
        magHand.SetActive(false);
    }
    void RefillMag()
    {
        magHand.SetActive(true);
    }
    void AttachMag()
    {
        RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
        weapon.magazine.SetActive(true);
        Destroy(magHand);
    }
}
