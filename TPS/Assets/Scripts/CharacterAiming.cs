using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using Photon.Pun;

public class CharacterAiming : MonoBehaviourPunCallbacks
{

    
    public float turnSpeed = 15f;
    public float aimDuration = 0.3f;
    
     public Camera mainCamera;
    RaycastWeapon weapon;
    Animator animator;
    int isAimingParam = Animator.StringToHash("isAiming");
    public bool isAiming;

    public Transform cameraLookAt;
    public AxisState xAxis , yAxis;
    public CinemachineVirtualCamera vCam;

    ActiveWeapon activeWeapon;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        //Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        weapon = GetComponentInChildren<RaycastWeapon>();
        animator = GetComponent<Animator>();
        activeWeapon = GetComponent<ActiveWeapon>();
        if (!photonView.IsMine)
        {
            vCam.enabled = false;
        }
    }

    private void Update()
    {
        isAiming = Input.GetMouseButton(1);
        animator.SetBool(isAimingParam, isAiming);
        weapon = activeWeapon.GetActiveWeapon();
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
        if(photonView.IsMine)
       {
            xAxis.Update(Time.fixedDeltaTime);
            yAxis.Update(Time.fixedDeltaTime);
            cameraLookAt.eulerAngles = new Vector3(yAxis.Value, xAxis.Value, 0);
            float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.fixedDeltaTime);
       }
        
    }
    
}
