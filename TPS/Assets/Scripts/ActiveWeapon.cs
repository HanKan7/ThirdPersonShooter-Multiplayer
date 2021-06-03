using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Animations.Rigging;
//using UnityEditor.Animations;
using Cinemachine;
using Photon.Pun;

public class ActiveWeapon : MonoBehaviourPunCallbacks
{

    public enum WeaponSlot
    {
        Primary = 0,Secondary = 1   //index values
    }
    int activeWeaponIndex = -1;
    bool isHolstered = false;
    public bool isPrimaryEquipped = false, isSecondaryEquipped = false;
    public Transform crossHairTarget;
    public Transform[] weaponSlots;
    public Transform LeftGrip,RightGrip;
    public RaycastWeapon[] equipped_Weapon = new RaycastWeapon[2];

    public Animator rigController;
    public CharacterAiming characterAiming;
    public AmmoWidget ammoWidget;


    public bool isChangingWeapon = false;
    ReloadWeapon reloadWeapon;
    CharacterLocomotion characterLocomotion;




    //AnimatorOverrideController overrideAnim;

    // Start is called before the first frame update
    void Start()
    {    
        RaycastWeapon existingWeapon = GetComponentInChildren<RaycastWeapon>();
        characterLocomotion = GetComponent<CharacterLocomotion>();
        reloadWeapon = GetComponent<ReloadWeapon>();
        ammoWidget = FindObjectOfType<AmmoWidget>();
        crossHairTarget = FindObjectOfType<CrossHairTarget>().transform;
        if (existingWeapon)
        {
            Equip(existingWeapon);
        }
    }

    public RaycastWeapon GetActiveWeapon()
    {
        return GetWeapon(activeWeaponIndex);
    }

    RaycastWeapon GetWeapon(int index)
    {
        if (index < 0 || index >= equipped_Weapon.Length) return null;
        return equipped_Weapon[index];
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {

            var weapon = GetWeapon(activeWeaponIndex);
            if ((weapon && !isHolstered))
            {
                //if (Input.GetButtonDown("Fire1")) //automatic
                //{
                //    Debug.Log("Firing " + equipped_Weapon[activeWeaponIndex].name);
                //    weapon.StartFiring();
                //}
                if (Input.GetMouseButtonDown(0) && !characterLocomotion.IsSprinting() && !reloadWeapon.isReloading) //automatic
                {
                    Debug.Log("Firing " + equipped_Weapon[activeWeaponIndex].name);
                    weapon.StartFiring();
                
                }
                if (Input.GetMouseButton(0) && weapon.isAutomatic && !characterLocomotion.IsSprinting() && !reloadWeapon.isReloading)
                {
                    Debug.Log("Firing continous " + equipped_Weapon[activeWeaponIndex].name);
                    weapon.UpdateFiring(Time.deltaTime);
                    //weapon.UpdateBullets(Time.deltaTime);
                }
            
                //if (Input.GetButtonUp("Fire1"))
                //{
                //    weapon.StopFiring();
                //}
                if (Input.GetMouseButtonUp(0))
                {
                    weapon.StopFiring();
                }

            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                ToggleActiveWeapon();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1)    && isPrimaryEquipped)
            {
                ammoWidget.ammoText.text = equipped_Weapon[0].ammoCount.ToString();
                SetActiveWeapon(WeaponSlot.Primary);
                //photonView.RPC("SetActiveWeapon", RpcTarget.All, WeaponSlot.Primary);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)    && isSecondaryEquipped)
            {
                ammoWidget.ammoText.text = equipped_Weapon[1].ammoCount.ToString();
                SetActiveWeapon(WeaponSlot.Secondary);
                //photonView.RPC("SetActiveWeapon", RpcTarget.All, WeaponSlot.Secondary);
            }
            if (Input.GetKeyDown(KeyCode.Q) && isPrimaryEquipped && isSecondaryEquipped)
            {
                var getWeapon = GetWeapon(activeWeaponIndex);
                if (getWeapon)
                {
                    if ((int)getWeapon.weaponSlot == 0 )
                    {
                        ammoWidget.ammoText.text = equipped_Weapon[1].ammoCount.ToString();
                        SetActiveWeapon(WeaponSlot.Secondary);
                        //photonView.RPC("SetActiveWeapon", RpcTarget.All, WeaponSlot.Secondary);
                    }
                    if ((int)getWeapon.weaponSlot == 1)
                    {
                        ammoWidget.ammoText.text = equipped_Weapon[0].ammoCount.ToString();
                        SetActiveWeapon(WeaponSlot.Primary);
                        //photonView.RPC("SetActiveWeapon", RpcTarget.All, WeaponSlot.Primary);
                    }
                }
            
            }
        }

    }

    public bool isFiring()
    {
        RaycastWeapon currentWeapon = GetActiveWeapon();
        if (!currentWeapon)
        {
            return false;
        }
        return currentWeapon.isFiring;
    }

    public void Equip(RaycastWeapon newWeapon)
    {
        int weaponSlotIndex = (int)newWeapon.weaponSlot;
        if (weaponSlotIndex == 0) isPrimaryEquipped = true;
        if (weaponSlotIndex == 1) isSecondaryEquipped = true;
        RaycastWeapon weapon = GetWeapon(weaponSlotIndex);
        if (weapon)
        {
            //Destroy(weapon.gameObject,0.001f);   
            weapon = null;
        }
        weapon = newWeapon;
        //Debug.Log("Weapon name = " + weapon.gameObject.name);
        weapon.raycastDestiation = crossHairTarget;
        weapon.recoil.characterAiming = characterAiming;
        weapon.recoil.rigController = rigController;
        weapon.transform.SetParent(weaponSlots[weaponSlotIndex], false);
        //weapon.transform.localPosition = Vector3.zero;
        //weapon.transform.localRotation = Quaternion.identity;
        
        equipped_Weapon[weaponSlotIndex] = weapon;
        SetActiveWeapon(newWeapon.weaponSlot);
       // photonView.RPC("SetActiveWeapon", RpcTarget.All, newWeapon.weaponSlot);
        ammoWidget.Refresh(weapon.ammoCount);
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
        rigController.SetInteger("weapon_index", activateIndex);
        yield return StartCoroutine(HolsterWeapon(holsterIndex));
        yield return StartCoroutine(ActivateWeapon(activateIndex));
        activeWeaponIndex = activateIndex;
    }

    IEnumerator HolsterWeapon(int index)
    {
        isChangingWeapon = true;
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
        isChangingWeapon = false;
    }
    IEnumerator ActivateWeapon(int index)
    {
        isChangingWeapon = true;
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
        isChangingWeapon = false;
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
