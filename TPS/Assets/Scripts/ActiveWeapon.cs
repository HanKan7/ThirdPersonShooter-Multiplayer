using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEditor.Animations;
using Cinemachine;

public class ActiveWeapon : MonoBehaviour
{

    public enum WeaponSlot
    {
        Primary = 0,Secondary = 1   //index values
    }
    int activeWeaponIndex = -1;
    bool isHolstered = false;
    public Transform crossHairTarget;
    public Transform[] weaponSlots;
    public Transform LeftGrip,RightGrip;
    public RaycastWeapon[] equipped_Weapon = new RaycastWeapon[2];

    public Animator rigController;
    public CinemachineFreeLook playerCamera;

    //AnimatorOverrideController overrideAnim;

    // Start is called before the first frame update
    void Start()
    {    
        RaycastWeapon existingWeapon = GetComponentInChildren<RaycastWeapon>();
        if (existingWeapon)
        {
            Equip(existingWeapon);
        }
    }

    RaycastWeapon GetWeapon(int index)
    {
        if (index < 0 || index >= equipped_Weapon.Length) return null;
        return equipped_Weapon[index];
    }

    // Update is called once per frame
    void Update()
    {
        var weapon = GetWeapon(activeWeaponIndex);
        if (weapon && !isHolstered)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                
                weapon.StartFiring();
            }
            if (weapon.isFiring)
            {
                weapon.UpdateFiring(Time.deltaTime);
                //weapon.UpdateBullets(Time.deltaTime);
            }
            
            if (Input.GetButtonUp("Fire1"))
            {
                weapon.StopFiring();
            }

        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            ToggleActiveWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetActiveWeapon(WeaponSlot.Primary);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetActiveWeapon(WeaponSlot.Secondary);
        }

    }

    public void Equip(RaycastWeapon newWeapon)
    {
        int weaponSlotIndex = (int)newWeapon.weaponSlot;
        var weapon = GetWeapon(weaponSlotIndex);
        if (weapon)
        {
            Destroy(weapon.gameObject);   
        }
        weapon = newWeapon;
        Debug.Log("Weapon name = " + weapon.gameObject.name);
        weapon.raycastDestiation = crossHairTarget;
        weapon.recoil.playerCamera = playerCamera;
        weapon.recoil.rigController = rigController;
        weapon.transform.SetParent(weaponSlots[weaponSlotIndex], false);
        //weapon.transform.localPosition = Vector3.zero;
        //weapon.transform.localRotation = Quaternion.identity;
        
        equipped_Weapon[weaponSlotIndex] = weapon;
        SetActiveWeapon(newWeapon.weaponSlot);
    }

    void ToggleActiveWeapon()
    {
        bool isHolstered = rigController.GetBool("holster_weapon");
        if (isHolstered)
        {
            StartCoroutine(ActivateWeapon(activeWeaponIndex));
        }
        else
        {
            StartCoroutine(HolsterWeapon(activeWeaponIndex));
        }
    }

    void SetActiveWeapon(WeaponSlot weaponSlot)
    {
        int holsterIndex = activeWeaponIndex;
        int activateIndex = (int)weaponSlot;
        if (holsterIndex == activateIndex) holsterIndex = -1;
        StartCoroutine(SwitchWeapon(holsterIndex, activateIndex));
    }

    IEnumerator SwitchWeapon(int holsterIndex, int activateIndex)
    {
        yield return StartCoroutine(HolsterWeapon(holsterIndex));
        yield return StartCoroutine(ActivateWeapon(activateIndex));
        activeWeaponIndex = activateIndex;
    }

    IEnumerator HolsterWeapon(int index)
    {
        isHolstered = true;
        var weapon = GetWeapon(index);
        if (weapon)
        {
            rigController.SetBool("holster_weapon", true);
            do
            {
                yield return new WaitForEndOfFrame();
            } while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
        }
    }
    IEnumerator ActivateWeapon(int index)
    {

        var weapon = GetWeapon(index);
        if (weapon)
        {
            rigController.SetBool("holster_weapon", false);
            rigController.Play("equip_" + weapon.weaponName);
            do
            {
                yield return new WaitForEndOfFrame();
            } while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
            isHolstered = false;
        }
    }


    //[ContextMenu("Save Weapon pose")]
    //void SaveWeaponPose()
    //{
    //    GameObjectRecorder recorder = new GameObjectRecorder(gameObject);
    //    recorder.BindComponentsOfType<Transform>(weaponParent.gameObject, false);
    //    recorder.BindComponentsOfType<Transform>(LeftGrip.gameObject, false);
    //    recorder.BindComponentsOfType<Transform>(RightGrip.gameObject, false);
    //    recorder.TakeSnapshot(0.0f);
    //    recorder.SaveToClip(weapon.weaponAnimation);
    //    UnityEditor.AssetDatabase.SaveAssets();
    //}
}
