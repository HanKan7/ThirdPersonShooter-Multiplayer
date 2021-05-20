using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;

public class CharacterAiming : MonoBehaviour
{


    public float turnSpeed = 15f;
    public float aimDuration = 0.3f;
    
    Camera mainCamera;
    RaycastWeapon weapon;
    Animator animator;
    int isAimingParam = Animator.StringToHash("isAiming");
    public bool isAiming;

    public Transform cameraLookAt;
    public AxisState xAxis , yAxis;

    ActiveWeapon activeWeapon;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        weapon = GetComponentInChildren<RaycastWeapon>();
        animator = GetComponent<Animator>();
        activeWeapon = GetComponent<ActiveWeapon>();
    }

    private void Update()
    {
        isAiming = Input.GetMouseButton(1);
        animator.SetBool(isAimingParam, isAiming);
        var weapon = activeWeapon.GetActiveWeapon();
        if (weapon)
        {
            weapon.recoil.recoilModifier = isAiming ? 0.6f : 1.0f;
        }
    }


    void FixedUpdate()
    {
        //if (Input.GetMouseButton(1))
        //{
        //    aimLayer.weight += Time.deltaTime / aimDuration;
        //}
        //else
        //{
        //    aimLayer.weight -= Time.deltaTime / aimDuration;
        //}
        xAxis.Update(Time.fixedDeltaTime);
        yAxis.Update(Time.fixedDeltaTime);
        cameraLookAt.eulerAngles = new Vector3(yAxis.Value, xAxis.Value, 0);
        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.fixedDeltaTime);
    }
}
