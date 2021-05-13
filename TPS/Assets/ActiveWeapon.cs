using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEditor.Animations;

public class ActiveWeapon : MonoBehaviour
{
    public Transform crossHairTarget;
    RaycastWeapon weapon;
    public Rig handIK;
    public Transform weaponParent;
    public Transform LeftGrip,RightGrip;

    Animator anim;
    AnimatorOverrideController overrideAnim;

    // Start is called before the first frame update
    void Start()
    {

        anim = GetComponent<Animator>();
        overrideAnim = anim.runtimeAnimatorController as AnimatorOverrideController;
        RaycastWeapon existingWeapon = GetComponentInChildren<RaycastWeapon>();
        if (existingWeapon)
        {
            Equip(existingWeapon);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (weapon)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                weapon.StartFiring();
            }
            if (weapon.isFiring)
            {
                weapon.UpdateFiring(Time.deltaTime);
            }
            //weapon.UpdateBullets(Time.deltaTime);
            if (Input.GetButtonUp("Fire1"))
            {
                weapon.StopFiring();
            }
        }
        else
        {
            handIK.weight = 0;
            //Debug.Log("No weapon = " + handIK.weight);
            anim.SetLayerWeight(1, 0.0f);
        }
    }

    public void Equip(RaycastWeapon newWeapon)
    {
        if (weapon)
        {
            Destroy(weapon.gameObject);
        }
        weapon = newWeapon;
        Debug.Log("Weapon name = " + weapon.gameObject.name);
        weapon.raycastDestiation = crossHairTarget;
        weapon.transform.parent = weaponParent;
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        handIK.weight = 1;
        anim.SetLayerWeight(1, 1.0f);
        Invoke(nameof(SetAnimationDelayed), 0.001f);
        //Debug.Log("Yes weapon = " + handIK.weight);
    }

    void SetAnimationDelayed()
    {
        overrideAnim["weapon_anim_empty"] = weapon.weaponAnimation;
    }

    [ContextMenu("Save Weapon pose")]
    void SaveWeaponPose()
    {
        GameObjectRecorder recorder = new GameObjectRecorder(gameObject);
        recorder.BindComponentsOfType<Transform>(weaponParent.gameObject, false);
        recorder.BindComponentsOfType<Transform>(LeftGrip.gameObject, false);
        recorder.BindComponentsOfType<Transform>(RightGrip.gameObject, false);
        recorder.TakeSnapshot(0.0f);
        recorder.SaveToClip(weapon.weaponAnimation);
        UnityEditor.AssetDatabase.SaveAssets();
    }
}
